using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace LastLivesRemorse
{
    public class BundleStarter
    {
        public static AssetBundle Bundle;
        public static void StartBundles()
        {
            Bundle = AssetBundleLoader.LoadAssetBundleFromLiterallyAnywhere("llr_bundle");
        }
    }
}
