using SnipingFarmer.Script.GameMeta;
using UnityEngine;

namespace SnipingFarmer.Script.Game.Item
{
    public class StrongItem : AbstractItem
    {
        private FruitStrong fruitStrong;

        public new void Start()
        {
            base.Start();
            
            fruitStrong = GameObject.Find("FruitStrong").GetComponent<FruitStrong>();
            if (fruitStrong == null)
            {
                Debug.LogError("FruitStrong GameObject is not found.");
            }
        }

        public override void Capture(Bullet bullet)
        {
            base.Capture(bullet);

            if (fruitStrong)
            {
                fruitStrong.Raise();
            }
        }
    }
}