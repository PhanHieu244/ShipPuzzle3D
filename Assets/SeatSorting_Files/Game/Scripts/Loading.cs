namespace EKStudio
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using DG.Tweening;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Random = UnityEngine.Random;
    
    public class Loading : MonoBehaviour
    {
        public GameData gameData;
        public float loadDuration;
    
        private void Awake()
        {
        }
    
        void Start()
        {
            SaveManager.LoadData(gameData);
    
            if (gameData.levelCount <= gameData.maxLevel)
                gameData.openLevel = gameData.levelCount;
            else
                gameData.openLevel = Random.Range(10, gameData.maxLevel +1);
    
            DOVirtual.DelayedCall(loadDuration, () => SceneManager.LoadScene(gameData.openLevel));
        }
    
    }
    
}
