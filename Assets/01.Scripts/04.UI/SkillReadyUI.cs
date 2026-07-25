using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Scripts._04.UI
{
    public class SkillReadyUI : MonoBehaviour
    {
        [Header("Cooldown Slider")]
        [SerializeField] private Slider cooldownSlider;

        [Header("Blink Target")]
        [SerializeField] private CanvasGroup blinkTargetGroup;

        [Header("Border Effect")]
        [SerializeField] private CanvasGroup borderGroup;
        [SerializeField] private RectTransform borderRect;

        [Header("Effect Option")]
        [SerializeField] private float blinkDuration = 0.12f;
        [SerializeField] private float borderScale = 1.1f;

        private bool _wasReady;
        private Sequence _sequence;

        private void Awake()
        {
            if (cooldownSlider == null)
            {
                cooldownSlider = GetComponent<Slider>();
            }

            if (blinkTargetGroup != null)
            {
                blinkTargetGroup.alpha = 1f;
            }

            if (borderGroup != null)
            {
                borderGroup.alpha = 0f;
                borderGroup.gameObject.SetActive(false);
            }

            if (cooldownSlider != null)
            {
                _wasReady = cooldownSlider.value >= 0.99f;
            }
        }

        private void OnEnable()
        {
            if (cooldownSlider != null)
            {
                cooldownSlider.onValueChanged.AddListener(OnCooldownChanged);
            }
        }

        private void OnDisable()
        {
            if (cooldownSlider != null)
            {
                cooldownSlider.onValueChanged.RemoveListener(OnCooldownChanged);
            }

            _sequence?.Kill();
        }

        private void OnCooldownChanged(float value)
        {
            bool isReady = value >= 0.99f;

            if (isReady && !_wasReady)
            {
                PlayReadyBlink();
            }

            _wasReady = isReady;
        }

        private void PlayReadyBlink()
        {
            if (blinkTargetGroup == null)
                return;

            _sequence?.Kill();

            blinkTargetGroup.alpha = 1f;

            if (borderGroup != null)
            {
                borderGroup.gameObject.SetActive(true);
                borderGroup.alpha = 1f;
            }

            if (borderRect != null)
            {
                borderRect.localScale = Vector3.one;
            }

            _sequence = DOTween.Sequence();
            _sequence.SetUpdate(true);

            _sequence.Append(
                blinkTargetGroup.DOFade(0f, blinkDuration)
            );

            _sequence.Append(
                blinkTargetGroup.DOFade(1f, blinkDuration)
            );

            if (borderGroup != null)
            {
                _sequence.Join(
                    borderGroup.DOFade(0f, blinkDuration)
                );
            }

            if (borderRect != null)
            {
                _sequence.Join(
                    borderRect.DOScale(Vector3.one * borderScale, blinkDuration)
                );
            }

            _sequence.OnComplete(() =>
            {
                blinkTargetGroup.alpha = 1f;

                if (borderGroup != null)
                {
                    borderGroup.alpha = 0f;
                    borderGroup.gameObject.SetActive(false);
                }

                if (borderRect != null)
                {
                    borderRect.localScale = Vector3.one;
                }
            });
        }
    }
}