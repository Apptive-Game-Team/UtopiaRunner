using System;
using System.Collections;
using System.Linq;
using _01.Scripts._00.Manager;
using UnityEngine;

namespace _01.Scripts._06.Weapon.DroneLauncher
{
    public class DroneLauncherController : WeaponController
    {
        [SerializeField] private int enhancedAttackCount;
        private int _attackCount;

        protected override AutoAttackProjectile SetAttackProjectile()
        {
            GameObject projectile = Instantiate(attackPrefab, transform.position, Quaternion.identity);

            _attackCount++;

            AutoAttackProjectile projectileScript = projectile.GetComponent<AutoAttackProjectile>();
                    
            if (projectileScript != null)
            {
                bool isEnhanced = (_attackCount == enhancedAttackCount);
                projectileScript.Init(attackDamage * (isEnhanced ? 2 : 1));
                if (isEnhanced)
                {
                    projectile.GetComponent<SpriteRenderer>().color = Color.red;
                    _attackCount = 0;
                }
            }

            return projectileScript;
        }
    }
}
