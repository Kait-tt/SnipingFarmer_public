using System.Reflection;
using UnityEngine;
using SnipingFarmer.Script.Game;

namespace SnipingFarmer.Script.Util
{
    public class Factory {
        public static GameObject SpawnPlayerObject()
        {
            var obj = new GameObject();
            obj.AddComponent(typeof(Player));
            obj.AddComponent(typeof(Weapon));

            var weapon = obj.GetComponent<Weapon>();
            weapon.Start();
        
            // weapon.bullet に仮のGameObjectを入れておく
            var prop = weapon.GetType().GetField("bullet", BindingFlags.NonPublic | BindingFlags.Instance);
            prop.SetValue(weapon, SpawnBulletObject());
        
            var player = obj.GetComponent<Player>();
            player.Start();
        
            // プレイヤーを適当な場所に立たせておく
            player.CurrentArea.Value = new GameObject().AddComponent<SnipingArea>();

            return obj;
        }

        public static GameObject SpawnBulletObject()
        {
            var obj = new GameObject();
            obj.AddComponent(typeof(Bullet));
            obj.AddComponent(typeof(Rigidbody));
        
            return obj;
        }
    }

}
