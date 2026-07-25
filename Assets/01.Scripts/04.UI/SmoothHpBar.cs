using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace _01.Scripts._04.UI
{
    public class SmoothHpBar : MonoBehaviour
    {
        private Slider _slider;
        
        [Header("UI References")]
        [SerializeField] private RectTransform mainFillRect;
        [SerializeField] private RectTransform delayedFillRect;

        [Header("Settings")]
        [SerializeField] private float delayTime = 0.5f;
        [SerializeField] private float shrinkDuration = 0.5f;

        private Tweener _delayedTween;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }
        
        public void UpdateHp(float current, float max, bool setSmooth)
        {
            float targetRatio = current / max;
            float previousRatio = _slider.value;
            
            _slider.value = targetRatio;
            
            if (_delayedTween != null && _delayedTween.IsActive())
            {
                _delayedTween.Kill();
            }
            
            Vector2 targetAnchorMax = mainFillRect.anchorMax;

            if (!setSmooth)
            {
                delayedFillRect.anchorMax = targetAnchorMax;
                return;
            }
            
            if (targetRatio < previousRatio)
            {
                if (delayedFillRect != null)
                {
                    _delayedTween = delayedFillRect.DOAnchorMax(targetAnchorMax, shrinkDuration)
                        .SetDelay(delayTime)
                        .SetEase(Ease.OutQuad);
                }
            }
            else
            {
                if (delayedFillRect != null)
                {
                    delayedFillRect.anchorMax = targetAnchorMax;
                }
            }
        }
    }
}