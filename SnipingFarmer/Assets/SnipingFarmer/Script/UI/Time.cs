using System;
using System.Collections;
using SnipingFarmer.Script.GameMeta;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SnipingFarmer.Script.UI
{
    [RequireComponent(typeof(Text))]
    public class Time : MonoBehaviourBase
    {
        private Text text;

        private GameTimer gameTimer;

        [SerializeField] private Color harryUpTextColor;
        [SerializeField] private float flashDurationSecond = 0.3f; // falseになったままの時間

        private void Start()
        {
            text = GetComponent<Text>();
        }

        public void Init(GameTimer setGameTimer)
        {
            gameTimer = setGameTimer;

            gameTimer.ElapsedTimeSecond
                .Where(_ => text != null)
                .Subscribe(time =>
                {
                    var remainedTime = gameTimer.MaxTimeSecond - time;
                    text.text = TextFormat(Math.Max(0, (int)Math.Ceiling(remainedTime)));
                })
                .AddTo(gameObject);

            gameTimer.ElapsedTimeSecond
                .Select(time => (int) Math.Ceiling(gameTimer.MaxTimeSecond - time))
                .Distinct()
                .Where(time => time <= gameTimer.HarryTimeSecond && time > 0)
                .Subscribe(_ =>
                {
                    StartCoroutine(StartSecondFlash());

                })
                .AddTo(gameObject);
                

            gameTimer.OnHarryUp
                .Subscribe(_ =>
                {
                    text.color = harryUpTextColor;
                })
                .AddTo(gameObject);
        }

        private string TextFormat(int timeSecond)
        {
            return string.Format("のこり {0:000} 秒", timeSecond);
        }

        private IEnumerator StartSecondFlash()
        {
            text.enabled = false;
            
            yield return new WaitForSeconds(flashDurationSecond);

            text.enabled = true;
        }
    }
}