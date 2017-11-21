using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace SnipingFarmer.Script.UI
{
    [RequireComponent(typeof(Image))]
    public class FruitFever : MonoBehaviourBase
    {
        [SerializeField] private float visibleTime = 2.5f;
        private Image feverEffect;
        
        public void ShowEffect()
        {
            ChangeImageAlpha(1f);
            StartCoroutine(DurationShowEnumerator());
        }

        public void Awake()
        {
            feverEffect = GetComponent<Image>();
            Assert.IsNotNull(feverEffect);

            feverEffect.enabled = false;
        }

        private IEnumerator DurationShowEnumerator()
        {
            feverEffect.enabled = true;
            yield return new WaitForSeconds(visibleTime);
            iTween.ValueTo(gameObject, iTween.Hash(
                "from", 1f, 
                "to", 0f, 
                "time", 2.5f, 
                "onupdate", "ChangeImageAlpha"
            ));
        }

        private void ChangeImageAlpha(float alpha)
        {
            var color = feverEffect.color;
            gameObject.GetComponent<UnityEngine.UI.Image>().color = new Color(color.r, color.g, color.b, alpha);
        }
    }
}