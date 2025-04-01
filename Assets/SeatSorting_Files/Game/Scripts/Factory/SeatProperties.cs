namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public abstract class SeatProperties : MonoBehaviour
    {
        private GameObject factoryProduct;
        public GameObject FactoryProduct
        {
            get { return factoryProduct; }
            set { factoryProduct = value; }
        }
    
        public abstract void SpawnSeat(BusSeatPassType seatType, Transform parent, params object[] objects);
    }
    
}
