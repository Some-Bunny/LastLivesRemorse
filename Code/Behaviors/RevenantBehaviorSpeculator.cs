using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LastLivesRemorse
{
    public class RevenantBehaviorSpeculator : MonoBehaviour
    {

        public List<RevenantBaseBehavior> SpecialBehaviors = new List<RevenantBaseBehavior>();

        public float Cooldown_Speed_Multiplier = 1;
        private bool Enabled = true;
        public bool ShouldSpeculate = true;
        public bool BehaviorsActive = false;
        public bool Enabled_Tick = true;
        public float AttackCooldown = 0;

        public void DoForceEndAllBehaviorTick()
        {
            foreach (var entry in SpecialBehaviors)
            {
                entry.OnAnyBehaviorStopped();
            }
        }

        public void Update()
        {
            if (this.gameObject)
            {
                if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
                {
                    this.Enabled = false;
                    ShouldSpeculate = false;
                    return;
                }
                if (BossKillCam.BossDeathCamRunning || GameManager.Instance.PreventPausing)
                {
                    this.Enabled = false;
                    ShouldSpeculate = false;
                    return;
                }
                if (TimeTubeCreditsController.IsTimeTubing)
                {
                    this.Enabled = false;
                    ShouldSpeculate = false;

                    base.gameObject.SetActive(false);
                    return;
                }
                else
                {
                    ShouldSpeculate = true;
                    this.Enabled = true;
                }
            }


            if (Enabled == false ) { return; }
            if (Enabled_Tick == false) { return; }
            foreach (var entry in SpecialBehaviors)
            {
                entry.UpdateCooldowns(Cooldown_Speed_Multiplier);
            }


            foreach (var entry in SpecialBehaviors)
            {
                entry.OnPreUpdated();
            }

            if (AttackCooldown < 0)
            {
                AttackCooldown -= BraveTime.DeltaTime;
                return;
            }

            foreach (var entry in SpecialBehaviors)
            {
                if (entry.currentState == RevenantBaseBehavior.BehaviorState.Active) { BehaviorsActive = true; return; }
            }
            if (BehaviorsActive == true)
            {
                foreach (var entry in SpecialBehaviors)
                {
                    entry.OnAnyBehaviorStopped();
                }
                BehaviorsActive = false;
            }


            foreach (var entry in SpecialBehaviors)
            {
                entry.OnUpdated();
            }


            List<RevenantBaseBehavior> beh = new List<RevenantBaseBehavior>();
            foreach (var entry in SpecialBehaviors)
            {
                if (entry.CanBeActivated() == true)
                {
                    beh.Add(entry);
                }
            }
            if (beh.Count > 0)
            {
                var behavior = SelectByWeight(beh);
                behavior.StartBehavior();
            }
        }


        public RevenantBaseBehavior SelectByWeight(List<RevenantBaseBehavior> b, bool useSeedRandom = false)
        {
            List<RevenantBaseBehavior> list = new List<RevenantBaseBehavior>();
            float num = 0f;
            for (int i = 0; i < b.Count; i++)
            {
                RevenantBaseBehavior weightedGameObject = b[i];
                bool flag = true;

                if (flag)
                {
                    list.Add(weightedGameObject);
                    num += weightedGameObject.Weight;
                }
            }
            float num2 = ((!useSeedRandom) ? UnityEngine.Random.value : BraveRandom.GenerationRandomValue()) * num;
            float num3 = 0f;
            for (int k = 0; k < list.Count; k++)
            {
                num3 += list[k].Weight;
                if (num3 > num2)
                {
                    return list[k];
                }
            }
            return list[list.Count - 1];
        }
    }
}
