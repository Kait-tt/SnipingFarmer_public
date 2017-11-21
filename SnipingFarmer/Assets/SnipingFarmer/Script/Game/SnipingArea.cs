using UniRx;
using UnityEngine;

namespace SnipingFarmer.Script.Game
{
    public class SnipingArea : MonoBehaviourBase
    {
        [SerializeField] private FloatReactiveProperty windSpeed = new FloatReactiveProperty(0f);
        [SerializeField] private FloatReactiveProperty windDirection = new FloatReactiveProperty(0f);

        public ReadOnlyReactiveProperty<float> WindSpeed
        {
            get { return windSpeed.ToReadOnlyReactiveProperty(); }
        }
        
        public ReadOnlyReactiveProperty<float> WindDrection 
        {
            get { return windDirection.ToReadOnlyReactiveProperty(); }
        }
    }
}