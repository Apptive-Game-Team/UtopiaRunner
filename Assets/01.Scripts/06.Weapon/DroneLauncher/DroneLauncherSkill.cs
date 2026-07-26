using System.Collections;
using _01.Scripts._00.Manager;
using _01.Scripts._05.Utility;
using UnityEngine;

namespace _01.Scripts._06.Weapon.DroneLauncher
{
    public class DroneLauncherSkill : WeaponSkillBase
    {
        [SerializeField] private float duration = 5f;
        [SerializeField] private GameObject shieldPrefab;

        public override void Activate()
        {
            if (isSkilling) return;

            Debug.Log("드론 런처 스킬 발동!");
            StartCoroutine(SkillRoutine());
        }

        private IEnumerator SkillRoutine()
        {
            isSkilling = true;
            InGameManager.Instance.mainCharacter.hasShield = true;
            Instantiate(shieldPrefab, InGameManager.Instance.mainCharacter.transform.position, InGameManager.Instance.mainCharacter.transform.rotation);

            float originSpeed = Owner.weaponInfo.attackSpeed;
            Owner.weaponInfo.attackSpeed *= (1 - ValueFormula.GetWeaponSkillValues(Owner.weaponInfo, GameManager.Instance.playerData.weaponGrade[Owner.weaponInfo.id])[1] / 100);

            yield return new WaitForSeconds(duration);
            
            Owner.weaponInfo.attackSpeed = originSpeed;

            isSkilling = false;
        }
    }
}