using System;
using _01.Scripts._00.Manager;
using _01.Scripts._05.Utility;
using UnityEngine;
using UnityEngine.Serialization;

namespace _01.Scripts._06.Weapon.Sniper
{
    public class SniperAutoAttackProjectile : AutoAttackProjectile
    {
        private GameObject _character;
        public bool isSkillUsed;
        private bool _isFirstTarget = true;

        public void SetCharacter(GameObject character)
        {
            _character = character;
        }
        
        protected override float CalculateDamage()
        {
            float dist = Vector3.Distance(transform.position, _character.transform.position);
            float t = Mathf.InverseLerp(5f, 10f, dist);
            float multiplier = Mathf.Lerp(1f, 1.5f, t);
        
            if (!isSkillUsed)
            {
                return Damage * multiplier;
            }

            float mul1 = ValueFormula.GetWeaponSkillValues(InGameManager.Instance.weapon.weaponInfo,
                GameManager.Instance.playerData.weaponGrade[InGameManager.Instance.weapon.weaponInfo.id])[0] / 100;
            float mul2 = ValueFormula.GetWeaponSkillValues(InGameManager.Instance.weapon.weaponInfo,
                GameManager.Instance.playerData.weaponGrade[InGameManager.Instance.weapon.weaponInfo.id])[1] / 100;
            return _isFirstTarget ? (Damage * multiplier * mul1) : (Damage * multiplier * mul2);
        }

        public void DelayedDestroy(float time)
        {
            Destroy(gameObject, time);
        }
        
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                ApplyHit(collision.gameObject);

                if (!isSkillUsed)
                {
                    Destroy(gameObject);
                }
                else if (_isFirstTarget)
                {
                    _isFirstTarget = false;
                }
            }
        }
    }
}
