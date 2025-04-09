namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Linq;
    using TMPro;
    using System;
    using DG.Tweening;
    using Random = UnityEngine.Random;
    
    public class UpgradeManager : MonoBehaviour
    {
        public Button shuffleButton, sortButton, vipSlotButton;
        public Button shuffleButtonParent, sortButtonParent, vipSlotButtonParent;
        public TextMeshProUGUI shufflePrice, sortPrice, vipSlotPrice;
        public GameObject shufflePanel, sortPanel, vipSlotPanel, notEnoughMoney;
        private bool sortDescending;
    
        GameManager manager => GameManager.Instance;
    
    
        private void Awake()
        {
            if (manager.gameData.levelCount == 1)
            {
                gameObject.SetActive(false);
            }
    
            sortDescending = Random.Range(0f, 1f) > 0.5f;
    
            shuffleButton.onClick.AddListener(() => ClickButton(manager.gameData.priceShuffle, ShuffleAllSeats));
            sortButton.onClick.AddListener(() => ClickButton(manager.gameData.priceSort, SortAllPassenger));
            vipSlotButton.onClick.AddListener(() => ClickButton(manager.gameData.priceVIP, UseVipSlot));
    
            shufflePrice.text = "USE     " + manager.gameData.priceShuffle.ToString();
            sortPrice.text = "USE     " + manager.gameData.priceSort.ToString();
            vipSlotPrice.text = "USE     " + manager.gameData.priceVIP.ToString();
        }
    
        private void Update()
        {
    
        }
    
        private void ChangeAllParentButtons(bool isOpen)
        {
            shuffleButtonParent.interactable = isOpen;
            sortButtonParent.interactable = isOpen;
            vipSlotButtonParent.interactable = isOpen;
        }
    
        private void CloseAllPanels()
        {
            shufflePanel.SetActive(false);
            sortPanel.SetActive(false);
            vipSlotPanel.SetActive(false);
        }
    
        private void ClickButton(int buttonPrice, Action buttonEvent)
        {
            if (buttonPrice > manager.gameData.totalMoney)
            {
                notEnoughMoney.SetActive(true);
                DOVirtual.DelayedCall(1f, () => { notEnoughMoney.SetActive(false); });
                return;
            }
    
            CloseAllPanels();
    
            manager.gameData.totalMoney -= buttonPrice;
    
            ChangeAllParentButtons(false);
    
            DOVirtual.DelayedCall(0.5f, buttonEvent.Invoke);
        }
    
    
        void ShuffleAllSeats()
        {
            //EventManager.Broadcast(GameEvent.OnPlaySound, "Sort");
    
            List<ObjectData> sortedSeatControllers = new List<ObjectData>();
    
            foreach (var seat in manager.allseatControllers)
            {
                sortedSeatControllers.Add(new ObjectData(seat.seatType, seat.currentColor));
            }
    
            //sortedSeatControllers.Sort((a, b) => UnityEngine.Random.Range(-1, 2));
    
            sortedSeatControllers = sortedSeatControllers.OrderBy(x => UnityEngine.Random.value).ToList();
    
            for (int i = 2; i < sortedSeatControllers.Count; i++)
            {
                if (sortedSeatControllers[i].objectType == sortedSeatControllers[i - 1].objectType &&
                    sortedSeatControllers[i].objectType == sortedSeatControllers[i - 2].objectType)
                {
                    for (int j = i + 1; j < sortedSeatControllers.Count; j++)
                    {
                        if (sortedSeatControllers[j].objectType != sortedSeatControllers[i].objectType)
                        {
                            var temp = sortedSeatControllers[i];
                            sortedSeatControllers[i] = sortedSeatControllers[j];
                            sortedSeatControllers[j] = temp;
                            break;
                        }
                    }
                }
            }
    
            for (int i = 0; i < manager.allseatControllers.Count; i++)
            {
                ObjectData seat = sortedSeatControllers[i];
                manager.allseatControllers[i].seatType = seat.objectType;
                manager.allseatControllers[i].currentColor = seat.currentColor;
                Renderer seatRenderer = manager.allseatControllers[i].GetComponentsInChildren<Renderer>()[1];
                seatRenderer.material.color = seat.currentColor;
            }
    
            ChangeAllParentButtons(true);
            EventManager.Broadcast(GameEvent.SeatMoved);
        }
    
        void SortAllPassenger()
        {
            //EventManager.Broadcast(GameEvent.OnPlaySound, "Sort");
    
            List<ObjectData> sortedPassengerControllers = new List<ObjectData>();
    
            foreach (var passenger in PassengerManager.Instance.currentPassengerList)
            {
                sortedPassengerControllers.Add(new ObjectData(passenger.passengerType, passenger.currentColor));
            }
    
            if (sortDescending) 
                sortedPassengerControllers = sortedPassengerControllers.OrderByDescending(passenger => passenger.objectType).ToList();
            else
                sortedPassengerControllers = sortedPassengerControllers.OrderBy(passenger => passenger.objectType).ToList();
    
            for (int i = 0; i < PassengerManager.Instance.currentPassengerList.Count; i++)
            {
                ObjectData passenger = sortedPassengerControllers[i];
                PassengerManager.Instance.currentPassengerList[i].passengerType = passenger.objectType;
                PassengerManager.Instance.currentPassengerList[i].currentColor = passenger.currentColor;
                //PASSENGER MESH
                var passengerRenderer = PassengerManager.Instance.currentPassengerList[i].GetComponentInChildren<MeshRenderer>();
                passengerRenderer.material.color = passenger.currentColor;
            }
    
            ChangeAllParentButtons(true);
            sortDescending = !sortDescending;
    
            BusSlotManager.Instance.PassengersGoToBus();
        }
    
    
        void UseVipSlot()
        {
            //EventManager.Broadcast(GameEvent.OnPlaySound, "VIPSlot");
    
            BusSlotController vipSlot = BusSlotManager.Instance.OpenUgradeSlot();
            manager.isFirstSeatMoved = true;
           
    
            SwapPassengerWithSameType(PassengerManager.Instance.currentPassengerList);
            BusController vipBus = new BusController();
            SwapBusSeatsWithSameType(manager.allseatControllers, ref vipBus);
    
            DOVirtual.DelayedCall(0.15f, () =>
            {
                vipBus.currentSlot = vipSlot;
                vipSlot.currentBus = vipBus;
                CheckAllSeatsVIP(vipBus);
                vipBus.transform.DOJump(vipSlot.transform.position, 1f, 1, 1f);
                vipBus.transform.DORotate(Vector3.zero, 1f);
                vipBus.transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 1f).OnComplete(() =>
                {
                    BusSlotManager.Instance.allBusInSlots.Add(vipBus);
                    BusSlotManager.Instance.PassengersGoToBus();
                    DOVirtual.DelayedCall(1f, BusSlotManager.Instance.CloseUgradeSlot);
                    ChangeAllParentButtons(true);
    
                    if (vipBus.relatedTunnel != null)
                        vipBus.relatedTunnel.BusSendFromTunnel();
    
                });
    
            });
        }
    
    
    
        void SwapPassengerWithSameType(List<PassengerController> list)
        {
            if (list.Count < 3) return;
    
            PassengerController first = list[0];
    
            int index = 1;
    
            for (int i = 1; i < list.Count; i++)
            {
                if (list[i].passengerType == first.passengerType)
                {
                    index++;
    
                    if (index == 3)
                    {
                        return;
                    }
                }
                else
                {
                    PassengerController otherDifferent = list[i];
                    ObjectData tempObjectData = new ObjectData(otherDifferent.passengerType, otherDifferent.currentColor);
    
                    for (int j = 3; j < list.Count; j++)
                    {
                        if (list[j].passengerType == first.passengerType)
                        {
                            PassengerController otherSame = list[j];
    
                            otherDifferent.passengerType = otherSame.passengerType;
                            otherDifferent.currentColor = otherSame.currentColor;
                            var otherRenderer = otherDifferent.GetComponentInChildren<MeshRenderer>();
                            otherRenderer.material.color = otherDifferent.currentColor;
    
                            otherSame.passengerType = tempObjectData.objectType;
                            otherSame.currentColor = tempObjectData.currentColor;
                            var sameRenderer = otherSame.GetComponentInChildren<MeshRenderer>();
                            sameRenderer.material.color = otherSame.currentColor;
    
                            index++;
    
                            if (index == 3)
                            {
                                return;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
    
            }
        }
    
        private void SwapBusSeatsWithSameType(List<SeatController> list, ref BusController relatedBus)
        {
            if (list.Count < 3)
            {
                return;
            }
    
            BusSeatPassType firstType = PassengerManager.Instance.currentPassengerList[0].passengerType;
    
            var availableBuses = manager.allbusControllers.Where(bus => !bus.isInTunnel).ToList();
    
            // Rastgele seçim yap
            if (availableBuses.Count > 0)
            {
                int randomNumber = Random.Range(0, availableBuses.Count);
                relatedBus = availableBuses[randomNumber];
            }
    
            //int randomNumber = Random.Range(0, manager.allbusControllers.Count);
            //relatedBus = manager.allbusControllers[randomNumber];
            manager.allbusControllers.Remove(relatedBus);
    
            int index = 0;
    
            print(relatedBus);
    
            for (int i = 0; i < relatedBus.slotList.Count; i++)
            {
                SeatController tempSeat = relatedBus.slotList[i].currentSeat;
    
                if (tempSeat != null)
                {
                    if (tempSeat.seatType == firstType)
                    {
                        tempSeat.transform.tag = "Untagged";
                        list.Remove(tempSeat);
                        EventManager.Broadcast(GameEvent.SeatMoved);
                        index++;
                        print(i + " same: " + index);
    
                        if (index == 3)
                        {
                            return;
                        }
                    }
                    else
                    {
                        SeatController otherDifferent = relatedBus.slotList[i].currentSeat;
                        ObjectData tempObjectData = new ObjectData(otherDifferent.seatType, otherDifferent.currentColor);
    
                        for (int j = 0; j < list.Count; j++)
                        {
                            if (list[j].seatType == firstType)
                            {
                                SeatController otherSame = list[j];
                                otherDifferent.seatType = otherSame.seatType;
                                otherDifferent.currentColor = otherSame.currentColor;
                                MeshRenderer otherRenderer = otherDifferent.GetComponentsInChildren<MeshRenderer>()[1];
                                otherRenderer.material.color = otherDifferent.currentColor;
    
                                otherSame.seatType = tempObjectData.objectType;
                                otherSame.currentColor = tempObjectData.currentColor;
                                MeshRenderer sameRenderer = otherSame.GetComponentsInChildren<MeshRenderer>()[1];
                                sameRenderer.material.color = otherSame.currentColor;
    
                                otherDifferent.transform.tag = "Untagged";
                                list.Remove(otherDifferent);
    
                                EventManager.Broadcast(GameEvent.SeatMoved);
                                index++;
    
                                print(i + " else: " + index);
    
                                if (index == 3)
                                {
                                    print("elsereturn");
    
                                    return;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (list[j].seatType == firstType)
                        {
                            SeatController otherSame = list[j];
    
                            otherSame.currentSlot = relatedBus.slotList[i];
                            otherSame.transform.SetParent(relatedBus.slotList[i].transform, true);
                            otherSame.transform.localPosition = Vector3.zero;
                            otherSame.transform.rotation = Quaternion.LookRotation(relatedBus.slotList[i].transform.forward);
    
                            otherSame.transform.tag = "Untagged";
                            list.Remove(otherSame);
    
                            EventManager.Broadcast(GameEvent.SeatMoved);
                            print("null");
    
                            index++;
    
                            if (index == 3)
                            {
                                print("elseelsereturn");
    
                                return;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
    
    
            }
        }
    
        void CheckAllSeatsVIP(BusController vipBus)
        {
            vipBus.isRayOpen = false;
            vipBus.currentBusState = BusState.OnParking;
    
            for (int i = 0; i < vipBus.seatTypeList.Count; i++)
            {
                vipBus.seatTypeList[i] = vipBus.slotList[i].currentSeatType;
            }
    
            // bütün seat slotlar dolu ise ve aynı ise
    
            vipBus.busType = vipBus.seatTypeList.Distinct().First();
            Color busColor = vipBus.slotList[0].currentSeat.currentColor;
            vipBus.busRenderer.material.DOColor(busColor, 0.5f);
    
            //foreach (SeatSlotController slot in vipBus.slotList)
            //{
            //    slot.currentSeat.transform.tag = "Untagged";
            //    manager.allseatControllers.Remove(slot.currentSeat);
            //}
        }
    }
    
    
    public class ObjectData
    {
        public BusSeatPassType objectType;
        public Color currentColor;
    
        public ObjectData(BusSeatPassType type, Color color)
        {
            objectType = type;
            currentColor = color;
        }
    }
}
