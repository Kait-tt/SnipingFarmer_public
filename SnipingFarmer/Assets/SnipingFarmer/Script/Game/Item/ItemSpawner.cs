using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace SnipingFarmer.Script.Game.Item
{
    [RequireComponent(typeof(AudioSource))]
    public class ItemSpawner : MonoBehaviourBase
    {
        [SerializeField] private GameObject[] itemList;
        [SerializeField] private IntReactiveProperty spawnIntervalSecond = new IntReactiveProperty(30);
        
        private Stack<GameObject> itemPool = new Stack<GameObject>();

        private List<Vector3> spawnPositions;

        private AudioSource itemSpawnAudio;

        public void Awake()
        {
            spawnPositions = GameObject
                .FindGameObjectsWithTag("ItemSpawnPoint")
                .Select(obj => obj.transform.position)
                .ToList();

            if (spawnPositions.Count == 0)
            {
                Debug.LogError("ItemSpawnPoint is not found");
            }
            
            itemSpawnAudio = GetComponent<AudioSource>();
            Assert.IsNotNull(itemSpawnAudio);
        }

        public void StartSpawnLoop()
        {
            InitItemPool();
            StartCoroutine(SpawnLoop());
        }

        public void StopSpawnLoop()
        {
            StartCoroutine(SpawnLoop());
        }

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnIntervalSecond.Value);

                SpawnNext();
            }
        }

        private void InitItemPool()
        {
            itemPool.Clear();
            
            var list = itemList.OrderBy(i => Guid.NewGuid());
                
            foreach (var item in list)
            {
                itemPool.Push(item);
            }
        }

        private void SpawnNext()
        {
            if (itemPool.Count == 0)
            {
                Debug.LogError("item pool size is 0. Cannot spawn next item.");
                return;
            }
            
            var item = itemPool.Pop();
            var pos = NextSpawnPosition();
            
            Instantiate(item, pos, Quaternion.identity);
            
            // Play Item Spawn SE
            itemSpawnAudio.Play();
        }

        private Vector3 NextSpawnPosition()
        {
            if (spawnPositions.Count == 0)
            {
                Debug.LogError("ItemSpawnPoint is not found");
                return Vector3.zero;
            }

            return spawnPositions[Random.Range(0, spawnPositions.Count)];
        }
    }
}