using System;
using System.Collections;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

namespace SnipingFarmer.Script.Game
{
    [RequireComponent(typeof(Image))]
    public class ReloadEffect : MonoBehaviourBase
    {
        private Weapon weapon;
        private Image progressBar;

        public void Awake()
        {
            progressBar = GetComponent<Image>();
            progressBar.fillAmount = 0f;

            var obj = GameObject.FindGameObjectWithTag("Player");

            if (obj == null)
            {
                Debug.LogError("GameObject tagged with Player is not found");
                return;
            }

            weapon = obj.GetComponent<Weapon>();
            if (weapon == null)
            {
                Debug.LogError("Weapon Component is not attached to Player");
                return;
            }
        }
        
        public void Start()
        {
            if (weapon == null) return;
            
            weapon.IsReloading.Skip(1).Subscribe(isReloading =>
            {
                if (isReloading)
                {
                    var hash = new Hashtable
                    {
                        {"from", 0f},
                        {"to", 1.0f},
                        {"time", weapon.ReloadDurationSecond},
                        {"easyType", iTween.EaseType.linear},
                        {"loopType", iTween.LoopType.none},
                        {"onupdate", "OnUpdateProgressBar"},
                        {"onupdatetarget", gameObject},
                        {"oncomplete", "CompleteReload"},
                        {"oncompletetarget", gameObject},
                        {"oncompleteparams", 0f}
                    };
                    iTween.ValueTo(gameObject, hash);
                }
            }).AddTo(gameObject);
        }

        private void OnUpdateProgressBar(float ratio)
        {
            progressBar.fillAmount = ratio;
        }

        private void CompleteReload(float ratio)
        {
            progressBar.fillAmount = ratio;
        }
    }
}
