using System.Collections.Generic;
using System.Linq;
using _01.Scripts._00.Manager;
using _01.Scripts._03.Data;
using _01.Scripts._05.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Scripts._04.UI
{
    public class SimpleSelectUI : MonoBehaviour
    {
        [SerializeField] private CharacterData characterData;
        [SerializeField] private WeaponData weaponData;
        [SerializeField] private SelectedCharacterUI[] selectedCharacters;
        [SerializeField] private Image selectedWeapon;
        [SerializeField] private GameObject characterButton;
        [SerializeField] private GameObject weaponButton;
        [SerializeField] private Sprite lockedImage;
        [SerializeField] private GameObject characterContent;
        [SerializeField] private GameObject weaponContent;

        private SelectedCharacterUI _selectedCharacter;
        private int _maxCharacterCount;
        private int _maxWeaponCount;
        private List<bool> _unlockedCharacters;
        private List<bool> _unLockedWeapons;

        private void Awake()
        {
            InitialSetting();
        }

        private void OnEnable()
        {
            InputManager.AddListener(ActionCode.Tag, InputType.Down, RegisterTag);
        }

        private void OnDisable()
        {
            InputManager.RemoveListener(ActionCode.Tag, InputType.Down, RegisterTag);
        }

        private void InitialSetting()
        {
            // 해금 리스트 관리
            _maxCharacterCount = characterData.characterInfos.Count;
            _unlockedCharacters = GameManager.Instance.playerData.unlockedCharacters;
            
            _maxWeaponCount = weaponData.weaponInfos.Count;
            _unLockedWeapons = GameManager.Instance.playerData.unlockedWeapons;
            
            // 선택된 캐릭터 관리
            _selectedCharacter = selectedCharacters[0];
            for (int i = 0; i < selectedCharacters.Length; i++)
            {
                int index = i;
                SelectedCharacterUI slot = selectedCharacters[index];

                if (StageManager.Instance.selectedCharacters[i] != -1)
                {
                    int characterIndex = StageManager.Instance.selectedCharacters[i];
                    slot.UpdateUI(characterIndex, characterData.characterInfos[characterIndex].sprite, characterData.characterInfos[characterIndex].name);
    
                    CanvasGroup canvasGroup = slot.GetComponentInChildren<CanvasGroup>();
                    canvasGroup.alpha = 1;
                    canvasGroup.interactable = true;
                }

                slot.tagButton.onClick.AddListener(() =>
                {
                    if (DG.Tweening.DOTween.IsTweening(slot.transform))
                    {
                        return;
                    }
                    
                    SwitchCard();
                });
            }
            

            // 캐릭터 리스트 버튼 관리
            for (int i = 0; i < _maxCharacterCount; i++)
            {
                Button button = Instantiate(characterButton.gameObject, characterContent.transform).GetComponent<Button>();
                Image characterImage = button.transform.GetChild(0).GetComponent<Image>();
                TextMeshProUGUI characterName = button.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                int index = i;

                _03.Data.CharacterInfo characterInfo = characterData.characterInfos[index].Clone();
                characterImage.sprite = characterInfo.sprite;
                characterName.text = characterInfo.name;

                if (!_unlockedCharacters[index])
                {
                    characterImage.sprite = lockedImage;
                    characterName.text = "???";
                    continue;
                }
                
                HoverTrigger hover = button.gameObject.AddComponent<HoverTrigger>();
                hover.SetTooltipData($"{characterInfo.name}     LV{GameManager.Instance.playerData.characterGrade[characterInfo.id]}\n",
                    $"{characterInfo.story}\n\n" + $"{characterInfo.skillDescription}\n\n" +
                    $"공격력 : {ValueFormula.GetCharacterAp(characterInfo)} " +
                    $"체력 : {ValueFormula.GetCharacterHp(characterInfo)}");

                
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    SelectedCharacterUI otherSlot = selectedCharacters[0] == _selectedCharacter
                        ? selectedCharacters[1] : selectedCharacters[0];
                    if (otherSlot.CharacterIndex == index)
                    {
                        return;
                    }
                    
                    int selectedSlot = _selectedCharacter == selectedCharacters[0] 
                        ? 0 : 1;
                    StageManager.Instance.selectedCharacters[selectedSlot] = index;
                    
                    _selectedCharacter.UpdateUI(index, characterInfo.sprite, characterInfo.name);
    
                    CanvasGroup canvasGroup = _selectedCharacter.GetComponentInChildren<CanvasGroup>();
                    canvasGroup.alpha = 1;
                    canvasGroup.interactable = true;
                });
            }
            
            // 무기 리스트 버튼 관리
            for (int i = 0; i < _maxWeaponCount; i++)
            {
                Button button = Instantiate(weaponButton.gameObject, weaponContent.transform).GetComponent<Button>();
                Image image = button.transform.GetChild(1).GetComponent<Image>();
                int index = i;

                WeaponInfo weaponInfo = weaponData.weaponInfos[index];
                image.sprite = weaponData.weaponInfos[i].sprite;

                if (!_unLockedWeapons[index])
                {
                    image.sprite = lockedImage;
                    continue;
                }
                
                HoverTrigger hover = button.gameObject.AddComponent<HoverTrigger>();
                hover.SetTooltipData($"{weaponInfo.name}     LV{GameManager.Instance.playerData.weaponGrade[weaponInfo.id]}\n",
                    $"{weaponInfo.characteristic}\n\n" + $"{weaponInfo.skillDescription}\n\n" +
                    $"공격력 : {weaponInfo.apList[GameManager.Instance.playerData.weaponGrade[weaponInfo.id]]} ");
                
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    selectedWeapon.sprite = weaponData.weaponInfos.Find(w => w.id == index).sprite;
                    StageManager.Instance.selectedWeapon = index;
                });
            }
            
            
        }
        
        private void SwitchCard()
        {
            SelectedCharacterUI currentFront = _selectedCharacter;
            SelectedCharacterUI currentBack = selectedCharacters.First(sc => sc != _selectedCharacter);
            
            currentBack.AnimateToForward(() =>
            {
                currentBack.transform.SetAsLastSibling(); 
            });
            
            currentFront.AnimateToBackward();
            
            _selectedCharacter = currentBack;
        }

        private void RegisterTag()
        {
            if (DG.Tweening.DOTween.IsTweening(_selectedCharacter.transform))
            {
                return;
            }
            
            SwitchCard();
        }
    }
}
