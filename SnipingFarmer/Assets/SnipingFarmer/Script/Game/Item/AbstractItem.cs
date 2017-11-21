using UnityEngine;

namespace SnipingFarmer.Script.Game.Item
{
    public abstract class AbstractItem : AutoDestroyMonoBehaviourBase, ICapturable
    {
        [SerializeField] private float rotateSpeed = 1.0f;
        [SerializeField] private int scorePoint = 150; 

        public void Update()
        {
            RotateCube();
        }

        public virtual void Capture(Bullet bullet)
        {
            bullet.ShotPlayer.Meta.Score.Value += scorePoint;
            DestroyIfExists(gameObject);
        }

        private void RotateCube()
        {
            transform.Rotate(Vector3.one * rotateSpeed);
        }
    }
}