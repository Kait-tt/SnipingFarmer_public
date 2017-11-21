using SnipingFarmer.Script.GameMeta;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SnipingFarmer.Script.UI
{
    public class GameEnd : MonoBehaviourBase
    {
        private Text gameEndTextComponent;
        
        [SerializeField] private string gameEndText = "おわり！";

        private GameTimer gameTimer;

        public void Start()
        {
            gameEndTextComponent = GetComponent<Text>();
        }
    
        public void Init(GameTimer gameTimer)
        {
            gameTimer.OnTimeUp.Subscribe(_ =>
            {
                gameEndTextComponent.text = gameEndText;
            }).AddTo(gameObject);
        }
    }
}
