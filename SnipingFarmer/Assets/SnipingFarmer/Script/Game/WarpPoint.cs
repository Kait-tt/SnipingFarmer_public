using UnityEngine;
using UnityEngine.Assertions;

namespace SnipingFarmer.Script.Game
{
    public class WarpPoint : MonoBehaviourBase
    {
        [SerializeField] private GameObject distArea;

        public Vector3 DistPosition
        {
            get
            {
                var bounds = distArea.GetComponent<Renderer>().bounds;
                
                return new Vector3(
                    bounds.center.x,
                    bounds.max.y,
                    bounds.center.z
                );
            }
        }

        public GameObject DistArea
        {
            get { return distArea; }
        }

        public void Awake()
        {
            Assert.IsNotNull(distArea);
            Assert.IsNotNull(distArea.GetComponent<SnipingArea>());
            Assert.IsNotNull(distArea.GetComponent<Renderer>());
        }

        public void Warp(GameObject player)
        {
            var isWarp = player.GetComponentInChildren<Player>().IsWarping;
            player.transform.position = DistPosition;

            isWarp.Value = true;
            isWarp.Value = false;
        }

        void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                Warp(collider.gameObject.transform.parent.gameObject);
            }
        }
        
    }
}