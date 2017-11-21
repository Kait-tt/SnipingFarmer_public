using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

namespace SnipingFarmer.Script.Game
{
    [RequireComponent(typeof(AudioSource))]
    public class Weapon : MonoBehaviourBase
    {
        /// <summary>
        /// リロード時間（秒）
        /// </summary>
        [SerializeField] private float reloadDurationSecond = 3;
        
        [SerializeField] private GameObject bullet;
        
        /// <summary>
        /// 打ち出す強さ
        /// </summary>
        [SerializeField] private float shootPower = 10000000;
        
        /// <summary>
        /// 打ち出す位置のoffset
        /// </summary>
        [SerializeField] private Vector3 shootPositionOffset = new Vector3(0, 0, 1f);

        private Player player;
        
        private Camera playerCamera;

        private AudioSource shootAudio;
        private AudioSource reloadAudio;
        

        [SerializeField] private BoolReactiveProperty isScoping = new BoolReactiveProperty(false);

        [SerializeField] private BoolReactiveProperty isReloading = new BoolReactiveProperty(false);

        public ReadOnlyReactiveProperty<bool> IsScoping
        {
            get { return isScoping.ToReadOnlyReactiveProperty(); }
        }

        public ReadOnlyReactiveProperty<bool> IsReloading
        {
            get { return isReloading.ToReadOnlyReactiveProperty(); }
        }

        public float ReloadDurationSecond
        {
            get { return reloadDurationSecond; }
        }

        /// <summary>
        /// 非リロード中に撃てる
        /// </summary>
        public ReadOnlyReactiveProperty<bool> CanShoot
        {
            get
            {
                return IsScoping 
                    .CombineLatest(
                        IsReloading,
                        (scoping, reloading) => !reloading)
                    .ToReadOnlyReactiveProperty();
            }
        }

        public void Start()
        {
            player = GetComponent<Player>();

            if (transform.parent == null)
            {
                Debug.LogError("parent is not found");
                return;
            }
            
            playerCamera = transform.parent.GetComponentInChildren<Camera>();

            shootAudio = GetComponents<AudioSource>()
                .Where(x => x.clip)
                .FirstOrDefault(x => Regex.IsMatch(x.clip.name, "shoo?t", RegexOptions.IgnoreCase));
            
            Assert.IsNotNull(shootAudio);
            
            reloadAudio = GetComponents<AudioSource>()
                .Where(x => x.clip)
                .FirstOrDefault(x => Regex.IsMatch(x.clip.name, "reload", RegexOptions.IgnoreCase));
            
            Assert.IsNotNull(reloadAudio);

            isReloading.Where(x => x == true).Subscribe(_ =>
            {
                // Reload SE Play
                reloadAudio.Play();
            }).AddTo(gameObject);
        }

        /// <summary>
        /// 発射可能なら弾を生成して返す。
        /// 発射不可能ならnullを返す。
        /// </summary>
        /// <returns>生成したGameObject</returns>
        public GameObject Shoot()
        {
            if (CanShoot.Value)
            {
                // gun shoot SE Play
                shootAudio.Play();
                
                StartCoroutine(Reload());
                
                var obj = Instantiate(
                    bullet,
                    transform.position,
                    playerCamera.transform.rotation
                );
                obj.transform.Translate(shootPositionOffset);

                var shotBullet = obj.GetComponent<Bullet>();
                shotBullet.Init(player, player.CurrentArea.Value);
                
                var rg = shotBullet.GetComponent<Rigidbody>();
                rg.AddForce(obj.transform.forward * shootPower);
                
                return obj;
            }

            return null;
        }

        public void Scope()
        {
            isScoping.Value = true;
        }

        public void Unscope()
        {
            isScoping.Value = false;
        }

        /// <summary>
        /// Scope/Unscopeの切り替え
        /// </summary>
        public void ToggleScoping()
        {
            if (IsScoping.Value)
            {
                Unscope();
            }
            else
            {
                Scope();
            }
        }

        private IEnumerator Reload()
        {
            isReloading.Value = true;
            yield return new WaitForSeconds(reloadDurationSecond);
            isReloading.Value = false;
        }
    }
}