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
using System.Security.Cryptography;


namespace LastLivesRemorse
{


    public class DemonHeart : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(DemonHeart))
        {
            Name = "Forsaken Demons Heart",
            Description = "Life Taken",
            LongDescription = "The ever-warm heart of a great demon. The heat radiated off of the heart never fades, providing great strength.\n\nDo not expect this action to go unnoticed.",
            ManualSpriteCollection = StaticCollections.Item_Collection,
            ManualSpriteID = StaticCollections.Item_Collection.GetSpriteIdByName("demonheart"),
            Quality = ItemQuality.SPECIAL,
            PostInitAction = PIA
        };

        public static void PIA(PickupObject pickupObject)
        {
            var item = (pickupObject as PassiveItem);
            item.AddPassiveStatModifier(PlayerStats.StatType.DamageToBosses, 0.15f, StatModifier.ModifyMethod.ADDITIVE);
            item.AddPassiveStatModifier(PlayerStats.StatType.Health, 1, StatModifier.ModifyMethod.ADDITIVE);
            item.CanBeDropped = false;
            ID = item.PickupObjectId;
        }
        public static GameActorFireEffect hotLeadEffect = PickupObjectDatabase.GetById(295).GetComponent<BulletStatusEffectItem>().FireModifierEffect;

        public static int ID;

        public override void Pickup(PlayerController player)
        {
            if (base.m_pickedUpThisRun == false)
            {
                if (player.characterIdentity == PlayableCharacters.Robot) { player.healthHaver.Armor += 4; }
                player.healthHaver.FullHeal();
            }
            player.OnKilledEnemyContext += Player_OnKilledEnemyContext;
            DamageTypeModifier fire = GenSpecImmunity(CoreDamageTypes.Fire);
            player.healthHaver.damageTypeModifiers.AddRange(new List<DamageTypeModifier>() { fire });
            base.Pickup(player);
        }
        public DamageTypeModifier GenSpecImmunity(CoreDamageTypes damageType)
        {
            DamageTypeModifier immunity = new DamageTypeModifier();
            immunity.damageMultiplier = 0f;
            immunity.damageType = damageType;
            return immunity;
        }
        private void Player_OnKilledEnemyContext(PlayerController arg1, HealthHaver arg2)
        {
            var room = arg1.CurrentRoom;
            if (arg2 && room != null) 
            {
                AkSoundEngine.PostEvent("Play_BOSS_lichB_charge_01", arg1.gameObject);
                ParticleSystem particleSystem = UnityEngine.Object.Instantiate(Particles.fireParticles.gameObject).GetComponent<ParticleSystem>();
                particleSystem.transform.localScale *= 0.4f;
                particleSystem.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
                particleSystem.gameObject.transform.position = arg2.aiActor.sprite.WorldCenter;
                particleSystem.Play();
                particleSystem.gameObject.SetActive(true);


                arg1.StartCoroutine(particleDeath(particleSystem));
                room.ApplyActionToNearbyEnemies(arg2.sprite.WorldCenter, 3.5f, HellFire);

            }
        }

        public IEnumerator particleDeath(ParticleSystem particleSystem)
        {
            float f = 0;
            while (f < 0.2f)
            {
                f += BraveTime.DeltaTime;
                yield return null;
            }
            particleSystem.Stop();
            Destroy(particleSystem.gameObject, 3);
            yield break;
        }

        public void HellFire(AIActor aIActor, float f)
        {
            if (aIActor) { aIActor.ApplyEffect(hotLeadEffect); }
        }

        public override void Update()
        {
        }
    }
}

