﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;
using Alexandria.ItemAPI;
using Planetside;
using System.Reflection;
using MonoMod.RuntimeDetour;


namespace GungeonAPI
{
    public static class StaticReferences
    {
        public static Dictionary<string, AssetBundle> AssetBundles;
        public static Dictionary<string, GenericRoomTable> RoomTables;
        public static SharedInjectionData subShopTable;

        public static Dictionary<string, string> roomTableMap = new Dictionary<string, string>()
        {
            { "castle", "Castle_RoomTable" },
            { "gungeon", "Gungeon_RoomTable" },
            { "mines", "Mines_RoomTable" },
            { "catacombs", "Catacomb_RoomTable" },
            { "forge", "Forge_RoomTable" },
            { "sewer", "Sewer_RoomTable" },
            { "cathedral", "Cathedral_RoomTable" },
            { "bullethell", "BulletHell_RoomTable" },
            { "resourcefulrat", "Resourceful Rat Maze Rooms" },

            //{ "unknown", "SecretHelpers_RoomTable" },
        };

        public static Dictionary<string, string> specialRoomTableMap = new Dictionary<string, string>()
        {
            { "special", "basic special rooms (shrines, etc)" },
            { "shop", "Shop Room Table" },
            { "secret", "secret_room_table_01" }
        };


        //=================== LIST OF BOSS ROOM POOLS 
        public static Dictionary<string, string> BossRoomGrabage = new Dictionary<string, string>()
        {
            //Floor 1
            { "gull", "bosstable_01_gatlinggull"},
            { "triggertwins", "bosstable_01_bulletbros"},
            { "bulletking", "bosstable_01_bulletking"},
            //Sewer
            { "blobby", "bosstable_01a_blobulord"},
            //Floor 2
            { "gorgun", "bosstable_02_meduzi"},
            { "beholster", "bosstable_02_beholster"},
            { "ammoconda", "bosstable_02_bashellisk"},
            //Abbey
            { "oldking", "bosstable_04_oldking"},

            //Floor 3
            { "tank", "bosstable_03_tank"},
            { "cannonballrog", "bosstable_03_powderskull"},
            { "flayer", "bosstable_03_mineflayer"},
            //Floor 4
            { "priest", "bosstable_02a_highpriest"},
            { "pillars", "bosstable_04_statues"},
            { "monger", "bosstable_04_demonwall"},
            //Door Lord
            { "doorlord", "bosstable_xx_bossdoormimic"}
        };

        public static Dictionary<string, string> MiniBossRoomPools = new Dictionary<string, string>()
        {
            {"blockner", "BlocknerMiniboss_Table_01" },
            {"shadeagunim","PhantomAgunim_Table_01" },
            //{"fuselier","fusebombroom01" }
        };


        public static string[] assetBundleNames = new string[]
        {
            "shared_auto_001",
            "shared_auto_002",
            "brave_resources_001",
        };



        public static string[] dungeonPrefabNames = new string[]
        {
            "base_gungeon",
            "base_castle",
            "base_mines",
            "base_catacombs",
            "base_forge",
            "base_sewer",
            "base_cathedral",
            "base_bullethell",
            "base_resourcefulrat"
        };


        public static void Init()
        {
            AssetBundles = new Dictionary<string, AssetBundle>();
            foreach (var name in assetBundleNames)
            {
                try
                {
                    var bundle = ResourceManager.LoadAssetBundle(name);
                    if(bundle == null)
                    {
                        //Tools.PrintError($"Failed to load asset bundle: {name}");
                        continue;
                    }
                    //Tools.PrintError($"Loaded assetbundle: {name}");

                    AssetBundles.Add(name, ResourceManager.LoadAssetBundle(name));
                }
                catch (Exception e)
                {
                    ETGModConsole.Log(e);
                    //Tools.PrintError($"Failed to load asset bundle: {name}");
                    //Tools.PrintException(e);
                }
            }
            InitStaticRoomObjects();

            RoomTables = new Dictionary<string, GenericRoomTable>();
            foreach (var entry in roomTableMap)
            {
                try
                {
                    var table = DungeonDatabase.GetOrLoadByName($"base_{entry.Key}").PatternSettings.flows[0].fallbackRoomTable;
                    if (entry.Key.Contains("resourcefulrat"))
                    {
                        table = GungeonAPI.OfficialFlows.GetDungeonPrefab("base_resourcefulrat").PatternSettings.flows[0].AllNodes[18].overrideRoomTable; 
                    }
                    RoomTables.Add(entry.Key, table);
                }
                catch (Exception e)
                {
                    ETGModConsole.Log(e);
                    //Tools.PrintError($"Failed to load room table: {entry.Key}:{entry.Value}");
                    //Tools.PrintException(e);
                }
            }

            foreach (var entry in specialRoomTableMap)
            {
                try
                {
                    var table = GetAsset<GenericRoomTable>(entry.Value);
                    RoomTables.Add(entry.Key, table);


                    /*
                    if (table.name.ToLower().Contains("basic special"))
                    {

                        Debug.Log("start Logging of basic special room tables");
                        if (table.includedRooms != null)
                        {
                            int r1 = 0;
                            int r2 = 0;
                            int r3 = 0;
                            int r4 = 0;
                            int r5 = 0;
                            for (int i = 0; i <= 999; i++)
                            {
                                WeightedRoom roomer = table.includedRooms.SelectByWeight();
                                if (roomer == table.includedRooms.elements[0])
                                {
                                    r1++;
                                }
                                if (roomer == table.includedRooms.elements[1])
                                {
                                    r2++;
                                }
                                if (roomer == table.includedRooms.elements[2])
                                {
                                    r3++;
                                }
                                if (roomer == table.includedRooms.elements[3])
                                {
                                    r4++;
                                }
                                if (roomer == table.includedRooms.elements[4])
                                {
                                    r5++;
                                }
                            }

                            Debug.Log("includedRoom name: " + table.includedRooms.elements[0].room.name +" got chosen "+r1+" times.");
                            Debug.Log("includedRoom name: " + table.includedRooms.elements[1].room.name + " got chosen " + r2 + " times.");
                            Debug.Log("includedRoom name: " + table.includedRooms.elements[2].room.name + " got chosen " + r3 + " times.");
                            Debug.Log("includedRoom name: " + table.includedRooms.elements[3].room.name + " got chosen " + r4 + " times.");
                            Debug.Log("includedRoom name: " + table.includedRooms.elements[4].room.name + " got chosen " + r5 + " times.");


                            DungeonPrerequisite[] reqs = table.includedRooms.elements[0].additionalPrerequisites;
                            foreach(var req in reqs)
                            {
                                if (req != null)
                                {
                                    ItemAPI.Tools.LogPropertiesAndFields(req, "", true);
                                }
                            }
                            //int i = 0;

                            
                            foreach (var room in table.includedRooms.elements)
                            {
                                Debug.Log("includedRoom in pos: "+i+" : with name: "+room.room.name);
                                Debug.Log("includedRoom weight: " + room.weight);
                                Debug.Log("======================");
                                i++;
                            }
                            
                        }


                    }
                    */




                }
                catch (Exception e)
                {
                    ETGModConsole.Log(e);
                    //Tools.PrintError($"Failed to load special room table: {entry.Key}:{entry.Value}");
                    // Tools.PrintException(e);
                }
            }

            //================================ Adss Boss Rooms into RoomTables
            foreach (var entry in BossRoomGrabage)
            {
                try
                {
                    var table = GetAsset<GenericRoomTable>(entry.Value);
                    RoomTables.Add(entry.Key, table);
                }
                catch (Exception e)
                {
                    ETGModConsole.Log(e);
                    //Tools.PrintError($"Failed to load special room table: {entry.Key}:{entry.Value}");
                    //Tools.PrintException(e);
                }
            }
            //================================ Adss Mini Boss Rooms into RoomTables
            foreach (var entry in MiniBossRoomPools)
            {
                try
                {
                    var table = GetAsset<GenericRoomTable>(entry.Value);
                    RoomTables.Add(entry.Key, table);
                }
                catch (Exception e)
                {
                    ETGModConsole.Log(e);
                    //Tools.PrintError($"Failed to load special room table: {entry.Key}:{entry.Value}");
                    //Tools.PrintException(e);
                }
            }

            subShopTable = AssetBundles["shared_auto_001"].LoadAsset<SharedInjectionData>("_global injected subshop table");
            //foreach(var data in subShopTable.InjectionData)
            //{
            //    Tools.LogPropertiesAndFields(data, data.annotation);
            //}



            //Tools.Print("Static references initialized.");
        }
       


        public static void InitStaticRoomObjects()
        {

            GameObject SingleCasingPlaceable = FakePrefab.Clone(PickupObjectDatabase.GetById(68).gameObject);
            if (SingleCasingPlaceable.GetComponent<PickupMover>()) UnityEngine.Object.Destroy(SingleCasingPlaceable.GetComponent<PickupMover>());
            GameObject FiveCasingPlaceable = FakePrefab.Clone(PickupObjectDatabase.GetById(70).gameObject);
            if (FiveCasingPlaceable.GetComponent<PickupMover>()) UnityEngine.Object.Destroy(FiveCasingPlaceable.GetComponent<PickupMover>());
            GameObject FiftyCasingPlaceable = FakePrefab.Clone(PickupObjectDatabase.GetById(74).gameObject);
            if (FiftyCasingPlaceable.GetComponent<PickupMover>()) UnityEngine.Object.Destroy(FiftyCasingPlaceable.GetComponent<PickupMover>());

            StoredRoomObjects.Add("SingleCasingPlaceable", SingleCasingPlaceable);
            StoredRoomObjects.Add("FiveCasingPlaceable", FiveCasingPlaceable);
            StoredRoomObjects.Add("FiftyCasingPlaceable", FiftyCasingPlaceable);

            var rngDungeon = DungeonDatabase.GetOrLoadByName("base_nakatomi");
            if (rngDungeon)
            {
                if (rngDungeon.PatternSettings.flows[0].name == "FS4_Nakatomi_Flow")
                {
                    if (rngDungeon.PatternSettings.flows[0].AllNodes.Count == 14)
                    {
                        GameObject MopAndBucket = rngDungeon.PatternSettings.flows[0].AllNodes[0].overrideExactRoom.placedObjects[0].nonenemyBehaviour.gameObject;
                        GameObject CardboardBox3 = rngDungeon.PatternSettings.flows[0].AllNodes[0].overrideExactRoom.placedObjects[2].nonenemyBehaviour.gameObject;
                        GameObject ACUnit = rngDungeon.PatternSettings.flows[0].AllNodes[1].overrideExactRoom.placedObjects[1].nonenemyBehaviour.gameObject;
                        GameObject ACVent = rngDungeon.PatternSettings.flows[0].AllNodes[1].overrideExactRoom.placedObjects[2].nonenemyBehaviour.gameObject;
                        StoredRoomObjects.Add("MopAndBucket", MopAndBucket);
                        StoredRoomObjects.Add("CardboardBox3", CardboardBox3);
                        StoredRoomObjects.Add("ACUnit", ACUnit);
                        StoredRoomObjects.Add("ACVent", ACVent);
                    }
                }
            }
            rngDungeon = null;
            var forgeDungeon = DungeonDatabase.GetOrLoadByName("Base_Forge");

            /*
            List<SharedInjectionData> datumes =  forgeDungeon.PatternSettings.flows[0].sharedInjectionData;

            
            foreach (var dates in datumes)
            {
                Debug.Log("===================================================");
                ItemAPI.Tools.LogPropertiesAndFields(dates, "", true);
                foreach (var data in dates.AttachedInjectionData)
                {
                    Debug.Log("===================================================");
                    Debug.Log(data.name != null ? data.name : "null");
                    ETGModConsole.Log("attachedInjData chance: "+ data.ChanceToSpawnOne.ToString());
                    ItemAPI.Tools.LogPropertiesAndFields(data, "", true);
                    foreach (var datum in data.InjectionData)
                    {
                        Debug.Log("===================================================");
                        Debug.Log(datum.annotation != null ? datum.annotation : "null");
                        ItemAPI.Tools.LogPropertiesAndFields(datum, "", true);
                        if (datum.roomTable != null)
                        {
                            Debug.Log("===================================================");
                            Debug.Log(datum.roomTable.name != null ? datum.roomTable.name : "null");
                            ItemAPI.Tools.LogPropertiesAndFields(datum.roomTable, "", true);

                        }

                    }
                    foreach (var es in data.AttachedInjectionData)
                    {
                        Debug.Log("===================================================");
                        Debug.Log(es.name != null ? data.name : "null");
                        ETGModConsole.Log("attachedInjData chance: " + es.ChanceToSpawnOne.ToString());
                        ItemAPI.Tools.LogPropertiesAndFields(es, "", true);
                        foreach (var datum in es.InjectionData)
                        {
                            Debug.Log(datum.annotation != null ? datum.annotation : "null");
                            ItemAPI.Tools.LogPropertiesAndFields(datum, "", true);
                            if (datum.roomTable != null)
                            {
                                Debug.Log("===================================================");
                                Debug.Log(datum.roomTable.name != null ? datum.roomTable.name : "null");
                                ItemAPI.Tools.LogPropertiesAndFields(datum.roomTable, "", true);

                            }

                        }
                    }

                


                }
                Debug.Log("===================================================");
                ETGModConsole.Log("baseharedinj Data chance: " + dates.ChanceToSpawnOne.ToString());

                foreach (var data in dates.InjectionData)
                {
                    Debug.Log("===================================================");
                    Debug.Log(data.annotation != null ? data.annotation : "null");

                    ItemAPI.Tools.LogPropertiesAndFields(data, "", true);

                    if (data.roomTable != null)
                    {
                        Debug.Log("===================================================");

                        Debug.Log(data.roomTable.name != null ? data.roomTable.name : "null");
                        ItemAPI.Tools.LogPropertiesAndFields(data.roomTable, "", true);
                        //data.roomTable.includedRoomTables
                    }
                }
            }
            */

              
            foreach (WeightedRoom wRoom in forgeDungeon.PatternSettings.flows[0].fallbackRoomTable.includedRooms.elements)
            {

                if (wRoom.room != null && !string.IsNullOrEmpty(wRoom.room.name))
                {
                    if (wRoom.room.name.ToLower().StartsWith("forge_normal_cubulead_03"))
                    {
                        GameObject VerticalCrusher = wRoom.room.placedObjects[0].nonenemyBehaviour.gameObject;
                        GameObject FireBarTrap = wRoom.room.placedObjects[7].nonenemyBehaviour.gameObject;
                        StoredRoomObjects.Add("VerticalCrusher", VerticalCrusher);
                        StoredRoomObjects.Add("FireBarTrap", FireBarTrap);
                    }
                    if (wRoom.room.name.ToLower().StartsWith("forge_connector_flamepipes_01"))
                    {
                        GameObject FlamePipeNorth = wRoom.room.placedObjects[1].nonenemyBehaviour.gameObject;
                        GameObject FlamePipeWest = wRoom.room.placedObjects[3].nonenemyBehaviour.gameObject;
                        GameObject FlamePipeEast = wRoom.room.placedObjects[2].nonenemyBehaviour.gameObject;
                        StoredRoomObjects.Add("FlamePipeNorth", FlamePipeNorth);
                        StoredRoomObjects.Add("FlamePipeWest", FlamePipeWest);
                        StoredRoomObjects.Add("FlamePipeEast", FlamePipeEast);
                    }
                }
            }

            foreach (DungeonFlow flows in forgeDungeon.PatternSettings.flows)
            {
                foreach (DungeonFlowNode node in flows.AllNodes)
                {
                    if (node.overrideExactRoom != null)
                    {
                        if (node.overrideExactRoom.name.ToLower().StartsWith("exit_room_forge"))
                        {
                            GameObject ForgeGearVertical = node.overrideExactRoom.placedObjects[0].nonenemyBehaviour.gameObject;
                            GameObject ForgeGearHorizontalLeft = node.overrideExactRoom.placedObjects[7].nonenemyBehaviour.gameObject;
                            GameObject ForgeGearHorizontalRight = node.overrideExactRoom.placedObjects[8].nonenemyBehaviour.gameObject;
                            GameObject BlacksmithLargeTorch = node.overrideExactRoom.placedObjects[19].nonenemyBehaviour.gameObject;

                            if (!StoredRoomObjects.ContainsKey("ForgeGearVertical")){StoredRoomObjects.Add("ForgeGearVertical", ForgeGearVertical);}
                            if (!StoredRoomObjects.ContainsKey("ForgeGearHorizontalLeft")) { StoredRoomObjects.Add("ForgeGearHorizontalLeft", ForgeGearHorizontalLeft); }
                            if (!StoredRoomObjects.ContainsKey("ForgeGearHorizontalRight")) { StoredRoomObjects.Add("ForgeGearHorizontalRight", ForgeGearHorizontalRight); }
                            if (!StoredRoomObjects.ContainsKey("BlacksmithLargeTorch")) { StoredRoomObjects.Add("BlacksmithLargeTorch", BlacksmithLargeTorch); }
                        }
                        if (node.overrideExactRoom.name.ToLower().StartsWith("blacksmith_testroom"))
                        {
                            GameObject BlacksmithLoungeChair = node.overrideExactRoom.placedObjects[9].nonenemyBehaviour.gameObject;
                            GameObject BlacksmithTableAndChair = node.overrideExactRoom.placedObjects[10].nonenemyBehaviour.gameObject;
                            GameObject BlacksmithHotBowl = node.overrideExactRoom.placedObjects[11].nonenemyBehaviour.gameObject;

                            if (!StoredRoomObjects.ContainsKey("BlacksmithLoungeChair")) { StoredRoomObjects.Add("BlacksmithLoungeChair", BlacksmithLoungeChair); }
                            if (!StoredRoomObjects.ContainsKey("BlacksmithTableAndChair")) { StoredRoomObjects.Add("BlacksmithTableAndChair", BlacksmithTableAndChair); }
                            if (!StoredRoomObjects.ContainsKey("BlacksmithHotBowl")) { StoredRoomObjects.Add("BlacksmithHotBowl", BlacksmithHotBowl); }
                        }
                    }
                }
            }
            forgeDungeon = null;

            var sewerDungeon = DungeonDatabase.GetOrLoadByName("Base_Sewer");
            foreach (WeightedRoom wRoom in sewerDungeon.PatternSettings.flows[0].fallbackRoomTable.includedRooms.elements)
            {
                if (wRoom.room != null && !string.IsNullOrEmpty(wRoom.room.name))
                {
                    if (wRoom.room.name.ToLower().StartsWith("sewer_trash_compactor_001"))
                    {
                        GameObject HorizontalCrusher = wRoom.room.placedObjects[0].nonenemyBehaviour.gameObject;
                        StoredRoomObjects.Add("HorizontalCrusher", HorizontalCrusher);
                    }
                }
            }
            sewerDungeon = null;


            var hellDungeon = DungeonDatabase.GetOrLoadByName("base_bullethell");
            if (hellDungeon)
            {
                foreach (WeightedRoom wRoom in hellDungeon.PatternSettings.flows[0].fallbackRoomTable.includedRooms.elements)
                {

                    if (wRoom.room != null && !string.IsNullOrEmpty(wRoom.room.name))
                    {
                        if (wRoom.room.name.ToLower().StartsWith("hell_connector_pathburst_01"))
                        {
                            /*
                            foreach (var placeable in wRoom.room.placedObjects)
                            {
                                Tools.LogPropertiesAndFields(placeable);
                            }
                            */
                            GameObject radialFireTrap = wRoom.room.placedObjects[3].nonenemyBehaviour.gameObject;
                            StoredRoomObjects.Add("RadialFireTrap", radialFireTrap);
                        }
                    }
                }
            }
            hellDungeon = null;




               
        }



        public static Dictionary<string, GameObject> StoredRoomObjects = new Dictionary<string, GameObject>(){};
        public static Dictionary<string, DungeonPlaceable> StoredDungeonPlaceables = new Dictionary<string, DungeonPlaceable>() {};


        public static GenericRoomTable GetRoomTable(GlobalDungeonData.ValidTilesets tileset)
        {
            switch (tileset)
            {
                case GlobalDungeonData.ValidTilesets.CASTLEGEON:
                    return RoomTables["castle"];
                case GlobalDungeonData.ValidTilesets.GUNGEON:
                    return RoomTables["gungeon"];
                case GlobalDungeonData.ValidTilesets.MINEGEON:
                    return RoomTables["mines"];
                case GlobalDungeonData.ValidTilesets.CATACOMBGEON:
                    return RoomTables["catacombs"];
                case GlobalDungeonData.ValidTilesets.FORGEGEON:
                    return RoomTables["forge"];
                case GlobalDungeonData.ValidTilesets.SEWERGEON:
                    return RoomTables["sewer"];
                case GlobalDungeonData.ValidTilesets.CATHEDRALGEON:
                    return RoomTables["cathedral"];
                case GlobalDungeonData.ValidTilesets.RATGEON:
                    return RoomTables["resourcefulrat"];
                case GlobalDungeonData.ValidTilesets.HELLGEON:
                    return RoomTables["bullethell"];
                default:
                    return RoomTables["gungeon"];
            }
        }
        
        public static T GetAsset<T>(string assetName) where T : UnityEngine.Object
        {
            T item = null;
            foreach (var bundle in AssetBundles.Values)
            {
                item = bundle.LoadAsset<T>(assetName);
                if (item != null)
                    break;
            }
            return item;
        }

    }
}
