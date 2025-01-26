using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using Verse;

namespace Shashlichnik
{
    public class DrawDataExposable : IExposable
    {
        public float ScaleFor(Pawn pawn)
        {
            float num = this.scale;
            return num;
        }

        public Vector3 OffsetForRot(Rot4 rot)
        {
            switch (rot.AsInt)
            {
                case 0:
                    {
                        Vector3? vector = (this.dataNorth != null) ? this.dataNorth.GetValueOrDefault().offset : null;
                        if (vector != null)
                        {
                            return vector.GetValueOrDefault();
                        }
                        Vector3? offset = this.defaultData.offset;
                        if (offset == null)
                        {
                            return Vector3.zero;
                        }
                        return offset.GetValueOrDefault();
                    }
                case 1:
                    {
                        Vector3? vector2 = (this.dataEast != null) ? this.dataEast.GetValueOrDefault().offset : null;
                        if (vector2 != null)
                        {
                            return vector2.Value;
                        }
                        Vector3? vector3 = (this.dataWest != null) ? this.dataWest.GetValueOrDefault().offset : null;
                        if (vector3 != null)
                        {
                            Vector3 value = vector3.Value;
                            value.x *= -1f;
                            return value;
                        }
                        Vector3? offset2 = this.defaultData.offset;
                        if (offset2 != null)
                        {
                            return offset2.Value;
                        }
                        return Vector3.zero;
                    }
                case 2:
                    {
                        Vector3? vector = (this.dataSouth != null) ? this.dataSouth.GetValueOrDefault().offset : null;
                        if (vector != null)
                        {
                            return vector.GetValueOrDefault();
                        }
                        Vector3? offset = this.defaultData.offset;
                        if (offset == null)
                        {
                            return Vector3.zero;
                        }
                        return offset.GetValueOrDefault();
                    }
                case 3:
                    {
                        Vector3? vector4 = (this.dataWest != null) ? this.dataWest.GetValueOrDefault().offset : null;
                        if (vector4 != null)
                        {
                            return vector4.Value;
                        }
                        Vector3? vector5 = (this.dataEast != null) ? this.dataEast.GetValueOrDefault().offset : null;
                        if (vector5 != null)
                        {
                            Vector3 value2 = vector5.Value;
                            value2.x *= -1f;
                            return value2;
                        }
                        Vector3? offset3 = this.defaultData.offset;
                        if (offset3 != null)
                        {
                            return offset3.Value;
                        }
                        Vector3? vector = (this.dataWest != null) ? this.dataWest.GetValueOrDefault().offset : null;
                        if (vector != null)
                        {
                            return vector.GetValueOrDefault();
                        }
                        Vector3? offset = this.defaultData.offset;
                        if (offset == null)
                        {
                            return Vector3.zero;
                        }
                        return offset.GetValueOrDefault();
                    }
                default:
                    {
                        Vector3? vector = this.defaultData.offset;
                        if (vector == null)
                        {
                            return Vector3.zero;
                        }
                        return vector.GetValueOrDefault();
                    }
            }
        }

        public float RotationOffsetForRot(Rot4 rot)
        {
            switch (rot.AsInt)
            {
                case 0:
                    {
                        float? num = (this.dataNorth != null) ? this.dataNorth.GetValueOrDefault().rotationOffset : null;
                        if (num != null)
                        {
                            return num.GetValueOrDefault();
                        }
                        float? rotationOffset = this.defaultData.rotationOffset;
                        if (rotationOffset == null)
                        {
                            return 0f;
                        }
                        return rotationOffset.GetValueOrDefault();
                    }
                case 1:
                    {
                        float? num = (this.dataEast != null) ? this.dataEast.GetValueOrDefault().rotationOffset : null;
                        if (num != null)
                        {
                            return num.GetValueOrDefault();
                        }
                        float? rotationOffset = this.defaultData.rotationOffset;
                        if (rotationOffset == null)
                        {
                            return 0f;
                        }
                        return rotationOffset.GetValueOrDefault();
                    }
                case 2:
                    {
                        float? num = (this.dataSouth != null) ? this.dataSouth.GetValueOrDefault().rotationOffset : null;
                        if (num != null)
                        {
                            return num.GetValueOrDefault();
                        }
                        float? rotationOffset = this.defaultData.rotationOffset;
                        if (rotationOffset == null)
                        {
                            return 0f;
                        }
                        return rotationOffset.GetValueOrDefault();
                    }
                case 3:
                    {
                        float? num = (this.dataWest != null) ? this.dataWest.GetValueOrDefault().rotationOffset : null;
                        if (num != null)
                        {
                            return num.GetValueOrDefault();
                        }
                        float? rotationOffset = this.defaultData.rotationOffset;
                        if (rotationOffset == null)
                        {
                            return 0f;
                        }
                        return rotationOffset.GetValueOrDefault();
                    }
                default:
                    {
                        float? num = this.defaultData.rotationOffset;
                        if (num == null)
                        {
                            return 0f;
                        }
                        return num.GetValueOrDefault();
                    }
            }
        }

        public float LayerForRot(Rot4 rot, float defaultLayer)
        {
            switch (rot.AsInt)
            {
                case 0:
                    {
                        float? num = (this.dataNorth != null) ? this.dataNorth.GetValueOrDefault().layer : null;
                        if (num != null)
                        {
                            return num.GetValueOrDefault();
                        }
                        float? layer = this.defaultData.layer;
                        if (layer == null)
                        {
                            return defaultLayer;
                        }
                        return layer.GetValueOrDefault();
                    }
                case 1:
                    {
                        float? num = (this.dataEast != null) ? this.dataEast.GetValueOrDefault().layer : null;
                        if (num != null)
                        {
                            return num.GetValueOrDefault();
                        }
                        float? layer = this.defaultData.layer;
                        if (layer == null)
                        {
                            return defaultLayer;
                        }
                        return layer.GetValueOrDefault();
                    }
                case 2:
                    {
                        float? num = (this.dataSouth != null) ? this.dataSouth.GetValueOrDefault().layer : null;
                        if (num != null)
                        {
                            return num.GetValueOrDefault();
                        }
                        float? layer = this.defaultData.layer;
                        if (layer == null)
                        {
                            return defaultLayer;
                        }
                        return layer.GetValueOrDefault();
                    }
                case 3:
                    {
                        float? num = (this.dataWest != null) ? this.dataWest.GetValueOrDefault().layer : null;
                        if (num != null)
                        {
                            return num.GetValueOrDefault();
                        }
                        float? layer = this.defaultData.layer;
                        if (layer == null)
                        {
                            return defaultLayer;
                        }
                        return layer.GetValueOrDefault();
                    }
                default:
                    {
                        float? num = this.defaultData.layer;
                        if (num == null)
                        {
                            return defaultLayer;
                        }
                        return num.GetValueOrDefault();
                    }
            }
        }

        public static DrawDataExposable NewWithData(params RotationalData[] data)
        {
            DrawDataExposable drawData = new DrawDataExposable();
            foreach (RotationalData value in data)
            {
                if (value.rotation == null)
                {
                    drawData.defaultData = value;
                }
                else
                {
                    switch (value.rotation.Value.AsInt)
                    {
                        case 0:
                            drawData.dataNorth = new RotationalData?(value);
                            break;
                        case 1:
                            drawData.dataEast = new RotationalData?(value);
                            break;
                        case 2:
                            drawData.dataSouth = new RotationalData?(value);
                            break;
                        case 3:
                            drawData.dataWest = new RotationalData?(value);
                            break;
                    }
                }
            }
            return drawData;
        }

        public void ExposeData()
        {
            Scribe_Deep.Look(ref defaultData, nameof(defaultData));
            Scribe_Deep.Look(ref dataNorth, nameof(dataNorth));
            Scribe_Deep.Look(ref dataEast, nameof(dataEast));
            Scribe_Deep.Look(ref dataSouth, nameof(dataSouth));
            Scribe_Deep.Look(ref dataWest, nameof(dataWest));
            Scribe_Values.Look(ref scale, nameof(scale));
        }

        private RotationalData defaultData;

        private RotationalData? dataNorth;

        private RotationalData? dataEast;

        private RotationalData? dataSouth;

        private RotationalData? dataWest;

        public float scale = 1f;


        public struct RotationalData : IExposable
        {
            public RotationalData(Rot4? rotation, float layer)
            {
                this = default(RotationalData);
                this.rotation = rotation;
                this.layer = new float?(layer);
                this.offset = null;
                this.rotationOffset = null;
            }

            public Rot4? rotation;
            public Vector3? offset;
            public float? rotationOffset;
            public float? layer;

            public void ExposeData()
            {
                Scribe_Values.Look(ref rotation, nameof(rotation));
                Scribe_Values.Look(ref offset, nameof(offset));
                Scribe_Values.Look(ref rotationOffset, nameof(rotationOffset));
                Scribe_Values.Look(ref layer, nameof(layer));
            }
        }

    }
}
