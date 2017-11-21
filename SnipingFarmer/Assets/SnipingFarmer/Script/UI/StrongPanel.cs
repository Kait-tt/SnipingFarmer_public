using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SnipingFarmer.Script.UI
{
    [RequireComponent(typeof(Text))]
    public class StrongPanel : MonoBehaviourBase
    {
        private Text text;
        
        public void Awake()
        {
            text = GetComponent<Text>();
        }

        public void Init(GameMeta.FruitStrong fruitStrong)
        {
            fruitStrong.IsEnabled
                .CombineLatest(
                    fruitStrong.RemainedTimeSecond,
                    (isEnabled, _) => isEnabled
                )
                .Subscribe(_ =>
                {
                    if (fruitStrong.IsEnabled.Value)
                    {
                        text.enabled = true;
                        text.text = TextFormat((int)Math.Ceiling(fruitStrong.RemainedTimeSecond.Value));
                    }
                    else
                    {
                        text.enabled = false;
                    }
                })
                .AddTo(gameObject);
        }

        public string TextFormat(int remainedTimeSecond)
        {
            return string.Format("活性化中！（{0:00}秒）", remainedTimeSecond);
        }
    }
}

