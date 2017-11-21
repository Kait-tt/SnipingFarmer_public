using UniRx;
using UnityEngine;

namespace SnipingFarmer.Script.Game
{
    public class PlayerShotPenalty : MonoBehaviourBase
    {
        [SerializeField] private float durationSecond;

        [SerializeField] public BoolReactiveProperty IsEnabled = new BoolReactiveProperty(false);

        public void EnablePenalty()
        {
            // TODO
        }
    }
}