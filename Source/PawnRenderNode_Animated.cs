using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static Shashlichnik.PawnRenderNodeProperties_Animated;
using static Shashlichnik.PawnRenderNodeProperties_Animated.KeyframeLine;
using AnimationState = Shashlichnik.AnimationComp.AnimationState;
namespace Shashlichnik
{
    public class PawnRenderNode_Animated : PawnRenderNode
    {
        public PawnRenderNode_Animated(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
            
        }
        public new PawnRenderNodeProperties_Animated Props => props as PawnRenderNodeProperties_Animated;
        private KeyframeExtended currentKeyframe;
        private AnimationState animationState;

        public KeyframeExtended CurrentKeyframe
        {
            get
            {
                return AnimationState.CurrentKeyframe;
            }

        }

        public AnimationState AnimationState
        {
            get
            {
                if (animationState == null)
                {
                    animationState = tree.pawn.GetComp<AnimationComp>().GetAnimationState(this);
                }
                return animationState;
            }
        }


        public string ID
        {
            get
            {

                if (!string.IsNullOrWhiteSpace(Props.id))
                {
                    return Props.id;
                }
                if (hediff != null)
                {
                    return $"{hediff.loadID}_{(hediff.def as IRenderNodePropertiesParent).RenderNodeProperties.IndexOf(this.Props)}";
                }
                if (gene != null)
                {
                    return $"{gene.loadID}_{(gene.def as IRenderNodePropertiesParent).RenderNodeProperties.IndexOf(this.Props)}";
                }
                if (apparel != null)
                {
                    return $"{apparel.ThingID}_{apparel.def.apparel.RenderNodeProperties.IndexOf(this.Props)}";
                }
                if (trait != null)
                {
                    return $"{trait.def.defName}_{trait.CurrentData.RenderNodeProperties.IndexOf(this.Props)}";
                }
                Log.ErrorOnce($"No ID", this.GetHashCode());
                return "";
            }
        }

        public string parentID => !string.IsNullOrWhiteSpace(Props.groupName) ? Props.groupName : (hediff?.loadID.ToString() ?? gene?.loadID.ToString() ?? apparel?.ThingID ?? this.trait?.def.defName);
        protected virtual IEnumerable<KeyframeLine> KeyframeLinesFor(Pawn pawn)
        {
            return AnimationState.AvailableLines;
        }
        public KeyframeLine CurrentLine => AnimationState.CurrentLine;
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
        public virtual AnimationState ToAnimationState()
        {
            var result = new AnimationState();
            result.availableLinesIds.Clear();
            var defaultLine = Props.keyframeLines[Math.Abs(this.tree.pawn.thingIDNumber.HashOffset() % Props.keyframeLines.Count)];
            var id = !string.IsNullOrEmpty(defaultLine.id) ? defaultLine.id : Props.keyframeLines.IndexOf(defaultLine).ToString();
            result.availableLinesIds.Add(id);
            if (defaultLine.tickOffset >= 0)
            {
                result.AnimationTick = defaultLine.tickOffset % defaultLine.AnimationLength;
            }
            else
            {
                result.AnimationTick = defaultLine.AnimationLength - (defaultLine.tickOffset % defaultLine.AnimationLength);
            }
            return result;
        }
    }
}
