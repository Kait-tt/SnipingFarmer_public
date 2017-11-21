using System.Collections;
using SnipingFarmer.Script.Game;
using SnipingFarmer.Script.Game.Fruit;
using SnipingFarmer.Script.Game.Item;
using SnipingFarmer.Script.GameMeta;
using SnipingFarmer.Script.Meta;
using SnipingFarmer.Script.UI;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using FruitFever = SnipingFarmer.Script.GameMeta.FruitFever;
using Time = SnipingFarmer.Script.UI.Time;

namespace SnipingFarmer.Script.Scene
{
    public class GameScene : AbstractScene
    {
        [SerializeField] private int transResultSceneSeconds = 3;
        [SerializeField] private string resultSceneName;

        public void Start()
        {
            Init();
        }

        public void Init()
        {
            var manager = GameObject.Find("Manager");
            if (manager == null)
            {
                Debug.LogError("Manager GameObject is not found");
                return;
            }
            
            var ui = GameObject.Find("UI");
            if (ui == null)
            {
                Debug.LogError("UI GameObject is not found");
                return;
            }

            var gameManager = manager.GetComponentInChildren<GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("GameManager is not found in Manager");
                return;
            }

            var gameTimer = manager.GetComponentInChildren<GameTimer>();
            if (gameTimer == null)
            {
                Debug.LogError("GameTimer Component is not found in Manager");
                return;
            }

            var fruitFever = manager.GetComponentInChildren<FruitFever>();
            if (fruitFever == null)
            {
                Debug.LogError("FruitFever Component is not found in Manager");
                return;
            }
            
            var fruitStrong = manager.GetComponentInChildren<FruitStrong>();
            if (fruitStrong == null)
            {
                Debug.LogError("FruitStrong Component is not found in Manager");
                return;
            }
            
            var fruitSpawner = manager.GetComponentInChildren<FruitSpawner>();
            if (fruitSpawner == null)
            {
                Debug.LogError("FruitSpawner Component is not found in Manager");
                return;
            }
            
            var itemSpawner = manager.GetComponentInChildren<ItemSpawner>();
            if (itemSpawner == null)
            {
                Debug.LogError("ItemSpawner Component is not found in Manager");
                return;
            }

            var uiTimer = ui.GetComponentInChildren<Time>();
            if (uiTimer == null)
            {
                Debug.LogError("Time Component is not found in UI");
                return;
            }

            var scorePanel = ui.GetComponentInChildren<ScorePanel>();
            if (scorePanel == null)
            {
                Debug.LogError("ScorePanel is not found in UI");
                return;
            }

            var feverPanel = ui.GetComponentInChildren<FeverPanel>();
            Assert.IsNotNull(feverPanel);

            var strongPanel = ui.GetComponentInChildren<StrongPanel>();
            Assert.IsNotNull(strongPanel);

            var fruitFeverUI = ui.GetComponentInChildren<UI.FruitFever>();
            Assert.IsNotNull(fruitFeverUI);

            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj == null)
            {
                Debug.LogError("GameObject attached Player tag is not found");
                return;
            }

            var player = playerObj.GetComponent<Player>();
            if (player == null)
            {
                Debug.LogError("Player Component is not attached to Player GameObject");
                return;
            }
            
            // init
            gameManager.playerList.Add(player);
            uiTimer.Init(gameTimer);
            scorePanel.Init(gameManager);
            feverPanel.Init(fruitFever);
            strongPanel.Init(fruitStrong);
            
            // start
            var gameStartCounter = manager.GetComponentInChildren<GameStartCounter>();
            if (gameStartCounter == null)
            {
                Debug.LogError("GameStartCounter is not found");
                return;
            }
            var gameStart = ui.GetComponentInChildren<GameStart>();
            if (gameStart == null)
            {
                Debug.LogError("GameStart is not found");
                return;
            }

            var gameEnd = ui.GetComponentInChildren<GameEnd>();
            if (gameEnd == null)
            {
                Debug.LogError("GameEnd is not found");
                return;
            }
            
            // ゲーム開始時のスタート処理
            gameStartCounter.Init();
            gameStart.Init(gameStartCounter);
            gameEnd.Init(gameTimer);
            
            // カウントダウン開始
            gameStartCounter.StartCountdown();
            
            // ゲーム開始から3秒後にspawnerやTimerを起動する
            gameStartCounter.CountdownIsFinish.Where(x => x == true).Subscribe(_ =>
            {
                gameTimer.StartTimer();
                itemSpawner.StartSpawnLoop();
                fruitSpawner.StartSpawnLoop();
            }).AddTo(gameObject);
            
            // ゲーム終了検知のため
            gameTimer.OnTimeUp.Subscribe(_ =>
            {
                GoToResult();
            }).AddTo(gameObject);
        }
        
        public void GoToRoby()
        {
            // TODO
        }

        public void GoToResult()
        {
            StartCoroutine(DelayTransitionResultScene());
        }

        public void StopGame()
        {
            // TODO
        }
        
        private IEnumerator DelayTransitionResultScene()
        {
            yield return new WaitForSeconds(transResultSceneSeconds);
            
            SceneManager.LoadScene(resultSceneName);
        }      

    }
}