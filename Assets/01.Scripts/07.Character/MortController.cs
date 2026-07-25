using _01.Scripts._00.Manager;
using _01.Scripts._05.Utility;
using UnityEngine;

namespace _01.Scripts._07.Character
{
    public class MortController : PlayerController
    {
        [Header("Enhancement Settings")]
        [SerializeField] private float maxEnhanceTime = 10f;
        [SerializeField] private float maxDamageMultiplier = 1.5f;

        private float _enhancementTimer;
        private float _originDamage;

        protected override void Update()
        {
            base.Update();
            
            if (_enhancementTimer < maxEnhanceTime)
            {
                _enhancementTimer += Time.deltaTime;
                UpdateWeaponDamage();
            }
        }

        private void UpdateWeaponDamage()
        {
            int increaseCount = Mathf.FloorToInt(_enhancementTimer / 3f);

            float currentMultiplier = 1f + increaseCount * ValueFormula.GetCharacterSkillValues(
                characterInfo, GameManager.Instance.playerData.characterGrade[characterInfo.id])[0] / 100;

            damage = _originDamage * currentMultiplier;
        }

        private void ResetEnhancement()
        {
            _enhancementTimer = 0f;
            UpdateWeaponDamage();
        }

        private void OnEnable()
        {
            if (!IsSet)
            {
                return;
            }
            
            OnJumpDetected += ResetEnhancement;
            OnSlideDetected += ResetEnhancement;
        }

        private void OnDisable()
        {
            if (!IsSet)
            {
                return;
            }
            
            OnJumpDetected -= ResetEnhancement;
            OnSlideDetected -= ResetEnhancement;
        }
        
        public override void Init()
        {
            base.Init();
            
            _originDamage = damage;
            if (gameObject.activeSelf)
            {
                OnJumpDetected += ResetEnhancement;
                OnSlideDetected += ResetEnhancement;
            }
        }
    }
}