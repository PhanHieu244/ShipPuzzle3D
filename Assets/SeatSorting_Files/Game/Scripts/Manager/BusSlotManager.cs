namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;
    using DG.Tweening;
    
    public class BusSlotManager : InstanceManager<BusSlotManager>
    {
        [SerializeField] private List<BusSlotController> allSlots;
        [SerializeField] private BusSlotController upgradeSlot;
        public BusSlotController avaliableSlot;
        public List<BusController> allBusInSlots;
    
        GameManager manager => GameManager.Instance;
    
    
        private void Awake()
        {
            CloseUgradeSlot();
            avaliableSlot = AvaliableSlot();
        }
    
        public BusSlotController AvaliableSlot()
        {
            for (int i = 0; i < allSlots.Count; i++)
            {
                if (allSlots[i].currentBus == null && !allSlots[i].isLock && allSlots[i].gameObject.activeSelf)
                {
                    avaliableSlot = allSlots[i];
                    return allSlots[i];
                }
            }
            avaliableSlot = null;
            return null;
        }
    
        public void PassengersGoToBus()
        {
            if (PassengerManager.Instance.currentPassengerList.Count == 0)
                return;
    
            bool isFail = true;
    
            PassengerController latestPassenger = PassengerManager.Instance.currentPassengerList[0];
    
            for (int i = 0; i < allBusInSlots.Count; i++)
            {
                BusController tempBusController = allBusInSlots[i];
    
                if (tempBusController.passengerCount < 3 && tempBusController.busType == latestPassenger.passengerType && !latestPassenger.isJumped)
                {
                    latestPassenger.isJumped = true;
                    latestPassenger.JumpToBus(tempBusController.slotList[tempBusController.passengerCount]);
                    tempBusController.passengerCount++;
                    return;
                }
            }
    
    
            DOVirtual.DelayedCall(2f, () => {
    
                for (int i = 0; i < allSlots.Count; i++)
                {
                    BusController busController = allSlots[i].currentBus;
    
                    if (busController != null && busController.busType == latestPassenger.passengerType)
                    {
                        isFail = false;
                        return;
                    }
                }
    
                if (AvaliableSlot() == null && isFail && !manager.isFinished)
                    DOVirtual.DelayedCall(0.1f, () => EventManager.Broadcast(GameEvent.OnLose));
            });
    
            
    
    
        }
    
        //public void PassengersGoToBus(SkierType skiLiftType)
        //{
        //    int countType = allSlots.Count(x => x.Skier?.skierType == skiLiftType);
        //    int nextCountType = 0;
        //    SkiLiftControl lastSkiLift = SkiLiftManager.Instance.allSkiLifts[0];
    
        //    if (SkiLiftManager.Instance.allSkiLifts.Count > 1)
        //    {
        //        nextCountType = allSlots.Count(x => x.Skier?.skierType == SkiLiftManager.Instance.allSkiLifts[1].skiLiftType);
        //    }
    
        //    for (int i = 0; i < allSlots.Count; i++)
        //    {
        //        if (allSlots[i].Skier?.skierType == skiLiftType && countType > 0 && lastSkiLift.countSend < 3 && lastSkiLift.isSkiersCome)
        //        {
        //            allSlots[i].Skier.currentSlot = SkiLiftManager.Instance.allWaitSlots[lastSkiLift.countSend];
        //            SkierManager.Instance.allSkiers.Remove(allSlots[i].Skier);
        //            SkiLiftManager.Instance.allSkiers.Add(allSlots[i].Skier.transform);
        //            allSlots[i].Skier.targetTransform = allSlots[i].Skier.currentSlot.transform;
        //            allSlots[i].Skier.isGoingWaitSlot = true;
        //            allSkiers.Remove(allSlots[i].Skier.transform);
        //            allSlots[i].Skier.SwitchState(allSlots[i].Skier.skierWalkState);
        //            allSlots[i].ClearSlot();
        //            lastSkiLift.countSend += 1;
        //        }
        //        // slotları dolması durumunda fail olmama durumu
        //        else if (lastSkiLift.countSend == 3 && nextCountType > 0 && AvaliableSlot() == null)
        //        {
    
        //        }
        //        // slotları dolması durumunda fail olma durumu
        //        else if (allSlots[i].Skier?.skierType != skiLiftType && countType == 0 && AvaliableSlot() == null)
        //        {
        //            DOVirtual.DelayedCall(1f, () => EventManager.Broadcast(GameEvent.OnLose));
        //        }
        //    }
        //}
    
        //public void RandomSelectUpgrade(SkierStateManager randomSelected, out bool isUsed)
        //{
        //    isUsed = false;
        //    SkiLiftControl lastSkiLift = SkiLiftManager.Instance.allSkiLifts[0];
    
        //    if (lastSkiLift.countSend < 3)
        //    {
        //        randomSelected.boxCollider.enabled = false;
        //        AstarPath.active.Scan();
    
        //        DOVirtual.DelayedCall(0.1f, () =>
        //        {
        //            if (!GameManager.Instance.isGateOpen)
        //                EventManager.Broadcast(GameEvent.OnCheckAIMap);
        //        }
        //        );
    
        //        randomSelected.currentSlot = SkiLiftManager.Instance.allWaitSlots[lastSkiLift.countSend];
        //        SkierManager.Instance.allSkiers.Remove(randomSelected);
        //        SkiLiftManager.Instance.allSkiers.Add(randomSelected.transform);
        //        randomSelected.targetTransform = randomSelected.currentSlot.transform;
        //        randomSelected.isGoingWaitSlot = true;
        //        randomSelected.aiPath.enabled = false;
        //        randomSelected.destinationSetter.enabled = false;
        //        randomSelected.SwitchState(randomSelected.skierWalkState);
    
        //        randomSelected.transform.DOJump(randomSelected.currentSlot.transform.position, 1f, 1, 1f).OnComplete(() =>
        //        {
        //            randomSelected.isGoingWaitSlot = true;
        //        });
    
        //        lastSkiLift.countSend += 1;
        //        isUsed = true;
        //    }
        //}
    
        public BusSlotController OpenUgradeSlot()
        {
            if (!upgradeSlot.gameObject.activeSelf)
                upgradeSlot.gameObject.SetActive(true);
    
            return upgradeSlot;
        }
    
        public void CloseUgradeSlot()
        {
            if (upgradeSlot.gameObject.activeSelf)
                upgradeSlot.gameObject.SetActive(false);
        }
    
        public void MoveBack(out bool isUsed)
        {
            isUsed = false;
            bool canBeUsed = true;
    
            //if (allSkiers.Count > 0 && canBeUsed && allSkiers[^1].GetComponent<SkierStateManager>().currentState == allSkiers[^1].GetComponent<SkierStateManager>().skierIdleState)
            //{
            //    canBeUsed = false;
            //    isUsed = true;
    
            //    if (isGateAvailable)
            //        EventManager.Broadcast(GameEvent.OnMoveBack);
    
            //    allSkiers[^1].DOJump(allSkiers[^1].GetComponent<SkierStateManager>().spawnPos, 1f, 1, 1f).OnComplete(() =>
            //    {
            //        allSkiers[^1].GetComponent<SkierStateManager>().ResetObject();
            //        AstarPath.active.Scan();
            //        EventManager.Broadcast(GameEvent.OnCheckAIMap);
            //        DOVirtual.DelayedCall(0.00f, () =>
            //        {
            //            allSkiers[^1].GetComponent<SkierStateManager>().aiPath.canMove = false;
            //        });
    
            //        allSkiers[^1].transform.position = allSkiers[^1].GetComponent<SkierStateManager>().spawnPos;
            //        allSkiers[^1].transform.rotation = Quaternion.identity;
            //        allSkiers.RemoveAt(allSkiers.Count - 1);
            //        canBeUsed = true;
    
            //    });
    
            //}
        }
    }
    
}
