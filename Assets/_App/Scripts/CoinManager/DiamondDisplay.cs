using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using EKStudio;
using TMPro;
using UnityEngine;

public class DiamondDisplay : MonoBehaviour
{
   public TextMeshProUGUI diamondTmp;

   void OnValidated()
   {
      diamondTmp = GetComponent<TextMeshProUGUI>();
   }

   private void Update()
   {
      diamondTmp.SetText(GameManager.Instance.gameData.totalMoney.ToString(CultureInfo.InvariantCulture));
   }
}
