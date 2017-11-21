using System.Collections;
using SnipingFarmer.Script.Meta;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;

namespace SnipingFarmer.Script.Scene
{
    public class ResultScene : AbstractScene
    {
        [SerializeField] private Button startsceneButton;
        [SerializeField] private string startSceneName = "StartScene";

        private Text scoreText;

        private PlayerMeta playerMeta;

        private AudioSource backAudio;
        
        void Awake()
        {
            backAudio = GameObject.Find("BackSe").GetComponent<AudioSource>();
            
            // GameSceneのFPSControllerにフォーカスがとられるため以下を設定
            Cursor.visible = true;
            Screen.lockCursor = false;

            startsceneButton.onClick.AsObservable()
                .Subscribe(_ =>
                {
                    backAudio.Play();
                    StartCoroutine(WaitForFinishSE(backAudio.clip.length));
                })
                .AddTo(gameObject);
        }

        void Start()
        {
            scoreText = GameObject.Find("ScoreText").GetComponent<Text>();

            playerMeta = FindObjectOfType<PlayerMeta>();

            if (playerMeta)
            {
                scoreText.text = playerMeta.Score.Value + " pts";
                Destroy(playerMeta.gameObject);
            }
            else
            {
                Debug.LogError("PlayerMeta is not found");
            }
        }

        private IEnumerator WaitForFinishSE(float time)
        {
            yield return new WaitForSeconds(time);
            
            SceneManager.LoadScene(startSceneName);
        }
    }
}