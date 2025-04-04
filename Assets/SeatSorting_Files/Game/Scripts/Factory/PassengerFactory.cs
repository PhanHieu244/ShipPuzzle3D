using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using Random = UnityEngine.Random;


namespace EKStudio
{
    public static class PassengerFactory
    {
        
        private static PassengerProperties PassengerSpawn()
        {
            var go = new GameObject("PassengerSpawn");
            return go.AddComponent<PassengerSpawn>();
        }

        
        private static Dictionary<BusSeatPassType, Func<PassengerProperties>> passengerFactory = new Dictionary<BusSeatPassType, Func<PassengerProperties>>
        {
            {BusSeatPassType.None, PassengerSpawn},
            {BusSeatPassType.Blue, PassengerSpawn},
            {BusSeatPassType.Red, PassengerSpawn},
            {BusSeatPassType.Green, PassengerSpawn},
            {BusSeatPassType.Orange, PassengerSpawn},
            {BusSeatPassType.Pink, PassengerSpawn},
            {BusSeatPassType.Purple, PassengerSpawn},
            {BusSeatPassType.Yellow, PassengerSpawn},
            {BusSeatPassType.Gray, PassengerSpawn},
            {BusSeatPassType.SpringGreen, PassengerSpawn},
        };

        public static PassengerProperties SpawnPassenger(BusSeatPassType passengerType, Transform parent, out GameObject factoryProduct)
        {
            if (passengerFactory.TryGetValue(passengerType, out var factory))
            {
                var passengerProperties = factory.Invoke();
                passengerProperties.SpawnPassenger(passengerType, parent);
                factoryProduct = passengerProperties.FactoryProduct;
                //factoryProduct.transform.localPosition = Vector3.zero;
                return passengerProperties;
            }
            else
            {
                factoryProduct = null;
                return null;
            }
        }
    }

    public class PassengerSpawn : PassengerProperties
    {

        GameManager manager => GameManager.Instance;

        public override void SpawnPassenger(BusSeatPassType passengerType, Transform parent, params object[] objects)
        {
            Dictionary<BusSeatPassType, GameObject> passengerObject = new Dictionary<BusSeatPassType, GameObject>
            {
                { BusSeatPassType.None, manager.gameData.allSO.passengerSO.passengerNone},
                { BusSeatPassType.Blue, manager.gameData.allSO.passengerSO.passengerBlue},
                { BusSeatPassType.Red, manager.gameData.allSO.passengerSO.passengerRed},
                { BusSeatPassType.Green, manager.gameData.allSO.passengerSO.passengerGreen},
                { BusSeatPassType.Orange, manager.gameData.allSO.passengerSO.passengerOrange},
                { BusSeatPassType.Pink, manager.gameData.allSO.passengerSO.passengerPink},
                { BusSeatPassType.Purple, manager.gameData.allSO.passengerSO.passengerPurple},
                { BusSeatPassType.Yellow, manager.gameData.allSO.passengerSO.passengerYellow},
                { BusSeatPassType.Gray, manager.gameData.allSO.passengerSO.passengerGray},
                { BusSeatPassType.SpringGreen, manager.gameData.allSO.passengerSO.passengerSpringGreen},
            };


            var spawnedObject = Instantiate(passengerObject[passengerType], parent.GetChild(0).transform.position, Quaternion.identity,parent);
            FactoryProduct = spawnedObject;

        }



    }

}
