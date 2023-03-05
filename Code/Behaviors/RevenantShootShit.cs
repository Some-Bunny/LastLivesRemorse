using Alexandria.EnemyAPI;
using Brave.BulletScript;
using Dungeonator;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LastLivesRemorse
{
    public  class RevenantShootShit : RevenantBaseBehavior
    {
        
        public override bool CustomCanBeActivated()
        {
            if (ReturnTrackedPlayer().CurrentRoom != this.Body.transform.position.GetAbsoluteRoom()) { return false; }
            if (Vector2.Distance(this.Body.UnitCenter, ReturnTrackedPlayer().transform.PositionVector2()) < 8) { return false; }
            bool b = false;
            if (this.controller.transform.position.GetAbsoluteRoom() != null)
            {
                var currentRoom = this.controller.transform.position.GetAbsoluteRoom();
                CellData nearestCellToPosition = currentRoom.GetNearestCellToPosition(this.controller.transform.PositionVector2());
                CellData nearestCellToPosition2 = currentRoom.GetNearestCellToPosition(this.controller.transform.PositionVector2() + Vector2.left);
                CellData nearestCellToPosition3 = currentRoom.GetNearestCellToPosition(this.controller.transform.PositionVector2() + Vector2.right);
                CellData nearestCellToPosition4 = currentRoom.GetNearestCellToPosition(this.controller.transform.PositionVector2() + Vector2.up);
                CellData nearestCellToPosition5 = currentRoom.GetNearestCellToPosition(this.controller.transform.PositionVector2() + Vector2.down);
                bool flag2 = !nearestCellToPosition.isNextToWall && !nearestCellToPosition2.isNextToWall && !nearestCellToPosition3.isNextToWall && !nearestCellToPosition4.isNextToWall && !nearestCellToPosition5.isNextToWall;
                if (!flag2)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return CooldownIsFull() == true && b == true;
        }

        public override void StartBehavior()
        {
            base.StartBehavior();
            this.Body.StartCoroutine(StartAttack());
        }

        public IEnumerator StartAttack()
        {
               
            var vel = Body.Velocity;
            float f = 0;
            while (f < 1)
            {
                f += BraveTime.DeltaTime;
                Body.Velocity = Vector2.Lerp(vel, Vector2.zero, f * 1.25f);
                yield return null;
            }
            AkSoundEngine.PostEvent("Play_ENM_cult_charge_01", this.Body.gameObject);
            f = 0;
            this.controller.AIAnimator.PlayForDuration("vomit", 1.1f, true, null, -1f, false);
            while (f < 0.5f)
            {
                f += BraveTime.DeltaTime;
                yield return null;
            }
            AkSoundEngine.PostEvent("Play_ENM_bulletking_slam_01", this.Body.gameObject);
            BulletScriptSource bulletScriptSource = Body.gameObject.GetOrAddComponent<BulletScriptSource>();
            bulletScriptSource.BulletManager = Body.GetComponent<AIBulletBank>();
            bulletScriptSource.BulletScript = new CustomBulletScriptSelector(typeof(Ball));
            bulletScriptSource.Initialize();

            this.FinishBehavior();
            yield break;
        }


        public override void OnUpdated()
        {
            base.OnUpdated();
        }
        public override float Cooldown => 15;
        public override float CooldownVariance => 10;

        public override float Weight => 3;

        public override BehaviorType behaviorType => BehaviorType.Active;
    }
    public class Ball : Script
    {
       
        public override IEnumerator Top()
        {
            for (int e = 0; e < 3; e++)
            {
                base.Fire(new Direction(0, DirectionType.Aim, -1f), new Speed(2 + e, SpeedType.Absolute), new SpeedChangingBullet("frogger", 11, 120));
                base.Fire(new Direction(20 + (e* 5), DirectionType.Aim, -1f), new Speed(2 + e, SpeedType.Absolute), new SpeedChangingBullet("frogger", 11, 120));
                base.Fire(new Direction(-20 - (e * 5), DirectionType.Aim, -1f), new Speed(2 + e, SpeedType.Absolute), new SpeedChangingBullet("frogger", 11, 120));
            }
            yield break;
        }
    }
}
