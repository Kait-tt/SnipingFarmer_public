using SnipingFarmer.Script.Meta;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SnipingFarmer.Script.UI
{
    public class GameStart : MonoBehaviourBase
    {
        private Text gameStartTextComponent;

        [SerializeField] private string gameStartText = "GO!!!";
    
        public void Awake()
        {
            gameStartTextComponent = GetComponent<Text>();
        }
    
        public void Init(GameStartCounter gameStartCounter)
        {
            gameStartCounter.TimeCount.Subscribe(timeCount =>
            {
                if (timeCount > 0)
                {
                    ShowCountdown(timeCount);
                }
                else
                {
                    ShowGameStart();
                }
            }).AddTo(gameObject);
        }


        private void ShowCountdown(int second)
        {
            gameStartTextComponent.text = second.ToString();
        }

        private void ShowGameStart()
        {
            gameStartTextComponent.text = gameStartText;
            DelayDestroy(gameObject, 1f);
        }
    }
}


