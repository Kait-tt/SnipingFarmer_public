using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SnipingFarmer.Script.UI
{
    [RequireComponent(typeof(Text))]
    public class FeverPanel : MonoBehaviourBase
    {
        private Text text;

        private readonly Hashtable fruitNameToDisplayName = new Hashtable
        {
            {"Apple", "リンゴ"},
            {"Banana", "バナナ"},
            {"Grape", "ブドウ"},
            {"Melon", "イチゴ"},
            {"Orange", "ミカン"}
        };
        
        public void Awake()
        {
            text = GetComponent<Text>();
        }

        public void Init(GameMeta.FruitFever fruitFever)
        {
            fruitFever.IsEnabled
                .CombineLatest(
                    fruitFever.RemainedTimeSecond,
                    (isEnabled, _) => isEnabled
                )
                .Subscribe(_ =>
                {
                    if (fruitFever.IsEnabled.Value)
                    {
                        text.enabled = true;
                        text.text = TextFormat(
                            fruitFever.GoodFruit.Value,
                            (int)Math.Ceiling(fruitFever.RemainedTimeSecond.Value)
                        );
                    }
                    else
                    {
                        text.enabled = false;
                    }
                })
                .AddTo(gameObject);
        }

        public string TextFormat(string fruitName, int remainedTimeSecond)
        {
            return string.Format("フィーバー！ {0}（{1:00}秒）",
                fruitNameToDisplayName[fruitName],
                remainedTimeSecond);
        }
    }
}

