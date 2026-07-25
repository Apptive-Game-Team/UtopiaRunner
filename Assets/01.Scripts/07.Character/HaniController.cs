using System.Collections;
using _01.Scripts._00.Manager;
using _01.Scripts._05.Utility;
using _01.Scripts._06.Weapon;
using UnityEngine;

namespace _01.Scripts._07.Character
{
    public class HaniController : PlayerController
    {
        private WeaponController _wc;
        private int _doubleJumpCount;
        
        protected override void Awake()
        {
            base.Awake();
            maxJumpCount = 3;
        }

        private void OnEnable()
        {
            if (!IsSet)
            {
                return;
            }
            
            OnJumpDetected += SetDoubleJumpEffect;
        }

        private void OnDisable()
        {
            if (!IsSet)
            {
                return;
            }
            
            OnJumpDetected -= SetDoubleJumpEffect;
        }

        public override void Init()
        {
            base.Init();
            
            _wc = FindAnyObjectByType<WeaponController>();
            if (gameObject.activeSelf)
            {
                OnJumpDetected += SetDoubleJumpEffect;
            }
        }

        private void SetDoubleJumpEffect()
        {
            if (jumpCount == 2) // 더블 점프 일때만?
            {
                _doubleJumpCount++;
            }

            if (_doubleJumpCount >= 10)
            {
                _wc.AddAttackEffect(1, (e, f) =>
                {
                    _wc.StartCoroutine(SetEnemySlow(e, f));
                });
                _doubleJumpCount = 0;
            }
        }

        private IEnumerator SetEnemySlow(GameObject enemy, float time)
        {
            float originSpeed = ValueFormula.enemyAttackSpeed;
            ValueFormula.SetEnemyAttackSpeed(ValueFormula.enemyAttackSpeed + originSpeed);
            yield return new WaitForSeconds(ValueFormula.GetCharacterSkillValues(
                characterInfo, GameManager.Instance.playerData.characterGrade[characterInfo.id])[0]);
            ValueFormula.SetEnemyAttackSpeed(ValueFormula.enemyAttackSpeed - originSpeed);
        }
    }
}
