using System.Collections;
using UnityEngine;

namespace SnipingFarmer.Script
{
    public abstract class MonoBehaviourBase : MonoBehaviour
    {
        public static void DestroyIfExists(GameObject gameObject)
        {
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }
        
        // StartCoroutineはstaticが使えない
        public void DelayDestroy(GameObject destroyGameObject, float delaySeconds)
        {
            StartCoroutine(DelayDestroyCoroutine(destroyGameObject, delaySeconds));
        }

        private static IEnumerator DelayDestroyCoroutine(GameObject destroyGameObject, float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);
            
            DestroyIfExists(destroyGameObject);
        }
    }
}