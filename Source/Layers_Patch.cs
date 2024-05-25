using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    [HarmonyPatch(typeof(PawnRenderUtility), nameof(PawnRenderUtility.AltitudeForLayer))]
    internal static class Layers_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.operand is float && (float)instruction.operand < 0f)
                {
                    instruction.operand = -50f;
                }
                yield return instruction;
            }
        }
    }
}
