using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SceneManagement
{
    public class Fader : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        private Coroutine currentActiveFade;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate()
        {
            _canvasGroup.alpha = 1;
        }

        public void FadeInImmediate()
        {
            _canvasGroup.alpha = 0;
        }

        public Coroutine FadeOut(float time)
        {
            print("time to fade out");
            return Fade(1, time);
        }

        public Coroutine FadeIn(float time)
        {
            print("time to fade in");
            return Fade(0, time);
        }

        public Coroutine Fade(float target, float time)
        {
            if (currentActiveFade != null)
            {
                StopCoroutine(currentActiveFade);
            }

            currentActiveFade = StartCoroutine(FadeRoutine(target, time));
            return currentActiveFade;
        }


        private IEnumerator FadeRoutine(float target, float time)
        {
            while (!Mathf.Approximately(_canvasGroup.alpha, target))
            {
                _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }
        }
    }
}