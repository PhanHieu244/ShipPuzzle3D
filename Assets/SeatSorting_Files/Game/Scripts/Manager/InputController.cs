namespace EKStudio
{
    using System.Collections.Generic;
    using UnityEngine;
    
    public class InputController : InstanceManager<InputController>
    {
        private bool isDragging = false;
        public LayerMask baseLayer;
        private SeatController selectedSeat;
        private SeatSlotController previousSlot;
        public float snapRange = 1.0f, moveHeight = 3f;
        GameManager manager => GameManager.Instance;
    
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
    
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.CompareTag("Seat"))
                    {
                        selectedSeat = hit.collider.gameObject.GetComponent<SeatController>();
                        previousSlot = selectedSeat.currentSlot;
                        selectedSeat.currentSlot = null;
                        selectedSeat.transform.SetParent(null, true);
                        isDragging = true;
    
                        if (PlayerPrefs.GetInt("IsHapticOpen") == 1)
                            Vibration.VibratePop();
                    }
                }
            }
            if (isDragging)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
    
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, baseLayer))
                {
                    //Vector3 movePosition = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - hit.point).normalized + hit.point;
                    Vector3 movePosition = new Vector3(hit.point.x, moveHeight, hit.point.z);
                    selectedSeat.transform.position = movePosition;
                }
            }
    
            if (Input.GetMouseButtonUp(0))
            {
                if (isDragging)
                {
                    isDragging = false;
    
                    SeatSlotController nearestSlot = GetNearestSeatSlot();
    
                    if (nearestSlot != null && nearestSlot.currentSeat == null)
                    {
                        selectedSeat.currentSlot = nearestSlot;
                        selectedSeat.transform.SetParent(nearestSlot.transform, true);
                        selectedSeat.transform.localPosition = Vector3.zero;
                        selectedSeat.transform.rotation = Quaternion.LookRotation(nearestSlot.transform.forward);
                        string particleName = "SeatPlace_" + selectedSeat.seatType.ToString();
                        EventManager.Broadcast(GameEvent.OnParticlePlay, particleName, selectedSeat.transform.position + Vector3.up, new Vector3(90, 0, 0));
                    }
                    else
                    {
                        selectedSeat.transform.SetParent(previousSlot.transform, true);
                        selectedSeat.currentSlot = previousSlot;
                        selectedSeat.transform.localPosition = Vector3.zero;
                        selectedSeat.transform.rotation = Quaternion.LookRotation(previousSlot.transform.forward);
                        string particleName = "SeatPlace_" + selectedSeat.seatType.ToString();
                        EventManager.Broadcast(GameEvent.OnParticlePlay, particleName, selectedSeat.transform.position + Vector3.up, new Vector3(90, 0, 0));
                    }
    
                    selectedSeat = null;
    
                    manager.isFirstSeatMoved = true;
    
                    if (PlayerPrefs.GetInt("IsHapticOpen") == 1)
                        Vibration.VibratePop();
    
                    //EventManager.Broadcast(GameEvent.OnPlaySound, "Pop");
    
                    EventManager.Broadcast(GameEvent.SeatMoved);
                }
            }
        }
    
        SeatSlotController GetNearestSeatSlot()
        {
            SeatSlotController nearestSlot = null;
            float minDistance = Mathf.Infinity;
    
            foreach (SeatSlotController slot in SeatSlotManager.Instance.emptySeatSlotList)
            {
                float distance = Vector3.Distance(selectedSeat.transform.position, slot.transform.position);
    
                if (distance < minDistance && distance <= snapRange)
                {
                    minDistance = distance;
                    nearestSlot = slot;
                }
            }
    
            return nearestSlot;
        }
    }
}
