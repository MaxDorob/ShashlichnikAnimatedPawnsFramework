using System;
using System.Collections.Generic;
using System.Linq;
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
        public KeyframeLine LineWithId(string id)
        {
            KeyframeLine result = keyframeLines.FirstOrDefault(x => x.id == id);
            if (result != null)
            {
                return result;
            }
            if (int.TryParse(id, out var index) && keyframeLines.Count > index)
            {
                result = keyframeLines[index];
            }
            if (result == null)
            {
                Log.ErrorOnce($"Missing line with id: {id}", id.GetHashCode());
            }
            return result;

        }
        public string LineId(KeyframeLine line)
        {
            if (line == null || !keyframeLines.Contains(line))
            {
                return null;
            }
            if (!string.IsNullOrEmpty(line.id))
            {
                return line.id;
            }
            return keyframeLines.IndexOf(line).ToString();
        }
        internal IEnumerable<KeyframeLine> DefaultLinesFor(Pawn pawn)
        {
            if (playOneLine)
            {
                foreach (KeyframeLine line in keyframeLines)
                {
                    yield return line;
                }
            }
            else
            {
                yield return keyframeLines[Math.Abs(pawn.thingIDNumber.HashOffset() % keyframeLines.Count)];
            }
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
        public string id;
        public string groupName;
        public bool playOneLine = true;
        public class KeyframeLine
        {
            public string id;
            public int tickOffset = 0;
            public int keyframesCount = 0;
            public int ticksPerAnimation = 3;
            public string keyframesTexPath;
            public List<KeyframeExtended> keyframes;
            public DrawData drawData;
            public int AnimationLength { get; private set; }
            public class KeyframeExtended : Keyframe
            {
                public string texPath;
            }

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
                if (tickOffset < 0)
                {
                    Log.Warning("Tick offset less than 0");
                    tickOffset = AnimationLength - (tickOffset % AnimationLength);
                }
                AnimationLength = keyframes.Max(x => x.tick) + this.ticksPerAnimation;
            }

        }
    }
}
