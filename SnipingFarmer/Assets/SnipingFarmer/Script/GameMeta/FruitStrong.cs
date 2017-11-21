using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

namespace SnipingFarmer.Script.GameMeta
{
    [RequireComponent(typeof(AudioSource))]
    public class FruitStrong : MonoBehaviourBase
    {

        [SerializeField] private int durationSecond = 30;
        [SerializeField] private BoolReactiveProperty isEnabled = new BoolReactiveProperty(false);
        [SerializeField] private FloatReactiveProperty remainedTimeSecond = new FloatReactiveProperty(0f);
        [SerializeField] private FloatReactiveProperty scoreRate = new FloatReactiveProperty(1.5f);
        [SerializeField] private FloatReactiveProperty speedRate = new FloatReactiveProperty(2f);

        private AudioSource strongAudio;

        private IDisposable timerDisposable;

        public ReadOnlyReactiveProperty<bool> IsEnabled
        {
            get { return isEnabled.ToReadOnlyReactiveProperty(); }
        }

        public ReadOnlyReactiveProperty<float> RemainedTimeSecond
        {
            get { return remainedTimeSecond.ToReadOnlyReactiveProperty(); }
        }
        
        public ReadOnlyReactiveProperty<float> ScoreRate
        {
            get { return scoreRate.ToReadOnlyReactiveProperty(); }
        }

        public ReadOnlyReactiveProperty<float> SpeedRate
        {
            get { return speedRate.ToReadOnlyReactiveProperty(); }
        }

        public void Raise()
        {
            var intervalSecond = 0.25f;

            if (timerDisposable != null)
            {
                timerDisposable.Dispose();
            }

            if (isEnabled.Value)
            {
                // SE鳴らすために一度falseにする
                isEnabled.Value = false;
            }
            
            isEnabled.Value = true;
            
            timerDisposable = Observable
                .Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(intervalSecond))
                .Subscribe(time => {
                    remainedTimeSecond.Value = durationSecond - time * intervalSecond;
                })
                .AddTo(gameObject);
        }

        public void Start()
        {
            strongAudio = GetComponent<AudioSource>();
            Assert.IsNotNull(strongAudio);
            var gameBGM = GameObject.Find("GameBGM").GetComponent<AudioSource>();
            Assert.IsNotNull(gameBGM);

            isEnabled.Where(x => x == true).Subscribe(_ =>
            {
                float audioSize = strongAudio.clip.length;
                StartCoroutine(DurationBGMMute(gameBGM, audioSize));
                strongAudio.Play();
            });
            
            remainedTimeSecond
                .Where(time => remainedTimeSecond.Value <= 0)
                .Subscribe(_ =>
                {
                    isEnabled.Value = false;

                    if (timerDisposable != null)
                    {
                        timerDisposable.Dispose();
                    }
                })
                .AddTo(gameObject);
        }
        
        private IEnumerator DurationBGMMute(AudioSource audioSource, float clipSize)
        {
            audioSource.mute = true;
            yield return new WaitForSeconds(clipSize);
            audioSource.mute = false;
        }
    }
}