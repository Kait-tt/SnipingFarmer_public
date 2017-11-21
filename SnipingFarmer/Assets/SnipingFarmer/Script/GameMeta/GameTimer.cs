using UnityEngine.Assertions;
using UniRx;
using UnityEngine;

namespace SnipingFarmer.Script.GameMeta
{
    [RequireComponent(typeof(AudioSource))]
    public class GameTimer : MonoBehaviourBase
    {
        private FloatReactiveProperty elapsedTimeSecond = new FloatReactiveProperty(0f);
        [SerializeField] private int maxTimeSecond = 300;
        [SerializeField] private int harryTimeSecond = 60;

        [SerializeField] private AudioClip gameEndSE;
        [SerializeField] private AudioClip gameEndVoice;
        [SerializeField] private AudioClip countdownSE;    
        [SerializeField] private AudioClip countdownVoice;

        private AudioSource seAudio;
        private AudioSource voiceAudio;

        [SerializeField] private float countdownStartPosition = 6.4f;

        [SerializeField] private float harryUpBGMPick = 1.2f;
        

        /// <summary>
        /// 経過時間
        /// </summary>
        public ReadOnlyReactiveProperty<float> ElapsedTimeSecond 
        {
            get { return elapsedTimeSecond.ToReadOnlyReactiveProperty(); }
        }

        public int MaxTimeSecond
        {
            get { return maxTimeSecond; }
        }

        public int HarryTimeSecond
        {
            get { return harryTimeSecond; }
        }

        public IObservable<bool> OnTimeUp
        {
            get
            {   
                return elapsedTimeSecond
                    .First(time => time >= maxTimeSecond)
                    .Select(_ => true);
            }
        }

        public IObservable<bool> OnHarryUp
        {
            get
            {
                return ElapsedTimeSecond
                    .First(time => maxTimeSecond- (int)time <= harryTimeSecond)
                    .Select(_ => true);
            }
        }

        public bool timerIsRunning = false;    // Timerが動いているかどうか

        public void StartTimer()
        {
            elapsedTimeSecond.Value = 0;
            timerIsRunning = true;

            OnTimeUp.Subscribe(_ =>
            {
                // ゲーム終了時
                seAudio.clip = gameEndSE;
                voiceAudio.clip = gameEndVoice;
                
                // カウントダウン時に再生位置をいじったためここでも調整
                seAudio.time = 0f;
                voiceAudio.time = 0f;

                seAudio.Play();
                voiceAudio.Play();
            }).AddTo(gameObject);

            var gameBGM = GameObject.Find("GameBGM").GetComponent<AudioSource>();
            Assert.IsNotNull(gameBGM);
            
            OnHarryUp.Subscribe(_ =>
            {
                gameBGM.pitch = harryUpBGMPick;
            }).AddTo(gameObject);
        }

        public void StopTimer()
        {
            timerIsRunning = false;
        }

        void Update()
        {
            if (timerIsRunning)
            {
                elapsedTimeSecond.Value += Time.deltaTime;
            }
        }

        public void Awake()
        {
            seAudio = GetComponents<AudioSource>()[0];
            voiceAudio = GetComponents<AudioSource>()[1];
            Assert.IsNotNull(seAudio);
            Assert.IsNotNull(voiceAudio);
            
            // ゲーム開始時カウントダウンSE
            seAudio.clip = countdownSE;
            voiceAudio.clip = countdownVoice;
            
            // 開始位置の調整
            seAudio.time = countdownStartPosition;
            voiceAudio.time = countdownStartPosition;
            
            seAudio.Play();
            voiceAudio.Play();
        }
        
        
    }
}