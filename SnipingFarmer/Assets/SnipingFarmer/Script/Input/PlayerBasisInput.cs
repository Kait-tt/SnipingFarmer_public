using SnipingFarmer.Script.Game;
using UniRx;
using UnityEngine;
using UEInput = UnityEngine.Input;

namespace SnipingFarmer.Script.Input
{
    // Playerの歩行・視界操作は（とりあえず）FirstPersonControllerに任せる
    // マルチプレイ対応時に直すかも
    public class PlayerBasisInput : MonoBehaviourBase
    {
        public IObservable<long> ShootInputStream = Observable
            .EveryUpdate()
            .Where(_ => UEInput.GetMouseButtonDown(0));

        public IObservable<long> ScopeInputStream = Observable
            .EveryUpdate()
            .Where(_ => UEInput.GetMouseButtonDown(1));

        public void Awake()
        {
            InitGameSceneInput();
        }

        private void InitGameSceneInput()
        {
            var weapon = GetComponentInChildren<Weapon>();
            if (weapon == null)
            {
                Debug.Log("weapon is not found");
                return;
            }

            ScopeInputStream.Subscribe(_ => weapon.ToggleScoping()).AddTo(gameObject);
            ShootInputStream.Subscribe(_ => weapon.Shoot()).AddTo(gameObject);
        }
    }
}