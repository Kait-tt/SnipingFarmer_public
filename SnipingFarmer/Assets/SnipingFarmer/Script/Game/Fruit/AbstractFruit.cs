using System.Collections;
using SnipingFarmer.Script.GameMeta;
using SnipingFarmer.Script.Meta;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace SnipingFarmer.Script.Game.Fruit
{
    [RequireComponent(typeof(AudioSource))]
    public abstract class AbstractFruit : MonoBehaviourBase, ICapturable
    {
        [SerializeField] private FloatReactiveProperty spawnProbability = new FloatReactiveProperty(1f); 
        [SerializeField] private IntReactiveProperty defaultScorePoint = new IntReactiveProperty(10);
        [SerializeField] private Material strongMaterial;
        public const string DISPLAY_NAME = "none";

        private ParticleSystem effect; 
        private Material originalMaterial;
        private FruitStrong fruitStrong;
        private FruitFever fruitFever;
        
        private AudioSource captureAudio;

        [SerializeField] private float captureEffectUpLimit = 3f;
        [SerializeField] private float captureEffectUpDuration = 3f;

        public ReadOnlyReactiveProperty<float> SpawnProbability
        {
            get { return spawnProbability.ToReadOnlyReactiveProperty(); }
        }

        public ReadOnlyReactiveProperty<int> ScorePoint
        {
            get
            {
                // null check
                if (IsStrong == null || IsFever == null || fruitStrong == null || fruitFever == null)
                {
                    return defaultScorePoint.ToReadOnlyReactiveProperty();
                }
                return defaultScorePoint
                    .CombineLatest(
                        IsStrong,
                        IsFever,
                        fruitStrong.ScoreRate,
                        fruitFever.ScoreRate,
                        (point, strong, fever, strongRate, feverRate) =>
                        {
                            if (strong && fever) // フィーバーと活性化の両方
                            {
                                return (int)(point * strongRate * feverRate);
                            }
                            else if (fever) // フィーバー時
                            {
                                return (int)(point * feverRate);
                            }
                            else if (strong) // 活性化時
                            {
                                return (int)(point * strongRate);
                            }
                            else // 通常時
                            {
                                return point;
                            }
                        }
                    )
                    .ToReadOnlyReactiveProperty();
            }
        }

        public ReadOnlyReactiveProperty<bool> IsStrong
        {
            get
            {
                return new ReactiveProperty<bool>(fruitStrong.IsEnabled).ToReadOnlyReactiveProperty();
            }
        }

        public ReadOnlyReactiveProperty<bool> IsFever
        {
            get
            {
                // GoodFruitのみIsFeverがtrueになるようにする
                return fruitFever.IsEnabled.CombineLatest(fruitFever.GoodFruit, (isEnabled, goodFruit) =>
                    isEnabled && goodFruit == GetType().Name
                ).ToReadOnlyReactiveProperty();
            }
        }
        
        public void Awake()
        {
            var fruitStrongObj = GameObject.Find("FruitStrong");
            originalMaterial = GetComponent<Renderer>().material;
            
            if (fruitStrongObj == null)
            {
                Debug.LogError("FruitStrong GameObject is not found");
            }
            else 
            {
                fruitStrong = fruitStrongObj.GetComponent<FruitStrong>();
                IsStrong.Subscribe(isStrong => {
                     if (isStrong == true)
                     {
                         GetComponent<Renderer>().material = strongMaterial;
                     }
                     else
                     {
                         GetComponent<Renderer>().material = originalMaterial;
                     }
                }).AddTo(gameObject);
            }
            var fruitFeverObj = GameObject.Find("FruitFever");
            effect = GetComponentInChildren<ParticleSystem>();
              
            if (fruitFeverObj == null)
            {
                Debug.LogError("FruitFever GameObject is not found");
            }
            else
            {
                fruitFever = fruitFeverObj.GetComponent<FruitFever>();
                IsFever.Subscribe(isFever =>
                {
                    if (isFever == true)
                    {
                        effect.Play();
                    }
                    else
                    {
                        effect.Stop();
                    }
                }).AddTo(gameObject);
            }
            
            captureAudio = GetComponent<AudioSource>();
            Assert.IsNotNull(captureAudio);
        }

        private IEnumerator DelayCapture(PlayerMeta playerMeta)
        {
            playerMeta.Score.Value += ScorePoint.Value;
            
            // Play Capture SE
            captureAudio.Play();
            
            var mat = GetComponent<Renderer>().material;
            if (mat.HasProperty("_EmissionColor"))
            {
                mat.SetColor("_EmissionColor", Color.white);
            }

            var rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;

            var agent = GetComponent<NavMeshAgent>();
            agent.isStopped = true;
            
            iTween.MoveBy(gameObject, iTween.Hash(
                "y", captureEffectUpLimit,
                "time", captureEffectUpDuration,
                "EaseType", iTween.EaseType.easeInOutCubic
            ));
            
            yield return new WaitForSeconds(captureEffectUpDuration + 0.2f);
            
            DestroyIfExists(gameObject);
        }

        public void Capture(Bullet bullet)
        {
            var rb = GetComponent<Rigidbody>();
            rb.detectCollisions = false;
            
            StartCoroutine(DelayCapture(bullet.ShotPlayer.Meta));
        }
    }
}