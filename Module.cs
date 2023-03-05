using BepInEx;
using Alexandria;
using Alexandria.ItemAPI;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using SaveAPI;
using LastLivesRemorse.Code.Items;
using SoundAPI;
using Dungeonator;
using Alexandria.DungeonAPI;
using BepInEx.Configuration;
using Alexandria.Misc;

namespace LastLivesRemorse
{
    [BepInDependency(Alexandria.Alexandria.GUID)] 
    [BepInDependency(ETGModMainBehaviour.GUID)]
    [BepInPlugin(GUID, NAME, VERSION)]
    public class LLRModule : BaseUnityPlugin
    {
        public const string GUID = "somebunny.etg.lastlivesremorse";
        public const string NAME = "Past Lives Remorse (G.C.C 3)";
        public const string VERSION = "1.2.1";
        public const string TEXT_COLOR = "#00FFFF";
        public void Awake()
        {
            SaveAPIManager.Setup("llr");
        }

        public void Start()
        {

            ETGModMainBehaviour.WaitForGameManagerStart(GMStart);
        }
        public static AdvancedStringDB Strings;


        public static ConfigFile configurationFile;

        public void GMStart(GameManager g)
        {
            configurationFile = Config;
            CreateConfig(configurationFile);


            GungeonAPI.StaticReferences.Init();

            LLRModule.Strings = new AdvancedStringDB();


            FilePathFolder = this.FolderPath();

            BundleStarter.StartBundles();
            StaticCollections.InitialiseCollections();
            StaticTextures.InitTextures();
            Particles.StartParticles();

            //==== Hooks ====//
            Hooks.InitHooks();
            //=============================//


            //==== Initialise SoundAPI and Soundbanks ====//
            SoundManager.Init();
            SoundManager.LoadBankFromModProject("LastLivesRemorse/LLR_Bank");
            //=============================//

            JuneLib.PrefixHandler.AddPrefixForAssembly("llr");
            JuneLib.Items.ItemTemplateManager.Init();
            RevenantSpawnController.InitRevenant();
            DamnedShrineClass.InitDamnedShrine();
            DamnedShrineSpawnController.InitShrineRoom();

            ConsoleMagic.LogButCool($"{NAME} v{VERSION} started successfully.  ", StaticTextures.IconTexture);
            //Log($"{NAME} v{VERSION} started successfully.", TEXT_COLOR);
            g.OnNewLevelFullyLoaded += G_OnNewLevelFullyLoaded;
            CustomActions.OnRunStart += ORE;

            ETGModConsole.Commands.AddGroup("plr", args =>
            {
            });
            ETGModConsole.Commands.GetGroup("plr").AddUnit("toggle_rage", ForceEnableMixedFloor);

        }

        public void ORE (PlayerController p1, PlayerController p2, GameManager.GameMode g)
        {
            if (SaveAPI.AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.REVENANT_WILL_SPAWN_NEXT_RUN) == true)
            {
                SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.REVENANT_WILL_SPAWN, true);
                if (RagePersistsBetweenRuns.Value == false)
                {
                    SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.REVENANT_WILL_SPAWN_NEXT_RUN, false);
                }
            }
            else
            {
                SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.REVENANT_WILL_SPAWN, false);
            }
        } 

        public static void ForceEnableMixedFloor(string[] s)
        {
            SaveAPIManager.SetFlag(CustomDungeonFlags.REVENANT_WILL_SPAWN_NEXT_RUN, !SaveAPIManager.GetFlag(CustomDungeonFlags.REVENANT_WILL_SPAWN_NEXT_RUN));
            ETGModConsole.Log("Revenant will now " + (SaveAPIManager.GetFlag(CustomDungeonFlags.REVENANT_WILL_SPAWN_NEXT_RUN) == true ? "" : "not") + " spawn.");
        }


        public static void CreateConfig(ConfigFile config)
        {
            ShrineWeight = config.Bind<float>("Past Lives Remorse:", "Shrine Weight", 0.1f, "(Default of 0.1f) The Chance that the shrine will appear as %.");
            ShrineSelectionChance = config.Bind<float>("Past Lives Remorse:", "Shrine Selection Chance", 0.5f, "(Default of 0.5f) The Chance that the shrine will be selected as a choice when floor is generated.");
            RagePersistsBetweenRuns = config.Bind<bool>("Past Lives Remorse:", "Persistent Rage", false, "(Default of false) Whether the Revenant will spawn on the run after it was enraged.");

        }
        public static ConfigEntry<float> ShrineWeight;
        public static ConfigEntry<float> ShrineSelectionChance;
        public static ConfigEntry<bool> RagePersistsBetweenRuns;



        private void G_OnNewLevelFullyLoaded()
        {
            GameManager.Instance.StartCoroutine(DoDelay());
        }

        public IEnumerator DoDelay()
        {

            bool aaa = false;
            Dungeon floor = GameManager.Instance.Dungeon;
            float f = 0;
            while (f < UnityEngine.Random.Range(12, 24) && floor != null)
            {
                bool b = SaveAPI.AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.REVENANT_WILL_SPAWN);
                if (f > 5 && b == true && aaa == false)
                {
                    aaa = !aaa;
                    GameUIRoot.Instance.notificationController.DoCustomNotification("SOMETHING RETURNS", "Revenge For Life Taken.", StaticCollections.Item_Collection, StaticCollections.Item_Collection.GetSpriteIdByName("revenant"), UINotificationController.NotificationColor.SILVER, false, false);
                }
                f += BraveTime.DeltaTime;
                yield return null;
            }
            if (floor == null) { yield break; }

            if (SaveAPI.AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.REVENANT_WILL_SPAWN) == true) 
            {
                RevenantSpawnController.SpawnRevenant(GameManager.Instance.BestActivePlayer.CurrentRoom.GetRandomAvailableCell().Value.ToCenterVector3(0));
            }

            yield break;
        }

        public static void Log(string text, string color="#FFFFFF")
        {
            ETGModConsole.Log($"<color={color}>{text}</color>");
        }
        public static string FilePathFolder;
    }
}
