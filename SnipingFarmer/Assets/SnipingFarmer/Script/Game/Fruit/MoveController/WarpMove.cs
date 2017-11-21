using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace SnipingFarmer.Script.Game.Fruit.MoveController
{
    [RequireComponent(typeof(AbstractFruit))]
    public class WarpMove : BaseMove
    {
        [SerializeField] private float warpDuration = 3f;
        
        // warpDistanceを大きくしたらarrivalEpsも大きくしないと目的地付近でウロウロすることがある
        [SerializeField] private float warpDistanceMin = 10f;
        [SerializeField] private float warpDistanceMax = 20f;
        
        // どちらもDeg
        [SerializeField] private float warpAngle = 30f;

        private GameObject fruitMoveArea;

        private AbstractFruit fruit;
        
        public new void Awake()
        {
            base.Awake();
            agent.updatePosition = false;

            Assert.IsTrue(warpDistanceMin <= warpDistanceMax);
            Assert.IsTrue(warpDuration >= 0f);
            
            fruitMoveArea = GameObject.FindGameObjectWithTag("FruitSpawnArea");
            if (fruitMoveArea == null)
            {
                Debug.LogError("FruitSpawnArea is not found.");
            }

            fruit = GetComponent<AbstractFruit>();
        }

        public new void Start()
        {
            base.Start();

            StartCoroutine(WarpLoop());
        }

        private IEnumerator WarpLoop()
        {
            while (true)
            {
                var duration = fruit.IsStrong.Value ? warpDuration / 2 : warpDuration;
                yield return new WaitForSeconds(duration);
                
                WarpToNext();
            }
        }

        private void WarpToNext()
        {
            var offset = NextWarpOffset();
            var pos = ClampWithArea(transform.position + offset);


            transform.position = pos;
            agent.nextPosition = pos;
        }

        private Vector3 NextWarpOffset()
        {
            var distance = NextMoveDistance();
            var angle = NextMoveAngle();
            
            var pos = new Vector3(
                distance * Mathf.Cos(angle),
                0f,
                distance * Mathf.Sin(angle) 
            );

            return pos;
        }

        // return: rad
        private float NextMoveAngle()
        {
            var deltaTheta = Random.Range(-warpAngle, warpAngle) * Mathf.Deg2Rad;
            var dir = agent.nextPosition - transform.position;
            var theta = Mathf.Atan2(dir.z, dir.x);
            return theta + deltaTheta;
        }

        private float NextMoveDistance()
        {
            return Random.Range(warpDistanceMin, warpDistanceMax);
        }

        // 外に出そうなら丸める
        private Vector3 ClampWithArea(Vector3 orig)
        {
            if (fruitMoveArea == null) return orig;
            
            var bounds = fruitMoveArea.GetComponent<Renderer>().bounds;

            var res = orig;

            return new Vector3(
                Mathf.Clamp(res.x, bounds.min.x, bounds.max.x),
                orig.y,
                Mathf.Clamp(res.z, bounds.min.z, bounds.max.z)
            );
        }
    }
}