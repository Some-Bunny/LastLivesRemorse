using Alexandria.PrefabAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;
using Alexandria.ItemAPI;
using Alexandria.EnemyAPI;
using System.Collections;
using Alexandria.TranslationAPI;
using SaveAPI;
using HutongGames.PlayMaker.Actions;
using System.ComponentModel;
using Alexandria.Misc;
using System.Runtime.Remoting.Messaging;
using DaikonForge.Tween;

namespace LastLivesRemorse
{
    public class DamnedShrineClass
    {
        public static void InitDamnedShrine()
        {
            DamnedShrineObject = PrefabBuilder.BuildObject("DamnedShrineObject_Object");

            var tk2d = DamnedShrineObject.AddComponent<tk2dSprite>();
            tk2d.IsOutlineSprite = false;
            tk2d.IsBraveOutlineSprite = false;

            tk2d.depthUsesTrimmedBounds = true;
            tk2d.ignoresTiltworldDepth = false;

            tk2d.renderLayer = 22;
            tk2d.HeightOffGround = -0.5f;
            tk2d.IsPerpendicular = true;
            tk2d.SortingOrder = 0;

            tk2d.Collection = StaticCollections.DamnedShrine_Collection;
            tk2d.SetSprite(StaticCollections.DamnedShrine_Collection.GetSpriteIdByName("damnationshrine_idle_001"));
            var tk2dAnim = DamnedShrineObject.AddComponent<tk2dSpriteAnimator>();
            tk2dAnim.Library = BundleStarter.Bundle.LoadAsset<GameObject>("DamnedShrineAnimation").GetComponent<tk2dSpriteAnimation>();
            tk2dAnim.defaultClipId = tk2dAnim.GetClipIdByName("idle");
            tk2dAnim.playAutomatically = true;
            DamnedShrineObject.layer = 22;

            tk2d.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2d.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 1);
            mat.SetFloat("_EmissivePower", 1);
            mat.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
            tk2d.renderer.material = mat;

            AdditionalBraveLight braveLight = DamnedShrineObject.AddComponent<AdditionalBraveLight>();
            braveLight.transform.position = tk2d.sprite.WorldCenter;
            braveLight.LightColor = new UnityEngine.Color(1.2f, 0.3f, 0f);
            braveLight.LightIntensity = 3f;
            braveLight.LightRadius = 10f;

            var damnedShrine = DamnedShrineObject.AddComponent<DamnedShrineController>();

            SpeculativeRigidbody specBody = DamnedShrineObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(new IntVector2(0, 0), new IntVector2(49, 56));
            specBody.PixelColliders.Clear();
            specBody.CollideWithTileMap = false;
            specBody.PixelColliders.Add(new PixelCollider
            {
                ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                CollisionLayer = CollisionLayer.HighObstacle,
                IsTrigger = false,
                BagleUseFirstFrameOnly = false,
                SpecifyBagelFrame = string.Empty,
                BagelColliderNumber = 0,
                ManualOffsetX = 7,
                ManualOffsetY = -4,
                ManualWidth = 32,
                ManualHeight = 32,
                ManualDiameter = 0,
                ManualLeftX = 0,
                ManualLeftY = 0,
                ManualRightX = 0,
                ManualRightY = 0,
            });
            specBody.PixelColliders.Add(new PixelCollider
            {
                ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                CollisionLayer = CollisionLayer.BeamBlocker,
                IsTrigger = false,
                BagleUseFirstFrameOnly = false,
                SpecifyBagelFrame = string.Empty,
                BagelColliderNumber = 0,
                ManualOffsetX = 7,
                ManualOffsetY = -4,
                ManualWidth = 32,
                ManualHeight = 32,
                ManualDiameter = 0,
                ManualLeftX = 0,
                ManualLeftY = 0,
                ManualRightX = 0,
                ManualRightY = 0,
            });
            specBody.PixelColliders.Add(new PixelCollider
            {
                ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                CollisionLayer = CollisionLayer.BulletBlocker,
                IsTrigger = false,
                BagleUseFirstFrameOnly = false,
                SpecifyBagelFrame = string.Empty,
                BagelColliderNumber = 0,
                ManualOffsetX = 7,
                ManualOffsetY = -4,
                ManualWidth = 32,
                ManualHeight = 32,
                ManualDiameter = 0,
                ManualLeftX = 0,
                ManualLeftY = 0,
                ManualRightX = 0,
                ManualRightY = 0,
            });
            damnedShrine.talkPoint = DamnedShrineObject.transform;
            LLRModule.Strings.Core.Set("#DAMNEDSHRINE_LLR_DEF", "The Damned Shrine sits, pulsating with life. Do you take its life for your benefit, at the risk of your future one?");
            LLRModule.Strings.Core.Set("#DAMNEDSHRINE_LLR_ACC", "Take its life for your own. ("+ ConsoleMagic.AddColorToLabelString("Put your Future Life at risk.") + ")");
            LLRModule.Strings.Core.Set("#DAMNEDSHRINE_LLR_CAN", "Decline.");
            damnedShrine.acceptOptionKey = "#DAMNEDSHRINE_LLR_ACC";
            damnedShrine.declineOptionKey = "#DAMNEDSHRINE_LLR_CAN";
            damnedShrine.displayTextKey = "#DAMNEDSHRINE_LLR_DEF";
            GungeonAPI.StaticReferences.StoredRoomObjects.Add("LLR_DamnedShrine", DamnedShrineObject);

            Tk2dSpriteAnimatorUtility.AddEventTriggersToAnimation(tk2dAnim, "use", new Dictionary<int, string>()
            {
                {6, "MEAT"},
            });
            Tk2dSpriteAnimatorUtility.AddSoundsToAnimationFrame(tk2dAnim, "use", new Dictionary<int, string>()
            {
                {5, "Play_ENM_blobulord_splash_01"},
            });

            ETGModConsole.ModdedShrines.Add("damned_shrine", DamnedShrineObject);
        }

        public static GameObject DamnedShrineObject;
    }


    public class DamnedShrineController : DungeonPlaceableBehaviour, IPlayerInteractable, IPlaceConfigurable
    {
        public void Start()
        {
            this.spriteAnimator.AnimationEventTriggered = (animator, clip, idX) =>
            {
                if (clip.GetFrame(idX).eventInfo.Contains("MEAT"))
                {
                    float f = BraveUtility.RandomAngle();
                    for (int i = 0; i < 3; i++) 
                    {
                        var o = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(333) as Gun).muzzleFlashEffects.effects[0].effects[0].effect, this.sprite.WorldBottomLeft + new Vector2(2.25f, 2.25f), Quaternion.Euler(0 ,0, f + (120* i)));
                        Destroy(o, 3);
                    }
                    var q = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(449) as TeleporterPrototypeItem).TelefragVFXPrefab, this.sprite.WorldBottomLeft + new Vector2(2.25f, 2.25f), Quaternion.identity);
                    GameUIRoot.Instance.notificationController.DoCustomNotification("HEART TAKEN", "Warm For Life.", StaticCollections.Item_Collection, StaticCollections.Item_Collection.GetSpriteIdByName("heartbreak"), UINotificationController.NotificationColor.SILVER, false, false);
                }
            };
        }


        public void ConfigureOnPlacement(RoomHandler room)
        {
            this.m_parentRoom = room;
            room.OptionalDoorTopDecorable = (ResourceCache.Acquire("Global Prefabs/Shrine_Lantern") as GameObject);
            if (!room.IsOnCriticalPath && room.connectedRooms.Count == 1)
            {
                room.ShouldAttemptProceduralLock = true;
                room.AttemptProceduralLockChance = 0;
            }
            this.RegisterMinimapIcon();
        }
        public void RegisterMinimapIcon()
        {
            this.m_instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(this.m_parentRoom, (GameObject)BraveResources.Load("Global Prefabs/Minimap_Shrine_Icon", ".prefab"), false);
        }
        private RoomHandler m_parentRoom;
        private GameObject m_instanceMinimapIcon;
        public Transform talkPoint;
        private int m_useCount = 0;

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        public float GetDistanceToPoint(Vector2 point)
        {
            if (base.sprite == null)
            {
                return 100f;
            }
            Vector3 v = BraveMathCollege.ClosestPointOnRectangle(point, base.specRigidbody.UnitBottomLeft, base.specRigidbody.UnitDimensions);
            return Vector2.Distance(point, v) / 1.5f;
        }

        public float GetOverrideMaxDistance()
        {
            return -1f;
        }



        public void Interact(PlayerController interactor)
        {
            if (TextBoxManager.HasTextBox(this.talkPoint))
            {
                return;
            }
            if (this.m_useCount > 0)
            {
                if (!string.IsNullOrEmpty(this.spentOptionKey))
                {
                    base.StartCoroutine(this.HandleSpentText(interactor));
                }
                return;
            }
            this.m_useCount++;
            base.StartCoroutine(this.HandleShrineConversation(interactor));
        }

        private IEnumerator HandleSpentText(PlayerController interactor)
        {
            TextBoxManager.ShowStoneTablet(this.talkPoint.position, this.talkPoint, -1f, StringTableManager.GetLongString(this.spentOptionKey), true, false);
            int selectedResponse = -1;
            interactor.SetInputOverride("shrineConversation");
            GameUIRoot.Instance.DisplayPlayerConversationOptions(interactor, null, StringTableManager.GetString(this.declineOptionKey), string.Empty);
            while (!GameUIRoot.Instance.GetPlayerConversationResponse(out selectedResponse))
            {
                yield return null;
            }
            interactor.ClearInputOverride("shrineConversation");
            TextBoxManager.ClearTextBox(this.talkPoint);
            yield break;
        }

        private IEnumerator HandleShrineConversation(PlayerController interactor)
        {
            string targetDisplayKey = this.displayTextKey;
            TextBoxManager.ShowStoneTablet(this.talkPoint.position, this.talkPoint, -1f, StringTableManager.GetLongString(targetDisplayKey), true, false);
            int selectedResponse = -1;
            interactor.SetInputOverride("shrineConversation");
            yield return null;
            bool canUse = true;//this.CheckCosts(interactor);
            if (canUse)
            {
                string text = StringTableManager.GetString(this.acceptOptionKey);
              
                GameUIRoot.Instance.DisplayPlayerConversationOptions(interactor, null, text, StringTableManager.GetString(this.declineOptionKey));
            }
            else
            {
                GameUIRoot.Instance.DisplayPlayerConversationOptions(interactor, null, StringTableManager.GetString(this.declineOptionKey), string.Empty);
            }
            while (!GameUIRoot.Instance.GetPlayerConversationResponse(out selectedResponse))
            {
                yield return null;
            }
            interactor.ClearInputOverride("shrineConversation");
            TextBoxManager.ClearTextBox(this.talkPoint);
            if (canUse && selectedResponse == 0)
            {
                AkSoundEngine.PostEvent("Play_ENM_darken_world_01", this.gameObject);
                this.spriteAnimator.Play("use");
                interactor.AcquirePassiveItemPrefabDirectly(PickupObjectDatabase.GetById(DemonHeart.ID) as PassiveItem);
                SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.REVENANT_WILL_SPAWN_NEXT_RUN, true);
                this.m_useCount++;
            }
            yield break;
        }


        public void OnEnteredRange(PlayerController interactor)
        {
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white);
        }

        public void OnExitRange(PlayerController interactor)
        {
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black);
        }

        public string displayTextKey;

        public string acceptOptionKey;

        public string declineOptionKey;

        public string spentOptionKey = "#SHRINE_GENERIC_SPENT";
    }
}
