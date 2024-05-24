using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    public class PawnRenderNodeProperties_Animated : PawnRenderNodeProperties
    {
        public PawnRenderNodeProperties_Animated()
        {
            workerClass = typeof(PawnRenderNodeWorker_Animated);
            nodeClass = typeof(PawnRenderNode_Animated);
        }
        public int tickOffset = 0;
        public List<KeyframeExtended> keyframes;
        public class KeyframeExtended : Keyframe
        {
            public string texPath;
        }
    }

}
