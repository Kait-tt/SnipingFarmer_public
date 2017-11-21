using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using SnipingFarmer.Script.Meta;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using UnityStandardAssets.Characters.FirstPerson;

namespace SnipingFarmer.Script.Game
{
    [RequireComponent(typeof(Weapon))]
    [RequireComponent(typeof(AudioSource))]
    public class Player : MonoBehaviourBase, ICapturable
    {
        [SerializeField] private FloatReactiveProperty unscopingWalkSpeed = new FloatReactiveProperty(5f);
        [SerializeField] private FloatReactiveProperty scopingWalkSpeed = new FloatReactiveProperty(0.5f);
        
        [SerializeField] private FloatReactiveProperty unscopingCameraSpeed = new FloatReactiveProperty(2f);
        [SerializeField] private FloatReactiveProperty scopingCameraSpeed = new FloatReactiveProperty(0.1f);
        
        [SerializeField] private FloatReactiveProperty scopingFieldOfView = new FloatReactiveProperty(3f);
        [SerializeField] private FloatReactiveProperty unscopingFieldOfView = new FloatReactiveProperty(60f);

        [SerializeField] private float scopeZoomDuration = 0.15f;

        private Weapon weapon;
        private AudioSource warpAudio;
        
        public ReadOnlyReactiveProperty<float> WalkSpeed
        {
            get
            {
                return weapon.IsScoping
                    .CombineLatest(
                        scopingWalkSpeed,
                        unscopingWalkSpeed,
                        (isScoping, yes, no) => isScoping ? yes : no
                    )
                    .ToReadOnlyReactiveProperty();
            }
        }
        
        public ReadOnlyReactiveProperty<float> CameraSpeed
        {
            get
            {
                return weapon.IsScoping
                    .CombineLatest(
                        scopingCameraSpeed,
                        unscopingCameraSpeed,
                        (isScoping, yes, no) => isScoping ? yes : no
                    )
                    .ToReadOnlyReactiveProperty();
            }
        }
        
        public ReadOnlyReactiveProperty<float> FieldOfView
        {
            get
            {
                return weapon.IsScoping
                    .CombineLatest(
                        scopingFieldOfView,
                        unscopingFieldOfView,
                        (isScoping, yes, no) => isScoping ? yes : no
                    )
                    .ToReadOnlyReactiveProperty();
            }
        }
        
        public PlayerMeta Meta { get; private set; }
        
        public void Init(PlayerMeta meta)
        {
            Meta = meta;
        }

        // Awake or Start 時に適当に座標見て設定すると良いかも
        public ReactiveProperty<SnipingArea> CurrentArea = new ReactiveProperty<SnipingArea>();

        public ReadOnlyReactiveProperty<bool> CanMove
        {
            // TODO
            get { return new ReactiveProperty<bool>(true).ToReadOnlyReactiveProperty(); }
        }

        public BoolReactiveProperty IsWarping = new BoolReactiveProperty(false);

        public void Awake()
        {
            weapon = GetComponent<Weapon>();
            
            // テスト時でも動くように適当なMetaを入れておく
            if (Meta == null)
            {
                var fakeMeta = new GameObject("PlayerMeta").AddComponent<PlayerMeta>();
                fakeMeta.Init("FakePlayer");
                Init(fakeMeta);
            }
        }

        public void Start()
        {
            InitScopingControl();
            
            warpAudio = GetComponents<AudioSource>()
                .Where(x => x.clip)
                .FirstOrDefault(x => Regex.IsMatch(x.clip.name, "warp", RegexOptions.IgnoreCase));
            Assert.IsNotNull(warpAudio);
            
            IsWarping.Where(x => x == true).Subscribe(_ =>
            {
                warpAudio.Play();
            }).AddTo(gameObject);
        }

        public void Capture(Bullet bullet)
        {
            // TODO
        }

        public void InitScopingControl()
        {
            var parent = transform.parent;
            if (parent == null)
            {
                Debug.LogError("Parent GameObject is not found");
                return;
            }

            var camera = transform.parent.GetComponentInChildren<Camera>();
            if (camera == null)
            {
                Debug.LogError("Camera is not found");
                return;
            }
            var firstPersonController = transform.parent.GetComponent<FirstPersonController>();
            if (firstPersonController == null)
            {
                Debug.LogError("FirstPersonController is not found");
                return;
            }

            WalkSpeed.Subscribe(walkSpeed =>
            {
                firstPersonController.m_WalkSpeed = walkSpeed;
            }).AddTo(gameObject);

            FieldOfView.Subscribe(fieldOfView =>
            {
                var beforeFieldOfView = camera.fieldOfView;

                var hash = new Hashtable
                {
                    {"from", beforeFieldOfView},
                    {"to", fieldOfView},
                    {"time", scopeZoomDuration},
                    {"easyType", iTween.EaseType.easeOutQuart},
                    {"onupdate", "ChangeCameraView"},
                    {"onupdatetarget", gameObject}
                };
                iTween.ValueTo(gameObject, hash);
            }).AddTo(gameObject);

            CameraSpeed.Subscribe(cameraSpeed =>
            {
                firstPersonController.m_MouseLook.XSensitivity = cameraSpeed;
                firstPersonController.m_MouseLook.YSensitivity = cameraSpeed;
            }).AddTo(gameObject);
        }
        
        private void ChangeCameraView(float fieldOfView)
        {
            var camera = transform.parent.GetComponentInChildren<Camera>();
            if (camera == null)
            {
                Debug.LogError("Camera is not found");
                return;
            }
            
            camera.fieldOfView = fieldOfView;
        }
    }
}
