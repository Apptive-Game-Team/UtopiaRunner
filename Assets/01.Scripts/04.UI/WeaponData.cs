using System;
using System.Collections.Generic;
using _01.Scripts._00.Manager;
using _01.Scripts._05.Utility;
using UnityEngine;
using UnityEngine.Serialization;

namespace _01.Scripts._04.UI
{
    [Serializable]
    public class WeaponInfo
    {
        [Header("Info")]
        public int id;
        public string name;
        public Sprite sprite;
        
        [Header("Stat")]
        public List<int> skillValue;
        public float coolTime;
        public float attackSpeed;
        
        [Header("Script")]
        [TextArea] public string characteristic;
        [TextArea] public string skillDescription;
        public string recommendedCharacter;
        
        public WeaponInfo Clone()
        {
            return new WeaponInfo
            {
                id = id,
                name = name,
                sprite = sprite,
                skillValue = new List<int>(skillValue),
                coolTime = coolTime,
                attackSpeed = attackSpeed,
                characteristic = characteristic,
                skillDescription = ValueFormula.GetFormattedSkillDescription(this, GameManager.Instance.playerData.weaponGrade[id]),
                recommendedCharacter = recommendedCharacter
            };
        }
    }
    
    [CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObject/WeaponData")]
    public class WeaponData : ScriptableObject
    {
        public List<WeaponInfo> weaponInfos;
    }
}
