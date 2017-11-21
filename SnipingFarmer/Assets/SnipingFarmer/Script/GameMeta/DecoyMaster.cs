using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace SnipingFarmer.Script.GameMeta
{
    public class DecoyMaster : MonoBehaviourBase
    {
        private ReactiveCollection<GameObject> decoyList = new ReactiveCollection<GameObject>();

        public IReadOnlyReactiveCollection<GameObject> DecoyList
        {
            get { return decoyList; }
        }

        public void AddDecoy(GameObject decoy)
        {
            decoyList.Add(decoy);

            decoy
                .OnDestroyAsObservable()
                .Subscribe(_ =>
                {
                    decoyList.Remove(decoy);
                })
                .AddTo(gameObject);
        }
    }
}