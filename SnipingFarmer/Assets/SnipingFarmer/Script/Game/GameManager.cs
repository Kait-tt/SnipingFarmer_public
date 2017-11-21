using UniRx;
using UnityEngine;

namespace SnipingFarmer.Script.Game
{
    public class GameManager : MonoBehaviourBase
    {
        public readonly ReactiveCollection<Player> playerList = new ReactiveCollection<Player>();
        
        public ReactiveProperty<bool> IsPlaying { get; private set; }

        public Player MyPlayer { get; private set; }

        public void StartGame()
        {
            // TODO
        }
    }
}