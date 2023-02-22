using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;


namespace LastLivesRemorse
{
    public static class StaticCollections
    {
        public static tk2dSpriteCollectionData Item_Collection;
        public static tk2dSpriteCollectionData Revenant_Collection;
        public static tk2dSpriteCollectionData DamnedShrine_Collection;

        public static void InitialiseCollections()
        {
            Item_Collection = DoFastSetup(BundleStarter.Bundle, "ItemCollection", "item material.mat");
            if (Item_Collection == null) { ETGModConsole.Log("Item_Collection is NULL"); }

            Revenant_Collection = DoFastSetup(BundleStarter.Bundle, "RevenantCollection", "revenant material.mat");
            if (Revenant_Collection == null) { ETGModConsole.Log("Revenant_Collection is NULL"); }

            DamnedShrine_Collection = DoFastSetup(BundleStarter.Bundle, "DanmedShrineCollection", "damnedshrine material.mat");
            if (DamnedShrine_Collection == null) { ETGModConsole.Log("DamnedShrine_Collection is NULL"); }
        }

        public static tk2dSpriteCollectionData DoFastSetup(AssetBundle bundle, string CollectionName, string MaterialName)
        {
            tk2dSpriteCollectionData Colection = bundle.LoadAsset<GameObject>(CollectionName).GetComponent<tk2dSpriteCollectionData>();
            Material material = bundle.LoadAsset<Material>(MaterialName);
            FastAssetBundleSpriteSetup(Colection, material);
            return Colection;
        }
        public static void FastAssetBundleSpriteSetup(tk2dSpriteCollectionData bundleData, Material mat)
        {
            Texture texture = mat.GetTexture("_MainTex");
            texture.filterMode = FilterMode.Point;
            mat.SetTexture("_MainTex", texture);
            bundleData.material = mat;

            bundleData.materials = new Material[]
            {
                mat,
            };
            bundleData.materialInsts = new Material[]
            {
                mat,
            };
            foreach (var c in bundleData.spriteDefinitions)
            {
                c.material = bundleData.materials[0];
                c.materialInst = bundleData.materials[0];
                c.materialId = 0;
            }
        }
    }
}
