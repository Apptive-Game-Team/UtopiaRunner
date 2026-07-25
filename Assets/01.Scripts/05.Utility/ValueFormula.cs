using System.Collections.Generic;
using _01.Scripts._00.Manager;
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

        private static float AddUpgradeValue(float start, float decrease, int level)
        {
            float value = 0f;

            for (int i = 0; i < level; i++)
            {
                value += start - (i + 1) * decrease;
            }

            return value;
        }

        public static float enemyAttackSpeed = 1f;
        public static void SetEnemyAttackSpeed(float speed)
        {
            enemyAttackSpeed = speed;
        }

        public static int GetCharacterUpgradeGold(int characterId)
        {
            return 50 * (GameManager.Instance.playerData.characterGrade[characterId] + 1);
        }
    }
}
