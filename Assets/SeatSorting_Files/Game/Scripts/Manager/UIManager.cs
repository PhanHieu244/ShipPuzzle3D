namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;
    using TMPro;
    using UnityEngine.Playables;
    
    public class UIManager : InstanceManager<UIManager>
    {
        public GameObject winPanel, losePanel;
        public Button nextButton, restartButton, freeReviveButton, replayButton;
        public TextMeshProUGUI totalMoneyUI, levelMoneyUI, levelCountUI, failLevelText;
        GameManager manager => GameManager.Instance;
    
    
        private void Awake()
        {
            nextButton.onClick.AddListener(manager.NextScene);
            restartButton.onClick.AddListener(manager.RestartScene);
            replayButton.onClick.AddListener(manager.RestartScene);
            levelMoneyUI.text = manager.gameData.levelMoney.ToString(); //* manager.gameData.levelCount).ToString();
            levelCountUI.text = "Level " + manager.gameData.levelCount.ToString("0");
            failLevelText.text = "Level " + manager.gameData.levelCount.ToString("0") + " Failed!";
        }
    
        void Start()
        {
            InvokeRepeating(nameof(UpdateText), 0f, 1f);
            EventManager.Broadcast(GameEvent.OnSoundStart, "music");
        }
    
        public void UpdateText()
        {
            totalMoneyUI.text = manager.gameData.totalMoney.ToString("0");
        }
    
    
        void OnEnable()
        {
            EventManager.AddHandler(GameEvent.OnStart, OnStart);
            EventManager.AddHandler(GameEvent.OnWin, OnWin);
            EventManager.AddHandler(GameEvent.OnLose, OnLose);
        }
    
        void OnDisable()
        {
            EventManager.RemoveHandler(GameEvent.OnStart, OnStart);
            EventManager.RemoveHandler(GameEvent.OnWin, OnWin);
            EventManager.RemoveHandler(GameEvent.OnLose, OnLose);
        }
        void OnStart()
        {
            winPanel.SetActive(false);
            losePanel.SetActive(false);
        }
    
        void OnWin()
        {
            winPanel.SetActive(true);
            EventManager.Broadcast(GameEvent.OnPlaySound, "win");
        }
    
        void OnLose()
        {
            losePanel.SetActive(true);
            EventManager.Broadcast(GameEvent.OnPlaySound, "loose");
        }
    
        public void OnFail(int failNo)
        {
            
        }
    
    }
    
}
