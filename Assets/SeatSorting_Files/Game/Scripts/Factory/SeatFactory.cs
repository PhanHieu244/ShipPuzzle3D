using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using Random = UnityEngine.Random;


namespace EKStudio
{
    public static class SeatFactory
    {
        private static SeatProperties SeatSpawn()
        {
            var go = new GameObject("SeatSpawn");
            return go.AddComponent<SeatSpawn>();
        }
        
        private static Dictionary<BusSeatPassType, Func<SeatProperties>> seatFactory = new Dictionary<BusSeatPassType, Func<SeatProperties>>
        {
            {BusSeatPassType.None, SeatSpawn },
            {BusSeatPassType.Blue, SeatSpawn },
            {BusSeatPassType.Red, SeatSpawn },
            {BusSeatPassType.Green, SeatSpawn },
            {BusSeatPassType.Orange, SeatSpawn },
            {BusSeatPassType.Pink, SeatSpawn },
            {BusSeatPassType.Purple, SeatSpawn },
            {BusSeatPassType.Yellow, SeatSpawn },
            {BusSeatPassType.Gray, SeatSpawn },
            {BusSeatPassType.SpringGreen, SeatSpawn },
        };

        public static SeatProperties SpawnSeat (BusSeatPassType seatType, Transform parent, out GameObject factoryProduct)
        {
            if (seatFactory.TryGetValue(seatType, out var factory))
            {
                var seatProperties = factory.Invoke();
                seatProperties.SpawnSeat(seatType, parent);
                factoryProduct = seatProperties.FactoryProduct;
                factoryProduct.transform.localPosition = Vector3.zero;
                return seatProperties;
            }
            else
            {
                factoryProduct = null;
                return null;
            }
        }
    }

    public class SeatSpawn : SeatProperties
    {

        GameManager manager => GameManager.Instance;

        public override void SpawnSeat(BusSeatPassType seatType, Transform parent, params object[] objects)
        {
            Dictionary<BusSeatPassType, GameObject> seatObject = new Dictionary<BusSeatPassType, GameObject>
            {
                { BusSeatPassType.None, manager.gameData.allSO.seatSO.seatNone},
                { BusSeatPassType.Blue, manager.gameData.allSO.seatSO.seatBlue},
                { BusSeatPassType.Red, manager.gameData.allSO.seatSO.seatRed},
                { BusSeatPassType.Green, manager.gameData.allSO.seatSO.seatGreen},
                { BusSeatPassType.Orange, manager.gameData.allSO.seatSO.seatOrange},
                { BusSeatPassType.Pink, manager.gameData.allSO.seatSO.seatPink},
                { BusSeatPassType.Purple, manager.gameData.allSO.seatSO.seatPurple},
                { BusSeatPassType.Yellow, manager.gameData.allSO.seatSO.seatYellow},
                { BusSeatPassType.Gray, manager.gameData.allSO.seatSO.seatGray},
                { BusSeatPassType.SpringGreen, manager.gameData.allSO.seatSO.seatSpringGreen},
            };


            var spawnedObject = Instantiate(seatObject[seatType], parent);
            FactoryProduct = spawnedObject;

        }



    }

}
