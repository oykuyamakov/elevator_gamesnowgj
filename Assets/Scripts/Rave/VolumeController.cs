using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

namespace Rave
{
    public class VolumeController : MonoBehaviour
    {
        private Volume volume => GetComponent<Volume>();

        private void Start()
        {
            StartCoroutine(HueAdjuster());
        }

        private IEnumerator HueAdjuster()
        {
            if(volume.profile.TryGet(out UnityEngine.Rendering.Universal.ColorAdjustments colorAdjustments))
            {

                yield return new WaitForSeconds(13f);
                
                DOTween.To(() => colorAdjustments.hueShift.value, x => colorAdjustments.hueShift.value = x, -180f, 1f);
                
                yield return new WaitForSeconds(1f);
                
                DOTween.To(() => colorAdjustments.hueShift.value, x => colorAdjustments.hueShift.value = x, 10f, 5f).SetLoops(-1,LoopType.Yoyo);
            }
            else
            {
                Debug.Log("ananisikim");
            }
        }
    }
}
