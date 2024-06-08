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
        public int keyframesCount = 0;
        public int ticksPerAnimation = 3;
        public string keyframesTexPath;
        public List<KeyframeExtended> keyframes;
        public class KeyframeExtended : Keyframe
        {
            public string texPath;
        }
        public override void ResolveReferences()
        {
            base.ResolveReferences();
            if (keyframes == null)
            {
                keyframes = new List<KeyframeExtended>();
            }
            if (keyframes.Count == 0)
            {
                if (keyframesCount <= 0 || keyframesTexPath.NullOrEmpty())
                {
                    Log.Error($"Keyframes not specified");
                }
                for (int i = 0; i < keyframesCount; i++)
                {
                    keyframes.Add(new KeyframeExtended() { texPath = keyframesTexPath + (i + 1), tick = i * ticksPerAnimation });
                }
            }
        }

    }

}
