using UniRx;
using UnityEngine;

namespace SnipingFarmer.Script.Meta
{
    public class PlayerMeta : MonoBehaviourBase
    {
        private ReactiveProperty<PlayerMetaState> state = new ReactiveProperty<PlayerMetaState>(PlayerMetaState.WAITING);

        public ReadOnlyReactiveProperty<PlayerMetaState> State
        {
            get { return state.ToReadOnlyReactiveProperty(); }
        }

        [SerializeField] public readonly IntReactiveProperty Score = new IntReactiveProperty(0);

        [SerializeField] public readonly BoolReactiveProperty IsVotedStopGame = new BoolReactiveProperty();

        public string Name { get; private set; }

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void Init(string name)
        {
            Name = name;
        }

        public void Ready()
        {
            // TODO
        }

        public void CancelReady()
        {
            // TODO
        }

        public void SetStartState()
        {
            // TODO
        }
    }
}