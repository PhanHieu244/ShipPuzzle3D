namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using DG.Tweening;
    
    public class BusController : MonoBehaviour
    {
        public float leftX, rightX, upZ, bottomZ;
        public bool isTurnedOnBottomLine, isTurnedOnSideLine, isTurnedOnUpLine, isTurnedToSlot;
        public bool isComingFromLeft, isMovedToSlot, isMovedToOut;
        public float busSpeed;
        public float angleRight, angleLeft, angleUp, angleDown, angleSlot;
        public BusSlotController currentSlot;
        public BusState currentBusState;
        public BusSeatPassType busType;
        public List<BusSeatPassType> seatTypeList = new List<BusSeatPassType>();
        public List<SeatSlotController> slotList = new List<SeatSlotController>();
        public BusSlotController targetSlot;
        public MeshRenderer busRenderer;
        public Animator animator;
    
        GameManager manager => GameManager.Instance;
        public bool isReadyToMove, isReadyToMoveOut, isInTunnel;
        public int passengerCount;
        public TunnelManager relatedTunnel;

        // BUS DETECTION OF OBSTACLES BY RAIL

        public Transform leftRayOrigin, rightRayOrigin;
        public float rayDistance = 5f;
        public LayerMask busLayer;
        public float bounceBackDistance = 2f, bounceBackDuration = 0.5f;
        public float returnDuration = 1f;
    
        private Vector3 initialPosition;
        private bool isBouncingBack = false;
        public bool isRayOpen = true;

        // THE ROCK EFFECT OF THE HITTING BUS

        public float shakeDuration = 0.5f, shakeMagnitude = 0.1f;
    
    
        private void Awake()
        {
            //for (int i = 0; i < seatSpawnType.Count; i++)
            //{
            //    currentSeatType.Add(seatSpawnType[i]);
            //}
        }
    
        void Start()
        {
            SeatSpawn();
    
            if (isInTunnel)
                initialPosition = relatedTunnel.frontPoint.position;
            else
                initialPosition = transform.position;
        }
    
        void SeatSpawn()
        {
            for (int i = 0; i < seatTypeList.Count; i++)
            {
                if (seatTypeList[i] != BusSeatPassType.None)
                {
                    SeatFactory.SpawnSeat(seatTypeList[i], slotList[i].transform, out GameObject factoryProduct);
                    manager.allseatControllers.Add(factoryProduct.GetComponent<SeatController>());
                }
            }
    
            EventManager.Broadcast(GameEvent.SeatMoved);
        }
    
        void CheckAllSeatsToMove()
        {
            // After the bus crashes and comes back, automatic sending is disabled

            if (busType == BusSeatPassType.None)
                Invoke(nameof(CheckAllSeatsToMoveDelayed), 0.1f);
        }
    
        void CheckAllSeatsToMoveDelayed()
        {
            for (int i = 0; i < seatTypeList.Count; i++)
            {
                seatTypeList[i] = slotList[i].currentSeatType;
            }
    
            int tempCount = seatTypeList.Count(x => x != BusSeatPassType.None);

            // if all seat slots are full and the same

            if (currentBusState == BusState.OnStaying && tempCount == slotList.Count && seatTypeList.Distinct().Count() == 1 && manager.isFirstSeatMoved)
            {
                isReadyToMove = true;
    
                //targetSlot = BusSlotManager.Instance.AvaliableSlot();
    
                //if (targetSlot != null)
                //{
                //    currentBusState = BusState.OnMovingToSlot;
                //    isReadyToMove = false;
                //}
    
                Color busColor = slotList[0].currentSeat.currentColor;
                float h, s, v;
                Color.RGBToHSV(busColor, out h, out s, out v);
                v = 1.0f;
                s = 0.65f;
                busColor = Color.HSVToRGB(h, s, v);
    
                busType = seatTypeList.Distinct().First();
                busRenderer.material.DOColor(busColor, 0.5f);
                string particleName = "ColorChange_Prtcle_" + busType.ToString();
                EventManager.Broadcast(GameEvent.OnParticlePlay, particleName, transform.position + new Vector3(0f, 3f, 0f), new Vector3(0, 0, 0));
                LockSeatSelectable();
            }
    
        }
    
        void LockSeatSelectable()
        {
            foreach (SeatSlotController slot in slotList)
            {
                slot.currentSeat.transform.tag = "Untagged";
                slot.currentSeat.transform.GetComponent<BoxCollider>().enabled = false;
                manager.allseatControllers.Remove(slot.currentSeat);
            }
            manager.allbusControllers.Remove(this);
        }
    
        private void OnMouseDown()
        {
            if (isInTunnel)
                return;
    
            targetSlot = BusSlotManager.Instance.AvaliableSlot();
    
            if (isReadyToMove && targetSlot != null)
            {
                currentBusState = BusState.OnMovingToSlot;
                isReadyToMove = false;
    
                if (PlayerPrefs.GetInt("IsHapticOpen") == 1)
                    Vibration.VibratePop();
    
                //EventManager.Broadcast(GameEvent.OnSoundStart, "BusMove");
            }
    
        }
    
        private void Update()
        {
            if (isInTunnel)
                return;
    
            switch (currentBusState)
            {
                case BusState.OnStaying:
    
                    break;
    
                case BusState.OnMovingToSlot:
                    MoveToSlots();
                    break;
    
                case BusState.OnParking:
                    if (isReadyToMoveOut)
                        currentBusState = BusState.OnMovingToOut;
                    break;
    
                case BusState.OnMovingToOut:
                    MoveToOut();
                    break;
                default:
                    break;
            }
    
            if (isRayOpen)
                RayController();
        }
    
        private void OnEnable()
        {
            EventManager.AddHandler(GameEvent.SeatMoved, CheckAllSeatsToMove);
        }
    
        private void OnDisable()
        {
            EventManager.RemoveHandler(GameEvent.SeatMoved, CheckAllSeatsToMove);
        }


        // BUS DETECTION OF OBSTACLES BY RAIL


        void RayController()
        {
            RaycastHit leftHit, rightHit;
            bool leftCollision = Physics.Raycast(leftRayOrigin.position, transform.forward, out leftHit, rayDistance, busLayer);
            bool rightCollision = Physics.Raycast(rightRayOrigin.position, transform.forward, out rightHit, rayDistance, busLayer);
    
            if ((leftCollision || rightCollision) && !isBouncingBack && currentBusState == BusState.OnMovingToSlot)
            {
                currentBusState = BusState.OnStaying;
    
                isMovedToSlot = false;
    
                if (currentSlot != null)
                    currentSlot.ClearSlot();
    
                currentSlot = null;
    
                StartCoroutine(BounceBack());
    
                if (leftCollision && leftHit.collider != null)
                {
                    leftHit.collider.transform.GetComponent<BusController>().animator.SetTrigger("isShake");
                    //leftHit.collider.transform.GetComponent<BusController>().StartShake();
                    //EventManager.Broadcast(GameEvent.OnParticlePlay, "HitParticle", leftHit.point, 1f);
                }
                else if (rightCollision && rightHit.collider != null)
                {
                    rightHit.collider.transform.GetComponent<BusController>().animator.SetTrigger("isShake");
                    //rightHit.collider.transform.GetComponent<BusController>().StartShake();
                    //EventManager.Broadcast(GameEvent.OnParticlePlay, "HitParticle", rightHit.point, 1f);
                }
    
                EventManager.Broadcast(GameEvent.OnParticlePlay, "HitParticle", transform.position + 3 * transform.forward + new Vector3(0f, 3f, 0f), new Vector3(0, 0, 0));
    
                if (PlayerPrefs.GetInt("IsHapticOpen") == 1)
                    Vibration.VibratePop();
    
                //EventManager.Broadcast(GameEvent.OnSoundStop);
                //EventManager.Broadcast(GameEvent.OnPlaySound, "Hit");
    
    
            }
    
            Debug.DrawRay(leftRayOrigin.position, transform.forward * rayDistance, Color.red);
            Debug.DrawRay(rightRayOrigin.position, transform.forward * rayDistance, Color.red);
        }
    
        IEnumerator BounceBack()
        {
            isBouncingBack = true;
    
            Vector3 bouncePosition = transform.position - transform.forward * bounceBackDistance;
            float elapsedTime = 0;
    
            while (elapsedTime < bounceBackDuration)
            {
                transform.position = Vector3.Lerp(transform.position, bouncePosition, (elapsedTime / bounceBackDuration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
    
            elapsedTime = 0;
            while (elapsedTime < returnDuration)
            {
                transform.position = Vector3.Lerp(transform.position, initialPosition, (elapsedTime / returnDuration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
    
            targetSlot = null;
            isReadyToMove = true;
            isBouncingBack = false;
        }

        //// THE ROCK EFFECT OF THE HITTING BUS

        //public void StartShake()
        //{
        //    if (isRayOpen)
        //    {
        //        StartCoroutine(Shake());
        //    }
        //}

        //private IEnumerator Shake()
        //{
        //    float elapsed = 0f;
        //    Quaternion originalRotation = transform.rotation;

        //    while (elapsed < shakeDuration)
        //    {
        //        float x = Random.Range(-shakeMagnitude, shakeMagnitude);
        //        //float z = Random.Range(-shakeMagnitude, shakeMagnitude);
        //        transform.rotation = originalRotation * Quaternion.Euler(x, 0, 0);

        //        elapsed += Time.deltaTime;
        //        yield return null;
        //    }

        //    transform.rotation = originalRotation;
        //}


        // MOVEMENT SECTIONS OF THE BUS

        public void MoveToSlots()
        {
            if (!isMovedToSlot)
            {
                isMovedToSlot = true;
                currentSlot = targetSlot;
                currentSlot.SetBus(this);
            }
    
            transform.position += transform.forward * Time.deltaTime * busSpeed;

            // Return on the bottom line

            if (transform.position.z < bottomZ && !isTurnedOnBottomLine)
            {
                if (transform.position.x > 0)
                {
                    transform.rotation = Quaternion.Euler(0, angleRight, 0);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, angleLeft, 0);
                }
    
                if (isRayOpen && relatedTunnel != null)
                    relatedTunnel.BusSendFromTunnel();
    
                isRayOpen = false;
                isTurnedOnBottomLine = true;
            }

            // Turn on Right-Left line

            if (transform.position.x < leftX && !isTurnedOnSideLine)
            {
                transform.rotation = Quaternion.Euler(0, angleUp, 0);
                isTurnedOnSideLine = true;
    
                if (isRayOpen && relatedTunnel != null)
                    relatedTunnel.BusSendFromTunnel();
    
                isRayOpen = false;
            }
            else if (transform.position.x > rightX && !isTurnedOnSideLine)
            {
                transform.rotation = Quaternion.Euler(0, angleUp, 0);
                isTurnedOnSideLine = true;
    
                if (isRayOpen && relatedTunnel != null)
                    relatedTunnel.BusSendFromTunnel();
    
                isRayOpen = false;
            }

            // Return on up line

            if (transform.position.z > upZ && !isTurnedOnUpLine)
            {
                if (transform.position.x <= currentSlot.transform.position.x)
                {
                    transform.rotation = Quaternion.Euler(0, angleRight, 0);
                    isComingFromLeft = true;
                    isTurnedOnUpLine = true;
    
                    if (isRayOpen && relatedTunnel != null)
                        relatedTunnel.BusSendFromTunnel();
    
                    isRayOpen = false;
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, angleLeft, 0);
                    isComingFromLeft = false;
                    isTurnedOnUpLine = true;
    
                    if (isRayOpen && relatedTunnel != null)
                        relatedTunnel.BusSendFromTunnel();
    
                    isRayOpen = false;
                }
    
            }

            // Return to slots

            if (transform.position.z > upZ && !isTurnedToSlot && isTurnedOnUpLine)
            {
                if (transform.position.x > currentSlot.transform.position.x && isComingFromLeft)
                {
                    transform.rotation = Quaternion.Euler(0, angleSlot, 0);
                    isTurnedToSlot = true;
                    transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.5f);
                    transform.position = new Vector3(currentSlot.transform.position.x, transform.position.y, transform.position.z);
                    //transform.SetParent(currentSlot.transform);
                }
                else if (transform.position.x < currentSlot.transform.position.x && !isComingFromLeft)
                {
                    transform.rotation = Quaternion.Euler(0, angleSlot, 0);
                    isTurnedToSlot = true;
                    transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.5f);
                    transform.position = new Vector3(currentSlot.transform.position.x, transform.position.y, transform.position.z);
                    //transform.SetParent(currentSlot.transform);
                }
            }


            // Stop in the slot

            float distance = (currentSlot.transform.position.z - transform.position.z);
    
            if (distance < 0.1f)
            {
                currentBusState = BusState.OnParking;
                transform.position = new Vector3(transform.position.x, transform.position.y, currentSlot.transform.position.z);
                BusSlotManager.Instance.allBusInSlots.Add(this);
                BusSlotManager.Instance.PassengersGoToBus();
                gameObject.layer = LayerMask.NameToLayer("Default");
            }
        }
    
    
        public void MoveToOut()
        {
            if (!isMovedToOut)
            {
                isMovedToOut = true;
                currentSlot.ClearSlot();
                currentSlot = null;
                BusSlotManager.Instance.allBusInSlots.Remove(this);
    
                //EventManager.Broadcast(GameEvent.OnPlaySound, "BusLeave");
            }
    
            // go back to the road
    
            if (transform.position.z > upZ)
            {
                transform.position -= transform.forward * Time.deltaTime * busSpeed;
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, angleRight, 0);
                transform.position += transform.forward * Time.deltaTime * busSpeed;
            }
    
            if (transform.position.x > 60)
            {
                if (manager.allbusControllers.Count == 0 && PassengerManager.Instance.currentPassengerList.Count == 0 && !manager.isFinished)
                    EventManager.Broadcast(GameEvent.OnWin);
    
                Destroy(gameObject);
            }
    
        }
    }
    
}
