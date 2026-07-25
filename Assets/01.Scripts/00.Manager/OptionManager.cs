using System.Collections;
using _01.Scripts._04.UI;
using _01.Scripts._05.Utility;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _01.Scripts._00.Manager
{
    public class OptionManager : SingletonObject<OptionManager>
    {
        [SerializeField] private GameObject optionPanel;
        [SerializeField] private GameObject outPanel;
        [SerializeField] private GameObject lobbyConfirmPanel;

        [SerializeField] private GameObject lobbyButton;

        protected override void Awake()
        {
            base.Awake();

            optionPanel.SetActive(false);
            outPanel.SetActive(false);
            lobbyConfirmPanel.SetActive(false);

            UpdateLobbyButtonState();

            Time.timeScale = 1f;
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            UpdateLobbyButtonState();
            UpdatePauseState();
        }

        public void OnClickOptionButton()
        {
            if (optionPanel.activeSelf)
            {
                GameManager.Instance.SaveSound();
            }

            Image optionUI = optionPanel.transform.GetChild(0).GetComponent<Image>();
            if (!optionPanel.activeSelf)
            {
                optionPanel.SetActive(true);
                UIOpenEffect(optionUI);
                UpdateLobbyButtonState();
                UpdatePauseState();
            }
            else
            {
                StartCoroutine(UICloseEffect(optionUI));
            }
        }
        
        private void UIOpenEffect(Image image)
        {
            if (image.gameObject.activeSelf)
            {
                return;
            }
            
            CanvasGroup cg = image.GetComponentInChildren<CanvasGroup>();
            RectTransform rt = image.GetComponent<RectTransform>();

            cg.alpha = 0;
            rt.localScale = new Vector3(1f, 0.01f, 1f);
            rt.anchoredPosition = new Vector2(0, -1080f);
            
            image.gameObject.SetActive(true);
            
            Sequence seq =  DOTween.Sequence();
            seq.SetUpdate(true);
            seq.Append(rt.DOAnchorPos(Vector2.zero, 0.2f).SetEase(Ease.OutCubic));
            seq.Append(rt.DOScale(Vector2.one, 0.2f).SetEase(Ease.OutCubic));
            seq.Append(cg.DOFade(1, 0.1f).SetEase(Ease.OutCubic));
        }

        private IEnumerator UICloseEffect(Image image)
        {
            if (!image.gameObject.activeSelf)
            {
                yield break;
            }
            
            CanvasGroup cg = image.GetComponentInChildren<CanvasGroup>();
            RectTransform rt = image.GetComponent<RectTransform>();

            Sequence seq =  DOTween.Sequence();
            seq.SetUpdate(true);
            seq.Append(cg.DOFade(0, 0.1f).SetEase(Ease.OutCubic));
            seq.Append(rt.DOScale(new Vector3(1f, 0.01f, 1f), 0.2f).SetEase(Ease.OutCubic));
            seq.Append(rt.DOAnchorPos(new Vector2(0, -1080f), 0.2f).SetEase(Ease.OutCubic));
             
            yield return seq.WaitForCompletion();
            
            image.gameObject.SetActive(false);
            optionPanel.SetActive(false);
            UpdateLobbyButtonState();
            UpdatePauseState();
            
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;
            cg.alpha = 1;
        }

        public void OnClickOutYesButton()
        {
            Time.timeScale = 1f;

            GameManager.Instance.SaveGame();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void OnClickOutNoButton()
        {
            outPanel.SetActive(!outPanel.activeSelf);

            UpdatePauseState();
        }

        public void OnClickLobbyButton()
        {
            lobbyConfirmPanel.SetActive(true);

            UpdatePauseState();
        }

        public void OnClickLobbyYesButton()
        {
            Time.timeScale = 1f;

            lobbyConfirmPanel.SetActive(false);
            SceneManager.LoadScene(SceneInfo.SceneNames[SceneName.Lobby]);
        }

        public void OnClickLobbyNoButton()
        {
            lobbyConfirmPanel.SetActive(false);

            UpdatePauseState();
        }

        private void UpdateLobbyButtonState()
        {
            if (lobbyButton == null) return;

            bool isInGameScene = SceneManager.GetActiveScene().name == SceneInfo.SceneNames[SceneName.InGame];

            lobbyButton.SetActive(isInGameScene);
        }

        private void UpdatePauseState()
        {
            bool isOptionOpen = optionPanel.activeSelf;
            bool isOutOpen = outPanel.activeSelf;
            bool isLobbyConfirmOpen = lobbyConfirmPanel.activeSelf;

            if (isOptionOpen || isOutOpen || isLobbyConfirmOpen)
                Time.timeScale = 0f;
            else
                Time.timeScale = 1f;
        }
    }
}