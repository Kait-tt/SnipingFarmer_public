using System.Collections;
using UniRx;
using UnityEngine;

namespace SnipingFarmer.Script.Meta
{
    public class GameStartCounter : MonoBehaviourBase
    {

        [SerializeField] private int countdownSecond = 3;

        private ReactiveProperty<int> timeCount = new ReactiveProperty<int>();
        private ReactiveProperty<bool> countdownIsFinish = new ReactiveProperty<bool>(false);

        public ReadOnlyReactiveProperty<bool> CountdownIsFinish
        {
            get { return countdownIsFinish.ToReadOnlyReactiveProperty(); }
        }
        
        public ReadOnlyReactiveProperty<int> TimeCount
        {
            get { return timeCount.ToReadOnlyReactiveProperty(); }
        }
        
        public void Init()
        {
            timeCount.Value = countdownSecond;
        }

        public void StartCountdown()
        {
            StartCoroutine(Countdown());
        }

        private IEnumerator Countdown()
        {
            timeCount.Value = countdownSecond;
            while (timeCount.Value > 0)
            {
                yield return new WaitForSeconds(1);
                --timeCount.Value;
            }
            countdownIsFinish.Value = true;
        }
    }
}
