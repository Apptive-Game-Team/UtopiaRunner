using System;
using System.Collections.Generic;
using _01.Scripts._00.Manager;
using _01.Scripts._05.Utility;
using UnityEngine;
using UnityEngine.Serialization;

namespace _01.Scripts._03.Data
{
    [Serializable]
    public class CharacterInfo
    {
        public int id;
        public string name;
        public Sprite sprite;
        public int hp;
        public int ap;
        [TextArea] public string story;
        [TextArea] public string skillDescription;
        public List<int> skillValue;
        public Sprite recommendedWeapon;

        public CharacterInfo Clone()
        {
            return new CharacterInfo()
            {
                id = id,
                name = name,
                sprite = sprite,
                hp = hp,
                ap = ap,
                story = story,
                skillDescription = ValueFormula.GetFormattedSkillDescription(this, GameManager.Instance.playerData.characterGrade[id]),
                skillValue = new List<int>(skillValue),
                recommendedWeapon = recommendedWeapon
            };
        }
    }
    
    [CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObject/CharacterData")]
    public class CharacterData : ScriptableObject
    {
        public List<CharacterInfo> characterInfos;
    }
}
