using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;


namespace LastLivesRemorse
{
    public class DamnedShrineSpawnController
    {
        public static void InitShrineRoom()
        {
            var protoRoom = GungeonAPI.RoomFactory.BuildFromResource(
                "LastLivesRemorse/damnationroom_p.room").room;
            protoRoom.UseCustomMusicState = true;
            protoRoom.UseCustomMusicSwitch = true;
            protoRoom.CustomMusicSwitch = "Stop_MUS_All";
            protoRoom.CustomMusicEvent = "Stop_MUS_All";
            protoRoom.usesCustomAmbientLight = true;
            protoRoom.customAmbientLight = new Color(0.2f, 0.2f, 0.2f);
            GungeonAPI.RoomFactory.AddInjection(protoRoom,
                "Damned_Shrine",
                new List<Dungeonator.ProceduralFlowModifierData.FlowModifierPlacementType>()
                {
                    Dungeonator.ProceduralFlowModifierData.FlowModifierPlacementType.END_OF_CHAIN
                },
                0,
                new List<DungeonPrerequisite>()
                {
                    new DungeonPrerequisite()
                    {
                        prerequisiteType = DungeonPrerequisite.PrerequisiteType.FLAG,
                        requireFlag = true,
                        saveFlagToCheck = GungeonFlags.BOSSKILLED_LICH
                    },
                    new SaveAPI.CustomDungeonPrerequisite()
                    {
                        customFlagToCheck = SaveAPI.CustomDungeonFlags.REVENANT_WILL_SPAWN_NEXT_RUN,
                        requireCustomFlag = false,
                        advancedPrerequisiteType = SaveAPI.CustomDungeonPrerequisite.AdvancedPrerequisiteType.CUSTOM_FLAG
                    }
                },
                "Damned_Shrine",
                LLRModule.ShrineSelectionChance.Value,//0.8f, 
                LLRModule.ShrineWeight.Value);//0.33f);
        }
    }
}
