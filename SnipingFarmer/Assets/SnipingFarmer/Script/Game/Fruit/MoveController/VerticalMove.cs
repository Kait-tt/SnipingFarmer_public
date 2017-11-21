using System.Collections;
using UnityEngine;

namespace SnipingFarmer.Script.Game.Fruit.MoveController
{
    [RequireComponent(typeof(Rigidbody))]
    public class VerticalMove : AbstractSpecialMove
    {
        [SerializeField] private float moveHeightLimit = 8f;

        protected override Hashtable TempTweenParams
        {
            get
            {
                return iTween.Hash(
                    "y", moveHeightLimit,
                    "speed", Speed.Value,
                    "EaseType", iTween.EaseType.easeInOutQuad,
                    "loopType", iTween.LoopType.pingPong,
                    "islocal", true
                );
            }
        }

        protected override Vector3 CalcNextOffset()
        {
            return Vector3.up * tempGameObject.transform.localPosition.y;
        }
    }
}