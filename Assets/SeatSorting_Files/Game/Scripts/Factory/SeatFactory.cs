using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using Random = UnityEngine.Random;


namespace EKStudio
{
    public static class SeatFactory
    {
        private static Dictionary<BusSeatPassType, Func<SeatProperties>> seatFactory = new Dictionary<BusSeatPassType, Func<SeatProperties>>
        {
            {BusSeatPassType.None, () => new SeatSpawn() },
            {BusSeatPassType.Blue, () => new SeatSpawn() },
            {BusSeatPassType.Red, () => new SeatSpawn() },
            {BusSeatPassType.Green, () => new SeatSpawn() },
            {BusSeatPassType.Orange, () => new SeatSpawn() },
            {BusSeatPassType.Pink, () => new SeatSpawn() },
            {BusSeatPassType.Purple, () => new SeatSpawn() },
            {BusSeatPassType.Yellow, () => new SeatSpawn() },
            {BusSeatPassType.Gray, () => new SeatSpawn() },
            {BusSeatPassType.SpringGreen, () => new SeatSpawn() },
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
