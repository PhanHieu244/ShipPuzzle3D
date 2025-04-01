namespace EKStudio
{
    using UnityEngine;
    using NaughtyAttributes;
    using System;
    
    [CreateAssetMenu(fileName = "GameData", order = 0)]
    public class GameData : ScriptableObject
    {
    
        public float totalMoney, levelMoney;
        public int levelCount, maxLevel, openLevel;
        public int priceShuffle, priceSort, priceVIP;
    
        public AllSO allSO;
    
        [Button]
        public void Reset()
        {
            totalMoney = 1000;
            levelMoney = 100;
            levelCount = 1;
            openLevel = 0;
            maxLevel = 26;
            priceShuffle = 750;
            priceSort = 750;
            priceVIP = 750;
            PlayerPrefs.DeleteAll();
        }
    
        [Button]
        public void Save()
        {
            SaveManager.SaveData(this);
        }
    }
    
    [Serializable]
    public class AllSO
    {
        public SeatTypeSO seatSO;
        public PassengerTypeSO passengerSO;
    }
    
}
