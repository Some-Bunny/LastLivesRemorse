using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LastLivesRemorse
{
    public class StaticTextures
    {
        public static Texture2D IconTexture;
        public static Texture Hell_Drag_Zone_Texture;

        public static void InitTextures()
        {
            IconTexture = BundleStarter.Bundle.LoadAsset<Texture2D>("Icon_Mod");

            var forgeDungeon = DungeonDatabase.GetOrLoadByName("Base_Forge");
            Hell_Drag_Zone_Texture = forgeDungeon.PatternSettings.flows[0].AllNodes.Where(node => node.overrideExactRoom != null && node.overrideExactRoom.name.Contains("EndTimes")).First().overrideExactRoom.placedObjects.Where(ppod => ppod != null && ppod.nonenemyBehaviour != null).First().nonenemyBehaviour.gameObject.GetComponentsInChildren<HellDragZoneController>()[0].HoleObject.GetComponent<MeshRenderer>().material.GetTexture("_PortalTex");
            forgeDungeon = null;
        }
    }
}
