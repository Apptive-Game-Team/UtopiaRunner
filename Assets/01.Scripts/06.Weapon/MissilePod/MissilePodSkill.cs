using _01.Scripts._00.Manager;
using _01.Scripts._05.Utility;
using UnityEngine;

namespace _01.Scripts._06.Weapon.MissilePod
{
    public class MissilePodSkill : WeaponSkillBase
    {
        [SerializeField] private GameObject missilePrefab;
        
        public override void Activate()
        {
            if (Owner is MissilePodController mc)
            {
                for (int i = 0; i < mc.missileCount + ValueFormula.GetWeaponSkillValues(Owner.weaponInfo, 
                         GameManager.Instance.playerData.weaponGrade[Owner.weaponInfo.id])[0]; i++)
                {
                    GameObject mp = Instantiate(missilePrefab, transform.position + (Vector3)Random.insideUnitCircle, Quaternion.identity);
                    mp.GetComponent<AutoAttackProjectile>().Init(Owner.attackDamage * 0.15f);
                }

                mc.missileCount = 0;
            }
        }
    }
}
