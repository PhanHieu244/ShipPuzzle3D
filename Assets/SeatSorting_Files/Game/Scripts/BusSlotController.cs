namespace EKStudio
{
    using TMPro;
    using UnityEngine;
    
    public class BusSlotController : MonoBehaviour
    {
        public bool isLock;
        public float lockOpenLevel;
        public GameObject lockArea;
        public BusController currentBus;
        public GameObject point;
        GameManager manager => GameManager.Instance;
    
        void Start()
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
    
    
        public void SetBus(BusController busController)
        {
            currentBus = busController;
        }
    
        public void ClearSlot()
        {
            currentBus = null;
        }
    }
    
}
