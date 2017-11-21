using System.Collections;
using UnityEngine;

namespace SnipingFarmer.Script.Game.Fruit.MoveController
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class AbstractSpecialMove : BaseMove 
    {
        /// <summary>
        /// 計算用のGameObject
        /// </summary>
        protected GameObject tempGameObject;

        /// <summary>
        /// 前回のoffset値
        /// </summary>
        protected Vector3 beforeOffsetVec;

        /// <summary>
        /// 初期座標のY値
        /// </summary>
        private float baseY;
        
        protected abstract Hashtable TempTweenParams { get; }

        private float beforeSpeed;

        private float memoTheta;
        
        public new void Awake()
        {
            base.Awake();
            tempGameObject = CreateTempGameObject();
            agent.updatePosition = false;
            baseY = transform.position.y;
        }

        public new void Start()
        {
            base.Start();
            StartTempTween();
        }

        private void StartTempTween()
        {
            var hash = TempTweenParams;
            
            hash.Add("oncomplete", "OnCompleteTempTween");
            hash.Add("oncompletetarget", gameObject);

            beforeSpeed = Speed.Value;

            ResetTempTransform();
            iTween.MoveTo(tempGameObject, hash);
        }

        private IEnumerator RestartTempTween()
        {
            iTween.Stop(tempGameObject);

            // ちょっと待たないとstartできない
            yield return new WaitForEndOfFrame();
            
            StartTempTween();
        }

        private void OnCompleteTempTween()
        {
            // speedが変わったらtweenをやり直す
            if (beforeSpeed != Speed.Value)
            {
                StartCoroutine(RestartTempTween());
            }
        }

        /// <summary>
        /// tempオブジェクトのtransformの初期化
        /// </summary>
        protected virtual void ResetTempTransform()
        {
            tempGameObject.transform.localPosition = Vector3.zero;
            tempGameObject.transform.position = transform.position;
            tempGameObject.transform.localRotation = Quaternion.identity;
            tempGameObject.transform.rotation = transform.rotation;
        }

        /// <summary>
        /// 次のoffset値を計算して返す。
        /// </summary>
        /// <returns>計算結果のoffset値</returns>
        protected abstract Vector3 CalcNextOffset();

        public void Update()
        {
            if (agent.isStopped) return;

            // 不自然な瞬間移動防止のため、position０付近のときのみ角度を計算し直す
            if (tempGameObject.transform.localPosition.sqrMagnitude < 0.1f)
            {
                UpdateMemoTheta();
            }
            
            var offsetVec = CalcNextOffset();

            var pos = agent.nextPosition - beforeOffsetVec + offsetVec;
            beforeOffsetVec = offsetVec;
            
            // yだけうまく動かないので強制的に変える
            pos.y = baseY + offsetVec.y;
            
            agent.nextPosition = pos;
            transform.position = pos;
        }

        protected GameObject CreateTempGameObject()
        {
            var temp = new GameObject();
            // 子にしておいて一緒にDestroyする
            temp.transform.parent = transform;
            return temp;
        }
         
        /// <summary>
        /// X値を、目的地に向いた時のXとZ（world座標）に変換する。
        /// </summary>
        /// <param name="x">変換するX値</param>
        /// <returns>変換結果のXとZを持つVector</returns>
        protected Vector3 ConvertXToXZ(float x)
        {
            return new Vector3(
                x * Mathf.Cos(memoTheta),
                0,
                x * Mathf.Sin(memoTheta)
            );
        }

        private void UpdateMemoTheta()
        {
            var dir = targetMovePoint - (agent.nextPosition - beforeOffsetVec);
            memoTheta = Mathf.Atan2(dir.z, dir.x) - Mathf.PI / 2f;
        }
    }
}