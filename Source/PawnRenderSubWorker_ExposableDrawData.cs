using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace Shashlichnik
{
    public class PawnRenderSubWorker_ExposableDrawData : PawnRenderSubWorker
    {
        public override void TransformScale(PawnRenderNode node, PawnDrawParms parms, ref Vector3 scale)
        {
            base.TransformScale(node, parms, ref scale);
            DrawDataExposable drawData;
            if (node is PawnRenderNode_Animated animated && (drawData = animated.AnimationState.drawData) != null)
            {
                scale *= drawData.ScaleFor(node.tree.pawn);
            }
        }
        public override void TransformRotation(PawnRenderNode node, PawnDrawParms parms, ref Quaternion rotation)
        {
            base.TransformRotation(node, parms, ref rotation);
            DrawDataExposable drawData;
            if (node is PawnRenderNode_Animated animated && (drawData = animated.AnimationState.drawData) != null)
            {
                rotation *= Quaternion.AngleAxis(drawData.RotationOffsetForRot(parms.facing), Vector3.up);
            }
        }
        public override void TransformOffset(PawnRenderNode node, PawnDrawParms parms, ref Vector3 offset, ref Vector3 pivot)
        {
            base.TransformOffset(node, parms, ref offset, ref pivot);
            var old = offset;
            DrawDataExposable drawData = null;
            if (node is PawnRenderNode_Animated animated && (drawData = animated.AnimationState.drawData) != null)
            {
                offset += drawData.OffsetForRot(parms.facing);
            }
        }
        public override void TransformLayer(PawnRenderNode node, PawnDrawParms parms, ref float layer)
        {
            base.TransformLayer(node, parms, ref layer);
            DrawDataExposable drawData;
            if (node is PawnRenderNode_Animated animated && (drawData = animated.AnimationState.drawData) != null)
            {
                layer = drawData.LayerForRot(parms.facing, layer);
            }
        }
    }
}
