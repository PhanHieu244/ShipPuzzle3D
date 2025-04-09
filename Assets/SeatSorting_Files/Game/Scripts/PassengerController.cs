namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using Dreamteck.Splines;
    using UnityEngine;
    using DG.Tweening;

    public class PassengerController : MonoBehaviour
    {
        public int currentIndex;
        public BusSeatPassType passengerType;
        private SplineFollower splineFollower => GetComponent<SplineFollower>();
        public Animator animator => GetComponentInChildren<Animator>();
        private float moveSpeed, moveToPosition;
        public bool isMoving, isSitting, isJumped;
        public Color currentColor;

        public void Initialize(SplineComputer spline, float initialPosition, float moveSpeed)
        {
            splineFollower.spline = spline;
            this.moveSpeed = moveSpeed;
            StartMoving(initialPosition);
            currentColor = GetComponentInChildren<MeshRenderer>().material.color;
        }

        public void StartMoving(float movePosition)
        {
            if (!isSitting)
            {
                isMoving = true;
                splineFollower.followSpeed = moveSpeed;
                moveToPosition = movePosition;
                animator.SetTrigger("isWalk");
            }
        }

        public bool HasReachedEnd()
        {
            return splineFollower.GetPercent() >= 1f;
        }

        private void Update()
        {
            if (!isSitting)
            {
                if (splineFollower.GetPercent() >= moveToPosition)
                {
                    splineFollower.followSpeed = 0;
                    animator.SetTrigger("isIdle");
                    isMoving = false;
                }
            }
        }

        public void JumpToBus(SeatSlotController jumpSlot)
        {
            isSitting = true;
            isMoving = false;

            if (splineFollower)
            {
                splineFollower.spline = null;
                Destroy(GetComponent<SplineFollower>());
            }

            transform.SetParent(jumpSlot.transform, true);
            transform.rotation = Quaternion.LookRotation(jumpSlot.transform.forward);
            ResetAllTriggers();
            animator.SetTrigger("isSit");

            transform.DOLocalJump(new Vector3(0f, 1.34f, 0.34f), 1f, 1, 0.25f).OnComplete(() => {
                PassengerManager.Instance.currentPassengerList.Remove(this);
                PassengerManager.Instance.MoveOtherPassengers();
                //When you sit down, the count increases.
                BusController jumpedBus = jumpSlot.transform.root.GetComponent<BusController>();
                //jumpedBus.passengerCount++;

                if (jumpedBus.passengerCount == 3)
                    jumpedBus.isReadyToMoveOut = true;

                BusSlotManager.Instance.PassengersGoToBus();
            });
        }

        void ResetAllTriggers()
        {
            foreach (AnimatorControllerParameter parameter in animator.parameters)
            {
                if (parameter.type == AnimatorControllerParameterType.Trigger)
                {
                    animator.ResetTrigger(parameter.name);
                }
            }
        }

    }
}
