using UnityEngine;

namespace SnipingFarmer.Script
{
    public abstract class AutoDestroyMonoBehaviourBase : MonoBehaviourBase
    {
        /// <summary>
        /// 生存時間
        /// ※生成後の変更不可
        /// </summary>
        [SerializeField] protected float liveSeconds;
        
        public void Start()
        {
            DelayDestroy(gameObject, liveSeconds);
        }
    }
}