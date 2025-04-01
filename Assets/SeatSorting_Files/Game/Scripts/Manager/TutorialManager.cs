namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using DG.Tweening;
    using TMPro;
    
    public class TutorialManager : InstanceManager<TutorialManager>
    {
        public GameObject fingerPrefab;
        public List<Transform> startPoints;
        public List<Transform> endPoints;
        public float moveDuration = 1f;
        public int currentIndex = 0;
        private GameObject fingerObject;
        public GameObject textObject;
        public TextMeshPro tutorialText;
        public List<string> tutorialTexts;
        private bool isMoving = false, startCheck = false;
        private bool isTapEffectPlaying = false;
        public List<SeatSlotController> otherSeatSlots;
        public SeatController blueSeat, redSeat;
        public BusController blueBus, redBus;
    
        GameManager manager => GameManager.Instance;
    
        private void Awake()
        {
            if (manager.gameData.levelCount > 1)
            {
                gameObject.SetActive(false);
            }
        }
        void Start()
        {
            DOVirtual.DelayedCall(1f, () =>
            {
                foreach (SeatSlotController slot in otherSeatSlots)
                {
                    slot.currentSeat.transform.tag = "Untagged";
                }
    
                redSeat = blueBus.slotList[1].currentSeat;
                blueSeat = redBus.slotList[1].currentSeat;
                blueSeat.transform.GetComponent<BoxCollider>().enabled = false;
                startCheck = true;
            });
    
            textObject.SetActive(true);
            SpawnFinger();
            MoveFingerToCurrentPoint();
        }
    
        void SpawnFinger()
        {
            fingerObject = Instantiate(fingerPrefab, startPoints[currentIndex].position, Quaternion.identity);
        }
    
        private void Update()
        {
            if (tutorialTexts.Count > currentIndex)
                tutorialText.text = tutorialTexts[currentIndex];
    
            if (!startCheck)
                return;
    
            switch (currentIndex)
            {
                case 0:
                    if (redSeat.currentSlot == endPoints[currentIndex].GetComponent<SeatSlotController>())
                    {
                        OnUserInteraction();
                        blueSeat.transform.GetComponent<BoxCollider>().enabled = true;
                    }
                    break;
                case 1:
                    if (blueSeat.currentSlot == endPoints[currentIndex].GetComponent<SeatSlotController>())
                    {
                        OnUserInteraction();
                    }
                    break;
                case 2:
                    if (blueBus.isMovedToSlot)
                    {
                        OnUserInteraction();
                    }
                    break;
                case 3:
                    if (redSeat.currentSlot == endPoints[currentIndex].GetComponent<SeatSlotController>())
                    {
                        OnUserInteraction();
                    }
                    break;
                case 4:
                    if (redBus.isMovedToSlot)
                    {
                        OnUserInteraction();
                    }
                    break;
                case 5:
                    break;
                default:
                    break;
            }
        }
    
        public void MoveFingerToCurrentPoint()
        {
            fingerObject.transform.DOKill();
    
            if (currentIndex >= startPoints.Count)
            {
                fingerObject.SetActive(false);
                textObject.SetActive(false);
                return;
            }
    
            isMoving = true;
            fingerObject.transform.position = startPoints[currentIndex].position;
            fingerObject.SetActive(true);
    
    
            if (endPoints[currentIndex] == null)
            {
                PlayFingerTapEffect();
                return;
            }
    
            DOVirtual.DelayedCall(0.3f, () =>
            {
                fingerObject.transform.DOMove(endPoints[currentIndex].position, moveDuration)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        if (isMoving)
                        {
                            DOVirtual.DelayedCall(0.3f, () =>
                            {
                                fingerObject.SetActive(false);
                                MoveFingerToCurrentPoint();
                            });
                        }
                    });
    
            });
    
        }
    
        private void PlayFingerTapEffect()
        {
            //if (fingerObject == null || isTapEffectPlaying) return;
    
            isTapEffectPlaying = true;
    
            fingerObject.transform.DOScale(Vector3.one * 0.8f, 0.1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    fingerObject.transform.DOScale(Vector3.one, 0.1f)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                        {
                            DOVirtual.DelayedCall(0.5f, () =>
                            {
                                isTapEffectPlaying = false;
                                fingerObject.SetActive(false);
                                MoveFingerToCurrentPoint();
                            });
                        });
                });
        }
    
        public void OnUserInteraction()
        {
            currentIndex++;
            MoveFingerToCurrentPoint();
        }
    }
    
}
