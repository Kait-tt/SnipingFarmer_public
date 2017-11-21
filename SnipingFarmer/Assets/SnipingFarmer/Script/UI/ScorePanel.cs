using System.Collections;
using SnipingFarmer.Script.Game;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace SnipingFarmer.Script.UI
{
    [RequireComponent(typeof(Text))]
    public class ScorePanel : MonoBehaviourBase
    {
        private Text scorePanelText;

        private GameManager gameManager;
        
        [SerializeField] int changeSize = 130;
        
        public void Awake()
        {
            scorePanelText = GetComponent<Text>();
        }

        public void Init(GameManager setGameManager)
        {
            gameManager = setGameManager;

            Assert.IsTrue(gameManager.playerList.Count > 0);
            var playerMeta = gameManager.playerList[0].Meta;
            Assert.IsNotNull(playerMeta);

            playerMeta.Score
                .Where(_ => scorePanelText != null)
                .Skip(1)
                .Subscribe(score =>
                {
                    scorePanelText.text = TextFormat(score);
                    int fontSize = scorePanelText.fontSize;
                    
                    var hash = new Hashtable  // 拡大アニメーション
                    {
                        {"from", fontSize},
                        {"to", changeSize},
                        {"time", 0.1f},
                        {"easyType", iTween.EaseType.easeInCubic},
                        {"onupdate", "FontUpper"},
                        {"onupdatetarget", gameObject},
                        {"oncomplete", "Complete"},
                        {"oncompletetarget", gameObject},
                        {"oncompleteparams", fontSize}
                    };
                    iTween.ValueTo(gameObject, hash);
                })
                .AddTo(gameObject);
        }

        public string TextFormat(int score)
        {
            return string.Format("{0:00000}", score);
        }

        private void Complete(int defaultSize)
        {
            var hash = new Hashtable  //縮小アニメーション
            {
                {"from", changeSize},
                {"to", defaultSize},
                {"time", 0.1f},
                {"easyType", iTween.EaseType.easeOutCubic},
                {"onupdate", "FontDowner"},
                {"onupdatetarget", gameObject},
            };
            iTween.ValueTo(gameObject, hash);
        }
        
        private void FontUpper(int fontSize)  //フォントサイズ拡大
        {
            scorePanelText.fontSize = fontSize;
        }

        private void FontDowner(int fontSize)　　//フォントサイズ縮小
        {
            scorePanelText.fontSize = fontSize;
        }
    }
}