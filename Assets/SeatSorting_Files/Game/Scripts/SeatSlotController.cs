namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    
    public class SeatSlotController : MonoBehaviour
    {
        public BusSeatPassType currentSeatType;
        public SeatController currentSeat;
    
        public bool isLock;
        public float lockOpenLevel;
        public GameObject lockArea;
    
        GameManager manager => GameManager.Instance;
    
        void Awake()
        {
            if (lockArea != null)
            {
                lockArea.GetComponentInChildren<TextMeshPro>().text = "Lvl. " + lockOpenLevel.ToString();
                if (manager.gameData.levelCount >= lockOpenLevel)
                {
                    isLock = false;
                    Destroy(lockArea);
                }
    
            }
        }
    
        //private void Awake()
        //{
        //    AddOrRemoveFromList();
        //}
    
        void CheckSeatType()
        {
            if (transform.childCount > 0 && lockArea == null)
            {
                currentSeat = transform.GetChild(0).GetComponent<SeatController>();
                currentSeatType = currentSeat.seatType;
                currentSeat.currentSlot = this;
    
            }
            else
            {
                currentSeatType = BusSeatPassType.None;
                currentSeat = null;
            }
        }
    
        void AddOrRemoveFromList()
        {
            if (isLock == false)
            {
                CheckSeatType();
    
                if (currentSeatType == BusSeatPassType.None && !SeatSlotManager.Instance.emptySeatSlotList.Contains(this))
                {
                    SeatSlotManager.Instance.emptySeatSlotList.Add(this);
                }
                else if (currentSeatType != BusSeatPassType.None && SeatSlotManager.Instance.emptySeatSlotList.Contains(this))
                {
                    SeatSlotManager.Instance.emptySeatSlotList.Remove(this);
                }
            }
    
        }
    
        private void OnEnable()
        {
            EventManager.AddHandler(GameEvent.SeatMoved, AddOrRemoveFromList);
        }
    
        private void OnDisable()
        {
            EventManager.RemoveHandler(GameEvent.SeatMoved, AddOrRemoveFromList);
        }
    
    }
    
}
