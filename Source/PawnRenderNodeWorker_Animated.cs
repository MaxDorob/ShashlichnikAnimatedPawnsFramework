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
        public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
        {
            var result = base.OffsetFor(node, parms, out pivot);

            if (node is PawnRenderNode_Animated animated && animated.CurrentLine.drawData != null)
            {
                Vector3 vector2 = animated.CurrentLine.drawData.OffsetForRot(parms.facing);
                if (node.Props.drawData.scaleOffsetByBodySize && parms.pawn.story != null)
                {
                    Vector2 bodyGraphicScale = parms.pawn.story.bodyType.bodyGraphicScale;
                    float d = (bodyGraphicScale.x + bodyGraphicScale.y) / 2f;
                    vector2 *= d;
                }
                result += vector2;
            }

            return result;
        }
        protected override Vector3 PivotFor(PawnRenderNode node, PawnDrawParms parms)
        {
            Vector3 vector = base.PivotFor(node, parms);
            if (node is PawnRenderNode_Animated animated && animated.CurrentLine.drawData != null)
            {
                vector -= (animated.CurrentLine.drawData.PivotForRot(parms.facing) - DrawData.PivotCenter).ToVector3();
            }
            return vector;
        }
        public override float LayerFor(PawnRenderNode node, PawnDrawParms parms)
        {
            if (node is PawnRenderNode_Animated animated && animated.CurrentLine.drawData != null)
            {
                var drawData = animated.CurrentLine.drawData;
                var result = drawData.LayerForRot(parms.facing, -1000f) + node.debugLayerOffset;
                if (result > -99f)
                {
                    return result;
                }
            }
            return base.LayerFor(node, parms);
        }
        public override Quaternion RotationFor(PawnRenderNode node, PawnDrawParms parms)
        {
            var result = base.RotationFor(node, parms);
            DrawData drawData;
            if (node is PawnRenderNode_Animated animated && (drawData = animated.CurrentLine.drawData) != null)
            {
                result *= Quaternion.AngleAxis(drawData.RotationOffsetForRot(parms.facing), Vector3.up);
            }
            return result;
        }
    }
}
