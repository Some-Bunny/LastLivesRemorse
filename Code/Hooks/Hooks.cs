using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LastLivesRemorse
{
    public class Hooks
    {
        public static void InitHooks()
        {

            new Hook(typeof(AmmonomiconDeathPageController).GetMethod("InitializeRightPage", BindingFlags.Instance | BindingFlags.NonPublic),
                typeof(Hooks).GetMethod("InitializeRightPageHook"));
        }
        public static void InitializeRightPageHook(Action<AmmonomiconDeathPageController> orig, AmmonomiconDeathPageController self)
        {
            orig(self);
            if (self.isVictoryPage == true)
            {
                SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(SaveAPI.CustomDungeonFlags.REVENANT_WILL_SPAWN_NEXT_RUN, false);
                SaveAPI.AdvancedGameStatsManager.Instance.SetFlag(SaveAPI.CustomDungeonFlags.REVENANT_WILL_SPAWN, false);
            }      
        }
    }
}
