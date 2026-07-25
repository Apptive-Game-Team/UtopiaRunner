using _01.Scripts._00.Manager;
using _01.Scripts._05.Utility;
using _01.Scripts._06.Weapon;
using UnityEngine;

namespace _01.Scripts._07.Character
{
    public class KarokController : PlayerController
    {
        private WeaponController _wc;
        private float _originDamage;
        private float _originWeaponAttackSpeed;

        public override void TakeDamage(float d)
        {
            base.TakeDamage(d);
            AttackEffect();
            DrainEffect();
        }

        public override void Heal(float d)
        {
            base.Heal(d);
            AttackEffect();
            DrainEffect();
        }

        private void AttackEffect()
        {
            if (hp < maxHp * 0.3f)
            {
                damage = _originDamage * (1 + ValueFormula.GetCharacterSkillValues(
                    characterInfo, GameManager.Instance.playerData.characterGrade[characterInfo.id])[0] / 100);
                _wc.weaponInfo.attackSpeed = _originWeaponAttackSpeed / (1 - ValueFormula.GetCharacterSkillValues(
                    characterInfo, GameManager.Instance.playerData.characterGrade[characterInfo.id])[0] / 100);
            }
            else
            {
                damage = _originDamage;
                _wc.weaponInfo.attackSpeed = _originWeaponAttackSpeed;
            }
        }

        private void DrainEffect()
        {
            if (hp < maxHp * 0.2f)
            {
                _wc.AddAttackEffect(9999, (_, d) =>
                {
                    if (hp <= maxHp / 0.2f) 
                    {
                        float healAmount = damage * ValueFormula.GetCharacterSkillValues(
                            characterInfo, GameManager.Instance.playerData.characterGrade[characterInfo.id])[1];
                        hp += healAmount;
                    }
                });
            }
        }

        public override void Init()
        {
            base.Init();
            
            _originDamage = damage;
        }

        public override void AfterInit()
        {
            _wc = FindAnyObjectByType<WeaponController>();
            _originWeaponAttackSpeed = _wc.weaponInfo.attackSpeed;
        }
    }
}
