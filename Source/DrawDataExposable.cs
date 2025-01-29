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
                        Vector3? vector = (this.dataNorth != null) ? this.dataNorth.offset : null;
                        if (vector != null)
                        {
                            return vector.GetValueOrDefault();
                        }
                        return Vector3.zero;

                    }
                case 1:
                    {
                        Vector3? vector2 = (this.dataEast != null) ? this.dataEast.offset : null;
                        if (vector2 != null)
                        {
                            return vector2.Value;
                        }
                        Vector3? vector3 = (this.dataWest != null) ? this.dataWest.offset : null;
                        if (vector3 != null)
                        {
                            Vector3 value = vector3.Value;
                            value.x *= -1f;
                            return value;
                        }
                        return Vector3.zero;
                    }
                case 2:
                    {
                        Vector3? vector = (this.dataSouth != null) ? this.dataSouth.offset : null;
                        if (vector != null)
                        {
                            return vector.GetValueOrDefault();
                        }
                        return Vector3.zero;

                    }
                case 3:
                    {
                        Vector3? vector4 = (this.dataWest != null) ? this.dataWest.offset : null;
                        if (vector4 != null)
                        {
                            return vector4.Value;
                        }
                        Vector3? vector5 = (this.dataEast != null) ? this.dataEast.offset : null;
                        if (vector5 != null)
                        {
                            Vector3 value2 = vector5.Value;
                            value2.x *= -1f;
                            return value2;
                        }
                        Vector3? vector = (this.dataWest != null) ? this.dataWest.offset : null;
                        if (vector != null)
                        {
                            return vector.GetValueOrDefault();
                        }
                        return Vector3.zero;
                    }
                default:
                    {
                        return Vector3.zero;
                    }
            }
        }

        public float RotationOffsetForRot(Rot4 rot)
        {
            switch (rot.AsInt)
            {
                case 0:
                    {
                        float? num = (this.dataNorth != null) ? this.dataNorth.rotationOffset : null;
                        if (num != null)
                        {
                            return num.GetValueOrDefault();
                        }
                        return 0f;
                    }
                case 1:
                    {
                        float? num = (this.dataEast != null) ? this.dataEast.rotationOffset : null;
                        if (num != null)
                        {
                            return num.GetValueOrDefault();
                        }
                        return 0f;
                    }
                case 2:
                    {
                        float? num = (this.dataSouth != null) ? this.dataSouth.rotationOffset : null;
                        if (num != null)
                        {
                            return num.GetValueOrDefault();
                        }
                        return 0f;
                    }
                case 3:
                    {
                        float? num = (this.dataWest != null) ? this.dataWest.rotationOffset : null;
                        if (num != null)
                        {
                            return num.GetValueOrDefault();
                        }
                        return 0f;
                    }
                default:
                    {
                        return 0f;
                    }
            }
        }

        public float LayerForRot(Rot4 rot, float defaultLayer)
        {
            switch (rot.AsInt)
            {
                case 0:
                    {
                        float? num = (this.dataNorth != null) ? this.dataNorth.layer : null;
                        if (num != null)
                        {
                            return num.GetValueOrDefault();
                        }
                        return defaultLayer;
                    }
                case 1:
                    {
                        float? num = (this.dataEast != null) ? this.dataEast.layer : null;
                        if (num != null)
                        {
                            return num.GetValueOrDefault();
                        }
                        return defaultLayer;
                    }
                case 2:
                    {
                        float? num = (this.dataSouth != null) ? this.dataSouth.layer : null;
                        if (num != null)
                        {
                            return num.GetValueOrDefault();
                        }
                        return defaultLayer;
                    }
                case 3:
                    {
                        float? num = (this.dataWest != null) ? this.dataWest.layer : null;
                        if (num != null)
                        {
                            return num.GetValueOrDefault();
                        }
                        return defaultLayer;
                    }
                default:
                    {
                        return defaultLayer;
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
                    
                }
                else
                {
                    switch (value.rotation.Value.AsInt)
                    {
                        case 0:
                            drawData.dataNorth = value;
                            break;
                        case 1:
                            drawData.dataEast = value;
                            break;
                        case 2:
                            drawData.dataSouth = value;
                            break;
                        case 3:
                            drawData.dataWest = value;
                            break;
                    }
                }
            }
            return drawData;
        }

        public void ExposeData()
        {
            Scribe_Deep.Look(ref dataNorth, nameof(dataNorth));
            Scribe_Deep.Look(ref dataEast, nameof(dataEast));
            Scribe_Deep.Look(ref dataSouth, nameof(dataSouth));
            Scribe_Deep.Look(ref dataWest, nameof(dataWest));
            Scribe_Values.Look(ref scale, nameof(scale));
        }
        public RotationalData RotationalDataFor(Rot4 rot, bool createIfNull = true)
        {
            switch (rot.AsInt)
            {
                case 0:
                    if (createIfNull && dataNorth == null)
                    {
                        dataNorth = new RotationalData(rot, null);
                    }
                    return dataNorth;
                case 1:
                    if (createIfNull && dataEast == null)
                    {
                        dataEast = new RotationalData(rot, null);
                    }
                    return dataEast;
                case 2:
                    if (createIfNull && dataSouth == null)
                    {
                        dataSouth = new RotationalData(rot, null);
                    }
                    return dataSouth;
                case 3:
                    if (createIfNull && dataWest == null)
                    {
                        dataWest = new RotationalData(rot, null);
                    }
                    return dataWest;
                default:
                    return null;
            }
        }
        public void SetXOffsetFor(Rot4 rot, float offset)
        {
            var drawData = RotationalDataFor(rot);
            var v = new Vector3(offset, 0, 0);
            if (drawData.offset == null)
            {
                drawData.offset = v;
            }
            else
            {
                drawData.offset = new Vector3(offset, drawData.offset.Value.y, drawData.offset.Value.z);
            }
        }
        public void SetYOffsetFor(Rot4 rot, float offset)
        {
            var drawData = RotationalDataFor(rot);
            var v = new Vector3(0, 0, offset);
            if (drawData.offset == null)
            {
                drawData.offset = v;
            }
            else
            {
                drawData.offset = new Vector3(drawData.offset.Value.x, drawData.offset.Value.y, offset);
            }
        }
        public void SetRotationFor(Rot4 rot, float offset)
        {
            var drawData = RotationalDataFor(rot);
            drawData.rotationOffset = offset;
        }

        public void SetLayerFor(Rot4 rot, float layer)
        {
            var drawData = RotationalDataFor(rot);
            drawData.layer = layer;
        }

        internal RotationalData dataNorth;

        internal RotationalData dataEast;

        internal RotationalData dataSouth;

        internal RotationalData dataWest;

        public float scale = 1f;


        public class RotationalData : IExposable
        {
            public RotationalData()
            {

            }
            public RotationalData(Rot4? rotation, float? layer = null)
            {
                this.rotation = rotation;
                this.layer = layer;
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
