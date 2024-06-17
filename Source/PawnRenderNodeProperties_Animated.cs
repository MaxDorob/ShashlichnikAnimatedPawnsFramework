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
        public List<KeyframeLine> keyframeLines;
        public PawnRenderNodeProperties_Animated()
        {
            workerClass = typeof(PawnRenderNodeWorker_Animated);
            nodeClass = typeof(PawnRenderNode_Animated);
        }

        public override void ResolveReferences()
        {
            base.ResolveReferences();
            if (keyframeLines?.Count == 0)
            {
                Log.Error($"No keyframe lines");
            }
            foreach (KeyframeLine line in keyframeLines)
            {
                line.ResolveReferences();
            }
        }

        public class KeyframeLine
        {
            public int tickOffset = 0;
            public int keyframesCount = 0;
            public int ticksPerAnimation = 3;
            public string keyframesTexPath;
            public List<KeyframeExtended> keyframes;
            public int AnimationLength { get; private set; }
            public class KeyframeExtended : Keyframe
            {
                public string texPath;
            }
            public DrawData drawData;

            public void ResolveReferences()
            {
                if (keyframes == null)
                {
                    keyframes = new List<KeyframeExtended>();
                }
                if (keyframes.Count < keyframesCount || keyframes.Count == 0)
                {
                    if (keyframesCount <= 0 || keyframesTexPath.NullOrEmpty())
                    {
                        Log.Error($"Keyframes not specified");
                    }
                    var tick = 0;
                    for (int i = 0; i < keyframesCount; i++)
                    {
                        var texPath = keyframesTexPath + (i + 1);
                        var keyframe = keyframes.FirstOrDefault(x => x.texPath == texPath);
                        if (keyframe == null)
                        {
                            keyframe = new KeyframeExtended() { texPath = texPath, tick = tick };
                            keyframes.Add(keyframe);
                        }
                        else
                        {
                            if (keyframe.tick < tick)
                            {
                                Log.Warning($"Tick of {keyframe.texPath} less than expected");
                            }
                            tick = keyframe.tick;
                        }
                        tick += ticksPerAnimation;
                    }
                    keyframes = keyframes.OrderBy(x => x.tick).ToList();
                }
                AnimationLength = keyframes.Max(x => x.tick) + this.ticksPerAnimation;
            }

        }
    }
}
