using SnipingFarmer.Script.GameMeta;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

namespace SnipingFarmer.Script.Game.Fruit.MovePoint
{
    public class Decoy : AutoDestroyMonoBehaviourBase 
    {
        [SerializeField] private float rotateSpeed = 1.0f;
        
        [SerializeField] private FloatReactiveProperty visibleDistance = new FloatReactiveProperty(200f);

        private AudioSource decoySpawnAudio;

        public ReadOnlyReactiveProperty<float> VisibleDistance
        {
            get { return visibleDistance.ToReadOnlyReactiveProperty(); }
        }

        public new void Start()
        {
            AddToDecoyMaster();
            base.Start();
            
            decoySpawnAudio = GetComponent<AudioSource>();
            Assert.IsNotNull(decoySpawnAudio);
            
            decoySpawnAudio.Play();
        }

        public void Update()
        {
            RotateDecoy();
        }

        private void RotateDecoy()
        {
            transform.Rotate(Vector3.up * rotateSpeed);
        }

        private void AddToDecoyMaster()
        {
            var decoyMasterObj = GameObject.Find("DecoyMaster");
            if (decoyMasterObj == null)
            {
                Debug.LogError("DecoyMaster GameObject is not found." +
                               " Cannot register this decoy with the master.");
                return;
            }

            var decoyMaster = decoyMasterObj.GetComponent<DecoyMaster>();
            if (decoyMaster == null)
            {
                Debug.LogError("DecoyMaster Compnent is not attached to DecoyMaster GameObject." +
                               " Cannot register this decoy with the master.");
                return;
            }
            
            decoyMaster.AddDecoy(gameObject);
        }
    }
}