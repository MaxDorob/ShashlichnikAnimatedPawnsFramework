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
        Random random;
        public PawnRenderNode_Animated_Random(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
            random = new Random();
        }
        protected override void OnAnimationRestart()
        {
            base.OnAnimationRestart();
        }
        protected override IEnumerable<KeyframeLine> KeyframeLinesFor(Pawn pawn)
        {
            return Props.keyframeLines;
        }
    }
}
