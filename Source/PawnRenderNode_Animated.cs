using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static Shashlichnik.PawnRenderNodeProperties_Animated;

namespace Shashlichnik
{
    public class PawnRenderNode_Animated : PawnRenderNode
    {
        public PawnRenderNode_Animated(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
            pawnDead = pawn.Dead;
            animationLength = Props.keyframes.Max(x => x.tick) + 3;
            personalTickOffset = pawn.thingIDNumber;
        }
        bool pawnDead;
        TickManager tickManager = Find.TickManager;
        public int personalTickOffset;
        public new PawnRenderNodeProperties_Animated Props => props as PawnRenderNodeProperties_Animated;
        public readonly int animationLength;
        public float debugTickOffset = 0;
        public int CurrentAnimationTick => pawnDead ? 0 + Props.tickOffset : (tickManager.TicksAbs + personalTickOffset + Props.tickOffset + (int)debugTickOffset) % animationLength;
        private KeyframeExtended currentKeyframe;
        private int lastRecachedAbsTick = -1;
        private int nextRecacheTick = 0;
        public KeyframeExtended CurrentKeyframe
        {
            get
            {
                //return Props.keyframes.LastOrDefault(x=>x.tick < CurrentAnimationTick) ?? Props.keyframes.First();
                var currentAnimationTick = CurrentAnimationTick;
                if (currentKeyframe == null || !pawnDead && nextRecacheTick <= currentAnimationTick)
                {
                    int count = Props.keyframes.Count;
                    int i = 0;
                    KeyframeExtended result = null;
                    for (; i < count; i++)
                    {
                        var current = Props.keyframes[i];
                        if (current.tick > currentAnimationTick)
                        {
                            break;
                        }
                        result = current;
                    }
                    var nextKeyframe = i < count - 1 ? Props.keyframes[i] : Props.keyframes[0];
                    currentKeyframe = result;
                    nextRecacheTick = nextKeyframe.tick;
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

        //public KeyframeExtended NextKeyframe => Props.keyframes.FirstOrDefault() ?? Props.keyframes.First();
        public new bool RecacheRequested
        {
            get
            {
                if (pawnDead)
                {
                    return false;
                }
                if (Math.Abs(tickManager.TicksAbs - lastRecachedAbsTick) > 30)
                {
                    currentKeyframe = null;
                    return true;
                }
                var currentKf = CurrentKeyframe;
                var currentTick = CurrentAnimationTick;
                if (Props.keyframes[Props.keyframes.Count - 1] == currentKeyframe)
                {
                    return currentTick <= Props.keyframes[Props.keyframes.Count - 2].tick;
                }
                return currentKf.tick >= currentTick;
            }
        }

        public new Graphic Graphic => graphics[CurrentKeyframe];//Change it to override when next update releases
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
        protected override void EnsureMaterialsInitialized()
        {
            base.EnsureMaterialsInitialized();
            var pawn = tree.pawn;
            if (graphics == null && HasGraphic(pawn))
            {
                graphics = new Dictionary<KeyframeExtended, Graphic>(Props.keyframes.Count);
                for (int i = 0; i < Props.keyframes.Count; i++)
                {
                    var keyframe = Props.keyframes[i];
                    graphics.Add(keyframe, GraphicFor(pawn, string.IsNullOrWhiteSpace(keyframe.texPath) ? TexPathFor(pawn) + i.ToString() : keyframe.texPath));
                }
            }
        }

        Dictionary<KeyframeExtended, Graphic> graphics;
        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(props.debugLabel.ToString()))
                return props.debugLabel;
            return base.ToString();
        }
    }
}
