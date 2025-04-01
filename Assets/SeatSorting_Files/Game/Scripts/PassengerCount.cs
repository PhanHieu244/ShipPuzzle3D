namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    
    public class PassengerCount : MonoBehaviour
    {
        public TextMeshPro leftText;
    
        void Update()
        {
            leftText.text = PassengerManager.Instance.currentPassengerList.Count.ToString();
        }
    }
    
}
