using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LastLivesRemorse
{
    public class RevenantBaseBehavior : ScriptableObject
    {

        public void Start()
        {
            OnStarted();
        }
        public virtual void OnStarted()
        {

        }


        public virtual void StartBehavior()
        {
            currentState = BehaviorState.Active;
        }

        public void Update()
        {

        }

        public virtual void OnUpdated()
        {

        }
        public virtual void OnPreUpdated()
        {

        }
        public void UpdateCooldowns(float CustomSpeedMult = 1)
        {
            if (this.behaviorType == RevenantBaseBehavior.BehaviorType.Neutral)
            { UpdateThem(CustomSpeedMult); return; }
            if (this.currentState == RevenantBaseBehavior.BehaviorState.Inactive) { UpdateThem(CustomSpeedMult); }
        }

        private void UpdateThem(float C)
        {
            if (current_Cooldown < Cooldown)
            {
                current_Cooldown += (BraveTime.DeltaTime* C);
            }
        }

        public virtual void OnAnyBehaviorStopped()
        {

        }


        public bool CooldownIsFull()
        {
            if (current_Cooldown < Cooldown)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool CanBeActivated()
        {
            if (currentState == BehaviorState.Active) { return false; }
            return CustomCanBeActivated();
        }

        public virtual bool CustomCanBeActivated()
        {
            return true;
        }

        public void FinishBehavior()
        {
            current_Cooldown = 0 - UnityEngine.Random.Range(0, CooldownVariance);
            currentState = BehaviorState.Inactive;
            speculator.AttackCooldown += AttackCooldown;
            OnFinished();
        }

        public void PointRevenantToPosition(Vector2 position)
        {
            if (aIAnimator)
            {
                float f = BraveMathCollege.Atan2Degrees(position - aIAnimator.spriteAnimator.sprite.WorldCenter);
                aIAnimator.FacingDirection = f;
                if (aIAnimator.ChildAnimator)
                {
                    aIAnimator.ChildAnimator.FacingDirection = f;
                }
            }
        }
        public virtual void OnFinished()
        {

        }

        public PlayerController ReturnTrackedPlayer()
        {
            return GameManager.Instance.GetActivePlayerClosestToPoint(this.Body.transform.PositionVector2());
        }

        public BehaviorState currentState = BehaviorState.Inactive;
        public virtual BehaviorType behaviorType 
        {
            get 
            {
                return BehaviorType.Active;
            }
        }

        public virtual float Cooldown
        {
            get
            {
                return 5;
            }
        }

        public virtual float CooldownVariance
        {
            get
            {
                return 0;
            }
        }

        public virtual float Weight
        {
            get
            {
                return 1;
            }
        }

        public virtual float AttackCooldown
        {
            get
            {
                return 1;
            }
        }

        private float current_Cooldown = 0;
        public SpeculativeRigidbody Body;
        public AIAnimator aIAnimator;
        public RevenantBehaviorSpeculator speculator;
        public RevenantController controller;

        public enum BehaviorState
        {
            Inactive,
            Active,
            Waiting
        }
        public enum BehaviorType
        {
            Active,
            Neutral
        }
    }
}
