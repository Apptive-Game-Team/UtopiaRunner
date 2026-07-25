using System;
using _01.Scripts._00.Manager;
using _01.Scripts._05.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _01.Scripts._04.UI
{
    public class StageClearButtonUI : MonoBehaviour
    {
        public void OnClickNextStageButton()
        {
            Time.timeScale = 1f;

            int currentStageIndex = (int)StageManager.Instance.selectedStageNum;
            int nextStageIndex = currentStageIndex + 1;

            if (!Enum.IsDefined(typeof(StageNum), nextStageIndex))
            {
                SceneManager.LoadScene(SceneInfo.SceneNames[SceneName.Lobby]);
                return;
            }

            StageManager.Instance.selectedStageNum = (StageNum)nextStageIndex;

            GameManager.Instance.SaveSelected();

            SceneManager.LoadScene(SceneInfo.SceneNames[SceneName.InGame]);
        }

        public void OnClickLobbyButton()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneInfo.SceneNames[SceneName.Lobby]);
        }

        public void OnClickRetryButton()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneInfo.SceneNames[SceneName.InGame]);
        }
    }
}