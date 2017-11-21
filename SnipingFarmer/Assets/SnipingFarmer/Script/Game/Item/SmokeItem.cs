using UnityEngine;

namespace SnipingFarmer.Script.Game.Item
{
    public class SmokeItem : AbstractItem
    {
        [SerializeField] private GameObject smoke;

        public override void Capture(Bullet bullet)
        {
            var pos = transform.position;
            
            base.Capture(bullet); 

            Instantiate(smoke, pos, Quaternion.identity);
        }
    }
}