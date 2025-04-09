namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class SeatController : MonoBehaviour
    {
        public BusSeatPassType seatType;
        public SeatSlotController currentSlot;
        public Color currentColor;
    
        private void Awake()
        {
            currentColor = GetComponentsInChildren<Renderer>()[1].material.color;
        }
    }
    
}
