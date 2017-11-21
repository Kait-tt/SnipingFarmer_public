using SnipingFarmer.Script.Meta;
using UnityEngine;

namespace SnipingFarmer.Script.Scene
{
    public class RobyScene : AbstractScene
    {
        public void GoToStart()
        {
            // TODO
        }

        public void GoToGame()
        {
            // TODO
        }

        // TODO: 戻り値をRoomにしたい
        private GameObject EnterRoom(PlayerMeta player)
        {
            // TODO
            var obj = new GameObject();
            obj.AddComponent<Room>();
            return obj;
        }

        // TODO: 戻り値をPlayerMetaにしたい
        private GameObject CreatePlayer()
        {
            // TODO
            var obj = new GameObject();
            obj.AddComponent<PlayerMeta>();
            return obj;
        }
    }
}