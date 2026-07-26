using System.Collections.Generic;
using _01.Scripts._00.Manager;
using _01.Scripts._04.UI;
using UnityEngine;
using CharacterInfo = _01.Scripts._03.Data.CharacterInfo;

namespace _01.Scripts._05.Utility
{
    public static class ValueFormula
    {
        // 캐릭터 스킬 설명
        public static string GetFormattedSkillDescription(CharacterInfo character, int level)
        {
            switch (character.id)
            {
                case 0: // 이카루스
                    float cooldown1 = character.skillValue[0] + AddUpgradeValue(1.1f, 0.1f, level);
                    return string.Format(character.skillDescription, cooldown1);

                case 1: // 이브
                    float heal2 = character.skillValue[0] + 0.5f * level;
                    float damage2 = character.skillValue[1] + AddUpgradeValue(1.1f, 0.1f, level);
                    return string.Format(character.skillDescription, heal2, damage2);

                case 2: // 하니
                    float duration3 = character.skillValue[0] + AddUpgradeValue(1.1f, 0.1f, level);
                    return string.Format(character.skillDescription, duration3);
                
                case 3: // 맥
                    float damage4 = character.skillValue[0] + AddUpgradeValue(1.1f, 0.1f, level) * 2;
                    return string.Format(character.skillDescription, damage4);
                
                case 4: // 모트
                    float damage5 = character.skillValue[0] + AddUpgradeValue(1.1f, 0.1f, level);
                    return string.Format(character.skillDescription, damage5);
                
                case 5: // 카록
                    float attackSpeed6 = character.skillValue[0] + AddUpgradeValue(1.1f, 0.1f, level);
                    float attackHeal6 = character.skillValue[1] + AddUpgradeValue(1.1f, 0.1f, level);
                    return string.Format(character.skillDescription, attackSpeed6, attackHeal6);
            }

            return character.skillDescription;
        }
        
        // 무기 스킬 설명
        public static string GetFormattedSkillDescription(WeaponInfo weapon, int level)
        {
            switch (weapon.id)
            {
                case 0: // 플라즈마 건
                    float damage1 = weapon.skillValue[0] + 25f * level;
                    return string.Format(weapon.skillDescription, damage1);
                case 1: // 저격총
                    float damage21 = weapon.skillValue[0] + 25f * level;
                    float damage22 = weapon.skillValue[0] + 12.5f * level;
                    return string.Format(weapon.skillDescription, damage21, damage22);
                case 2: // 미사일 포드
                    int count3 = weapon.skillValue[0] + 2 * level;
                    return string.Format(weapon.skillDescription, count3);
                case 3: // 드론 런처
                    int count4 = level == WeaponMaxLevel ? weapon.skillValue[0] + 1 : weapon.skillValue[0];
                    float attackSpeed4 = weapon.skillValue[1] + 10 * level;
                    return string.Format(weapon.skillDescription, count4, attackSpeed4);
                case 4: // RPG
                    float damage51 = weapon.skillValue[0] + 500 * level;
                    float damage52 = weapon.skillValue[1] + level;
                    return string.Format(weapon.skillDescription, damage51, damage52);
                case 5: // 차원 생성기
                    float time6 = weapon.skillValue[0] + level;
                    return string.Format(weapon.skillDescription, time6);
            }
            
            return weapon.skillDescription;
        }
        
        // 캐릭터 스킬 수치
        public static List<float> GetCharacterSkillValues(CharacterInfo character, int level)
        {
            List<float> ap = new List<float>();

            switch (character.id)
            {
                case 0:
                    ap.Add(character.skillValue[0] + AddUpgradeValue(1.1f, 0.1f, level));
                    break;
                case 1:
                    ap.Add(character.skillValue[0] + 0.5f * level);
                    ap.Add(character.skillValue[1] + AddUpgradeValue(1.1f, 0.1f, level));
                    break;
                case 2:
                    ap.Add(character.skillValue[0] + AddUpgradeValue(1.1f, 0.1f, level));
                    break;
                case 3:
                    ap.Add(character.skillValue[0] + AddUpgradeValue(1.1f, 0.1f, level) * 2);
                    break;
                case 4:
                    ap.Add(character.skillValue[0] + AddUpgradeValue(1.1f, 0.1f, level));
                    break;
                case 5:
                    ap.Add(character.skillValue[0] + AddUpgradeValue(1.1f, 0.1f, level));
                    ap.Add(character.skillValue[1] + AddUpgradeValue(1.1f, 0.1f, level));
                    break;
            }

            return ap;
        }

        // 무기 스킬 수치
        public static List<float> GetWeaponSkillValues(WeaponInfo weapon, int level)
        {
            List<float> ap = new List<float>();

            switch (weapon.id)
            {
                case 0:
                    ap.Add(weapon.skillValue[0] + 25f * level);
                    break;
                case 1:
                    ap.Add(weapon.skillValue[0] + 25f * level);
                    ap.Add(weapon.skillValue[0] + 12.5f * level);
                    break;
                case 2:
                    ap.Add(weapon.skillValue[0] + 2 * level);
                    break;
                case 3:
                    int count4 = level == WeaponMaxLevel ? weapon.skillValue[0] + 1 : weapon.skillValue[0];
                    ap.Add(count4);
                    ap.Add(weapon.skillValue[1] + 10 * level);
                    break;
                case 4:
                    ap.Add(weapon.skillValue[0] + 500 * level);
                    ap.Add(weapon.skillValue[1] + level);
                    break;
                case 5:
                    ap.Add(weapon.skillValue[0] + level);
                    break;
            }

            return ap;
        }

        private static float AddUpgradeValue(float start, float decrease, int level)
        {
            float value = 0f;

            for (int i = 0; i < level; i++)
            {
                value += start - (i + 1) * decrease;
            }

            return value;
        }

        public static float EnemyAttackSpeed = 1f;
        public static void SetEnemyAttackSpeed(float speed)
        {
            EnemyAttackSpeed = speed;
        }

        public const int CharacterMaxLevel = 4;
        public const int CharacterUpgradeGold = 50;
        public const int CharacterHpUpgradeAmount = 10;
        public const int CharacterApUpgradeAmount = 1;

        public const int WeaponMaxLevel = 4;
        public const int WeaponUpgradeGold = 100;
        
        public static int GetCharacterUpgradeGold(int characterId)
        {
            return CharacterUpgradeGold * (GameManager.Instance.playerData.characterGrade[characterId] + 1);
        }

        public static int GetCharacterHp(CharacterInfo characterInfo)
        {
            return characterInfo.hp + GameManager.Instance.playerData.characterGrade[characterInfo.id] * CharacterHpUpgradeAmount;
        }

        public static int GetCharacterAp(CharacterInfo characterInfo)
        {
            return characterInfo.ap + GameManager.Instance.playerData.characterGrade[characterInfo.id] * CharacterApUpgradeAmount;
        }

        public static int GetWeaponUpgradeGold(int weaponId)
        {
            return WeaponUpgradeGold * (GameManager.Instance.playerData.weaponGrade[weaponId] + 1);
        }
    }
}
