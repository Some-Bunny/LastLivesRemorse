using HutongGames.PlayMaker.Actions;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LastLivesRemorse.Storage.Behaviors
{
    public  class RevenantMoveBehavior : RevenantBaseBehavior
    {


        public override bool CustomCanBeActivated()
        {
            return false;
        }

        public override void OnAnyBehaviorStopped()
        {
            this.Body.StartCoroutine(Lerp());
        }

        public IEnumerator Lerp()
        {
            float f = 0;
            while (f < 1)
            {
                SpeedMult = f;
                f += BraveTime.DeltaTime;
                yield return null;
            }
            SpeedMult = 1;
            yield break;
        }

        public override void OnUpdated()
        {
            base.OnUpdated();
            if (aIAnimator)
            {
                float f = BraveMathCollege.Atan2Degrees(ReturnTrackedPlayer().transform.PositionVector2() - aIAnimator.spriteAnimator.sprite.WorldCenter);
                aIAnimator.FacingDirection = f;
                aIAnimator.ChildAnimator.FacingDirection = f;
                var y = Mathf.Sin(Time.realtimeSinceStartup * 2);
                aIAnimator.ChildAnimator.gameObject.transform.position = Vector3.Lerp(this.aIAnimator.transform.position + new Vector3(0.125f, -0.5f), this.aIAnimator.transform.position + new Vector3(0.125f, -0.75f), y);
            }

            Vector2 centerPosition = ReturnTrackedPlayer().CenterPosition;
            Vector2 vector = centerPosition - Body.UnitCenter;
            //float magnitude = vector.magnitude;
            float a = Vector2.Distance(Body.UnitCenter, centerPosition);
            //float d = Mathf.Lerp(MaxSpeed, MinSpeed, Mathf.Lerp(-1.25f, MaxSpeed, (a) / 15));
            float d = Vector2.Distance(Body.UnitCenter, centerPosition) < 11 ? Mathf.Lerp(-1.25f, MaxSpeed, (a) / 15) : MaxSpeed;
            Body.Velocity = vector.normalized * (d* SpeedMult);       

            foreach(var scarf in controller.scarves)
            {
                scarf.LastDirection = scarf.BaseLastDirection + Toolbox.GetUnitOnCircle(controller.AIAnimator.FacingDirection, Mathf.Max(0.5f, Mathf.Min(Mathf.Lerp(-1.25f, d, (a) / 15), 1f))) ;
            }
            StoredVelocity = Body.Velocity;
        }

        public override BehaviorType behaviorType => BehaviorType.Neutral;

        public float MinSpeed = 6;
        public float MaxSpeed = 10;
        public float SuperFarRange = 50;
        public float SpeedMult = 1;


        private Vector2 StoredVelocity;
    }
}
