using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _01.Scripts._00.Manager;
using _01.Scripts._04.UI;
using UnityEngine;

namespace _01.Scripts._06.Weapon
{
    public class WeaponController : MonoBehaviour
    {
        public class AttackEffectInstance
        {
            public Action<GameObject, float> Effect;
            public int RemainingCount;

            public AttackEffectInstance(int count, Action<GameObject, float> effect)
            {
                RemainingCount = count;
                Effect = effect;
            }
        }
        
        [Header("Components")]
        public WeaponInfo weaponInfo;
        public int weaponId;
        public GameObject attackPrefab;
        public GameObject skillPrefab;
        public float attackDamage;

        public Action<float, float> OnCoolDownChanged;
    
        private WeaponSkillBase _skill;
        private float _skillCooldownTimer;
        private float _currentCooldown;
        
        protected Action<GameObject, float> PendingEffect;
        protected int EffectCount;
        protected List<AttackEffectInstance> ActiveEffectInstances = new();

        protected virtual void Start()
        {
            InputManager.AddListener(ActionCode.Skill, InputType.Down, SkillInput);
        }

        public void Initialize(float characterDamage)
        {
            SetDamage(characterDamage);
            
            _skill = Instantiate(skillPrefab, transform).GetComponent<WeaponSkillBase>();

            if (_skill != null)
            {
                _skill.Init(this);
            }

            StartCoroutine(AutoAttack());
        }

        public void SetDamage(float characterDamage)
        {
            attackDamage = characterDamage;
        }

        private void Update()
        {
            if (_currentCooldown > 0)
            {
                _currentCooldown -= Time.deltaTime;
                OnCoolDownChanged?.Invoke(_currentCooldown, weaponInfo.coolTime);
            }
        }

        private void SkillInput()
        {
            if (_currentCooldown <= 0)
            {
                _skill.Activate();
                _currentCooldown = weaponInfo.coolTime;
                OnCoolDownChanged?.Invoke(_currentCooldown, weaponInfo.coolTime);
            }
        }
        
        public void ReduceCooldown(float percent)
        {
            float reduction = weaponInfo.coolTime * (percent / 100f);
            _currentCooldown -= reduction;

            if (_currentCooldown < 0)
            {
                _currentCooldown = 0;
            }
            
            OnCoolDownChanged?.Invoke(_currentCooldown, weaponInfo.coolTime);
        }
        
        public void AddAttackEffect(int count, Action<GameObject, float> effect)
        {
            var existingInstance = ActiveEffectInstances.Find(instance => instance.Effect == effect);

            if (existingInstance != null)
            {
                existingInstance.RemainingCount += count;
            }
            else
            {
                ActiveEffectInstances.Add(new AttackEffectInstance(count, effect));
            }
        }

        protected virtual IEnumerator AutoAttack()
        {
            while (true)
            {
                if (GameObject.FindWithTag("Enemy") != null)
                {
                    AutoAttackProjectile projectileScript = SetAttackProjectile();
                    SetAttackEffect(projectileScript);
                }

                yield return new WaitForSeconds(weaponInfo.attackSpeed);
            }
        }

        protected virtual AutoAttackProjectile SetAttackProjectile()
        {
            GameObject projectile = Instantiate(attackPrefab, transform.position, Quaternion.identity);
            AutoAttackProjectile projectileScript = projectile.GetComponent<AutoAttackProjectile>();

            projectileScript.Init(attackDamage);
            
            return projectileScript;
        }

        protected virtual void SetAttackEffect(AutoAttackProjectile projectileScript)
        {
            foreach (var effect in ActiveEffectInstances.ToList())
            {
                projectileScript.AddEffect(effect.Effect);
                effect.RemainingCount--;
                        
                if (effect.RemainingCount <= 0)
                {
                    ActiveEffectInstances.Remove(effect);
                }
            }
        }

        private void OnDestroy()
        {
            InputManager.RemoveListener(ActionCode.Skill, InputType.Down, SkillInput);
        }
    }
}
