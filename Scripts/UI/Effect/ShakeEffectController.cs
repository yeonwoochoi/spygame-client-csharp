using System;
using System.Collections;
using Event;
using UI.Popup.Qna;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UI.Effect
{
    public class ShakeEffectController: MonoBehaviour
    {
        private RectTransform rectTransform;
        
        private readonly float duration = 0.5f;
        private readonly float magnitude = 2f;

        private bool isPlaying = false;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            SpyQnaPopupBehavior.CaptureSpyEvent += SpyQnaWrongEffect;
            ItemQnaPopupBehavior.ItemGetEvent += ItemQnaWrongEffect;
        }

        private void OnDisable()
        {
            SpyQnaPopupBehavior.CaptureSpyEvent -= SpyQnaWrongEffect;
            ItemQnaPopupBehavior.ItemGetEvent -= ItemQnaWrongEffect;
        }


        private void SpyQnaWrongEffect(object _, CaptureSpyEventArgs e)
        {
            if (isPlaying) return;
            if (e.IsCorrect()) return;
            isPlaying = true;
            StartCoroutine(Shake());
        }

        private void ItemQnaWrongEffect(object _, ItemGetEventArgs e)
        {
            if (isPlaying) return;
            if (e.type == ItemGetType.Get) return;
            isPlaying = true;
            StartCoroutine(Shake());
        }

        private IEnumerator Shake()
        {
            var originalPos = rectTransform.localPosition;
            var elapsed = 0.0f;

            while (elapsed < duration)
            {
                var x = Random.Range(-1f, 1f) * magnitude;
                var y = Random.Range(-1f, 1f) * magnitude;

                rectTransform.localPosition = new Vector3(x, y, originalPos.z);

                elapsed += Time.deltaTime;

                yield return null;
            }

            rectTransform.localPosition = originalPos;
            isPlaying = false;
        }
    }
}