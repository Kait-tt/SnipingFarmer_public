using System.Collections;
using UnityEngine;
using UniRx;

namespace SnipingFarmer.Script.Game.Fruit
{
    public class FruitSpawner : MonoBehaviourBase
    {
        [SerializeField] private GameObject[] fruitList;
        [SerializeField] private IntReactiveProperty spawnIntervalSecond = new IntReactiveProperty(10);
        [SerializeField] private IntReactiveProperty spawnSize = new IntReactiveProperty(5);
        [SerializeField] private IntReactiveProperty spawnLimitSize = new IntReactiveProperty(60);
        [SerializeField] private FloatReactiveProperty spawnOffsetY = new FloatReactiveProperty(10f);

        private GameObject fruitSpawnArea;

        public void Awake()
        {
            if (fruitList.Length == 0)
            {
                Debug.LogError("fruitList length is zero.");
            }

            fruitSpawnArea = GameObject.FindGameObjectWithTag("FruitSpawnArea");
            if (fruitSpawnArea == null)
            {
                Debug.LogError("FruitSpawnArea is not found.");
            }
        }

        public void StartSpawnLoop()
        {
            StartCoroutine(SpawnLoop());
        }

        public void StopSpawnLoop()
        {
            StopCoroutine(SpawnLoop());
        }

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnIntervalSecond.Value);

                SpawnNext();
            }
        }

        private void SpawnNext()
        {
            int existsCount = GameObject.FindGameObjectsWithTag("Fruit").Length;
            
            for (int i = 0; i < spawnSize.Value; ++i)
            {
                if (existsCount + i >= spawnLimitSize.Value) break;
                
                var fruit = NextSpawnFruit();
                var pos = NextSpawnPos();

                Instantiate(fruit, pos, Quaternion.identity);
            }
        }

        private GameObject NextSpawnFruit()
        {
            if (fruitList.Length == 0)
            {
                Debug.LogError("fruitList length is zero. Cannot spawn next fruit.");
                return null;
            }

            return fruitList[Random.Range(0, fruitList.Length)];
        }

        private Vector3 NextSpawnPos()
        {
            if (fruitSpawnArea == null) return Vector3.zero;

            var bounds = fruitSpawnArea.GetComponent<Renderer>().bounds;

            while (true)
            {
                var vec = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    bounds.max.y + spawnOffsetY.Value,
                    Random.Range(bounds.min.z, bounds.max.z)
                );
                
                // 湧き場所にオブジェクトが無ければそこにする
                if (!Physics.Raycast(vec, Vector3.one / 10))
                {
                    return vec;
                }
            } 
        }
    }
}