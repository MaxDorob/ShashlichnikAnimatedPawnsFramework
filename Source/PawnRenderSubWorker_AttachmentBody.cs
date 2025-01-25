using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Shashlichnik
{
    /// <summary>
    /// Technically the same <see cref="PawnRenderNodeWorker_AttachmentBody"/> 
    /// </summary>
    public class PawnRenderSubWorker_AttachmentBody : PawnRenderSubWorker
    {
        public override void TransformScale(PawnRenderNode node, PawnDrawParms parms, ref Vector3 scale)
        {
            base.TransformScale(node, parms, ref scale);
            Vector2 bodyGraphicScale = parms.pawn.story.bodyType.bodyGraphicScale;
            scale *= (bodyGraphicScale.x + bodyGraphicScale.y) / 2f;
        }
    }
}
