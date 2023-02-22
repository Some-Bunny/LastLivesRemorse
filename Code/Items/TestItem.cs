using Alexandria.EnemyAPI;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using Alexandria.PrefabAPI;
using JuneLib.Items;
using LastLivesRemorse;
using LastLivesRemorse.Code.Items;
using LastLivesRemorse.Storage.Behaviors;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static ETGMod;
using static UnityEngine.ParticleSystem;


namespace LastLivesRemorse
{
    public class RevenantController : MonoBehaviour
    {
        public AIAnimator AIAnimator;
        public GameObject ChildObject;
        public SpeculativeRigidbody Body;
        public RevenantBehaviorSpeculator speculator;
        public List<CustomScarfDoerActorless> scarves = new List<CustomScarfDoerActorless>();


        public void AddBehavior(RevenantBaseBehavior obj)
        {
            var m = obj;
            m.Body = Body;
            m.aIAnimator = AIAnimator;
            m.speculator = speculator;
            m.controller = this;
            speculator.SpecialBehaviors.Add(m);
        }

        public void Start()
        {
            if (this.GetComponent<AIAnimator>())
            {
                AIAnimator = this.GetComponent<AIAnimator>();
                ChildObject = AIAnimator.ChildAnimator.gameObject;
            }
            Body = this.GetComponent<SpeculativeRigidbody>();
            speculator = this.GetComponent<RevenantBehaviorSpeculator>();
            speculator.Enabled_Tick = false;
            var m = ScriptableObject.CreateInstance<RevenantMoveBehavior>();
            m.Body = Body;
            m.aIAnimator = AIAnimator;
            m.speculator = speculator;
            m.controller = this;


            AddBehavior(ScriptableObject.CreateInstance<RevenantMoveBehavior>());
            AddBehavior(ScriptableObject.CreateInstance<RevenantChargeBehavior>());
            AddBehavior(ScriptableObject.CreateInstance<RevenantShootProtection>());
            AddBehavior(ScriptableObject.CreateInstance<RevenantQuickChargeBehavior>());
            AddBehavior(ScriptableObject.CreateInstance<RevenantShootShit>());

            /*
            var m1 = ScriptableObject.CreateInstance<RevenantChargeBehavior>();
            m1.Body = Body;
            m1.aIAnimator = AIAnimator;
            m1.speculator = speculator;
            m1.controller = this;

            var m2 = ScriptableObject.CreateInstance<RevenantShootProtection>();
            m2.Body = Body;
            m2.aIAnimator = AIAnimator;
            m2.speculator = speculator;
            m2.controller = this;


            speculator.SpecialBehaviors.Add(m);
            speculator.SpecialBehaviors.Add(m1);
            */
            Body.OnPreRigidbodyCollision += DoCollision;


            for (int i =0; i < 5; i++)
            {
                CustomScarfDoerActorless scorf = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(436) as BlinkPassiveItem).ScarfPrefab.gameObject).AddComponent<CustomScarfDoerActorless>();
                Destroy(scorf.gameObject.GetComponent<ScarfAttachmentDoer>());
                scorf.AttachTarget = this.gameObject;
                scorf.AttachTransform = this.gameObject.transform;
                scorf.ScarfMaterial = (PickupObjectDatabase.GetById(436) as BlinkPassiveItem).ScarfPrefab.ScarfMaterial;
                scorf.StartWidth = 0.1f;
                scorf.EndWidth = 0.03f;
                scorf.AnimationSpeed = UnityEngine.Random.Range(12, 25);

                scorf.ScarfLength = 0.7f + (0.1f * i);
                scorf.BaseScarfLength = 0.7f + (0.1f * i);

                scorf.AngleLerpSpeed = UnityEngine.Random.Range(3, 20);
                scorf.BackwardZOffset = -0.2f;
                scorf.CatchUpScale = UnityEngine.Random.Range(0.5f, 1.2f);
                scorf.SinSpeed = UnityEngine.Random.Range(4, 7);
                scorf.AmplitudeMod = UnityEngine.Random.Range(0.1f, 0.5f);
                scorf.WavelengthMod = UnityEngine.Random.Range(0.5f, 1.5f);
                scorf.ScarfMaterial.SetColor("_OverrideColor", new Color(0.23f, 0.2f, 0.2f, 1));
                scorf.LastDirection = Toolbox.GetUnitOnCircle(Vector2.up.ToAngle() + UnityEngine.Random.Range(-20, 20), 1) + Toolbox.GetUnitOnCircle(AIAnimator.FacingDirection, 0.5f);
                scorf.BaseLastDirection = Toolbox.GetUnitOnCircle(Vector2.up.ToAngle() + UnityEngine.Random.Range(-20, 20), 1);
                scorf.offset = new Vector2(0.5f, 0.125f);
                scarves.Add(scorf);
            }
            this.StartCoroutine(SpawnSequence());
        }

        public IEnumerator SpawnSequence()
        {
            Vector2 l = this.gameObject.transform.position - new Vector3(0, 1);
            AkSoundEngine.PostEvent("Play_RoarSpawn", this.Body.gameObject);
            this.AIAnimator.PlayUntilFinished("spawn");
            ParticleSystem particleSystem = UnityEngine.Object.Instantiate(Particles.fireParticles.gameObject).GetComponent<ParticleSystem>();
            particleSystem.transform.localScale *= 0.4f;
            particleSystem.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
            particleSystem.gameObject.transform.position = this.AIAnimator.sprite.WorldCenter;
            particleSystem.Play();
            particleSystem.gameObject.SetActive(true);
            foreach (var scarf in scarves)
            {
                scarf.LastDirection = scarf.BaseLastDirection +Toolbox.GetUnitOnCircle(Vector2.up.ToAngle() + UnityEngine.Random.Range(- 30, 30), 2f);
            }
            float f = 0;
            while (this.AIAnimator.IsPlaying("spawn")) 
            {
                f += BraveTime.DeltaTime;
                foreach (var scarf in scarves)
                {
                    scarf.ScarfLength = Mathf.Lerp(0.25f, scarf.BaseScarfLength * 1.5f, f * 0.7f);
                }
                this.gameObject.transform.position = Vector3.Lerp(l, l + new Vector2(0, 1), f);
                yield return null;
            }
            f = 0;
            particleSystem.Stop();
            while (f < 1)
            {
                f += BraveTime.DeltaTime;
                foreach (var scarf in scarves)
                {
                    scarf.ScarfLength = Mathf.Lerp(scarf.BaseScarfLength * 1.5f, scarf.BaseScarfLength,f);
                }
                yield return null;
            }

            speculator.DoForceEndAllBehaviorTick();
            speculator.Enabled_Tick = true;
            Destroy(particleSystem.gameObject, 3);
            yield break;
        }


        private void DoCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            PhysicsEngine.SkipCollision = true;
            if (otherRigidbody.gameObject.GetComponent<PlayerController>())
            {
                if (OnPlayerContacted != null) { OnPlayerContacted(otherRigidbody.gameObject.GetComponent<PlayerController>(), this.gameObject); }
            }
            if (otherRigidbody.gameObject.GetComponent<AIActor>())
            {
                if (OnEnemyContacted != null) { OnEnemyContacted(otherRigidbody.gameObject.GetComponent<AIActor>(), this.gameObject); }
            }
        }
        public Action<PlayerController, GameObject> OnPlayerContacted;
        public Action<AIActor, GameObject> OnEnemyContacted;

        public void Update()
        {
           

      
        }

    }






    public class WIP : PlayerItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(WIP))
        {
            Name = "WIP",
            Description = "",
            LongDescription = "",
            ManualSpriteCollection = StaticCollections.Item_Collection,
            ManualSpriteID = StaticCollections.Item_Collection.GetSpriteIdByName("WIP"),
            Quality = ItemQuality.EXCLUDED,
        };


        public override void Start() { }
        public override void Update()
        {
        }

        public override void DoEffect(PlayerController user) 
        {
            RevenantSpawnController.SpawnRevenant(user.transform.position);
        }
    }
}

