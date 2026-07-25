using _01.Scripts._00.Manager;
using _01.Scripts._05.Utility;
using _01.Scripts._06.Weapon;
using UnityEngine;

namespace _01.Scripts._07.Character
{
    public class IcarusController : PlayerController
    {
        [SerializeField] private float coolTime;
        
        private WeaponController _wc;
        private float _timeCounter = -99f;
        
        private void OnEnable()
        {
            if (!IsSet)
            {
                return;
            }
            
            OnJumpDetected += CoolDownSkill;
            OnSlideDetected += CoolDownSkill;
        }

        private void OnDisable()
        {
            if (!IsSet)
            {
                return;
            }
            
            OnJumpDetected -= CoolDownSkill;
            OnSlideDetected -= CoolDownSkill;
        }

        private void CoolDownSkill()
        {
            if (Time.time - _timeCounter < coolTime)
            {
                return;
            }
            
            _wc.ReduceCooldown(ValueFormula.GetCharacterSkillValues(
                characterInfo, GameManager.Instance.playerData.characterGrade[characterInfo.id])[0]);
            _timeCounter = Time.time;
        }

        public override void Init()
        {
            base.Init();
            
            _wc = FindAnyObjectByType<WeaponController>();
            if (gameObject.activeSelf)
            {
                OnJumpDetected += CoolDownSkill;
                OnSlideDetected += CoolDownSkill;
            }
        }
    }
}
