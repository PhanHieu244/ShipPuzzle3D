using System;
using System.Collections;
using CI.WSANative.Common;
using CI.WSANative.Store;
using Jackal;

public class IAPKey
{
    public const string PACK1_RE = "ship_puzzle_pack_coin_1";
    public const string PACK2_RE = "ship_puzzle_pack_coin_2";
    public const string PACK3_RE = "ship_puzzle_pack_coin_3";
    public const string PACK4_RE = "ship_puzzle_pack_coin_4";
    public const string PACK5_RE = "ship_puzzle_pack_coin_5";
}

public class IAPManager : PersistentSingleton<IAPManager>
{
    public static Action OnPurchaseSuccess;

    private bool _isBuyFromShop;
    void Awake()
    {
        WSANativeCore.Initialise();
    }
   
    //store id get from microsoft partner 
    public void BuyProductID(string storeid, int points = 0)
    {
        WSANativeStore.RequestPurchase(storeid, result =>
        {
            UnityEngine.Debug.Log(result.Status);
            if(result.Status == WSAStorePurchaseStatus.Succeeded)
            {
                // do something here to add point value
                OnPurchaseSuccess?.Invoke();
            }
        });
    }
}