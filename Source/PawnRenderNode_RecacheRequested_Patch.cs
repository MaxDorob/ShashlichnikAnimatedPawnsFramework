﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    [HarmonyPatch(typeof(PawnRenderNode), nameof(PawnRenderNode.RecacheRequested), MethodType.Getter)]
    public static class PawnRenderNode_RecacheRequested_Patch
    {
        public static bool Prefix(PawnRenderNode __instance, ref bool __result)
        {
            if (__instance is PawnRenderNode_Animated animated)
            {
                __result = animated.RecacheRequested;
                return !__result;
            }
            return true;
        }
    }
}
