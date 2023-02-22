using Alexandria.EnemyAPI;
using Brave.BulletScript;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LastLivesRemorse.Storage.Behaviors
{
    public  class RevenantChargeBehavior : RevenantBaseBehavior
    {
        
        public override bool CustomCanBeActivated()
        {
            float f = Vector2.Distance(this.Body.UnitCenter, ReturnTrackedPlayer().transform.PositionVector2());
            return f < 5f && CooldownIsFull() == true;
        }

        public override void StartBehavior()
        {
            base.StartBehavior();
            this.Body.StartCoroutine(StartPew());
        }

        public IEnumerator StartPew()
        {
            float f = 0;
            Vector2 storedVel = this.Body.Velocity;
            AkSoundEngine.PostEvent("Play_Pissed", this.Body.gameObject);
            this.controller.AIAnimator.PlayForDuration("chargeup", 1.25f, true, null, -1f, false);

            while (f < 1.25f)
            {
                aIAnimator.FacingDirection = (ReturnTrackedPlayer().CenterPosition - aIAnimator.transform.PositionVector2()).ToAngle();
                aIAnimator.ChildAnimator.gameObject.transform.position = Vector3.Lerp(this.aIAnimator.ChildAnimator.transform.position, this.aIAnimator.transform.position + new Vector3(0.125f, -0.5f), f);
                this.Body.Velocity = Vector2.Lerp(storedVel, Vector2.zero, f);
                f += BraveTime.DeltaTime;
                yield return null;
            }

            f = 0;

            controller.OnPlayerContacted += OnTouchedPlayer;
            controller.OnEnemyContacted += OnTouchedEnemy;

            this.controller.AIAnimator.PlayForDuration("charge", 0.6f, true, null, -1f, false);

            Vector2 centerPosition = ReturnTrackedPlayer().CenterPosition;
            Vector2 vector = centerPosition - Body.UnitCenter;


            this.controller.AIAnimator.LockFacingDirection = true;
            this.controller.AIAnimator.facingType = AIAnimator.FacingType.Movement;

            ParticleSystem particleSystem = UnityEngine.Object.Instantiate(Particles.fireParticles.gameObject).GetComponent<ParticleSystem>();
            particleSystem.Stop();
            particleSystem.transform.localScale *= 0.3f;
            particleSystem.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
            AkSoundEngine.PostEvent("Play_Attack_Dash", this.Body.gameObject);
            while (f < 0.4f)
            {
                ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
                {
                    position = this.aIAnimator.sprite.WorldCenter,
                    randomSeed = (uint)UnityEngine.Random.Range(1, 1000)
                };
                particleSystem.gameObject.SetActive(true);
                particleSystem.Emit(emitParams, 1);
                Body.Velocity = vector.normalized * Mathf.Lerp(0, 30, f * 4);
                this.controller.AIAnimator.FacingDirection = Body.Velocity.ToAngle();
                foreach (var scarf in controller.scarves)
                {
                    scarf.ScarfLength = 3;
                    scarf.LastDirection = scarf.BaseLastDirection + Toolbox.GetUnitOnCircle(controller.AIAnimator.FacingDirection, 4);
                }
                aIAnimator.ChildAnimator.gameObject.transform.position = Vector3.Lerp(this.aIAnimator.ChildAnimator.transform.position, this.aIAnimator.transform.position + new Vector3(0.125f, -0.5f) + Toolbox.GetUnitOnCircle(aIAnimator.FacingDirection - 180, 0.4f).ToVector3ZisY(), f * 6);

                f += BraveTime.DeltaTime;
                yield return null;
            }
            foreach (var scarf in controller.scarves)
            {
                scarf.ScarfLength = scarf.BaseScarfLength;
            }

            var speed = Body.Velocity;
            f = 0;
            while (f < 0.5f)
            {
                ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
                {
                    position = this.aIAnimator.sprite.WorldCenter,
                    randomSeed = (uint)UnityEngine.Random.Range(1, 1000)
                };
                particleSystem.gameObject.SetActive(true);
                particleSystem.Emit(emitParams, 1);

                Body.Velocity = Vector3.Lerp(speed, Vector2.zero, f* 2);

                f += BraveTime.DeltaTime;
                yield return null;
            }
            Destroy(particleSystem.gameObject, 3);
            this.controller.AIAnimator.LockFacingDirection = false;
            this.controller.AIAnimator.facingType = AIAnimator.FacingType.Target;
            controller.OnEnemyContacted -= OnTouchedEnemy;

            controller.OnPlayerContacted -= OnTouchedPlayer;
            this.FinishBehavior();
            yield break;
        }

        public void OnTouchedPlayer(PlayerController player, GameObject REVENANT)
        {
            if (speculator.ShouldSpeculate == true)
            {
                player.healthHaver.ApplyDamage(1f, Vector2.zero, "Revenant", CoreDamageTypes.None, DamageCategory.Normal, true, null, false);
            }
        }
        public void OnTouchedEnemy(AIActor enemy, GameObject REVENANT)
        {
            if (speculator.ShouldSpeculate == true)
            {
                enemy.healthHaver.ApplyDamage(3f, Vector2.zero, "Revenant", CoreDamageTypes.None, DamageCategory.Normal, true, null, false);
            }
        }

        public override void OnUpdated()
        {
            base.OnUpdated();
        }
        public override float Cooldown => 4;

        public override float AttackCooldown => 2;

        public override BehaviorType behaviorType => BehaviorType.Active;
    }
}
