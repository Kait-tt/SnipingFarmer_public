using UnityEngine;

namespace SnipingFarmer.Script.Game.Item
{
    public class DecoyItem : AbstractItem
    {
        [SerializeField] private GameObject decoy;

        public override void Capture(Bullet bullet)
        {
            var pos = transform.position;

            base.Capture(bullet);
            
            Instantiate(decoy, pos, Quaternion.identity);
        }
    }
}