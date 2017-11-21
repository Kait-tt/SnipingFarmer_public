using System.Collections;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

namespace SnipingFarmer.Script.UI
{
    public class MiniMap : MonoBehaviourBase
    {
        [SerializeField] private int showDuration;
        [SerializeField] private int showInterval;

        private Camera miniMapCamera;
        private int originCullingMask;

        public int ShowDuration
        {
            get { return showDuration; }
        }

        public int ShowInterval
        {
            get { return showInterval; }
        }

        public void Awake()
        {
            miniMapCamera = GetComponent<Camera>();
            originCullingMask = miniMapCamera.cullingMask;
        }

        public void Start()
        {
            StartCameraLoop();
        }

        public void StartCameraLoop()
        {
            StartCoroutine(CameraLoop());
        }

        public void StopCameraLoop()
        {
            StopCoroutine(CameraLoop());
        }

        private IEnumerator CameraLoop()
        {
            // show から始める
            while (true)
            {
                // show
                miniMapCamera.cullingMask = originCullingMask;
                
                yield return new WaitForSeconds(showDuration);
                
                // hide
                miniMapCamera.cullingMask = 0;
                
                yield return new WaitForSeconds(showInterval);
            }
        }
    }
}