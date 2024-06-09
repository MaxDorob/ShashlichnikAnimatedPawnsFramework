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
            currentLine = Props.keyframeLines[0];
            pawnDead = pawn.Dead;
            animationLength = CurrentLine.AnimationLength;
            personalTickOffset = pawn.thingIDNumber.HashOffset() % animationLength;
            animationStartedTick = tickManager.TicksAbs - Math.Abs(personalTickOffset) - CurrentLine.tickOffset;
        }
        bool pawnDead;
        TickManager tickManager = Find.TickManager;
        public int personalTickOffset;
        public new PawnRenderNodeProperties_Animated Props => props as PawnRenderNodeProperties_Animated;
        public readonly int animationLength;
        public float debugTickOffset = 0;
        public int CurrentAnimationTick => pawnDead ? 0 + CurrentLine.tickOffset : (tickManager.TicksAbs - animationStartedTick + (int)debugTickOffset) % animationLength;
        private KeyframeExtended currentKeyframe;
        private int lastRecachedAbsTick = -1;
        private int nextRecacheTick = 0;
        int animationStartedTick;
        private KeyframeLine currentLine;
        public KeyframeExtended CurrentKeyframe
        {
            get
            {
                var currentAnimationTick = CurrentAnimationTick;
                var currentAbsTick = tickManager.TicksAbs;
                if (Math.Abs(currentAbsTick - animationStartedTick) > animationLength)
                {
                    currentKeyframe = null;
                }
                if (currentKeyframe == null || !pawnDead && (currentAnimationTick >= nextRecacheTick || Math.Abs(currentAbsTick - lastRecachedAbsTick) > animationLength))
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
                    animationStartedTick += ((currentAbsTick - animationStartedTick) / animationLength) * animationLength;
                    lastRecachedAbsTick = tickManager.TicksAbs;
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

        public virtual KeyframeLine CurrentLine
        {
            get => currentLine;
        }
        protected virtual IEnumerable<KeyframeLine> KeyframeLinesFor(Pawn pawn)
        {
            yield return currentLine;
        }
        protected override IEnumerable<Graphic> GraphicsFor(Pawn pawn)
        {
            if (graphics == null && HasGraphic(pawn))
            {
                var lines = KeyframeLinesFor(pawn).ToList();
                graphics = new Dictionary<KeyframeExtended, Graphic>(lines.Sum(x=>x.keyframes.Count));
                foreach (var line in lines)
                {
                    for (int i = 0; i < line.keyframes.Count; i++)
                    {
                        var keyframe = line.keyframes[i];
                        var graphic = GraphicFor(pawn, string.IsNullOrWhiteSpace(keyframe.texPath) ? TexPathFor(pawn) + i.ToString() : keyframe.texPath);
                        graphics.Add(keyframe, graphic);
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
        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(props.debugLabel?.ToString()))
                return props.debugLabel;
            return base.ToString();
        }
    }
}
