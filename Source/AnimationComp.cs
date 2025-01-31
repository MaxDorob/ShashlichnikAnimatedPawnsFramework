using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static HarmonyLib.Code;
using static Shashlichnik.PawnRenderNodeProperties_Animated;
using static Shashlichnik.PawnRenderNodeProperties_Animated.KeyframeLine;
using static UnityEngine.Random;

namespace Shashlichnik
{
    public class AnimationComp : ThingComp
    {
        private List<AnimationState> animationStates = new List<AnimationState>();
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref animationStates, nameof(animationStates));
            Scribe_Values.Look(ref ticks, nameof(ticks));
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                foreach (var state in animationStates)
                {
                    state.PostLoad(parent as Pawn);
                }
            }
        }
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            var pawn = parent as Pawn;
            if (!pawn.IsWorldPawn())
            {
                foreach (var state in animationStates.ToList())
                {
                    if (string.IsNullOrEmpty(state.id) || (pawn.Drawer.renderer.renderTree.Resolved && state.RenderNode == null))
                    {
                        animationStates.Remove(state);
                    }
                }
            }
        }
        public override void CompTick()
        {
            base.CompTick();
            if (animationStates.NullOrEmpty())
            {
                return;
            }
            var pawn = parent as Pawn;
            if (pawn.Map == null || (pawn.DeadOrDowned && !pawn.Crawling))
            {
                return;
            }
            ticks = Math.Max(ticks + 1, 0);
        }
        int ticks = 0;

        public void UpdateStatesIfNeeded()
        {
            if (ticks > 0)
            {
                foreach (var animationState in animationStates)
                {
                    animationState.Tick(ticks);
                }
                ticks = 0;
            }
        }

        public AnimationState GetAnimationState(PawnRenderNode_Animated renderNode)
        {
            var id = renderNode.ID;
            var result = animationStates.FirstOrDefault(x => x.id == renderNode.ID);
            if (result == null)
            {
                result = new AnimationState(renderNode);
                animationStates.Add(result);
            }
            return result;
        }
        public class AnimationState : IExposable
        {
            public List<string> availableLinesIds = new List<string>();
            private string currentLineId = null;
            private KeyframeLine currentLine;
            public DrawDataExposable drawData;
            private AnimationComp parent;
            private int animationTick = 0;

            private PawnRenderNode_Animated renderNode;
            public Pawn pawn;
            public KeyframeExtended currentKeyframe;
            public string id;
            public AnimationState() { }
            public AnimationState(PawnRenderNode_Animated renderNode)
            {
                this.renderNode = renderNode;
                PostLoad(renderNode.tree.pawn);
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
                Scribe_Values.Look(ref id, nameof(id));
                Scribe_Deep.Look(ref drawData, nameof(drawData));
                Scribe_Collections.Look(ref availableLinesIds, nameof(availableLinesIds));
                Scribe_Values.Look(ref currentLineId, nameof(currentLineId));
                Scribe_Values.Look(ref animationTick, nameof(animationTick));

            }
            public void PostLoad(Pawn pawn)
            {
                this.pawn = pawn;
                parent = pawn.GetComp<AnimationComp>();
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
                    var currentLine = this.CurrentLine;
                    var animationLength = CurrentLine.AnimationLength;
                    if (value >= animationLength)
                    {
                        if (!RenderNode.Props.playOneLine)
                        {
                            CurrentLine = null;
                            animationLength = CurrentLine.AnimationLength;
                        }
                        animationTick = value % animationLength;
                    }
                    else if (value < 0)
                    {
                        animationTick = animationLength + (value % animationLength);
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
                    if (value != null)
                    {
                        currentLineId = RenderNode.Props.LineId(value);
                    }
                    else
                    {
                        currentLineId = null;
                    }
                }
            }
            public KeyframeExtended CurrentKeyframe
            {
                get
                {
                    parent.UpdateStatesIfNeeded();
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
            public void Tick(int ticksAmount)
            {
                AnimationTick += ticksAmount;
            }
        }
    }
}
