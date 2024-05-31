using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Shashlichnik
{
    public class PawnRenderNodeWorker_Animated : PawnRenderNodeWorker
    {
        protected override Graphic GetGraphic(PawnRenderNode node, PawnDrawParms parms)
        {
            if (node is PawnRenderNode_Animated animated)
            {
                return animated.Graphic;
            }
            return base.GetGraphic(node, parms);
        }
    }
}
