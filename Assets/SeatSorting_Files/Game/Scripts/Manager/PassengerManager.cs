namespace EKStudio

{

    using System.Collections;

    using System.Collections.Generic;

    using UnityEngine;

    using Dreamteck.Splines;

    using System;

    

    public class PassengerManager : InstanceManager<PassengerManager>

    {

        public List<BusSeatPassType> allPassengerTypeList, tempPassengerTypeList = new List<BusSeatPassType>();

        public List<PassengerController> currentPassengerList = new List<PassengerController>();

        private int activeCount = 1000;

        public SplineComputer splineComputer => GetComponent<SplineComputer>();

        public float moveSpeed = 5f;

        private int typeCount, passengerCount;

        GameManager manager => GameManager.Instance;

    

        private void Awake()

        {

            passengerCount = (manager.busCountInScene + manager.busCountInTunnel) * 3;

            typeCount = manager.typeCount;

    

            if (manager.allTunnels.Count > 0)

            {

                EventManager.Broadcast(GameEvent.TunnelInitialSpawn);

            }

    

            DistributePassengerTypes(passengerCount, typeCount);

    

            DistributeSeatTypes();

        }

    

        void Start()

        {

            StartCoroutine(InitialSpawn());

        }

    

        void DistributePassengerTypes(int total, int count)

        {

            List<BusSeatPassType> selectedTypes = GetRandomEnumTypes(count);

            List<int> distribution = GetThreeMultipleDistribution(total, count);

    

            for (int i = 0; i < count; i++)

            {

                for (int j = 0; j < distribution[i]; j++)

                {

                    tempPassengerTypeList.Add(selectedTypes[i]);

                }

            }

    

            allPassengerTypeList = Shuffle(tempPassengerTypeList);

        }

    

        List<BusSeatPassType> GetRandomEnumTypes(int count)

        {

            List<BusSeatPassType> types = new List<BusSeatPassType>((BusSeatPassType[])Enum.GetValues(typeof(BusSeatPassType)));

            types.RemoveAt(0);

    

            count = Mathf.Clamp(count, 1, types.Count);

    

            return types.GetRange(0, count);

        }

    

        List<int> GetThreeMultipleDistribution(int total, int count)

        {

            List<int> result = new List<int>(new int[count]);

            int baseAmount = (total / count / 3) * 3;

            int remainder = total - (baseAmount * count);

    

            for (int i = 0; i < count; i++)

            {

                result[i] = baseAmount;

            }

    

            for (int i = 0; i < count && remainder >= 3; i++)

            {

                result[i] += 3;

                remainder -= 3;

            }

    

            return result;

        }

    

        List<T> Shuffle<T>(List<T> list)

        {

            List<T> shuffledList = new List<T>(list);

            for (int i = 0; i < shuffledList.Count; i++)

            {

                int randomIndex = UnityEngine.Random.Range(i, shuffledList.Count);

                T temp = shuffledList[i];

                shuffledList[i] = shuffledList[randomIndex];

                shuffledList[randomIndex] = temp;

            }

    

            return shuffledList;

        }

    

        public List<BusSeatPassType> SortSeatTypeList(List<BusSeatPassType> inputList)

        {

            inputList.Sort();

    

            List<BusSeatPassType> result = new List<BusSeatPassType>(inputList);

    

            result.Sort((a, b) => UnityEngine.Random.Range(-1, 2));

    

            for (int i = 2; i < result.Count; i++)

            {

                if (result[i] == result[i - 1] && result[i] == result[i - 2])

                {

                    int swapIndex = i + 1;

    

                    while (swapIndex < result.Count && result[swapIndex] == result[i])

                    {

                        swapIndex++;

                    }

                    if (swapIndex < result.Count)

                    {

                        BusSeatPassType temp = result[i];

                        result[i] = result[swapIndex];

                        result[swapIndex] = temp;

                    }

                }

            }

    

            return result;

        }

    

        void DistributeSeatTypes()

        {

            List<BusSeatPassType> tempList = SortSeatTypeList(tempPassengerTypeList);

            List<BusController> busControllers = new List<BusController>(FindObjectsOfType<BusController>());

    

            if (manager.gameData.levelCount > 1)

            {

                int k = 0;

    

                for (int i = 0; i < busControllers.Count; i++)

                {

                    for (int j = 0; j < 3; j++)

                    {

                        busControllers[i].seatTypeList.Add(tempList[k]);

                        k++;

                    }

                }

            }

        }

    

        IEnumerator InitialSpawn()

        {

            int tempCount = allPassengerTypeList.Count < activeCount ? allPassengerTypeList.Count : activeCount;

    

            for (int i = 0; i < tempCount; i++)

            {

                PassengerFactory.SpawnPassenger(allPassengerTypeList[i], transform, out GameObject factoryProduct);

                currentPassengerList.Add(factoryProduct.GetComponent<PassengerController>());

                //float movePosition = 1f - (i / (float)activeCount);

                float movePosition = 1f - (i / 20f);

                movePosition = movePosition < 0 ? 0 : movePosition;

                factoryProduct.GetComponent<PassengerController>().Initialize(splineComputer, movePosition, moveSpeed);

                yield return new WaitForSeconds(0.01f);

            }

    

            for (int i = 0; i < tempCount; i++)

            {

                allPassengerTypeList.RemoveAt(0);

            }

        }

    

        void InGameSpawn(float queueIndex)

        {

            if (allPassengerTypeList.Count > 0 && currentPassengerList.Count < activeCount)

            {

                PassengerFactory.SpawnPassenger(allPassengerTypeList[0], transform, out GameObject factoryProduct);

                currentPassengerList.Add(factoryProduct.GetComponent<PassengerController>());

                factoryProduct.GetComponent<PassengerController>().Initialize(splineComputer, queueIndex, moveSpeed);

                allPassengerTypeList.RemoveAt(0);

            }

        }

    

        //public void OnBusArrival()

        //{

        //    StartCoroutine(MovePeople());

        //}

    

        public void MoveOtherPassengers()

        {

    

            for (int i = 0; i < currentPassengerList.Count; i++)

            {

                //float movePosition = 1f - (i / (float)activeCount);

                float movePosition = 1f - (i / 20f);

                movePosition = movePosition < 0 ? 0 : movePosition;

    

                currentPassengerList[i].StartMoving(movePosition);

            }

    

            float lastMovePosition = 1f - (currentPassengerList.Count / (float)activeCount);

            InGameSpawn(lastMovePosition);

    

            //BusSlotManager.Instance.PassengersGoToBus();

        }

    

    

    }

    

}

