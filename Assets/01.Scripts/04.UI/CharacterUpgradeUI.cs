using _01.Scripts._00.Manager;
using _01.Scripts._03.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Scripts._04.UI
{
    public class CharacterUpgradeUI : MonoBehaviour
    {
        [SerializeField] private CharacterData characterData;
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private Image characterImage;
        [SerializeField] private TextMeshProUGUI upgradeStat;
        [SerializeField] private Button characterSelectButton;
        [SerializeField] private Button characterUpgradeButton;
        [SerializeField] private TextMeshProUGUI characterStory;
        [SerializeField] private TextMeshProUGUI characterSkillDescription;
        [SerializeField] private Image characterWeapon;

        [Header("Arrow Buttons")]
        [SerializeField] private Button prevButton;
        [SerializeField] private Button nextButton;

        [Header("Upgrade")]
        [SerializeField] private int upgradeGoldCost = 100;

        public int characterIndex;

        private _01.Scripts._03.Data.CharacterInfo _characterInfo;

        private void OnEnable()
        {
            if (prevButton != null)
            {
                prevButton.onClick.RemoveAllListeners();
                prevButton.onClick.AddListener(OnClickPrevButton);
            }

            if (nextButton != null)
            {
                nextButton.onClick.RemoveAllListeners();
                nextButton.onClick.AddListener(OnClickNextButton);
            }

            RefreshUI();
        }

        private void OnClickPrevButton()
        {
            do
            {
                characterIndex--;

                if (characterIndex < 0)
                {
                    characterIndex = characterData.characterInfos.Count - 1;
                }
            } while (!GameManager.Instance.playerData.unlockedCharacters[characterIndex]);

            RefreshUI();
        }

        private void OnClickNextButton()
        {
            do
            {
                characterIndex++;

                if (characterIndex >= characterData.characterInfos.Count)
                {
                    characterIndex = 0;
                }
            } while (!GameManager.Instance.playerData.unlockedCharacters[characterIndex]);

            RefreshUI();
        }

        private void RefreshUI()
        {
            _characterInfo = characterData.characterInfos[characterIndex].Clone();

            characterImage.sprite = _characterInfo.sprite;
            characterNameText.text = _characterInfo.name;
            characterStory.text = _characterInfo.story;
            characterSkillDescription.text = _characterInfo.skillDescription;
            characterWeapon.sprite = _characterInfo.recommendedWeapon;

            UpdateStatText();
            RefreshSelectButton();
            RefreshUpgradeButton();
        }

        private void RefreshSelectButton()
        {
            characterSelectButton.onClick.RemoveAllListeners();

            if (StageManager.Instance.selectedCharacters[0] == characterIndex ||
                StageManager.Instance.selectedCharacters[1] == characterIndex)
            {
                characterSelectButton.interactable = false;
            }
            else
            {
                characterSelectButton.interactable = true;

                characterSelectButton.onClick.AddListener(() =>
                {
                    StageManager.Instance.selectedCharacters[
                        StageManager.Instance.selectedCharacterIdx
                    ] = characterIndex;
                });
            }
        }

        private void RefreshUpgradeButton()
        {
            PlayerData playerData = GameManager.Instance.playerData;

            characterUpgradeButton.onClick.RemoveAllListeners();

            if (playerData.characterGrade[characterIndex] >= _characterInfo.apList.Count - 1)
            {
                characterUpgradeButton.interactable = false;
                return;
            }

            characterUpgradeButton.interactable = true;

            characterUpgradeButton.onClick.AddListener(() =>
            {
                if (GoldManager.Instance == null)
                {
                    return;
                }

                if (!GoldManager.Instance.TrySpendGold(upgradeGoldCost))
                {
                    return;
                }

                if (GameManager.Instance.playerData.characterGrade[characterIndex] == 4)
                {
                    return;
                }

                playerData.characterGrade[characterIndex] =
                    Mathf.Min(
                        _characterInfo.apList.Count - 1,
                        playerData.characterGrade[characterIndex] + 1
                    );

                GameManager.Instance.SaveGame();

                RefreshUI();
            });
        }

        private void UpdateStatText()
        {
            PlayerData playerData = GameManager.Instance.playerData;

            int grade = playerData.characterGrade[characterIndex];

            upgradeStat.text = $"Hp : {_characterInfo.hpList[grade]}";

            if (grade < _characterInfo.hpList.Count - 1)
            {
                upgradeStat.text +=
                    $"<color=grey>({_characterInfo.hpList[grade + 1]})</color>";
            }

            upgradeStat.text += $" / Ap : {_characterInfo.apList[grade]}";

            if (grade < _characterInfo.apList.Count - 1)
            {
                upgradeStat.text +=
                    $"<color=grey>({_characterInfo.apList[grade + 1]})</color>";
            }
        }
    }
}