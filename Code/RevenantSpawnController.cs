using Alexandria.EnemyAPI;
using Alexandria.PrefabAPI;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using Alexandria.ItemAPI;
using static ETGMod;

namespace LastLivesRemorse.Code.Items
{
    public class RevenantSpawnController
    {
        public static void InitRevenant()
        {
            RevenantObject = PrefabBuilder.BuildObject("Revenant_Object");
            var tk2d = RevenantObject.AddComponent<tk2dSprite>();
            tk2d.Collection = StaticCollections.Revenant_Collection;
            tk2d.SetSprite(StaticCollections.Revenant_Collection.GetSpriteIdByName("revenant_head_closed_002"));

            tk2d.usesOverrideMaterial = true;
            Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat.mainTexture = tk2d.renderer.material.mainTexture;
            mat.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat.SetFloat("_EmissiveColorPower", 3f);
            mat.SetFloat("_EmissivePower", 80);
            mat.SetFloat("_EmissiveThresholdSensitivity", 0.1f);
            tk2d.renderer.material = mat;

            AdditionalBraveLight braveLight = RevenantObject.AddComponent<AdditionalBraveLight>();
            braveLight.transform.position = tk2d.sprite.WorldCenter;
            braveLight.LightColor = new UnityEngine.Color(1, 0.4f, 0.01f);
            braveLight.LightIntensity = 2f;
            braveLight.LightRadius = 4f;


            var tk2dAnim = RevenantObject.AddComponent<tk2dSpriteAnimator>();
            tk2dAnim.Library = BundleStarter.Bundle.LoadAsset<GameObject>("RevenantHeadAnimation").GetComponent<tk2dSpriteAnimation>();
            AIAnimator aiAnimatorBody = RevenantObject.AddComponent<AIAnimator>();
            aiAnimatorBody.facingType = AIAnimator.FacingType.Target;
            aiAnimatorBody.LockFacingDirection = false;
            aiAnimatorBody.IdleAnimation = new DirectionalAnimation
            {
                Type = DirectionalAnimation.DirectionType.SixWay,
                Flipped = new DirectionalAnimation.FlipType[6],
                AnimNames = new string[]
                {
                        "headclosed_B",
                        "headclosed_BR",
                        "headclosed_FR",
                        "headclosed_F",
                        "headclosed_FL",
                        "headclosed_BL"
                }
            };
            EnemyBuildingTools.AddNewDirectionAnimation(aiAnimatorBody, "charge", new string[] {
                        "headopen_B",
                        "headopen_BR",
                        "headopen_FR",
                        "headopen_F",
                        "headopen_FL",
                        "headopen_BL" }, 
            new DirectionalAnimation.FlipType[6], DirectionalAnimation.DirectionType.SixWay);
            EnemyBuildingTools.AddNewDirectionAnimation(aiAnimatorBody, "chargeup", new string[] {
                        "charge_B",
                        "charge_BR",
                        "charge_FR",
                        "charge_F",
                        "charge_FL",
                        "charge_BL" },
            new DirectionalAnimation.FlipType[6], DirectionalAnimation.DirectionType.SixWay);

            var bulletb = RevenantObject.gameObject.AddComponent<AIBulletBank>();
            bulletb.Bullets = new List<AIBulletBank.Entry>()
            {
                  EnemyDatabase.GetOrLoadByGuid("68a238ed6a82467ea85474c595c49c6e").bulletBank.GetBullet("frogger"),
            };
            RevenantObject.gameObject.layer = 23;

            SpeculativeRigidbody specBody = RevenantObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(new IntVector2(0, 0), new IntVector2(49, 56));
            specBody.PixelColliders.Clear();
            specBody.CollideWithTileMap = false;
            specBody.PixelColliders.Add(new PixelCollider
            {

                ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                CollisionLayer = CollisionLayer.EnemyHitBox,
                IsTrigger = false,
                BagleUseFirstFrameOnly = false,
                SpecifyBagelFrame = string.Empty,
                BagelColliderNumber = 0,
                ManualOffsetX = 0,
                ManualOffsetY = 0,
                ManualWidth = 20,
                ManualHeight = 20,
                ManualDiameter = 0,
                ManualLeftX = 0,
                ManualLeftY = 0,
                ManualRightX = 0,
                ManualRightY = 0,
            });
            specBody.PixelColliders.Add(new PixelCollider
            {

                ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                CollisionLayer = CollisionLayer.Projectile,
                IsTrigger = false,
                BagleUseFirstFrameOnly = false,
                SpecifyBagelFrame = string.Empty,
                BagelColliderNumber = 0,
                ManualOffsetX = 0,
                ManualOffsetY = 0,
                ManualWidth = 20,
                ManualHeight = 20,
                ManualDiameter = 0,
                ManualLeftX = 0,
                ManualLeftY = 0,
                ManualRightX = 0,
                ManualRightY = 0,
            });
            EnemyBuildingTools.AddNewDirectionAnimation(aiAnimatorBody, "spawn", new string[] {
                        "spawn" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
            EnemyBuildingTools.AddNewDirectionAnimation(aiAnimatorBody, "vomit", new string[] {
                        "vomit" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
            var spec = RevenantObject.AddComponent<RevenantBehaviorSpeculator>();
            Transform scarfPoint = EnemyBuildingTools.GenerateShootPoint(RevenantObject, new Vector2(0.5f, 0), "ScarfPoint").transform;


            var Ribs = PrefabBuilder.BuildObject("Revenant_Object_Ribs");
            var tk2d1 = Ribs.AddComponent<tk2dSprite>();
            tk2d1.Collection = StaticCollections.Revenant_Collection;
            tk2d1.SetSprite(StaticCollections.Revenant_Collection.GetSpriteIdByName("revenant_ribs_002"));

            Material spriteMat1 = new Material(EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").sprite.renderer.material);
            spriteMat1.mainTexture = tk2d1.renderer.material.mainTexture;
            tk2d1.renderer.material = spriteMat1;


            tk2d1.usesOverrideMaterial = true;
            Material mat1 = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
            mat1.mainTexture = tk2d.renderer.material.mainTexture;
            mat1.SetColor("_EmissiveColor", new Color32(255, 255, 255, 255));
            mat1.SetFloat("_EmissiveColorPower", 1);
            mat1.SetFloat("_EmissivePower", 1);
            mat1.SetFloat("_EmissiveThresholdSensitivity", 0.05f);
            tk2d1.renderer.material = mat1;

            var tk2dAnim1 = Ribs.AddComponent<tk2dSpriteAnimator>();
            tk2dAnim1.Library = BundleStarter.Bundle.LoadAsset<GameObject>("RevenantRibsAnimation").GetComponent<tk2dSpriteAnimation>();
            AIAnimator aiAnimatorBody1 = Ribs.AddComponent<AIAnimator>();
            aiAnimatorBody1.facingType = AIAnimator.FacingType.Target;
            aiAnimatorBody1.LockFacingDirection = false;
            aiAnimatorBody1.IdleAnimation = new DirectionalAnimation
            {
                Type = DirectionalAnimation.DirectionType.SixWay,
                Flipped = new DirectionalAnimation.FlipType[6],
                AnimNames = new string[]
                {
                        "ribs_B",
                        "ribs_BR",
                        "ribs_FR",
                        "ribs_F",
                        "ribs_FL",
                        "ribs_BL"
                }
            };
            EnemyBuildingTools.AddNewDirectionAnimation(aiAnimatorBody1, "charge", new string[] {
                        "ribssqueezed_B",
                        "ribssqueezed_BR",
                        "ribssqueezed_FR",
                        "ribssqueezed_F",
                        "ribssqueezed_FL",
                        "ribssqueezed_BL" }, 
            new DirectionalAnimation.FlipType[6], DirectionalAnimation.DirectionType.SixWay);
            EnemyBuildingTools.AddNewDirectionAnimation(aiAnimatorBody1, "chargeup", new string[] {
                        "ribssqueezed_B",
                        "ribssqueezed_BR",
                        "ribssqueezed_FR",
                        "ribssqueezed_F",
                        "ribssqueezed_FL",
                        "ribssqueezed_BL" },
            new DirectionalAnimation.FlipType[6], DirectionalAnimation.DirectionType.SixWay);

            EnemyBuildingTools.AddNewDirectionAnimation(aiAnimatorBody1, "spawn", new string[] {
                        "spawn" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
            EnemyBuildingTools.AddNewDirectionAnimation(aiAnimatorBody1, "vomit", new string[] {
                        "ribs_F" }, new DirectionalAnimation.FlipType[1], DirectionalAnimation.DirectionType.Single);
            Ribs.gameObject.layer = 23;




            /*
            Alexandria.EnemyAPI.EnemyBuildingTools.AddNewDirectionAnimation(aiAnimatorBody1, "idle", new string[] {
                        "ribs_B",
                        "ribs_BR",
                        "ribs_FR",
                        "ribs_F",
                        "ribs_FL",
                        "ribs_BL" },
            new DirectionalAnimation.FlipType[6], DirectionalAnimation.DirectionType.SixWay);
            */

            Ribs.transform.parent = RevenantObject.transform;
            Ribs.transform.localPosition += new Vector3(0.125f, -0.5f);

            aiAnimatorBody.ChildAnimator = aiAnimatorBody1;
        }
        public static GameObject RevenantObject;



        public static void SpawnRevenant(Vector3 position)
        {
            GameObject portalObj = UnityEngine.Object.Instantiate<GameObject>(PickupObjectDatabase.GetById(155).GetComponent<SpawnObjectPlayerItem>().objectToSpawn.GetComponent<BlackHoleDoer>().HellSynergyVFX, position, Quaternion.Euler(0f, 0f, 0f));
            portalObj.gameObject.SetLayerRecursively(LayerMask.NameToLayer("BG_Critical"));
            MeshRenderer mesh = portalObj.GetComponent<MeshRenderer>();
            mesh.material.SetTexture("_PortalTex", StaticTextures.Hell_Drag_Zone_Texture);
            GameManager.Instance.StartCoroutine(DoHoleTear(mesh));
        }



        public static IEnumerator DoHoleTear(MeshRenderer holeMesh)
        {
            AkSoundEngine.PostEvent("Play_SkadooshGround", holeMesh.gameObject);
            float elapsed = 0f;
            while (elapsed < 3)
            {
                elapsed += BraveTime.DeltaTime * 6;
                float t = elapsed;
                if (holeMesh.gameObject == null) { yield break; }
                {
                    GlobalSparksDoer.DoSingleParticle(holeMesh.transform.PositionVector2() + Toolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(holeMesh.material.GetFloat("_UVDistCutoff") * 6, (holeMesh.material.GetFloat("_UVDistCutoff") * 12) + 0.33f)), Vector3.up * (BraveUtility.RandomAngle() / (60/t+1)), null, null, null, GlobalSparksDoer.SparksType.STRAIGHT_UP_FIRE);
                    holeMesh.material.SetFloat("_UVDistCutoff", Mathf.Lerp(0, 0.04f * t, t));
                    holeMesh.material.SetFloat("_HoleEdgeDepth", Mathf.Lerp(12, 2, t));
                }
                yield return null;
            }
            var obj = UnityEngine.Object.Instantiate(RevenantObject, holeMesh.transform.position, Quaternion.identity);
            obj.GetOrAddComponent<RevenantController>();
            elapsed = 0;
            while (elapsed < 1)
            {
    
                elapsed += BraveTime.DeltaTime;
                yield return null;
            }
            elapsed = 0;
            AkSoundEngine.PostEvent("Play_SkadooshGround", holeMesh.gameObject);
            while (elapsed < 1)
            {
                elapsed += BraveTime.DeltaTime * 2;
                float t = elapsed;
                if (holeMesh.gameObject == null) { yield break; }
                {
                    GlobalSparksDoer.DoSingleParticle(holeMesh.transform.PositionVector2() + Toolbox.GetUnitOnCircle(BraveUtility.RandomAngle(), UnityEngine.Random.Range(holeMesh.material.GetFloat("_UVDistCutoff") * 6, (holeMesh.material.GetFloat("_UVDistCutoff") * 12) + 0.33f)), Vector3.up * (BraveUtility.RandomAngle() / (60 / t + 1)), null, null, null, GlobalSparksDoer.SparksType.STRAIGHT_UP_FIRE);
                    holeMesh.material.SetFloat("_UVDistCutoff", Mathf.Lerp(0.12f, 0, t));
                    holeMesh.material.SetFloat("_HoleEdgeDepth", Mathf.Lerp(2, 12, t));
                }
                yield return null;
            }
            UnityEngine.Object.Destroy(holeMesh.gameObject);
            yield break;
        }

    }
}
