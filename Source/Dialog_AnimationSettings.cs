using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Shashlichnik
{
    public class Dialog_AnimationSettings : MainTabWindow
    {
        // Token: 0x17001D8D RID: 7565
        // (get) Token: 0x0600A488 RID: 42120 RVA: 0x00380649 File Offset: 0x0037E849
        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(600f, 600f);
            }
        }

        // Token: 0x17001D8E RID: 7566
        // (get) Token: 0x0600A489 RID: 42121 RVA: 0x0011F964 File Offset: 0x0011DB64
        protected override float Margin
        {
            get
            {
                return 10f;
            }
        }
        private IEnumerable<PawnRenderNode_Animated> NodesToChange
        {
            get
            {
                if (currentObject is PawnRenderNode_Animated pawnRenderNode)
                {
                    yield return pawnRenderNode;
                }
                else if (currentObject is IEnumerable<PawnRenderNode_Animated> nodes)
                {
                    foreach (var node in nodes)
                    {
                        yield return node;
                    }
                }
            }
        }

        // Token: 0x0600A48A RID: 42122 RVA: 0x0038065C File Offset: 0x0037E85C


        public Dialog_AnimationSettings() : base()
        {
            var pawn = Find.Selector.SelectedPawns.FirstOrDefault();
            this.doCloseX = true;
            this.preventCameraMotion = false;
            this.closeOnAccept = false;
            this.draggable = true;
            this.Init(pawn);
        }

        // Token: 0x0600A48B RID: 42123 RVA: 0x003806AC File Offset: 0x0037E8AC
        public void Init(Pawn pawn)
        {
            this.currentObject = null;
            this.scrollPosition = Vector2.zero;
            this.scrollHeight = 0f;
            this.nodeExpanded.Clear();
            this.pawn = pawn;
            this.tree = pawn.Drawer.renderer.renderTree;
            this.currentAnimation = this.tree.currentAnimation;
            this.optionalTitle = pawn.LabelShortCap + " (" + pawn.RaceProps.renderTree.defName + ")";
            this.drawParms = new PawnDrawParms
            {
                pawn = pawn,
                rotDrawMode = RotDrawMode.Fresh,
                flags = (PawnRenderFlags.Headgear | PawnRenderFlags.Clothes)
            };
            PawnRenderNode rootNode = this.tree.rootNode;
            if (rootNode == null)
            {
                return;
            }
            groups.Clear();
            foreach (var nodeGroup in rootNode.AllRenderNodes().OfType<PawnRenderNode_Animated>().GroupBy(x => x.parentID))
            {
                groups.Add(nodeGroup.Key, nodeGroup);
            }
            foreach (var group in groups)
            {
                nodeExpanded.Add(group.Value, false);
                foreach (var node in group.Value)
                {
                    nodeExpanded.Add(node, false);
                }
            }

            //this.AddNode(rootNode, null);

        }

        // Token: 0x0600A48C RID: 42124 RVA: 0x0038077C File Offset: 0x0037E97C
        private void AddNode(PawnRenderNode node, PawnRenderNode parent)
        {
            if (parent != null && !this.nodeExpanded.ContainsKey(parent) && parent is PawnRenderNode_Animated)
            {
                this.nodeExpanded.Add(parent, true);
            }
            if (!node.children.NullOrEmpty<PawnRenderNode>())
            {
                foreach (PawnRenderNode node2 in node.children)
                {
                    this.AddNode(node2, node);
                }
            }
        }

        // Token: 0x0600A48D RID: 42125 RVA: 0x003807D8 File Offset: 0x0037E9D8
        public override void WindowUpdate()
        {
            base.WindowUpdate();
            Pawn pawn = Find.Selector.SelectedPawns.FirstOrDefault<Pawn>();
            if (pawn != null && pawn != this.pawn)
            {
                this.Init(pawn);
            }
            this.drawParms.facing = this.pawn.Rotation;
            this.drawParms.posture = this.pawn.GetPosture();
            this.drawParms.bed = this.pawn.CurrentBed();
            this.drawParms.coveredInFoam = this.pawn.Drawer.renderer.FirefoamOverlays.coveredInFoam;
            Pawn_CarryTracker carryTracker = this.pawn.carryTracker;
            this.drawParms.carriedThing = ((carryTracker != null) ? carryTracker.CarriedThing : null);
            this.drawParms.dead = this.pawn.Dead;
            this.drawParms.rotDrawMode = this.pawn.Drawer.renderer.CurRotDrawMode;
        }

        // Token: 0x0600A48E RID: 42126 RVA: 0x003808CD File Offset: 0x0037EACD
        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            this.LeftRect(inRect.LeftHalf());
            this.RightRect(inRect.RightHalf());
        }

        // Token: 0x0600A48F RID: 42127 RVA: 0x003808F0 File Offset: 0x0037EAF0
        private void LeftRect(Rect inRect)
        {
            if (this.pawn == null || this.tree == null || !this.tree.Resolved)
            {
                this.Close(true);
                return;
            }
            Widgets.DrawMenuSection(inRect);
            inRect = inRect.ContractedBy(this.Margin / 2f);
            Widgets.HorizontalSlider(new Rect(inRect.x + this.Margin, inRect.y, inRect.width - this.Margin * 2f, 25f), ref this.alpha, FloatRange.ZeroToOne, "Alpha " + this.alpha.ToStringPercent(), 0.01f);
            this.pawn.Drawer.renderer.renderTree.debugTint = new Color?(Color.white.ToTransparent(this.alpha));
            inRect.yMin += 25f + this.Margin;
            Rect rect = new Rect(inRect.x + this.Margin, inRect.y, "Animation".GetWidthCached() + 4f, 25f);
            using (new TextBlock(GameFont.Tiny, TextAnchor.MiddleLeft))
            {
                Widgets.Label(rect, "Animation");
            }
            Rect rect2 = new Rect(rect.xMax + 4f, rect.y, inRect.width - rect.width - this.Margin * 2f, 25f);
            Widgets.DrawLightHighlight(rect2);
            Widgets.DrawHighlightIfMouseover(rect2);
            using (new TextBlock(TextAnchor.MiddleCenter))
            {
                Widgets.Label(rect2, (this.currentAnimation == null) ? "None" : this.currentAnimation.defName);
            }
            if (Widgets.ButtonInvisible(rect2, true))
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                list.Add(new FloatMenuOption("None", delegate ()
                {
                    this.currentAnimation = null;
                    this.pawn.Drawer.renderer.SetAnimation(null);
                }, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
                foreach (AnimationDef animationDef in DefDatabase<AnimationDef>.AllDefsListForReading)
                {
                    AnimationDef def = animationDef;
                    list.Add(new FloatMenuOption(animationDef.defName, delegate ()
                    {
                        this.currentAnimation = def;
                        this.pawn.Drawer.renderer.SetAnimation(def);
                    }, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
                }
                Find.WindowStack.Add(new FloatMenu(list));
            }
            inRect.yMin += 25f + this.Margin;
            using (new TextBlock(GameFont.Tiny))
            {
                Widgets.CheckboxLabeled(new Rect(inRect.x + this.Margin, inRect.y, inRect.width - this.Margin * 2f, 25f), "Show all nodes", ref this.showAll, false, null, null, false, false);
            }
            inRect.yMin += 25f + this.Margin;
            Widgets.BeginScrollView(inRect, ref this.scrollPosition, new Rect(0f, 0f, inRect.width - 16f, this.scrollHeight), true);
            float num = 0f;
            foreach (var group in groups)
            {
                this.ListNode(group.Value, 0, ref num, inRect.width - 16f);
            }
            if (Event.current.type == EventType.Layout)
            {
                this.scrollHeight = num;
            }
            Widgets.EndScrollView();
        }

        private float previousXOffset, previousYOffset, previousLayer, previousRotation, previousScale;
        private void RightRect(Rect inRect)
        {


            Widgets.DrawMenuSection(inRect);
            inRect = inRect.ContractedBy(Margin / 2f);
            Widgets.BeginGroup(inRect);
            Rect rect = new Rect(0f, 0f, inRect.width, Text.LineHeight);
            Rect rect2 = new Rect(Margin, rect.height + Margin, inRect.width - Margin * 2f, 30f);
            Widgets.DrawLightHighlight(rect);
            if (CurrentNode != null)
            {
                InitCachedPositions();
                float XOffset = previousXOffset, YOffset = previousYOffset, layer = previousLayer, rotation = previousRotation, scale = previousScale;
                using (new TextBlock(TextAnchor.MiddleCenter))
                {
                    Widgets.Label(rect, CurrentNode.ToString().Truncate(inRect.width, null));
                }

                Widgets.Label(rect2, "Tick");
                rect2.y += rect2.height;

                if (Widgets.ButtonText(rect2.LeftPart(1f / 3f), "<"))
                {
                    foreach (var nodeToChange in NodesToChange)
                    {
                        nodeToChange.AnimationState.AnimationTick--;
                    }
                }
                Widgets.Label(rect2.RightHalf().ExpandedBy(6, 0).ContractedBy(0, 4), CurrentNode.AnimationState.AnimationTick.ToString());
                if (Widgets.ButtonText(rect2.RightPart(1f / 3f), ">"))
                {
                    foreach (var nodeToChange in NodesToChange)
                    {
                        nodeToChange.AnimationState.AnimationTick++;
                    }
                }
                rect2.y += Margin;

                rect2.y += rect2.height; Widgets.HorizontalSlider(rect2, ref scale, ScaleRange, "Scale: " + previousScale, 0.01f);
                rect2.y += rect2.height;
                if ((NodesToChange.All(x => x.Props.playOneLine) || NodesToChange.All(x => !x.Props.playOneLine) && NodesToChange.SelectMany(x => x.Props.keyframeLines.Select(line => x.Props.LineId(line))).Distinct().Count() == CurrentNode.Props.keyframeLines.Count))
                {
                    rect2.y += Margin;
                    Widgets.Label(rect2, "Animation type:");
                    rect2.y += rect2.height;
                    foreach (var lineId in NodesToChange.SelectMany(x => x.Props.keyframeLines.Select(line => x.Props.LineId(line))).Distinct())
                    {
                        var optionChecked = NodesToChange.All(x => x.AnimationState.availableLinesIds.Contains(lineId));
                        var playOneLine = NodesToChange.All(x => x.Props.playOneLine);
                        if (NodesToChange.Any(x => x.AnimationState.CurrentLine == x.Props.LineWithId(lineId)))
                        {
                            Widgets.DrawHighlight(rect2);
                        }
                        if (Widgets.ButtonInvisible(rect2))
                        {
                            if (playOneLine)
                            {
                                optionChecked = true;
                            }
                            else
                            {
                                optionChecked = !optionChecked;
                            }


                            foreach (var nodeToChange in NodesToChange)
                            {
                                if (playOneLine)
                                {
                                    nodeToChange.AnimationState.availableLinesIds.Clear();
                                }
                                if (optionChecked)
                                {
                                    nodeToChange.AnimationState.availableLinesIds.Add(lineId);
                                }
                                else if (!playOneLine)
                                {
                                    if (nodeToChange.AnimationState.availableLinesIds.Count <= 1)
                                    {
                                        continue;
                                    }
                                    nodeToChange.AnimationState.availableLinesIds.Remove(lineId);
                                }

                                nodeToChange.AnimationState.CurrentLine = null;
                                if (playOneLine)
                                {
                                    nodeToChange.AnimationState.SetInitialAnimationTick();
                                }
                            }

                        }
                        if (playOneLine)
                        {
                            Widgets.RadioButtonLabeled(rect2, lineId, optionChecked);
                        }
                        else
                        {
                            Widgets.CheckboxLabeled(rect2, lineId, ref optionChecked);
                        }
                        rect2.y += rect2.height;
                    }
                }
                Widgets.Dropdown<object, Rot4>(rect2, this, (arg) => CurrentRot, RotMenuGenerator, CurrentRot.ToStringHuman());
                rect2.y += rect2.height;
                rect2.y += Margin;
                Vector3 vector = new Vector3(previousXOffset, 0, previousYOffset);
                vector.y = CurrentNode.Worker.AltitudeFor(CurrentNode, drawParms);
                float num = CurrentNode.Props.baseLayer;
                if (CurrentNode.Props.drawData != null)
                {
                    num = CurrentNode.Props.drawData.LayerForRot(drawParms.facing, num);
                }
                num += CurrentNode.debugLayerOffset;
                Widgets.Label(rect2, string.Concat(new object[]
                {
                    "Offset ",
                    vector.ToString("F2"),
                    " Layer ",
                    num
                }));
                rect2.y += rect2.height;
                Widgets.HorizontalSlider(rect2, ref XOffset, OffsetRange, "X: " + previousXOffset, 0.05f);
                rect2.y += rect2.height;
                Widgets.HorizontalSlider(rect2, ref YOffset, OffsetRange, "Z: " + previousYOffset, 0.05f);
                rect2.y += rect2.height;
                Widgets.HorizontalSlider(rect2, ref layer, LayerRange, "Layer: " + previousLayer, 1f);
                rect2.y += rect2.height;
                rect2.y += Margin;

                float y = previousRotation;
                Widgets.Label(rect2, "Rotation " + y.ToString("F0") + "°");
                rect2.y += rect2.height;
                Widgets.HorizontalSlider(rect2, ref rotation, AngleRange, "Angle: " + previousRotation.ToString("F0") + "°", 1f);
                rect2.y += rect2.height;
                rect2.y += Margin;
                //Widgets.Label(rect2, "Scale " + previousScale);
                //rect2.y += rect2.height;


                if ((CurrentNode.debugAngleOffset != 0f || CurrentNode.debugScale != 1f || CurrentNode.debugOffset != Vector3.zero || CurrentNode.debugLayerOffset != 0f || !CurrentNode.debugEnabled || CurrentNode.debugPivotOffset != DrawData.PivotCenter) && Widgets.ButtonText(new Rect(0f, inRect.height - 25f, inRect.width, 25f), "Reset", true, true, true, null))
                {
                    CurrentNode.debugAngleOffset = 0f;
                    CurrentNode.debugScale = 1f;
                    CurrentNode.debugOffset = Vector3.zero;
                    CurrentNode.debugLayerOffset = 0f;
                    CurrentNode.debugEnabled = true;
                    CurrentNode.requestRecache = true;
                    CurrentNode.debugPivotOffset = DrawData.PivotCenter;
                }
                foreach (var toChange in NodesToChange)
                {
                    toChange.requestRecache = true;
                    if (previousXOffset != XOffset)
                    {
                        toChange.AnimationState.drawData ??= new DrawDataExposable();
                        toChange.AnimationState.drawData.SetXOffsetFor(CurrentRot, XOffset);
                    }
                    if (previousYOffset != YOffset)
                    {
                        toChange.AnimationState.drawData ??= new DrawDataExposable();
                        toChange.AnimationState.drawData.SetYOffsetFor(CurrentRot, YOffset);
                    }
                    if (previousRotation != rotation)
                    {
                        toChange.AnimationState.drawData ??= new DrawDataExposable();
                        toChange.AnimationState.drawData.SetRotationFor(CurrentRot, rotation);
                    }
                    if (previousLayer != layer)
                    {
                        toChange.AnimationState.drawData ??= new DrawDataExposable();
                        toChange.AnimationState.drawData.SetLayerFor(CurrentRot, layer);
                    }
                    if (previousScale != scale)
                    {
                        toChange.AnimationState.drawData ??= new DrawDataExposable();
                        toChange.AnimationState.drawData.scale = scale;
                    }
                }
                CurrentNode.tree.pawn.Drawer.renderer.EnsureGraphicsInitialized();


            }
            else
            {
                using (new TextBlock(TextAnchor.MiddleCenter))
                {
                    Widgets.Label(rect, "No node selected");
                }
            }
            Widgets.EndGroup();
        }

        private IEnumerable<Widgets.DropdownMenuElement<Rot4>> RotMenuGenerator(object arg)
        {
            foreach (var rot in Rot4.AllRotations)
            {
                var currentRot = rot;
                yield return new Widgets.DropdownMenuElement<Rot4>()
                {
                    option = new FloatMenuOption(rot.ToStringHuman(), () => CurrentRot = currentRot),
                    payload = currentRot
                };
            }
        }

        // Token: 0x0600A491 RID: 42129 RVA: 0x0038139C File Offset: 0x0037F59C
        private void ListNode(object node1, int indent, ref float curY, float width)
        {
            var node = (node1 as PawnRenderNode) ?? (node1 as IEnumerable<PawnRenderNode_Animated>).FirstOrDefault();
            if (!this.showAll && !node.Worker.ShouldListOnGraph(node, this.drawParms))
            {
                return;
            }
            var subnodes = node1 as IEnumerable<PawnRenderNode_Animated> ?? node.children?.OfType<PawnRenderNode_Animated>();
            Rect rect = new Rect((float)(indent + 1) * 12f, curY, width, 25f)
            {
                xMax = width
            }.ContractedBy(2f);
            Widgets.DrawHighlight(rect);
            Widgets.DrawHighlightIfMouseover(rect);
            if (this.currentObject == node1)
            {
                Widgets.DrawHighlight(rect);
            }
            Rect rect2 = new Rect(rect.x, rect.y, rect.height, rect.height);
            Widgets.DrawLightHighlight(rect2);
            if (node.Props.useGraphic)
            {
                Graphic graphic = node.Graphic;
                Texture texture;
                if (graphic == null)
                {
                    texture = null;
                }
                else
                {
                    Material material = graphic.MatAt(Rot4.South, null);
                    texture = ((material != null) ? material.mainTexture : null);
                }
                Texture texture2 = texture;
                if (texture2 != null)
                {
                    GUI.DrawTexture(rect2, texture2);
                }
            }
            Rect rect3 = new Rect(rect2.xMax + 4f, rect.y, rect.width - rect.height - 4f, rect.height);
            if (!node.Worker.CanDrawNow(node, this.drawParms) || (node.Props.useGraphic && node.Graphic == null))
            {
                GUI.color = ColoredText.SubtleGrayColor;
            }
            Widgets.Label(rect3, node.ToString().Truncate(rect3.width, null));
            GUI.color = Color.white;
            if (Widgets.ButtonInvisible(rect, true))
            {
                this.currentObject = node1;
            }
            if (!subnodes.EnumerableNullOrEmpty())
            {
                Rect rect4 = new Rect((float)indent * 12f, curY + 6.5f, 12f, 12f);
                Widgets.DrawHighlightIfMouseover(rect4);
                if (Widgets.ButtonImage(rect4, this.nodeExpanded[node1] ? TexButton.Minus : TexButton.Plus, true, null))
                {
                    this.nodeExpanded[node1] = !this.nodeExpanded[node1];
                }
            }
            curY += 25f;
            if (nodeExpanded[node1])
            {
                if (!subnodes.EnumerableNullOrEmpty())
                {
                    foreach (PawnRenderNode node2 in subnodes)
                    {
                        this.ListNode(node2, indent + 1, ref curY, width);
                    }
                }
            }
        }

        // Token: 0x0600A492 RID: 42130 RVA: 0x003815E0 File Offset: 0x0037F7E0
        protected override void SetInitialSizeAndPosition()
        {
            Vector2 initialSize = this.InitialSize;
            this.windowRect = new Rect(5f, 5f, initialSize.x, initialSize.y).Rounded();
        }
        private void InitCachedPositions()
        {
            if (CurrentNode != null)
            {
                var drawData = CurrentNode.AnimationState.drawData;
                if (drawData == null)
                {
                    drawData = new DrawDataExposable();
                    CurrentNode.AnimationState.drawData = drawData;
                }
                var offset = drawData.OffsetForRot(CurrentRot);
                previousXOffset = offset.x;
                previousYOffset = offset.z;
                previousLayer = drawData.LayerForRot(CurrentRot, CurrentNode.Worker.LayerFor(CurrentNode, drawParms));
                previousRotation = drawData.RotationOffsetForRot(CurrentRot);
                previousScale = drawData.ScaleFor(pawn);
            }
        }

        // Token: 0x04005A85 RID: 23173
        public Pawn pawn;

        // Token: 0x04005A86 RID: 23174
        private PawnRenderTree tree;

        // Token: 0x04005A87 RID: 23175
        private Vector2 scrollPosition;

        // Token: 0x04005A88 RID: 23176
        private float scrollHeight;

        // Token: 0x04005A89 RID: 23177
        private PawnRenderNode_Animated CurrentNode
        {
            get
            {
                if (currentObject is PawnRenderNode_Animated renderNode)
                {
                    return renderNode;
                }
                if (currentObject is IEnumerable<PawnRenderNode_Animated> group)
                {
                    return group.OfType<PawnRenderNode_Animated>().FirstOrDefault();
                }
                return null;
            }
        }

        // Token: 0x04005A8A RID: 23178
        private float alpha = 1f;

        // Token: 0x04005A8B RID: 23179
        private PawnDrawParms drawParms;
        private Dictionary<string, IEnumerable<PawnRenderNode_Animated>> groups = new Dictionary<string, IEnumerable<PawnRenderNode_Animated>>();

        // Token: 0x04005A8C RID: 23180
        private Dictionary<object, bool> nodeExpanded = new Dictionary<object, bool>();

        // Token: 0x04005A8D RID: 23181
        private bool showAll;

        // Token: 0x04005A8E RID: 23182
        private AnimationDef currentAnimation;

        // Token: 0x04005A8F RID: 23183
        private const float NodeHeight = 25f;

        // Token: 0x04005A90 RID: 23184
        private const float IndentWidth = 12f;

        // Token: 0x04005A91 RID: 23185
        private const float IndentHeightOffset = 6.5f;

        // Token: 0x04005A92 RID: 23186
        private static readonly FloatRange AngleRange = new FloatRange(-180f, 180f);

        // Token: 0x04005A93 RID: 23187
        private static readonly FloatRange ScaleRange = new FloatRange(0.01f, 10f);

        // Token: 0x04005A94 RID: 23188
        private static readonly FloatRange OffsetRange = new FloatRange(-1f, 1f);

        // Token: 0x04005A95 RID: 23189
        private static readonly FloatRange LayerRange = new FloatRange(-50f, 100f);
        private object currentObject;
        private Rot4 currentRot;

        private Rot4 CurrentRot
        {
            get => currentRot;
            set
            {
                currentRot = value;
                InitCachedPositions();
            }
        }

    }
}
