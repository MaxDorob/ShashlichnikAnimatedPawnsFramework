using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace Shashlichnik
{
    [HarmonyPatch(typeof(Dialog_DebugRenderTree), "RightRect")]
    public static class DebugRenderTree_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var _instructions = instructions.ToList();
            for (int i = 0; i < _instructions.Count; i++)
            {
                if (i == _instructions.Count - 2)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(_instructions[i]);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return CodeInstruction.LoadField(typeof(Dialog_DebugRenderTree), "currentNode");
                    yield return new CodeInstruction(OpCodes.Ldloca_S, 1);
                    yield return CodeInstruction.Call(typeof(DebugRenderTree_Patch), nameof(Extension));

                }
                yield return _instructions[i];
            }
        }
        static void Extension(Dialog_DebugRenderTree instance, PawnRenderNode currentNode, ref Rect rect2)
        {
            if (currentNode == null || !(currentNode is PawnRenderNode_Animated animated))
            {
                return;
            }
            rect2.y += 18f;
            Widgets.Label(rect2, "Tick offset " + (animated.debugTickOffset + animated.CurrentLine.tickOffset));
            var current = animated.Props.keyframeLines.IndexOf(animated.ForcedLine);
            var text = "Next line. Cur.: " + (current < 0 ? "-" : (current + 1).ToString()) + $"/{animated.Props.keyframeLines.Count}";
            if (Widgets.ButtonText(new Rect(rect2.width / 2, rect2.y - 4f, rect2.width / 2, 25f), text, true, true, true, null))
            {
                animated.ForcedLine = animated.Props.keyframeLines[current + 1 < animated.Props.keyframeLines.Count ? current + 1 : 0];
            }
            rect2.y += rect2.height;
            Widgets.HorizontalSlider(rect2, ref animated.debugTickOffset, new FloatRange(-animated.CurrentLine.tickOffset, animated.CurrentLine.AnimationLength - animated.CurrentLine.tickOffset), "Tick offset: " + animated.debugTickOffset.ToStringByStyle(ToStringStyle.Integer) + $" ({animated.CurrentAnimationTick}/{animated.animationLength})", 3f);
            animated.requestRecache = true;
            animated.CurrentKeyframe = null;
        }
    }
}
