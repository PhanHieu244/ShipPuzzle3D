using System.Collections;
using System.Collections.Generic;
using EKStudio;
using UnityEngine;

public class PurchasingManager : MonoBehaviour
{
   GameManager Manager => GameManager.Instance;
   
   public void OnPressDown(int i)
   {
      switch (i)
      {
         case 1:
            IAPManager.OnPurchaseSuccess = () =>
            {
               
            };
             IAPManager.Instance.BuyProductID(IAPKey.PACK1);
            break;
         case 2:
            IAPManager.OnPurchaseSuccess = () =>
            {

            };
            IAPManager.Instance.BuyProductID(IAPKey.PACK2);
            break;
         case 3:
            IAPManager.OnPurchaseSuccess = () =>
            {
               Manager.gameData.totalMoney += 500;
            };
            IAPManager.Instance.BuyProductID(IAPKey.PACK3);
            break;
         case 4:
            IAPManager.OnPurchaseSuccess = () =>
            {
               Manager.gameData.totalMoney += 1000;
            };
            IAPManager.Instance.BuyProductID(IAPKey.PACK4);
            break;
         case 5:
            IAPManager.OnPurchaseSuccess = () =>
            {
               Manager.gameData.totalMoney += 3000;
            };
            IAPManager.Instance.BuyProductID(IAPKey.PACK5);
            break;
         case 6:
            IAPManager.OnPurchaseSuccess = () =>
            {
               Manager.gameData.totalMoney += 5000;
            };
            IAPManager.Instance.BuyProductID(IAPKey.PACK6);
            break;
         case 7:
            IAPManager.OnPurchaseSuccess = () =>
            {
               Manager.gameData.totalMoney += 10000;
            };
            IAPManager.Instance.BuyProductID(IAPKey.PACK7);
            break;
         
      }
   }

   public void Sub(int i)
   {
      GameDataManager.Instance.playerData.SubDiamond(i);
   }
}
