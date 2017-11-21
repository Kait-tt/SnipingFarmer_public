using SnipingFarmer.Script.Game;
using UniRx;
using UnityEngine;

namespace SnipingFarmer.Script.Util
{
    public class MockCaturable : MonoBehaviourBase, ICapturable
    {
        public IObservable<ICapturable> OnCaptured { get; set; }
        public void Capture(Bullet bullet)
        {
            Debug.Log("on capture");
        }
    }
}