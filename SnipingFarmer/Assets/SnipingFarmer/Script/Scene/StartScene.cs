using System;
using System.Collections;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SnipingFarmer.Script.Scene
{
    public class StartScene : AbstractScene
    {
        [SerializeField] private string gameSceneName;

        [SerializeField] private float intervalSecondForFlushingAnyKeyImage1 = 1.5f;
        [SerializeField] private float intervalSecondForFlushingAnyKeyImage2 = 0.3f;

        private AudioSource gameStartAudio;

        private IDisposable anykeyDisposable;

        private Image anyKeyImage;

        public void Awake()
        {
            gameStartAudio = GameObject.Find("GameStartSe").GetComponent<AudioSource>();
            anyKeyImage = GameObject.Find("AnyKeyText").GetComponent<Image>();

            anykeyDisposable = this.UpdateAsObservable()
                .Where(_ => UnityEngine.Input.anyKey)
                .Subscribe(_ =>
                {
                    gameStartAudio.Play();
                    StartCoroutine(WaitForFinishSE(gameStartAudio.clip.length));
                    anykeyDisposable.Dispose();
                })
                .AddTo(gameObject);
        }

        public void Start()
        {
            StartCoroutine(FlushAnyKeyImage());
        }

        private IEnumerator FlushAnyKeyImage()
        {
            while (true)
            {
                yield return new WaitForSeconds(intervalSecondForFlushingAnyKeyImage1);

                anyKeyImage.enabled = false;
                
                yield return new WaitForSeconds(intervalSecondForFlushingAnyKeyImage2);
                
                anyKeyImage.enabled = true;
            }
        }

        private IEnumerator WaitForFinishSE(float time)
        {
            yield return new WaitForSeconds(time);
            
            SceneManager.LoadScene(gameSceneName);
        }
    }
}