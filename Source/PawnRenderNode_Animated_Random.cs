using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static Shashlichnik.PawnRenderNodeProperties_Animated;

namespace Shashlichnik
{
    public class PawnRenderNode_Animated_Random : PawnRenderNode_Animated
    {
        public PawnRenderNode_Animated_Random(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
        }
        protected override void OnAnimationRestart()
        {
            base.OnAnimationRestart();
            this.currentLine = Props.keyframeLines[Math.Abs(Gen.HashCombineInt(Find.TickManager.TicksAbs, tree.pawn.thingIDNumber) % Props.keyframeLines.Count)];
            animationLength = CurrentLine.AnimationLength;
        }
        protected override IEnumerable<KeyframeLine> KeyframeLinesFor(Pawn pawn)
        {
            return Props.keyframeLines;
        }
    }
}
