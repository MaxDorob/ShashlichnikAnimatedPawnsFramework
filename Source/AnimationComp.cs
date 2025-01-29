using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static HarmonyLib.Code;
using static Shashlichnik.PawnRenderNodeProperties_Animated;
using static Shashlichnik.PawnRenderNodeProperties_Animated.KeyframeLine;
using static UnityEngine.Random;

namespace Shashlichnik
{
    public class AnimationComp : ThingComp
    {
        private Dictionary<string, AnimationState> animationStates = new System.Collections.Generic.Dictionary<string, AnimationState>();
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref animationStates, nameof(animationStates));
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {

                foreach (var state in animationStates)
                {
                    state.Value.pawn = parent as Pawn;
                    state.Value.id = state.Key;
                }
            }
        }
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
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
            if (!animationStates.TryGetValue(id, out var result))
            {
                result = new AnimationState();
                result.Init(renderNode);
                animationStates.Add(id, result);
            }
            return result;
        }
        public class AnimationState : IExposable
        {
            public List<string> availableLinesIds = new List<string>();
            private string currentLineId = null;
            private KeyframeLine currentLine;
            public DrawDataExposable drawData;
            private int animationTick = 0;

            private PawnRenderNode_Animated renderNode;
            public Pawn pawn;
            public KeyframeExtended currentKeyframe;
            public string id;

            public void Init(PawnRenderNode_Animated renderNode)
            {
                this.renderNode = renderNode;
                this.pawn = renderNode.tree.pawn;
                availableLinesIds.Clear();
                availableLinesIds.AddRange(renderNode.Props.DefaultLinesFor(renderNode.tree.pawn).Select(renderNode.Props.LineId));
                SetInitialAnimationTick();
            }
            public void SetInitialAnimationTick()
            {
                var line = CurrentLine;
                if (line.tickOffset >= 0)
                {
                    AnimationTick = line.tickOffset % line.AnimationLength;
                }
                else
                {
                    AnimationTick = line.AnimationLength - (line.tickOffset % line.AnimationLength);
                }

            }

            public void ExposeData()
            {
                Scribe_Deep.Look(ref drawData, nameof(drawData));
                Scribe_Collections.Look(ref availableLinesIds, nameof(availableLinesIds));
                Scribe_Values.Look(ref currentLineId, nameof(currentLineId));
                Scribe_Values.Look(ref animationTick, nameof(animationTick));

            }
            public PawnRenderNode_Animated RenderNode
            {
                get
                {
                    if (renderNode == null)
                    {
                        renderNode = pawn.Drawer.renderer.renderTree.rootNode.AllRenderNodes().OfType<PawnRenderNode_Animated>().FirstOrDefault(x => x.ID == id);
                    }
                    return renderNode;
                }
            }
            public int AnimationTick
            {
                get
                {
                    return animationTick;
                }
                set
                {
                    if (value >= CurrentLine.AnimationLength)
                    {
                        animationTick = value % CurrentLine.AnimationLength;
                    }
                    else if (value < 0)
                    {
                        animationTick = CurrentLine.AnimationLength + value;
                    }
                    else
                    {
                        animationTick = value;
                    }
                    currentKeyframe = null;
                }
            }
            public IEnumerable<KeyframeLine> AvailableLines
            {
                get
                {
                    return availableLinesIds.Select(RenderNode.Props.LineWithId);
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
                            currentLine = RenderNode.Props.LineWithId(currentLineId);
                        }
                        return currentLine;
                    }
                }
                set
                {
                    animationTick = 0;
                    currentLine = value;
                    currentLineId = RenderNode.Props.LineId(value);
                }
            }
            public KeyframeExtended CurrentKeyframe
            {
                get
                {
                    if (currentKeyframe == null)
                    {
                        for (int i = CurrentLine.keyframes.Count - 1; i >= 0; i--)
                        {
                            var keyframe = CurrentLine.keyframes[i];
                            if (i == 0 || keyframe.tick <= animationTick)
                            {
                                currentKeyframe = keyframe;
                                break;
                            }
                        }
                    }
                    return currentKeyframe;

                }
            }
            public void Tick()
            {
                if (pawn.DeadOrDowned && !pawn.Crawling)
                {
                    return;
                }
                AnimationTick++;
                if (RenderNode == null)
                {
                    return;
                }
                if (CurrentLine.AnimationLength <= animationTick)
                {
                    animationTick = animationTick % CurrentLine.AnimationLength;
                    if (!RenderNode.Props.playOneLine)
                    {
                        CurrentLine = null;
                    }
                }
            }
        }
    }
}
