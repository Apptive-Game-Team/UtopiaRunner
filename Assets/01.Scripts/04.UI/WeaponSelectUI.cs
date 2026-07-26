using System.Collections.Generic;
using _01.Scripts._00.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Scripts._04.UI
{
    public class WeaponSelectUI : MonoBehaviour
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

        private int _maxWeaponCount;
        private List<bool> _unLockedWeapons;
        private Button _selectedWeaponButton;

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
                Button button = Instantiate(weaponButtonPrefab.gameObject, content.transform).GetComponent<Button>();
                Image image = button.transform.GetChild(1).GetComponent<Image>();
                int index = i;

                image.sprite = weaponData.weaponInfos[i].sprite;

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
                    
                    weaponName.text = weaponData.weaponInfos[index].name;
                    weaponImage.sprite = weaponData.weaponInfos[index].sprite;
                    weaponCharacteristic.text = weaponData.weaponInfos[index].characteristic;
                    weaponSkillDescription.text = weaponData.weaponInfos[index].skillDescription;
                    recommendedCharacter.text = $"추천 캐릭터 : {weaponData.weaponInfos[index].recommendedCharacter}";

                    selectButton.onClick.RemoveAllListeners();
                    selectButton.onClick.AddListener(() =>
                    {
                        HighlightButton(button);
                        StageManager.Instance.selectedWeapon = index; 
                    });
                });

                if (index == StageManager.Instance.selectedWeapon)
                {
                    HighlightButton(button);
                }
            }
        }

        private void HighlightButton(Button button)
        {
            if (_selectedWeaponButton != null)
            {
                _selectedWeaponButton.transform.GetChild(0).gameObject.SetActive(false);
            }
            _selectedWeaponButton = button;
            _selectedWeaponButton.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
