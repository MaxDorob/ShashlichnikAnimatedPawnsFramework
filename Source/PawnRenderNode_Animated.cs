using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static Shashlichnik.PawnRenderNodeProperties_Animated;
using static Shashlichnik.PawnRenderNodeProperties_Animated.KeyframeLine;

namespace Shashlichnik
{
    public class PawnRenderNode_Animated : PawnRenderNode
    {
        public PawnRenderNode_Animated(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
            currentLine = KeyframeLinesFor(pawn).First();
            pawnDead = pawn.Dead;
            animationLength = CurrentLine.AnimationLength;
            personalTickOffset = pawn.thingIDNumber.HashOffset() % animationLength;
            animationStartedTick = tickManager.TicksAbs - Math.Abs(personalTickOffset) - CurrentLine.tickOffset;
        }
        bool pawnDead;
        TickManager tickManager = Find.TickManager;
        public int personalTickOffset;
        public new PawnRenderNodeProperties_Animated Props => props as PawnRenderNodeProperties_Animated;
        public int animationLength;
        public float debugTickOffset = 0;
        public int CurrentAnimationTick => pawnDead ? 0 + CurrentLine.tickOffset : (tickManager.TicksAbs - AnimationStartedTick + (int)debugTickOffset) % animationLength;
        private KeyframeExtended currentKeyframe;
        private int nextRecacheTick = 0;
        int animationStartedTick;
        protected KeyframeLine currentLine;
        private KeyframeLine forcedLine;
        public int AnimationStartedTick
        {
            get => animationStartedTick;
            set
            {
                if (animationStartedTick != value)
                {
                    animationStartedTick = value;
                    OnAnimationRestart();
                }
            }
        }
        public KeyframeExtended CurrentKeyframe
        {
            get
            {
                var currentAnimationTick = CurrentAnimationTick;
                var currentAbsTick = tickManager.TicksAbs;
                AnimationStartedTick += ((currentAbsTick - (animationStartedTick)) / animationLength) * animationLength + (int)debugTickOffset;
                if (currentKeyframe == null || (!pawnDead && currentAnimationTick >= nextRecacheTick))
                {
                    int count = CurrentLine.keyframes.Count;
                    int i = 0;
                    KeyframeExtended result = CurrentLine.keyframes[0];
                    for (; i < count; i++)
                    {
                        var current = CurrentLine.keyframes[i];
                        if (current.tick > currentAnimationTick)
                        {
                            break;
                        }
                        result = current;
                    }
                    var nextKeyframe = i < count - 1 ? CurrentLine.keyframes[i] : CurrentLine.keyframes[0];
                    currentKeyframe = result;
                    nextRecacheTick = nextKeyframe.tick;
                    return result;
                }
                return currentKeyframe;
            }
            internal set
            {
                currentKeyframe = value;
                nextRecacheTick = 0;
            }
        }

        public KeyframeLine ForcedLine
        {
            get => forcedLine;
            set
            {
                forcedLine = value;
                AnimationStartedTick = tickManager.TicksAbs;
            }
        }
        public virtual KeyframeLine CurrentLine
        {
            get => ForcedLine ?? currentLine;
        }
        protected virtual IEnumerable<KeyframeLine> KeyframeLinesFor(Pawn pawn)
        {
            if (forcedLine != null)
            {
                yield return forcedLine;
            }
            yield return Props.keyframeLines[Math.Abs(pawn.thingIDNumber.HashOffset() % Props.keyframeLines.Count)];
        }
        protected override IEnumerable<Graphic> GraphicsFor(Pawn pawn)
        {
            if (HasGraphic(pawn))
            {
                var lines = KeyframeLinesFor(pawn).Distinct().ToList();
                graphics = new Dictionary<KeyframeExtended, Graphic>(lines.Sum(x => x.keyframes.Count));
                foreach (var line in lines)
                {
                    for (int i = 0; i < line.keyframes.Count; i++)
                    {
                        var keyframe = line.keyframes[i];
                        var graphic = GraphicFor(pawn, string.IsNullOrWhiteSpace(keyframe.texPath) ? TexPathFor(pawn) + i.ToString() : keyframe.texPath);
                        graphics[keyframe] = graphic;
                        yield return graphic;
                    }
                }
            }
        }
        public new Graphic Graphic => graphics[CurrentKeyframe];
        public virtual Graphic GraphicFor(Pawn pawn, string text)
        {
            if (text.NullOrEmpty())
            {
                return null;
            }
            Shader shader = ShaderFor(pawn);
            if (shader == null)
            {
                return null;
            }
            return GraphicDatabase.Get<Graphic_Multi>(text, shader, Vector2.one, ColorFor(pawn));
        }

        Dictionary<KeyframeExtended, Graphic> graphics;
        protected virtual void OnAnimationRestart()
        {
            currentKeyframe = null;
        }
        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(props.debugLabel?.ToString()))
                return props.debugLabel;
            return base.ToString();
        }
    }
}
