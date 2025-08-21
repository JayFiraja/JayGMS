using System;
using System.Collections;
using UnityEngine;
using Cinemachine;

namespace GMS.Samples
{
    /// <summary>
    /// Manges the FadeIn and Out from the Cinemachine Storyboard reference
    /// </summary>
    [RequireComponent(typeof(CinemachineStoryboard))]
    public class CameraStoryBoard : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField]
        private AnimationCurve fadeInSpeed;
        [SerializeField]
        private AnimationCurve fadeOutSpeed;
        [SerializeField]
        private float fadeOutWaitTime = 1;

        private CinemachineStoryboard _cinemachineStoryboard;
        // variables
        private Coroutine _fadeCoroutine;
        private float _fadeStartTime;

        private WaitForSeconds onCompleteWait;

        public void Initialize()
        {
            TryGetComponent(out _cinemachineStoryboard);
            onCompleteWait = new WaitForSeconds(fadeOutWaitTime);
        }

        public void UnInitialize()
        {
            _fadeCoroutine = null;
        }

        public void FadeIn()
        {
            FadeIn(false, false);
        }

        public void FadeIn(bool autoFadeOut, bool immediate = false)
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }

            if (immediate)
            {
                float endValue = fadeInSpeed[fadeInSpeed.keys.Length - 1].value;
                _cinemachineStoryboard.m_Alpha = endValue;

                if (autoFadeOut)
                {
                    FadeOut();
                }

                return;
            }

            if (autoFadeOut)
            {
                _fadeCoroutine = StartCoroutine(FadeStoryboardTowards(fadeInSpeed, FadeOut, onCompleteWait));
            }
            else
            {
                _fadeCoroutine = StartCoroutine(FadeStoryboardTowards(fadeInSpeed));
            }
            /*     Dev Note: Anonymous Function example to be avoided, although I admit it looks simpler by using less lines, or looks less repetitive.
                            The drawbacks are: 1. Generates garbage, 2.- harder to debug and follow in the call stack

                           _fadeCoroutine = StartCoroutine(FadeStoryboardTowards(fadeInSpeed, () =>
                           if (autoFadeOut)
                           {
                               FadeOut();
                           }
                           }));

                 Note: however they should be avoided whenever possible
                       because they generate Garbage
             */
        }

        private void FadeOut()
        {
            FadeOut(false, false);
        }

        public void FadeOut(bool autoFadeIn, bool immediate = false)
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }

            if (immediate)
            {
                float endValue = fadeOutSpeed[fadeOutSpeed.keys.Length - 1].value;
                _cinemachineStoryboard.m_Alpha = endValue;

                if (autoFadeIn)
                {
                    FadeIn();
                }

                return;
            }

            if (autoFadeIn)
            {
                _fadeCoroutine = StartCoroutine(FadeStoryboardTowards(fadeOutSpeed, FadeIn, onCompleteWait));
            }
            else
            {
                _fadeCoroutine = StartCoroutine(FadeStoryboardTowards(fadeOutSpeed));
            }
        }

        private IEnumerator FadeStoryboardTowards(AnimationCurve fadeSpeed, Action onComplete = null, WaitForSeconds onCompleteWait = null)
        {
            _fadeStartTime = Time.timeSinceLevelLoad;
            float targetAlpha = fadeSpeed[fadeSpeed.keys.Length - 1].value;

            // correctly terminate the loop when the current alpha is close enough to the target alpha. as we are comparing floats here
            while (!Mathf.Approximately(targetAlpha, _cinemachineStoryboard.m_Alpha))
            {
                float progress = Time.timeSinceLevelLoad - _fadeStartTime;
                float getCurveValue = fadeSpeed.Evaluate(time: progress);
                float newAlpha = Mathf.MoveTowards(current: _cinemachineStoryboard.m_Alpha, target: targetAlpha, maxDelta: Time.deltaTime / getCurveValue);

                _cinemachineStoryboard.m_Alpha = newAlpha;
                yield return null;
            }

            // ensure we have the exact final value assigned 
            _cinemachineStoryboard.m_Alpha = targetAlpha;

            if (onCompleteWait != null)
            {
                yield return onCompleteWait;
            }

            _fadeCoroutine = null;
            onComplete?.Invoke();
        }
    }
}
