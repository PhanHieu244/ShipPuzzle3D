namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public abstract class PassengerProperties : MonoBehaviour
    {
        private GameObject factoryProduct;
        public GameObject FactoryProduct
        {
            get { return factoryProduct; }
            set { factoryProduct = value; }
        }
    
        public abstract void SpawnPassenger(BusSeatPassType passengerType, Transform parent, params object[] objects);
    }
    
}
