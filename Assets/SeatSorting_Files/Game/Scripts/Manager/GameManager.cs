namespace EKStudio
{
    using System.Collections.Generic;
    using System.Linq;
    using DG.Tweening;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UIElements;
    
    [DefaultExecutionOrder(-20)]
    public class GameManager : InstanceManager<GameManager>
    {
    
    
        public GameData gameData;
        public bool isFinished, isFirstSeatMoved;
        public List<BusController> allbusControllers;
        public List<SeatController> allseatControllers;
        public int busCountInScene, busCountInTunnel, typeCount;
        public List<TunnelManager> allTunnels;
        public List<int> busCountsInTunnels, typeCountList;
        public GameObject busPrefab;
    
        //public CinemachineVirtualCamera virtualCamera;
    
        private void Awake()
        {
            allbusControllers = new List<BusController>(FindObjectsOfType<BusController>());
            busCountInScene = allbusControllers.Count(bus => !bus.isInTunnel);
    
            allTunnels = new List<TunnelManager>(FindObjectsOfType<TunnelManager>());
            int minCount = gameData.levelCount < 15 ? 2 : 5;
            int maxCount = gameData.levelCount > 30 ? 15 : gameData.levelCount / 2;
    
            typeCountList = new List<int> { 2, 2, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 7 };
            typeCount = gameData.levelCount <= typeCountList.Count ? typeCountList[gameData.levelCount-1]: typeCountList[^1] ;
    
    
            if (allTunnels.Count > 0)
            {
                for (int i = 0; i < allTunnels.Count; i++)
                {
                    int count = Random.Range(minCount, maxCount);
                    count = count > 9 ? 9 : count;
                    busCountsInTunnels.Add(count);
                    busCountInTunnel += count;
                }
            }
            else
            {
                busCountInTunnel = 0;
            }
    
            Vibration.Init();
    
        }
    
        void Start()
        {
            EventManager.Broadcast(GameEvent.OnStart);
            isFinished = false;
            InvokeRepeating(nameof(SaveData), 1f, 1f);
    
            //allseatControllers = new List<SeatController>(FindObjectsOfType<SeatController>());
        }
    
        void Update()
        {
    
        }
    
        public void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    
        public void NextScene()
        {
            gameData.totalMoney += gameData.levelMoney;// * gameData.levelCount;
            gameData.levelCount++;
    
            if (gameData.levelCount <= gameData.maxLevel)
                gameData.openLevel = gameData.levelCount;
            else
                gameData.openLevel = Random.Range(10, gameData.maxLevel + 1);
    
            SaveManager.SaveData(gameData);
            DOVirtual.DelayedCall(0.1f, () => SceneManager.LoadScene(gameData.openLevel));
        }
    
        public void SaveData()
        {
            SaveManager.SaveData(gameData);
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
            Time.timeScale = 1;
        }
    
        void OnWin()
        {
            //EventManager.Broadcast(GameEvent.OnPlaySound, "Win");
            isFinished = true;
        }
    
        void OnLose()
        {
            //EventManager.Broadcast(GameEvent.OnPlaySound, "Fail");
            isFinished = true;
        }
    }
    
    
}
