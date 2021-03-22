using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UGUI描边
/// </summary>
[DisallowMultipleComponent]
public class OutlineEx : BaseMeshEffect
{
    private bool IsShaderOutline => ShaderOutline && SystemInfo.graphicsShaderLevel > 35;

    public Material material;
    public Color OutlineColor = Color.white;
    [Range(0, 8)] public float OutlineWidth = 0;
    public bool ShaderOutline = true;

    private float RealOutlineWidth => enabled ? OutlineWidth : 0;
    private static List<UIVertex> m_VetexList = new List<UIVertex>();

    private bool isInit = false;

    private Material _Material
    {
        get
        {
            if (!material)
            {
                var texMaterial = Resources.Load<Material>("OutlineExOpt");
                if (texMaterial)
                {
                    material = texMaterial;
                }
            }

            return material;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        if (!IsShaderOutline)
        {
            var shadows = GetComponents<Shadow>();
            foreach (var shadow in shadows)
            {
                shadow.enabled = false;
            }
        }
    }

    protected override void Start()
    {
        base.Start();

#if UNITY_EDITOR
        var outlineExList = gameObject.GetComponents<OutlineEx>();
        if (outlineExList.Length > 1)
        {
            Debug.LogError("存在多个OutlineEx =====> " + gameObject.name);
        }
#endif

        if (enabled)
        {
            SetOutline();
        }
        else
        {
            CancelOutline();
        }

        isInit = true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

#if UNITY_EDITOR
        SetOutline();
        return;
#endif

        if (isInit)
        {
            SetOutline();
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

#if UNITY_EDITOR
        CancelOutline();
        return;
#endif

        if (isInit)
        {
            CancelOutline();
        }
    }

    private void SetOutline()
    {
        if (!graphic)
        {
            return;
        }

        if (IsShaderOutline)
        {
            if (graphic.material == null || graphic.material.shader.name != "OutlineEx/UI/OutlineExOpt")
            {
                graphic.material = _Material;
            }

            if (graphic.canvas)
            {
                var v1 = graphic.canvas.additionalShaderChannels;
                var v2 = AdditionalCanvasShaderChannels.TexCoord1;
                if ((v1 & v2) != v2)
                {
                    graphic.canvas.additionalShaderChannels |= v2;
                }

                v2 = AdditionalCanvasShaderChannels.TexCoord2;
                if ((v1 & v2) != v2)
                {
                    graphic.canvas.additionalShaderChannels |= v2;
                }

                v2 = AdditionalCanvasShaderChannels.TexCoord3;
                if ((v1 & v2) != v2)
                {
                    graphic.canvas.additionalShaderChannels |= v2;
                }

                v2 = AdditionalCanvasShaderChannels.Tangent;
                if ((v1 & v2) != v2)
                {
                    graphic.canvas.additionalShaderChannels |= v2;
                }

                v2 = AdditionalCanvasShaderChannels.Normal;
                if ((v1 & v2) != v2)
                {
                    graphic.canvas.additionalShaderChannels |= v2;
                }
            }
        }
        else
        {
            CancelOutline();
        }

        Refresh();
    }

    private void CancelOutline()
    {
#if UNITY_EDITOR
        GetComponent<Text>().material = null;
#endif

        if (!graphic)
        {
            return;
        }

        graphic.material = null;
        Refresh();
    }

#if UNITY_EDITOR

    protected override void OnValidate()
    {
        base.OnValidate();

        if (enabled)
        {
            SetOutline();
        }
        else
        {
            CancelOutline();
        }
    }

#endif

    private void Refresh()
    {
        graphic.SetVerticesDirty();
    }


    public override void ModifyMesh(VertexHelper vh)
    {
        if (!gameObject.activeInHierarchy || !enabled)
        {
            return;
        }

        if (IsShaderOutline)
        {
            vh.GetUIVertexStream(m_VetexList);

            ProcessVertices();

            vh.Clear();
            vh.AddUIVertexTriangleStream(m_VetexList);
        }
        else
        {
            this.iVertices.Clear();
            this._ProcessVertices(vh);
        }
    }


    private void ProcessVertices()
    {
        for (int i = 0, count = m_VetexList.Count - 3; i <= count; i += 3)
        {
            var v1 = m_VetexList[i];
            var v2 = m_VetexList[i + 1];
            var v3 = m_VetexList[i + 2];

            // 计算原顶点坐标中心点
            var minX = _Min(v1.position.x, v2.position.x, v3.position.x);
            var minY = _Min(v1.position.y, v2.position.y, v3.position.y);
            var maxX = _Max(v1.position.x, v2.position.x, v3.position.x);
            var maxY = _Max(v1.position.y, v2.position.y, v3.position.y);
            var posCenter = new Vector2(minX + maxX, minY + maxY) * 0.5f;

            // 计算原始顶点坐标和UV的方向
            Vector2 triX, triY, uvX, uvY;
            Vector2 pos1 = v1.position;
            Vector2 pos2 = v2.position;
            Vector2 pos3 = v3.position;

            if (Mathf.Abs(Vector2.Dot((pos2 - pos1).normalized, Vector2.right))
                > Mathf.Abs(Vector2.Dot((pos3 - pos2).normalized, Vector2.right)))
            {
                triX = pos2 - pos1;
                triY = pos3 - pos2;
                uvX = v2.uv0 - v1.uv0;
                uvY = v3.uv0 - v2.uv0;
            }
            else
            {
                triX = pos3 - pos2;
                triY = pos2 - pos1;
                uvX = v3.uv0 - v2.uv0;
                uvY = v2.uv0 - v1.uv0;
            }

            // 计算原始UV框
            var uvMin = _Min(v1.uv0, v2.uv0, v3.uv0);
            var uvMax = _Max(v1.uv0, v2.uv0, v3.uv0);
            int colorV = 100000000;
            int red = (int) (OutlineColor.r * 100);
            red = red > 99 ? 99 : red;
            int green = (int) (OutlineColor.g * 100);
            green = green > 99 ? 99 : green;
            int blue = (int) (OutlineColor.b * 100);
            blue = blue > 99 ? 99 : blue;
            int alpha = (int) (OutlineColor.a * 100);
            alpha = alpha > 99 ? 99 : alpha;
            colorV += red * 1000000 + green * 10000 + blue * 100 + alpha;

//            Debug.Log("colorV = " + colorV);
//            float a = (colorV % 100) * 0.01f;
//            float b = (colorV % 10000 / 100) * 0.01f;
//            float g = (colorV % 1000000 / 10000) * 0.01f;
//            float r = (colorV % 100000000 / 1000000) * 0.01f;
//            Debug.Log($"r = {r} | g = {g} | b = {b} | a = {a}");

            //将rgba组合成一个float传入uv3 然后shader那边解析 可以不再占用normal和tangent，解决在放大和旋转时产生的问题
            var col_rg = new Vector2(colorV, RealOutlineWidth);

            // 为每个顶点设置新的Position和UV，并传入原始UV框
            v1 = _SetNewPosAndUV(v1, this.RealOutlineWidth, posCenter, triX, triY, uvX, uvY, uvMin, uvMax);
            v1.uv3 = col_rg;
            v2 = _SetNewPosAndUV(v2, this.RealOutlineWidth, posCenter, triX, triY, uvX, uvY, uvMin, uvMax);
            v2.uv3 = col_rg;
            v3 = _SetNewPosAndUV(v3, this.RealOutlineWidth, posCenter, triX, triY, uvX, uvY, uvMin, uvMax);
            v3.uv3 = col_rg;


            // 应用设置后的UIVertex
            m_VetexList[i] = v1;
            m_VetexList[i + 1] = v2;
            m_VetexList[i + 2] = v3;
        }
    }

    private static UIVertex _SetNewPosAndUV(UIVertex pVertex, float pOutLineWidth,
        Vector2 pPosCenter,
        Vector2 pTriangleX, Vector2 pTriangleY,
        Vector2 pUVX, Vector2 pUVY,
        Vector2 pUVOriginMin, Vector2 pUVOriginMax)
    {
        // Position
        var pos = pVertex.position;
        var posXOffset = pos.x > pPosCenter.x ? pOutLineWidth : -pOutLineWidth;
        var posYOffset = pos.y > pPosCenter.y ? pOutLineWidth : -pOutLineWidth;
        pos.x += posXOffset;
        pos.y += posYOffset;
        pVertex.position = pos;
        // UV
        var uv = pVertex.uv0;
        uv += pUVX / pTriangleX.magnitude * posXOffset * (Vector2.Dot(pTriangleX, Vector2.right) > 0 ? 1 : -1);
        uv += pUVY / pTriangleY.magnitude * posYOffset * (Vector2.Dot(pTriangleY, Vector2.up) > 0 ? 1 : -1);
        pVertex.uv0 = uv;

        pVertex.uv1 = pUVOriginMin;
        pVertex.uv2 = pUVOriginMax;

        return pVertex;
    }


    private static float _Min(float pA, float pB, float pC)
    {
        return Mathf.Min(Mathf.Min(pA, pB), pC);
    }


    private static float _Max(float pA, float pB, float pC)
    {
        return Mathf.Max(Mathf.Max(pA, pB), pC);
    }


    private static Vector2 _Min(Vector2 pA, Vector2 pB, Vector2 pC)
    {
        return new Vector2(_Min(pA.x, pB.x, pC.x), _Min(pA.y, pB.y, pC.y));
    }


    private static Vector2 _Max(Vector2 pA, Vector2 pB, Vector2 pC)
    {
        return new Vector2(_Max(pA.x, pB.x, pC.x), _Max(pA.y, pB.y, pC.y));
    }


    ///////////////////////////////////////////////////////////////////////////////////


    private List<UIVertex> iVertices = new List<UIVertex>();
    private Vector3[] m_OutLineDis = new Vector3[8];
    private Color curColor = new Color();

    private void _ProcessVertices(VertexHelper vh)
    {
        var count = vh.currentVertCount;
        if (count == 0)
            return;

        float textAlpha = graphic.color.a;
        curColor.r = OutlineColor.r;
        curColor.g = OutlineColor.g;
        curColor.b = OutlineColor.b;
        curColor.a = textAlpha * textAlpha * OutlineColor.a;

        /*
         *  TL--------TR
         *  |          |^
         *  |          ||
         *  CL--------CR
         *  |          ||
         *  |          |v
         *  BL--------BR
         * **/


        for (int i = 0; i < count; i++)
        {
            UIVertex vertex = UIVertex.simpleVert;
            vh.PopulateUIVertex(ref vertex, i);
            this.iVertices.Add(vertex);
        }

        vh.Clear();

        for (int i = 0; i < this.iVertices.Count; i += 4)
        {
            UIVertex TL = GeneralUIVertex(this.iVertices[i + 0]);
            UIVertex TR = GeneralUIVertex(this.iVertices[i + 1]);
            UIVertex BR = GeneralUIVertex(this.iVertices[i + 2]);
            UIVertex BL = GeneralUIVertex(this.iVertices[i + 3]);

            this.m_OutLineDis[0].Set(-this.OutlineWidth, this.OutlineWidth, 0); //LT
            this.m_OutLineDis[1].Set(this.OutlineWidth, this.OutlineWidth, 0); //RT
            this.m_OutLineDis[2].Set(-this.OutlineWidth, -this.OutlineWidth, 0); //LB
            this.m_OutLineDis[3].Set(this.OutlineWidth, -this.OutlineWidth, 0); //RB

            this.m_OutLineDis[4].Set(this.OutlineWidth, 0, 0); //R
            this.m_OutLineDis[5].Set(-this.OutlineWidth, 0, 0); //L
            this.m_OutLineDis[6].Set(0, this.OutlineWidth, 0); //T
            this.m_OutLineDis[7].Set(0, -this.OutlineWidth, 0); //B

            for (int j = 0; j < 8; j++)
            {
                //四个方向
                UIVertex o_TL = GeneralUIVertex(TL);
                UIVertex o_TR = GeneralUIVertex(TR);
                UIVertex o_BR = GeneralUIVertex(BR);
                UIVertex o_BL = GeneralUIVertex(BL);

                o_TL.position += this.m_OutLineDis[j];
                o_TR.position += this.m_OutLineDis[j];
                o_BR.position += this.m_OutLineDis[j];
                o_BL.position += this.m_OutLineDis[j];

                o_TL.color = this.curColor;
                o_TR.color = this.curColor;
                o_BR.color = this.curColor;
                o_BL.color = this.curColor;

                vh.AddVert(o_TL);
                vh.AddVert(o_TR);

                vh.AddVert(o_BR);
                vh.AddVert(o_BL);
            }

            vh.AddVert(TL);
            vh.AddVert(TR);

            vh.AddVert(BR);
            vh.AddVert(BL);
        }

        for (int i = 0; i < vh.currentVertCount; i += 4)
        {
            vh.AddTriangle(i + 0, i + 1, i + 2);
            vh.AddTriangle(i + 2, i + 3, i + 0);
        }
    }

    private static UIVertex GeneralUIVertex(UIVertex vertex)
    {
        UIVertex result = UIVertex.simpleVert;
        result.normal = new Vector3(vertex.normal.x, vertex.normal.y, vertex.normal.z);
        result.position = new Vector3(vertex.position.x, vertex.position.y, vertex.position.z);
        result.tangent = new Vector4(vertex.tangent.x, vertex.tangent.y, vertex.tangent.z, vertex.tangent.w);
        result.uv0 = new Vector2(vertex.uv0.x, vertex.uv0.y);
        result.uv1 = new Vector2(vertex.uv1.x, vertex.uv1.y);
        result.color = vertex.color;
        return result;
    }
}