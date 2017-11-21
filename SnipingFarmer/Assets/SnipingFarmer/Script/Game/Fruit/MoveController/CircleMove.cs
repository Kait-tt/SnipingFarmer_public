using System.Collections;
using UnityEngine;

namespace SnipingFarmer.Script.Game.Fruit.MoveController
{
    [RequireComponent(typeof(Rigidbody))]
    public class CircleMove : AbstractSpecialMove
    {
        [SerializeField] private float moveWidthLimit = 8f;
        [SerializeField] private float moveHeightLimit = 8f;

        protected override Hashtable TempTweenParams
        {
            get
            {
                return iTween.Hash(
                    "y", moveHeightLimit,
                    "speed", Speed.Value / 2f,
                    "easetype", iTween.EaseType.linear,
                    "looptype", iTween.LoopType.loop,
                    "islocal", true
                );
            }
        }

        protected override Vector3 CalcNextOffset()
        {
            // itweenで動かしたyをtとしてy, xを計算する
            var t = tempGameObject.transform.localPosition.y / moveHeightLimit;
            var t2 = t * 2f * Mathf.PI;
            
            // t=0ならtx=0にしたいのでtxの計算でsinを使う（実質問題ない）
            var tx = Mathf.Sin(t2) / 2f; // [-0.5, +0.5]
            var ty = (Mathf.Cos(t2) + 1f) / 2f; // [+0.0, +1.0]
            
            var x = moveWidthLimit * tx;
            var y = moveHeightLimit * ty;

            return ConvertXToXZ(x) + Vector3.up * y;
        }
    }
}
