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

namespace LastLivesRemorse
{
    [BepInDependency(Alexandria.Alexandria.GUID)] 
    [BepInDependency(ETGModMainBehaviour.GUID)]
    [BepInPlugin(GUID, NAME, VERSION)]
    public class LLRModule : BaseUnityPlugin
    {
        public const string GUID = "somebunny.etg.lastlivesremorse";
        public const string NAME = "Past Lives Remorse (G.C.C 3)";
        public const string VERSION = "1.0.0";
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

            ETGModConsole.Commands.AddGroup("llr", args =>
            {
            });
            ETGModConsole.Commands.GetGroup("llr").AddUnit("set_flag", ForceEnableMixedFloor);

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
        }
        public static ConfigEntry<float> ShrineWeight;
        public static ConfigEntry<float> ShrineSelectionChance;



        private void G_OnNewLevelFullyLoaded()
        {
            GameManager.Instance.StartCoroutine(DoDelay());
        }

        public IEnumerator DoDelay()
        {
            
            bool b = false;
            if (SaveAPI.AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.REVENANT_WILL_SPAWN_NEXT_RUN) == true)
            {
                b = !b;
                SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.REVENANT_WILL_SPAWN, true);
            }
            else
            {
                SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.REVENANT_WILL_SPAWN, false);
            }
            Dungeon floor = GameManager.Instance.Dungeon;
            float f = 0;
            while (f < UnityEngine.Random.Range(15, 35) && floor != null)
            {
                if (f > 5 && b == true)
                {
                    b = false;
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
