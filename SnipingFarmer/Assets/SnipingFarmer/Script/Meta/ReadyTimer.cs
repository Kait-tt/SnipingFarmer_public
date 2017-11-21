using UniRx;
using UnityEngine;

namespace SnipingFarmer.Script.Meta
{
    public class ReadyTimer : MonoBehaviourBase
    {
        [SerializeField] private IntReactiveProperty maxTimeSecond = new IntReactiveProperty(5);
        [SerializeField] private BoolReactiveProperty isRunning = new BoolReactiveProperty(false);
        [SerializeField] private FloatReactiveProperty elapsedTimeSecond = new FloatReactiveProperty(0f);

        public ReadOnlyReactiveProperty<int> MaxTimeSecond
        {
            get { return maxTimeSecond.ToReadOnlyReactiveProperty(); }
        }

        public ReadOnlyReactiveProperty<bool> IsRunning
        {
            get { return isRunning.ToReadOnlyReactiveProperty(); }
        }

        public ReadOnlyReactiveProperty<float> ElapsedTimeSecond
        {
            get { return elapsedTimeSecond.ToReadOnlyReactiveProperty(); }
        }

        public IObservable<bool> OnTimeUp { get; private set; }

        public void StartTimer()
        {
            // TODO
        }

        public void StopTimer()
        {
            // TODO
        }
    }
}