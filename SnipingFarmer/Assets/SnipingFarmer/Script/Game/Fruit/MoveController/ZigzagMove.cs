using System.Collections;
using UnityEngine;

namespace SnipingFarmer.Script.Game.Fruit.MoveController
{
    [RequireComponent(typeof(Rigidbody))]
    public class ZigzagMove : AbstractSpecialMove
    {
        [SerializeField] private float moveWidthLimit = 8f;

        protected override Hashtable TempTweenParams
        {
            get
            {
                return iTween.Hash(
                    "x", moveWidthLimit / 2f,
                    "speed", Speed.Value,
                    "easetype", iTween.EaseType.linear,
                    "looptype", iTween.LoopType.pingPong,
                    "islocal", true,
                    "space", Space.World
                );
            }
        }

        protected override void ResetTempTransform()
        {
            base.ResetTempTransform();
            tempGameObject.transform.localPosition = new Vector3(
                -moveWidthLimit / 2f,
                0f,
                0f
            );
        }

        protected override Vector3 CalcNextOffset()
        {
            var x = tempGameObject.transform.localPosition.x;
            return ConvertXToXZ(x);
        }
    }
}