using UniRx;
using UnityEngine;

namespace SnipingFarmer.Script.Meta
{
    public class Room : MonoBehaviourBase
    {
        private ReactiveProperty<RoomState> state = new ReactiveProperty<RoomState>(RoomState.WAITING);        
        
        public ReadOnlyReactiveProperty<RoomState> State
        {
            get { return state.ToReadOnlyReactiveProperty(); }
        }

        public readonly ReactiveCollection<PlayerMeta> playerMetaList = new ReactiveCollection<PlayerMeta>();

        public readonly ReactiveProperty<PlayerMeta> myPlayerMeta = new ReactiveProperty<PlayerMeta>();

        public ReadOnlyReactiveProperty<bool> IsAllReady
        {
            // TODO
            get { return new ReactiveProperty<bool>().ToReadOnlyReactiveProperty(); }
        }

        public void AddPlayer(PlayerMeta playerMeta)
        {
            // TODO
        }

        public void RemovePlayer(PlayerMeta playerMeta)
        {
            // TODO
        }

        public void StartGame()
        {
            // TODO
        }
    }
}