using UniRx;
using UnityEngine;

namespace SnipingFarmer.Script.Scene
{
    public class SceneMaster : MonoBehaviourBase
    {
        public ReactiveProperty<AbstractScene> CurrentScene { get; private set; }
    }
}