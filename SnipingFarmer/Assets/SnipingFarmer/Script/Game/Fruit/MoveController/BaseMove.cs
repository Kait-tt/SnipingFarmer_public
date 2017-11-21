using System.Linq;
using SnipingFarmer.Script.Game.Fruit.MovePoint;
using SnipingFarmer.Script.GameMeta;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;

namespace SnipingFarmer.Script.Game.Fruit.MoveController
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class BaseMove : MonoBehaviourBase
    {
        [SerializeField] protected FloatReactiveProperty defaultSpeed = new FloatReactiveProperty(10f);
        
        protected Vector3 targetMovePoint;

        /// <summary>
        /// 到着判定に用いる誤差許容値
        /// </summary>
        [SerializeField] protected FloatReactiveProperty arrivalEps = new FloatReactiveProperty(5f);

        protected NavMeshAgent agent;

        private FruitStrong fruitStrong;

        public ReadOnlyReactiveProperty<float> Speed
        {
            get
            {
                // null check
                if (IsStrong == null || fruitStrong == null)
                {
                    return defaultSpeed.ToReadOnlyReactiveProperty();
                }
                
                return new ReactiveProperty<float>(defaultSpeed)
                    .CombineLatest(
                        IsStrong,
                        fruitStrong.SpeedRate,
                        (speed, strong, speedRate) =>
                        {
                            if (strong)
                            {
                                return speed * speedRate;
                            }
                            else
                            {
                                return speed;
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
        
        private ReactiveProperty<bool> isMovingToDecoy = new ReactiveProperty<bool>(false);

        private DecoyMaster decoyMaster;

        public IObservable<bool> OnArrival
        {
            get
            {
                return this.UpdateAsObservable()
                    .Select(_ => IsArrived())
                    .DistinctUntilChanged()
                    .Where(isArrived => isArrived);
            }
        }

        public void Awake()
        {
            agent = GetComponent<NavMeshAgent>();

            var fruitStrongObj = GameObject.Find("FruitStrong");
            if (fruitStrongObj == null)
            {
                Debug.LogError("FruitStrong GameObject is not found");
            }
            else
            {
                fruitStrong = fruitStrongObj.GetComponent<FruitStrong>();
            }

            var decoyMasterObj = GameObject.Find("DecoyMaster");
            if (decoyMasterObj == null)
            {
                Debug.LogError("DecoyMaster GameObject is not found.");
            }
            else
            {
                decoyMaster = decoyMasterObj.GetComponent<DecoyMaster>();
                if (decoyMaster == null)
                {
                    Debug.LogError("DecoyMaster Component is not attached to DecoyMaster GameObject.");
                }
            }
        }

        public void Start()
        {
            // NavMeshAgentの移動設定の同期
            Speed.Subscribe(s =>
            {
                agent.speed = s;
                agent.acceleration = s; // とりあえずspeedと同じ値にしておく
            }).AddTo(gameObject);
                
            Move();
            
            // 到着したら次の目的地へ向かう
            // Decoyに向かってる時はそのままにする
            OnArrival
                .CombineLatest(
                    isMovingToDecoy,
                    (arrival, movingToDecoy) => arrival && !movingToDecoy
                )
                .Where(_ => _)
                .Subscribe(_ => Move())
                .AddTo(gameObject);
            
            // Decoyが周囲にあればそこに向かう
            // 既にDecoyに向かっていれば目的地を変更しない
            if (decoyMaster != null)
            {
                this.UpdateAsObservable()
                    .Where(_ => !isMovingToDecoy.Value)
                    .Where(_ => decoyMaster.DecoyList.Count > 0)
                    .Select(_ => decoyMaster.DecoyList
                        .Where(decoy =>
                        {
                            var p = decoy.transform.position;
                            var q = transform.position;
                            var d = decoy.GetComponent<Decoy>().VisibleDistance.Value;
                            p.y = 0; // yは無視
                            q.y = 0;
                            return (p - q).magnitude < d;
                        })
                        .FirstOrDefault())
                    .Where(decoy => decoy != null)
                    .Subscribe(decoy =>
                    {
                        isMovingToDecoy.Value = true;
                        MoveTo(decoy.transform.position);

                        decoy.OnDestroyAsObservable()
                            .Subscribe(_ =>
                            {
                                isMovingToDecoy.Value = false;
                            })
                            .AddTo(gameObject);
                    })
                    .AddTo(gameObject);
            }
        }

        public void Move()
        {
            MoveTo(NextMovePoint());
        }

        public void MoveTo(Vector3 target)
        {
            targetMovePoint = target;
            agent.SetDestination(targetMovePoint);
        }

        private Vector3 NextMovePoint()
        {
            var dests = GameObject
                .FindGameObjectsWithTag("MovePoint")
                .ToList()
                .Where(point => point.GetComponent<Decoy>() == null) // Decoyは弾く
                .Select(point => point.transform.position)
                .Where(position => position != targetMovePoint) // 同じ座標は弾く
                .Where(position => (position - transform.position).magnitude > arrivalEps.Value * 3f) // 近いのは弾く
                .ToList();

            if (dests.Count == 0)
            {
                Debug.LogError("Next move point is not found. Instead, set (0, 0, 0).");
                return Vector3.zero;
            }

            return dests[Random.Range(0, dests.Count)];
        }

        protected bool IsArrived()
        {
            var p = targetMovePoint;
            var q = transform.position;

            // y座標は無視する
            p.y = 0;
            q.y = 0;
            
            return (p - q).magnitude <= arrivalEps.Value;
        }
    }
}