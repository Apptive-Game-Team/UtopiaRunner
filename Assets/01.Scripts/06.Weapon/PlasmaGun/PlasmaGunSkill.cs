using UnityEngine;

namespace _01.Scripts._06.Weapon.PlasmaGun
{
    public class PlasmaGunSkill : WeaponSkillBase
    {
        [SerializeField] private GameObject skillAttackPrefab;
        private float _skillDamage;
        
        public override void Activate()
        {
            _skillDamage = Owner.attackDamage * Owner.weaponInfo.skillValue[0] / 100;
            GameObject skillAttack = Instantiate(skillAttackPrefab, new Vector3(2f, 1f, 0f), Quaternion.identity);
            skillAttack.GetComponent<AttackObject>().Init(_skillDamage);
            skillAttack.SetActive(true);
        }
    }
}
