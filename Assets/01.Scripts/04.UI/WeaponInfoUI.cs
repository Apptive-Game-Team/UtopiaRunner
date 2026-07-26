using System.Collections.Generic;
using _01.Scripts._00.Manager;
using _01.Scripts._05.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _01.Scripts._04.UI
{
    public class WeaponInfoUI : MonoBehaviour
    {
        [SerializeField] private WeaponData weaponData;
        [SerializeField] private Button weaponButtonPrefab;
        [SerializeField] private Sprite lockedImage;
        [SerializeField] private CanvasGroup weaponInfoGroup;
        [SerializeField] private TextMeshProUGUI weaponName;
        [SerializeField] private Image weaponImage;
        [SerializeField] private TextMeshProUGUI weaponCharacteristic;
        [SerializeField] private TextMeshProUGUI weaponSkillDescription;
        [SerializeField] private TextMeshProUGUI upgradeStat;
        [SerializeField] private TextMeshProUGUI recommendedCharacter;
        [SerializeField] private GameObject content;
        [SerializeField] private Button selectButton;
        [SerializeField] private Button upgradeButton;

        private int _maxWeaponCount;
        private List<bool> _unLockedWeapons;

        private void Awake()
        {
            InitialSetting();
        }

        private void InitialSetting()
        {
            _maxWeaponCount = weaponData.weaponInfos.Count;
            _unLockedWeapons = GameManager.Instance.playerData.unlockedWeapons;

            for (int i = 0; i < _maxWeaponCount; i++)
            {
                PlayerData playerData = GameManager.Instance.playerData;
                
                Button button = Instantiate(weaponButtonPrefab.gameObject, content.transform).GetComponent<Button>();
                Image image = button.transform.GetChild(1).GetComponent<Image>();
                int index = i;
                WeaponInfo weaponInfo = weaponData.weaponInfos[i].Clone();
                
                image.sprite = weaponInfo.sprite;

                if (!_unLockedWeapons[index])
                {
                    image.sprite = lockedImage;
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() =>
                    {
                        weaponInfoGroup.alpha = 0;
                    });
                    continue;
                }
                
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    weaponInfoGroup.alpha = 1;
                    
                    weaponName.text = weaponInfo.name;
                    weaponImage.sprite = weaponInfo.sprite;
                    weaponCharacteristic.text = weaponInfo.characteristic;
                    weaponSkillDescription.text = weaponInfo.skillDescription;
                    recommendedCharacter.text = $"추천 캐릭터 : {weaponInfo.recommendedCharacter}"; 
                    
                    selectButton.onClick.RemoveAllListeners();
                    selectButton.onClick.AddListener(() =>
                    {
                        StageManager.Instance.selectedWeapon = index;
                    });
                    
                    if (playerData.weaponGrade[index] >= ValueFormula.WeaponMaxLevel)
                    {
                        upgradeButton.interactable = false;
                    }
                    else
                    {
                        upgradeButton.interactable = true;
                        
                        upgradeButton.onClick.RemoveAllListeners();
                        upgradeButton.onClick.AddListener(() =>
                        {
                            if (playerData.weaponGrade[index] >= ValueFormula.WeaponMaxLevel)
                            {
                                return;
                            }

                            if (!GoldManager.Instance)
                            {
                                return;
                            }

                            if (!GoldManager.Instance.TrySpendGold(ValueFormula.GetWeaponUpgradeGold(index)))
                            {
                                return;
                            }

                            playerData.weaponGrade[index]++;
                            weaponInfo = weaponData.weaponInfos[index].Clone();
                            weaponSkillDescription.text =
                                ValueFormula.GetFormattedSkillDescription(weaponInfo, playerData.weaponGrade[index]);
                            
                            if (playerData.weaponGrade[index] >= ValueFormula.WeaponMaxLevel)
                            {
                                upgradeButton.interactable = false;
                            }
                            
                            GameManager.Instance.SaveGame();
                        });
                    }
                });
            }
        }
    }
}
