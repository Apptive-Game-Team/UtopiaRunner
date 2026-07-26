using _01.Scripts._00.Manager;
using UnityEngine;

namespace _01.Scripts._06.Weapon.RPG
{
    public class RpgSkill : WeaponSkillBase
    {
        [SerializeField] private GameObject skillAttackPrefab;
        private float _skillDamage;
        
        public override void Activate()
        {
            _skillDamage = Owner.attackDamage * Owner.weaponInfo.skillValue[0] / 100;
            InGameManager.Instance.mainCharacter.TakeDamage(InGameManager.Instance.mainCharacter.hp * 0.3f);
            GameObject skillAttack = Instantiate(skillAttackPrefab, new Vector3(2f, 1f, 0f), Quaternion.identity);
            skillAttack.GetComponent<AttackObject>().Init(_skillDamage, true, 
                true, Owner.weaponInfo.skillValue[1], 1f, 5.1f);
            skillAttack.SetActive(true);
        }
    }
}
