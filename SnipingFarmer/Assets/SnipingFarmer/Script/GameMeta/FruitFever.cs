using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace SnipingFarmer.Script.GameMeta
{
    [RequireComponent(typeof(AudioSource))]

    public class FruitFever : MonoBehaviourBase
    {
        [SerializeField] private int durationSecond = 30;
        [SerializeField] private int[] raiseTimeSecondList;
        
        [SerializeField] private string[] fruitNames =
        {
            "Apple",
            "Banana",
            "Grape",
            "Melon",
            "Orange"
        };
        
        [SerializeField] private BoolReactiveProperty isEnabled = new BoolReactiveProperty(false);
        [SerializeField] private StringReactiveProperty goodFruitName = new StringReactiveProperty();
        [SerializeField] private FloatReactiveProperty remainedTimeSecond = new FloatReactiveProperty(0f);
        [SerializeField] private FloatReactiveProperty scoreRate = new FloatReactiveProperty(2f);

        private AudioSource feverAudio;

        private IDisposable timerDisposable;

        public ReadOnlyReactiveProperty<float> ScoreRate
        {
            get { return scoreRate.ToReadOnlyReactiveProperty(); }
        }

        public ReadOnlyReactiveProperty<bool> IsEnabled
        {
            get { return isEnabled.ToReadOnlyReactiveProperty(); }
        }

        public ReadOnlyReactiveProperty<float> RemainedTimeSecond
        {
            get { return remainedTimeSecond.ToReadOnlyReactiveProperty(); }
        }

        public ReadOnlyReactiveProperty<string> GoodFruit
        {
            get { return goodFruitName.ToReadOnlyReactiveProperty();  }
        }

        public void Start()
        {
            var gameTimerObj = GameObject.Find("GameTimer");
            if (gameTimerObj == null)
            {
                Debug.LogError("GameTimer GameObject is not found");
                return;
            }

            var gameTimer = gameTimerObj.GetComponent<GameTimer>();
            if (gameTimer == null)
            {
                Debug.LogError("GameTimer Component is not attached to GameTiemr GameObject");
                return;
            }

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

            var index = 0;

            gameTimer.ElapsedTimeSecond
                .Subscribe(time =>
                {
                    if (index < raiseTimeSecondList.Length && time >= raiseTimeSecondList[index])
                    {
                        ++index;
                        Raise();
                    }
                })
                .AddTo(gameObject);
            
            // SE
            feverAudio = GetComponent<AudioSource>();
            Assert.IsNotNull(feverAudio);
            
            var gameBGM = GameObject.Find("GameBGM").GetComponent<AudioSource>();
            Assert.IsNotNull(gameBGM);

            var feverStart = GameObject.Find("FeverStart").GetComponent<UI.FruitFever>();
            Assert.IsNotNull(feverStart);
            
            // Fever Start SE
            IsEnabled.Where(x => x == true).Subscribe(_ =>
            {
                float audioSize = feverAudio.clip.length;
                
                // show Fever Effect
                feverStart.ShowEffect();
                
                StartCoroutine(DurationBGMMute(gameBGM, audioSize));
                
                feverAudio.Play();
            }).AddTo(gameObject);
        }

        private void Raise()
        {
            var intervalSecond = 0.25f;

            if (timerDisposable != null)
            {
                timerDisposable.Dispose();
            }
            
            isEnabled.Value = true;
            SetRandomGoodFruit();
            
            timerDisposable = Observable
                .Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(intervalSecond))
                .Subscribe(time => {
                    remainedTimeSecond.Value = durationSecond - time * intervalSecond;
                })
                .AddTo(gameObject);
        }

        private void SetRandomGoodFruit()
        {
            if (fruitNames.Length == 0)
            {
                Debug.LogError("Length of Fruit Names is 0. Cannot set good fruit name.");
                return;
            }
            
            goodFruitName.Value = fruitNames[Random.Range(0, fruitNames.Length)];
        }

        private IEnumerator DurationBGMMute(AudioSource audioSource, float clipSize)
        {
            audioSource.mute = true;
            yield return new WaitForSeconds(clipSize);
            audioSource.mute = false;
        }
    }
}