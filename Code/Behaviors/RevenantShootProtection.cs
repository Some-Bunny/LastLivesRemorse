using Alexandria.EnemyAPI;
using Brave.BulletScript;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LastLivesRemorse.Storage.Behaviors
{
    public  class RevenantShootProtection : RevenantBaseBehavior
    {
        
        public override bool CustomCanBeActivated()
        {
            if (ReturnTrackedPlayer().CurrentRoom == null) { return false; }
            if (ReturnTrackedPlayer().CurrentRoom.activeEnemies == null) { return false; }
            if (ReturnTrackedPlayer().CurrentRoom.activeEnemies.Count == 0) { return false; }
            return CooldownIsFull() == true;
        }

        public override void StartBehavior()
        {
            base.StartBehavior();
            this.Body.StartCoroutine(StartAttack());
        }

        public IEnumerator StartAttack()
        {
            AIActor enemyToProtect = BraveUtility.RandomElement<AIActor>(ReturnTrackedPlayer().CurrentRoom.activeEnemies);
            float f = 0;
            while (enemyToProtect.healthHaver.IsDead == false && Vector2.Distance(enemyToProtect.sprite.WorldCenter, this.aIAnimator.gameObject.transform.PositionVector2()) > 2.25f)
            {
                if (enemyToProtect)
                {
                    PointRevenantToPosition(enemyToProtect.transform.PositionVector2());
                }

                Vector2 centerPosition = enemyToProtect.CenterPosition;
                Vector2 vector = centerPosition - Body.UnitCenter;
                float a = Vector2.Distance(Body.UnitCenter, centerPosition);
                float d = a < 10 ? Mathf.Lerp(-1.25f, 20, (a) / 15) : 20;
                Body.Velocity = vector.normalized * (d * f);
                foreach (var scarf in controller.scarves)
                {
                    scarf.LastDirection = scarf.BaseLastDirection + Toolbox.GetUnitOnCircle(controller.AIAnimator.FacingDirection, Mathf.Max(0.5f, Mathf.Min(Mathf.Lerp(-1.25f, d, a / 15), 1f)));
                }
                if (f < 1)
                {
                    f += BraveTime.DeltaTime;
                }
                yield return null;
            }
            var vel = Body.Velocity;
            f = 0;
            while (f < 1)
            {
                f += BraveTime.DeltaTime * 3;
                Body.Velocity = Vector2.Lerp(vel, Vector2.zero, f);
                yield return null;
            }
            f = 0;
            this.controller.AIAnimator.PlayForDuration("vomit", 0.9f, true, null, -1f, false);
            while (f < 0.45f)
            {
                f += BraveTime.DeltaTime;
                yield return null;
            }
            if (enemyToProtect != null)
            {
                AkSoundEngine.PostEvent("Play_Rage", this.Body.gameObject);
                BulletScriptSource bulletScriptSource = Body.gameObject.GetOrAddComponent<BulletScriptSource>();
                bulletScriptSource.BulletManager = Body.GetComponent<AIBulletBank>();
                bulletScriptSource.BulletScript = new CustomBulletScriptSelector(typeof(Shot));
                bulletScriptSource.Initialize();
            }
            else
            {
                this.FinishBehavior();
            }
            this.FinishBehavior();
            yield break;
        }


        public override void OnUpdated()
        {
            base.OnUpdated();
        }
        public override float Cooldown => 25;
        public override float CooldownVariance => 20;

        public override float Weight => 2;

        public override BehaviorType behaviorType => BehaviorType.Neutral;
    }
    public class Shot : Script
    {
       
        public override IEnumerator Top()
        {
            float r = BraveUtility.RandomAngle();
            for (int i = 0; i < 16; i++)
            { 
                base.Fire(new Direction(r + (22.5f * i), DirectionType.Absolute, -1f), new Speed((BraveUtility.RandomAngle() / 120) + 2, SpeedType.Absolute), new LingeringBullet());
                yield return this.Wait(3);
            }


            yield break;
        }

        public class LingeringBullet : Bullet
        {
            public LingeringBullet() : base("frogger", false, false, false)
            {

            }
            public override IEnumerator Top()
            {
                this.Projectile.collidesOnlyWithPlayerProjectiles = true;
                this.Projectile.collidesWithProjectiles = true;
                this.Projectile.UpdateCollisionMask();
                float f = this.Speed / 90;
                for (int e = 0; e < 90; e++)
                {
                    this.Projectile.UpdateCollisionMask();
                    this.Speed -= f;
                    this.Speed = Mathf.Max(0.1f, this.Speed);
                    this.UpdateVelocity();

                    yield return this.Wait(1);
                }
                yield return this.Wait(840);
                base.Vanish(false);
                yield break;
            }
        }
    }
}
