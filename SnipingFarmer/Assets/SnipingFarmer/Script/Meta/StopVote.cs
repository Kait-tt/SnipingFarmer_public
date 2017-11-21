using UniRx;
using UnityEngine;

namespace SnipingFarmer.Script.Meta
{
    public class StopVote : MonoBehaviourBase
    {
        public ReactiveProperty<int> VotedCount { get; private set; }

        public IObservable<bool> VotedAll { get; private set; }
    }
}