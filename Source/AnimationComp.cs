using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static Shashlichnik.PawnRenderNodeProperties_Animated;
using static Shashlichnik.PawnRenderNodeProperties_Animated.KeyframeLine;

namespace Shashlichnik
{
    public class AnimationComp : ThingComp
    {
        private Dictionary<string, AnimationState> animationStates = new System.Collections.Generic.Dictionary<string, AnimationState>();
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref animationStates, nameof(animationStates));

        }
        public override void CompTick()
        {
            base.CompTick();
            foreach (var animationState in animationStates.Values)
            {
                animationState.Tick();
            }
        }

        public AnimationState GetAnimationState(PawnRenderNode_Animated renderNode)
        {
            var id = renderNode.ID;
            AnimationState result;
            if (!animationStates.TryGetValue(id, out result))
            {
                result = renderNode.ToAnimationState();
                animationStates.Add(id, result);
            }
            result.renderNode ??= renderNode;
            return result;
        }
        public class AnimationState : IExposable
        {
            public List<string> availableLinesIds = new List<string>();
            private string currentLineId = null;
            private KeyframeLine currentLine;
            private DrawDataExposable drawData;
            private int animationTick = 0;

            public PawnRenderNode_Animated renderNode;
            public KeyframeExtended currentKeyframe;

            public void ExposeData()
            {
                Scribe_Deep.Look(ref drawData, nameof(drawData));
                Scribe_Collections.Look(ref availableLinesIds, nameof(availableLinesIds));
                Scribe_Values.Look(ref currentLineId, nameof(currentLineId));
                Scribe_Values.Look(ref animationTick, nameof(animationTick));

            }
            public int AnimationTick
            {
                get
                {
                    return animationTick;
                }
                set
                {
                    animationTick = value;
                    currentKeyframe = null;
                }
            }
            public IEnumerable<KeyframeLine> AvailableLines
            {
                get
                {
                    return availableLinesIds.Select(renderNode.Props.LineWithId);
                }
            }
            public KeyframeLine CurrentLine
            {
                get
                {
                    {
                        if (currentLine == null || currentLineId == null)
                        {
                            if (currentLineId == null)
                            {
                                currentLineId = availableLinesIds.RandomElement();
                            }
                            currentLine = renderNode.Props.LineWithId(currentLineId);
                        }
                        return currentLine;
                    }
                }
                set
                {
                    currentLine = value;
                    currentLineId = renderNode.Props.LineId(value);
                }
            }
            public KeyframeExtended CurrentKeyframe
            {
                get
                {
                    if (currentKeyframe == null)
                    {
                        currentKeyframe = CurrentLine.keyframes.FirstOrDefault(x => x.tick >= animationTick) ?? CurrentLine.keyframes[0];
                    }
                    return currentKeyframe;

                }
            }
            public void Tick()
            {
                if (renderNode.tree.pawn.DeadOrDowned && !renderNode.tree.pawn.Crawling)
                {
                    return;
                }
                AnimationTick++;
                if (CurrentLine.AnimationLength < animationTick)
                {
                    CurrentLine = null;
                    animationTick = animationTick % CurrentLine.AnimationLength;
                }
            }
        }
    }
}
