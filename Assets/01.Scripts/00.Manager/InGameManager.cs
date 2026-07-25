using System;
using System.Collections;
using System.Linq;
using _01.Scripts._03.Data;
using _01.Scripts._04.UI;
using _01.Scripts._07.Character;
using _01.Scripts._06.Weapon;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _01.Scripts._00.Manager
{
    public class InGameManager : MonoBehaviour
    {
        [Header("In Game Setting")] 
        [SerializeField] private CharacterData characterData;
        [SerializeField] private GameObject gameOverPrefab;
        [SerializeField] private WeaponData weaponData;
        [SerializeField] private GameObject[] characters;
        [SerializeField] private GameObject[] weapons;
        [SerializeField] private Vector2 startPosition;

        [Header("Tag Option")]
        [SerializeField] private float tagCooldown = 5f;
        private bool _canTag = true;

        public PlayerController mainCharacter;
        public PlayerController subCharacter;
        public WeaponController weapon;
        
        private SmoothHpBar _mainCharacterHp;
        private SmoothHpBar _subCharacterHp;
        private Image _weaponImage;
        private Slider _skillCoolTimeSlider;
        private HoverTrigger _mainCharacterHover;
        private HoverTrigger _subCharacterHover;

        private void Awake()
        {
            InitialCaching();
        }

        private void InitialCaching()
        {
            GameObject inGameUI = GameObject.Find("InGameUI");

            _mainCharacterHp = inGameUI.transform.GetChild(0).GetComponent<SmoothHpBar>();
            _subCharacterHp = inGameUI.transform.GetChild(1).GetComponent<SmoothHpBar>();
            _weaponImage = inGameUI.transform.GetChild(2).GetComponent<Image>();
            _skillCoolTimeSlider = inGameUI.transform.GetChild(3).GetComponent<Slider>();
        }

        private void Start()
        {
            InitialSetting();
        }

        private void InitialSetting()
        {
            InputManager.AddListener(ActionCode.Tag, InputType.Down, TagInput);
            
            var characterIds = StageManager.Instance.selectedCharacters;
            GameObject mainChar = characters.First(go => go.GetComponent<PlayerController>().id == characterIds[0]);
            GameObject subChar = characters.First(go => go.GetComponent<PlayerController>().id == characterIds[1]);

            var weaponId = StageManager.Instance.selectedWeapon;
            GameObject wp = weapons.First(go => go.GetComponent<WeaponController>().weaponId == weaponId);
            
            mainCharacter = Instantiate(mainChar, startPosition, Quaternion.identity).GetComponent<PlayerController>();
            mainCharacter.characterInfo = characterData.characterInfos.First(info => info.id == mainCharacter.id);
            
            subCharacter = Instantiate(subChar, startPosition, Quaternion.identity).GetComponent<PlayerController>();
            subCharacter.characterInfo = characterData.characterInfos.First(info => info.id == subCharacter.id);
            subCharacter.gameObject.SetActive(false);
            
            weapon = Instantiate(wp, mainCharacter.transform).GetComponent<WeaponController>();
            weapon.weaponInfo = weaponData.weaponInfos[weaponId].Clone();
            
            mainCharacter.Init();
            subCharacter.Init();
            
            weapon.Initialize(mainCharacter.damage);
            _weaponImage.sprite = weapon.weaponInfo.sprite;
            
            HoverTrigger weaponHover = _weaponImage.gameObject.AddComponent<HoverTrigger>();
            weaponHover.SetTooltipData($"{weapon.weaponInfo.name}     LV{GameManager.Instance.playerData.weaponGrade[weapon.weaponInfo.id]}\n",
                $"{weapon.weaponInfo.characteristic}\n\n" + $"{weapon.weaponInfo.skillDescription}\n\n" +
                $"공격력 : {weapon.weaponInfo.apList[GameManager.Instance.playerData.weaponGrade[weapon.weaponInfo.id]]} ");
            
            mainCharacter.AfterInit();
            subCharacter.AfterInit();

            Image mainCharacterImage = _mainCharacterHp.GetComponentInChildren<Image>();
            Image subCharacterImage = _subCharacterHp.GetComponentInChildren<Image>();
            mainCharacterImage.sprite = mainCharacter.characterInfo.sprite;
            subCharacterImage.sprite = subCharacter.characterInfo.sprite;
            
            _mainCharacterHover = mainCharacterImage.gameObject.AddComponent<HoverTrigger>();
            _subCharacterHover = subCharacterImage.gameObject.AddComponent<HoverTrigger>();
            
            RefreshCharacterTooltipData();

            mainCharacter.OnHpChanged += UpdateHpUI;
            subCharacter.OnHpChanged += UpdateHpUI;
            UpdateHpUI(false);

            mainCharacter.OnDead += CheckDead;
            subCharacter.OnDead += CheckDead;
            
            weapon.OnCoolDownChanged += SetSkillCoolTime;
        }
        
        private void RefreshCharacterTooltipData()
        {
            if (_mainCharacterHover != null && mainCharacter != null)
            {
                _mainCharacterHover.SetTooltipData(
                    $"{mainCharacter.characterInfo.name}     LV{GameManager.Instance.playerData.characterGrade[mainCharacter.characterInfo.id]}\n",
                    $"{mainCharacter.characterInfo.story}\n\n{mainCharacter.characterInfo.skillDescription}\n\n" +
                    $"공격력 : {mainCharacter.characterInfo.apList[GameManager.Instance.playerData.characterGrade[mainCharacter.characterInfo.id]]} " +
                    $"체력 : {mainCharacter.characterInfo.hpList[GameManager.Instance.playerData.characterGrade[mainCharacter.characterInfo.id]]}");
            }

            if (_subCharacterHover != null && subCharacter != null)
            {
                _subCharacterHover.SetTooltipData(
                    $"{subCharacter.characterInfo.name}     LV{GameManager.Instance.playerData.characterGrade[subCharacter.characterInfo.id]}\n",
                    $"{subCharacter.characterInfo.story}\n\n{subCharacter.characterInfo.skillDescription}\n\n" +
                    $"공격력 : {subCharacter.characterInfo.apList[GameManager.Instance.playerData.characterGrade[subCharacter.characterInfo.id]]} " +
                    $"체력 : {subCharacter.characterInfo.hpList[GameManager.Instance.playerData.characterGrade[subCharacter.characterInfo.id]]}");
            }
        }
        
        private void UpdateHpUI()
        {
            _mainCharacterHp.UpdateHp(mainCharacter.hp, mainCharacter.maxHp, true);
            _subCharacterHp.UpdateHp(subCharacter.hp, subCharacter.maxHp, true);
        }

        private void UpdateHpUI(bool setSmooth)
        {
            _mainCharacterHp.UpdateHp(mainCharacter.hp, mainCharacter.maxHp, setSmooth);
            _subCharacterHp.UpdateHp(subCharacter.hp, subCharacter.maxHp, setSmooth);
        }

        private void SetSkillCoolTime(float current, float max)
        {
            float ratio = (max - current) / max;
            _skillCoolTimeSlider.value = ratio;

            if (current == 0)
            {
                // todo : 스킬 쿨타임 아이콘 번쩍? 이펙트 추가
            }
        }

        private void TagInput()
        {
            if (_canTag)
            {
                Tag();
                StartCoroutine(TagCooldown());
            }
        }

        private void Tag()
        {
            HoverUI.Instance?.HideTooltip();

            subCharacter.gameObject.SetActive(true);
            weapon.transform.SetParent(subCharacter.transform, false);
            mainCharacter.gameObject.SetActive(false);
            
            (mainCharacter, subCharacter) = (subCharacter, mainCharacter);
            
            _mainCharacterHp.GetComponentInChildren<Image>().sprite = mainCharacter.characterInfo.sprite;
            _subCharacterHp.GetComponentInChildren<Image>().sprite = subCharacter.characterInfo.sprite;
            
            RefreshCharacterTooltipData();

            UpdateHpUI(false);
            
            weapon.SetDamage(mainCharacter.GetComponent<PlayerController>().damage);
        }

        private IEnumerator TagCooldown()
        {
            _canTag = false;
            yield return new WaitForSeconds(tagCooldown);
            _canTag = true;
        }

        private void CheckDead()
        {
            _canTag = false;
            if (!subCharacter.isDead)
            {
                Tag();
            }
            else
            {
                mainCharacter.gameObject.SetActive(false);
                Instantiate(gameOverPrefab);
                Time.timeScale = 0;
            }
        }

        private void OnDestroy()
        {
            InputManager.RemoveListener(ActionCode.Tag, InputType.Down, TagInput);
            
            weapon.OnCoolDownChanged -= SetSkillCoolTime;
        }
    }
}