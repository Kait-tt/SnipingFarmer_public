using System.Collections;
using UnityEngine;

namespace SnipingFarmer.Script.Game
{
    [RequireComponent(typeof(Renderer))]
    public class Bullet : AutoDestroyMonoBehaviourBase
    {

        [SerializeField] private GameObject effect;

        private Renderer renderer;
        
        public Player ShotPlayer { get; private set; }

        public SnipingArea ShotArea { get; private set; }

        public void Awake()
        {
            renderer = GetComponent<Renderer>();
            renderer.enabled = false;
        }

        public new IEnumerator Start()
        {
            base.Start();
            
            // チラつき防止用に数フレーム非表示にする
            yield return null;
            yield return null;
            yield return null;

            renderer.enabled = true;
        }
        
        void OnCollisionEnter(Collision collision)
        {
            Land(collision.gameObject);
        }

        public void Init(Player shotPlayer, SnipingArea shotArea)
        {
            ShotPlayer = shotPlayer;
            ShotArea = shotArea;
        }

        private void Land(GameObject target)
        {
            var capturable = target.GetComponent<ICapturable>();
            if (capturable != null)
            {
                capturable.Capture(this);
            }
            Instantiate(effect, transform.position, Quaternion.identity);
            
            DestroyIfExists(gameObject);
        }
    }
}