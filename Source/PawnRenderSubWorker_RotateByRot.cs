using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Shashlichnik
{
    public class PawnRenderSubWorker_RotateByRot : PawnRenderSubWorker
    {
        public override void TransformRotation(PawnRenderNode node, PawnDrawParms parms, ref Quaternion rotation)
        {
            rotation = rotation * Quaternion.AngleAxis(parms.facing.AsAngle, Vector3.up);
        }
    }
}
