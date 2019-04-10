// Decompiled with JetBrains decompiler
// Type: ReflectionProbeEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class ReflectionProbeEx : MonoBehaviour
{
  private static float[] octaVerts = new float[72]
  {
    0.0f,
    1f,
    0.0f,
    0.0f,
    0.0f,
    -1f,
    1f,
    0.0f,
    0.0f,
    0.0f,
    1f,
    0.0f,
    1f,
    0.0f,
    0.0f,
    0.0f,
    0.0f,
    1f,
    0.0f,
    1f,
    0.0f,
    0.0f,
    0.0f,
    1f,
    -1f,
    0.0f,
    0.0f,
    0.0f,
    1f,
    0.0f,
    -1f,
    0.0f,
    0.0f,
    0.0f,
    0.0f,
    -1f,
    0.0f,
    -1f,
    0.0f,
    1f,
    0.0f,
    0.0f,
    0.0f,
    0.0f,
    -1f,
    0.0f,
    -1f,
    0.0f,
    0.0f,
    0.0f,
    1f,
    1f,
    0.0f,
    0.0f,
    0.0f,
    -1f,
    0.0f,
    -1f,
    0.0f,
    0.0f,
    0.0f,
    0.0f,
    1f,
    0.0f,
    -1f,
    0.0f,
    0.0f,
    0.0f,
    -1f,
    -1f,
    0.0f,
    0.0f
  };
  private static readonly ReflectionProbeEx.CubemapFaceMatrices[] cubemapFaceMatrices = new ReflectionProbeEx.CubemapFaceMatrices[6]
  {
    new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0.0f, 0.0f, -1f), new Vector3(0.0f, -1f, 0.0f), new Vector3(-1f, 0.0f, 0.0f)),
    new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0.0f, 0.0f, 1f), new Vector3(0.0f, -1f, 0.0f), new Vector3(1f, 0.0f, 0.0f)),
    new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1f), new Vector3(0.0f, -1f, 0.0f)),
    new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, -1f), new Vector3(0.0f, 1f, 0.0f)),
    new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0.0f, 0.0f), new Vector3(0.0f, -1f, 0.0f), new Vector3(0.0f, 0.0f, -1f)),
    new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(-1f, 0.0f, 0.0f), new Vector3(0.0f, -1f, 0.0f), new Vector3(0.0f, 0.0f, 1f))
  };
  private static readonly ReflectionProbeEx.CubemapFaceMatrices[] cubemapFaceMatricesD3D11 = new ReflectionProbeEx.CubemapFaceMatrices[6]
  {
    new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0.0f, 0.0f, -1f), new Vector3(0.0f, 1f, 0.0f), new Vector3(-1f, 0.0f, 0.0f)),
    new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0.0f, 0.0f, 1f), new Vector3(0.0f, 1f, 0.0f), new Vector3(1f, 0.0f, 0.0f)),
    new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, -1f), new Vector3(0.0f, -1f, 0.0f)),
    new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1f), new Vector3(0.0f, 1f, 0.0f)),
    new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0.0f, 0.0f), new Vector3(0.0f, 1f, 0.0f), new Vector3(0.0f, 0.0f, -1f)),
    new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(-1f, 0.0f, 0.0f), new Vector3(0.0f, 1f, 0.0f), new Vector3(0.0f, 0.0f, 1f))
  };
  private static readonly ReflectionProbeEx.CubemapFaceMatrices[] shadowCubemapFaceMatrices = new ReflectionProbeEx.CubemapFaceMatrices[6]
  {
    new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0.0f, 0.0f, 1f), new Vector3(0.0f, -1f, 0.0f), new Vector3(-1f, 0.0f, 0.0f)),
    new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0.0f, 0.0f, -1f), new Vector3(0.0f, -1f, 0.0f), new Vector3(1f, 0.0f, 0.0f)),
    new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1f), new Vector3(0.0f, 1f, 0.0f)),
    new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, -1f), new Vector3(0.0f, -1f, 0.0f)),
    new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0.0f, 0.0f), new Vector3(0.0f, -1f, 0.0f), new Vector3(0.0f, 0.0f, 1f)),
    new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(-1f, 0.0f, 0.0f), new Vector3(0.0f, -1f, 0.0f), new Vector3(0.0f, 0.0f, -1f))
  };
  private static readonly int[] tab32 = new int[32]
  {
    0,
    9,
    1,
    10,
    13,
    21,
    2,
    29,
    11,
    14,
    16,
    18,
    22,
    25,
    3,
    30,
    8,
    12,
    20,
    28,
    15,
    17,
    24,
    7,
    19,
    27,
    23,
    6,
    26,
    5,
    4,
    31
  };
  public ReflectionProbeRefreshMode refreshMode;
  public bool timeSlicing;
  public int resolution;
  [InspectorName("HDR")]
  public bool hdr;
  public float shadowDistance;
  public ReflectionProbeClearFlags clearFlags;
  public Color background;
  public float nearClip;
  public float farClip;
  public Transform attachToTarget;
  public Light directionalLight;
  public float textureMipBias;
  public bool highPrecision;
  public bool enableShadows;
  public ReflectionProbeEx.ConvolutionQuality convolutionQuality;
  public List<ReflectionProbeEx.RenderListEntry> staticRenderList;
  public Cubemap reflectionCubemap;
  public float reflectionIntensity;
  private Mesh blitMesh;
  private Mesh skyboxMesh;
  private ReflectionProbeEx.CubemapFaceMatrices[] platformCubemapFaceMatrices;

  private void CreateMeshes()
  {
    if (Object.op_Equality((Object) this.blitMesh, (Object) null))
      this.blitMesh = ReflectionProbeEx.CreateBlitMesh();
    if (!Object.op_Equality((Object) this.skyboxMesh, (Object) null))
      return;
    this.skyboxMesh = ReflectionProbeEx.CreateSkyboxMesh();
  }

  private void DestroyMeshes()
  {
    if (Object.op_Inequality((Object) this.blitMesh, (Object) null))
    {
      Object.DestroyImmediate((Object) this.blitMesh);
      this.blitMesh = (Mesh) null;
    }
    if (!Object.op_Inequality((Object) this.skyboxMesh, (Object) null))
      return;
    Object.DestroyImmediate((Object) this.skyboxMesh);
    this.skyboxMesh = (Mesh) null;
  }

  private static Mesh CreateBlitMesh()
  {
    Mesh mesh = new Mesh();
    mesh.set_vertices(new Vector3[4]
    {
      new Vector3(-1f, -1f, 0.0f),
      new Vector3(-1f, 1f, 0.0f),
      new Vector3(1f, 1f, 0.0f),
      new Vector3(1f, -1f, 0.0f)
    });
    mesh.set_uv(new Vector2[4]
    {
      new Vector2(0.0f, 0.0f),
      new Vector2(0.0f, 1f),
      new Vector2(1f, 1f),
      new Vector2(1f, 0.0f)
    });
    mesh.set_triangles(new int[6]{ 0, 1, 2, 0, 2, 3 });
    return mesh;
  }

  private static ReflectionProbeEx.CubemapSkyboxVertex SubDivVert(
    ReflectionProbeEx.CubemapSkyboxVertex v1,
    ReflectionProbeEx.CubemapSkyboxVertex v2)
  {
    Vector3 vector3_1 = new Vector3(v1.x, v1.y, v1.z);
    Vector3 vector3_2;
    ((Vector3) ref vector3_2).\u002Ector(v2.x, v2.y, v2.z);
    Vector3 vector3_3 = vector3_2;
    Vector3 vector3_4 = Vector3.Normalize(Vector3.Lerp(vector3_1, vector3_3, 0.5f));
    ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex;
    cubemapSkyboxVertex.x = cubemapSkyboxVertex.tu = (float) vector3_4.x;
    cubemapSkyboxVertex.y = cubemapSkyboxVertex.tv = (float) vector3_4.y;
    cubemapSkyboxVertex.z = cubemapSkyboxVertex.tw = (float) vector3_4.z;
    cubemapSkyboxVertex.color = Color.get_white();
    return cubemapSkyboxVertex;
  }

  private static void Subdivide(
    List<ReflectionProbeEx.CubemapSkyboxVertex> destArray,
    ReflectionProbeEx.CubemapSkyboxVertex v1,
    ReflectionProbeEx.CubemapSkyboxVertex v2,
    ReflectionProbeEx.CubemapSkyboxVertex v3)
  {
    ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex1 = ReflectionProbeEx.SubDivVert(v1, v2);
    ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex2 = ReflectionProbeEx.SubDivVert(v2, v3);
    ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex3 = ReflectionProbeEx.SubDivVert(v1, v3);
    destArray.Add(v1);
    destArray.Add(cubemapSkyboxVertex1);
    destArray.Add(cubemapSkyboxVertex3);
    destArray.Add(cubemapSkyboxVertex1);
    destArray.Add(v2);
    destArray.Add(cubemapSkyboxVertex2);
    destArray.Add(cubemapSkyboxVertex2);
    destArray.Add(cubemapSkyboxVertex3);
    destArray.Add(cubemapSkyboxVertex1);
    destArray.Add(v3);
    destArray.Add(cubemapSkyboxVertex3);
    destArray.Add(cubemapSkyboxVertex2);
  }

  private static void SubdivideYOnly(
    List<ReflectionProbeEx.CubemapSkyboxVertex> destArray,
    ReflectionProbeEx.CubemapSkyboxVertex v1,
    ReflectionProbeEx.CubemapSkyboxVertex v2,
    ReflectionProbeEx.CubemapSkyboxVertex v3)
  {
    float num1 = Mathf.Abs(v2.y - v1.y);
    float num2 = Mathf.Abs(v2.y - v3.y);
    float num3 = Mathf.Abs(v3.y - v1.y);
    ReflectionProbeEx.CubemapSkyboxVertex v1_1;
    ReflectionProbeEx.CubemapSkyboxVertex v2_1;
    ReflectionProbeEx.CubemapSkyboxVertex v2_2;
    if ((double) num1 < (double) num2 && (double) num1 < (double) num3)
    {
      v1_1 = v3;
      v2_1 = v1;
      v2_2 = v2;
    }
    else if ((double) num2 < (double) num1 && (double) num2 < (double) num3)
    {
      v1_1 = v1;
      v2_1 = v2;
      v2_2 = v3;
    }
    else
    {
      v1_1 = v2;
      v2_1 = v3;
      v2_2 = v1;
    }
    ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex1 = ReflectionProbeEx.SubDivVert(v1_1, v2_1);
    ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex2 = ReflectionProbeEx.SubDivVert(v1_1, v2_2);
    destArray.Add(v1_1);
    destArray.Add(cubemapSkyboxVertex1);
    destArray.Add(cubemapSkyboxVertex2);
    Vector3 vector3_1;
    ((Vector3) ref vector3_1).\u002Ector(cubemapSkyboxVertex2.x - v2_1.x, cubemapSkyboxVertex2.y - v2_1.y, cubemapSkyboxVertex2.z - v2_1.z);
    Vector3 vector3_2;
    ((Vector3) ref vector3_2).\u002Ector(cubemapSkyboxVertex1.x - v2_2.x, cubemapSkyboxVertex1.y - v2_2.y, cubemapSkyboxVertex1.z - v2_2.z);
    if (vector3_1.x * vector3_1.x + vector3_1.y * vector3_1.y + vector3_1.z * vector3_1.z > vector3_2.x * vector3_2.x + vector3_2.y * vector3_2.y + vector3_2.z * vector3_2.z)
    {
      destArray.Add(cubemapSkyboxVertex1);
      destArray.Add(v2_1);
      destArray.Add(v2_2);
      destArray.Add(cubemapSkyboxVertex2);
      destArray.Add(cubemapSkyboxVertex1);
      destArray.Add(v2_2);
    }
    else
    {
      destArray.Add(cubemapSkyboxVertex2);
      destArray.Add(cubemapSkyboxVertex1);
      destArray.Add(v2_1);
      destArray.Add(cubemapSkyboxVertex2);
      destArray.Add(v2_1);
      destArray.Add(v2_2);
    }
  }

  private static Mesh CreateSkyboxMesh()
  {
    List<ReflectionProbeEx.CubemapSkyboxVertex> destArray = new List<ReflectionProbeEx.CubemapSkyboxVertex>();
    for (int index = 0; index < 24; ++index)
    {
      ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex = new ReflectionProbeEx.CubemapSkyboxVertex();
      Vector3 vector3 = Vector3.Normalize(new Vector3(ReflectionProbeEx.octaVerts[index * 3], ReflectionProbeEx.octaVerts[index * 3 + 1], ReflectionProbeEx.octaVerts[index * 3 + 2]));
      cubemapSkyboxVertex.x = cubemapSkyboxVertex.tu = (float) vector3.x;
      cubemapSkyboxVertex.y = cubemapSkyboxVertex.tv = (float) vector3.y;
      cubemapSkyboxVertex.z = cubemapSkyboxVertex.tw = (float) vector3.z;
      cubemapSkyboxVertex.color = Color.get_white();
      destArray.Add(cubemapSkyboxVertex);
    }
    for (int index1 = 0; index1 < 3; ++index1)
    {
      List<ReflectionProbeEx.CubemapSkyboxVertex> cubemapSkyboxVertexList = new List<ReflectionProbeEx.CubemapSkyboxVertex>(destArray.Count);
      cubemapSkyboxVertexList.AddRange((IEnumerable<ReflectionProbeEx.CubemapSkyboxVertex>) destArray);
      int count = cubemapSkyboxVertexList.Count;
      destArray.Clear();
      destArray.Capacity = count * 4;
      for (int index2 = 0; index2 < count; index2 += 3)
        ReflectionProbeEx.Subdivide(destArray, cubemapSkyboxVertexList[index2], cubemapSkyboxVertexList[index2 + 1], cubemapSkyboxVertexList[index2 + 2]);
    }
    for (int index1 = 0; index1 < 2; ++index1)
    {
      List<ReflectionProbeEx.CubemapSkyboxVertex> cubemapSkyboxVertexList = new List<ReflectionProbeEx.CubemapSkyboxVertex>(destArray.Count);
      cubemapSkyboxVertexList.AddRange((IEnumerable<ReflectionProbeEx.CubemapSkyboxVertex>) destArray);
      int count = cubemapSkyboxVertexList.Count;
      float num = Mathf.Pow(0.5f, (float) index1 + 1f);
      destArray.Clear();
      destArray.Capacity = count * 4;
      for (int index2 = 0; index2 < count; index2 += 3)
      {
        if ((double) Mathf.Max(Mathf.Max(Mathf.Abs(cubemapSkyboxVertexList[index2].y), Mathf.Abs(cubemapSkyboxVertexList[index2 + 1].y)), Mathf.Abs(cubemapSkyboxVertexList[index2 + 2].y)) > (double) num)
        {
          destArray.Add(cubemapSkyboxVertexList[index2]);
          destArray.Add(cubemapSkyboxVertexList[index2 + 1]);
          destArray.Add(cubemapSkyboxVertexList[index2 + 2]);
        }
        else
          ReflectionProbeEx.SubdivideYOnly(destArray, cubemapSkyboxVertexList[index2], cubemapSkyboxVertexList[index2 + 1], cubemapSkyboxVertexList[index2 + 2]);
      }
    }
    Mesh mesh = new Mesh();
    Vector3[] vector3Array = new Vector3[destArray.Count];
    Vector2[] vector2Array = new Vector2[destArray.Count];
    int[] numArray = new int[destArray.Count];
    for (int index = 0; index < destArray.Count; ++index)
    {
      vector3Array[index] = new Vector3(destArray[index].x, destArray[index].y, destArray[index].z);
      vector2Array[index] = Vector2.op_Implicit(new Vector3(destArray[index].tu, destArray[index].tv));
      numArray[index] = index;
    }
    mesh.set_vertices(vector3Array);
    mesh.set_uv(vector2Array);
    mesh.set_triangles(numArray);
    return mesh;
  }

  private bool InitializeCubemapFaceMatrices()
  {
    GraphicsDeviceType graphicsDeviceType = SystemInfo.get_graphicsDeviceType();
    if (graphicsDeviceType != 2)
    {
      switch (graphicsDeviceType - 16)
      {
        case 0:
          this.platformCubemapFaceMatrices = ReflectionProbeEx.cubemapFaceMatricesD3D11;
          break;
        case 1:
          this.platformCubemapFaceMatrices = ReflectionProbeEx.cubemapFaceMatrices;
          break;
        case 2:
          this.platformCubemapFaceMatrices = ReflectionProbeEx.cubemapFaceMatricesD3D11;
          break;
        case 5:
          this.platformCubemapFaceMatrices = ReflectionProbeEx.cubemapFaceMatricesD3D11;
          break;
        default:
          this.platformCubemapFaceMatrices = (ReflectionProbeEx.CubemapFaceMatrices[]) null;
          break;
      }
    }
    else
      this.platformCubemapFaceMatrices = ReflectionProbeEx.cubemapFaceMatricesD3D11;
    if (this.platformCubemapFaceMatrices != null)
      return true;
    Debug.LogError((object) ("[ReflectionProbeEx] Initialization failed. No cubemap ortho basis defined for " + (object) SystemInfo.get_graphicsDeviceType()));
    return false;
  }

  private int FastLog2(int value)
  {
    value |= value >> 1;
    value |= value >> 2;
    value |= value >> 4;
    value |= value >> 8;
    value |= value >> 16;
    return ReflectionProbeEx.tab32[(int) ((uint) ((ulong) value * 130329821UL) >> 27)];
  }

  private uint ReverseBits(uint bits)
  {
    bits = bits << 16 | bits >> 16;
    bits = (uint) (((int) bits & 16711935) << 8) | (bits & 4278255360U) >> 8;
    bits = (uint) (((int) bits & 252645135) << 4) | (bits & 4042322160U) >> 4;
    bits = (uint) (((int) bits & 858993459) << 2) | (bits & 3435973836U) >> 2;
    bits = (uint) (((int) bits & 1431655765) << 1) | (bits & 2863311530U) >> 1;
    return bits;
  }

  private void SafeCreateMaterial(ref Material mat, Shader shader)
  {
    if (!Object.op_Equality((Object) mat, (Object) null))
      return;
    mat = new Material(shader);
  }

  private void SafeCreateMaterial(ref Material mat, string shaderName)
  {
    if (!Object.op_Equality((Object) mat, (Object) null))
      return;
    this.SafeCreateMaterial(ref mat, Shader.Find(shaderName));
  }

  private void SafeCreateCubeRT(
    ref RenderTexture rt,
    string name,
    int size,
    int depth,
    bool mips,
    TextureDimension dim,
    FilterMode filter,
    RenderTextureFormat format,
    RenderTextureReadWrite readWrite = 1)
  {
    if (!Object.op_Equality((Object) rt, (Object) null) && rt.IsCreated())
      return;
    this.SafeDestroy<RenderTexture>(ref rt);
    ref RenderTexture local = ref rt;
    RenderTexture renderTexture = new RenderTexture(size, size, depth, format, readWrite);
    ((Object) renderTexture).set_hideFlags((HideFlags) 52);
    local = renderTexture;
    ((Object) rt).set_name(name);
    ((Texture) rt).set_dimension(dim);
    if (dim == 5)
      rt.set_volumeDepth(6);
    rt.set_useMipMap(mips);
    rt.set_autoGenerateMips(false);
    ((Texture) rt).set_filterMode(filter);
    ((Texture) rt).set_anisoLevel(0);
    rt.Create();
  }

  private void SafeCreateCB(ref CommandBuffer cb, string name)
  {
    if (cb != null)
      return;
    cb = new CommandBuffer();
    cb.set_name(name);
  }

  private void SafeDestroy<T>(ref T obj) where T : Object
  {
    if (!Object.op_Inequality((Object) (object) obj, (Object) null))
      return;
    Object.DestroyImmediate((Object) (object) obj);
    obj = default (T);
  }

  private void SafeDispose<T>(ref T obj) where T : IDisposable
  {
    if ((object) obj == null)
      return;
    obj.Dispose();
    obj = default (T);
  }

  public ReflectionProbeEx()
  {
    base.\u002Ector();
  }

  [Serializable]
  public enum ConvolutionQuality
  {
    Lowest,
    Low,
    Medium,
    High,
    VeryHigh,
  }

  [Serializable]
  public struct RenderListEntry
  {
    public Renderer renderer;
    public bool alwaysEnabled;

    public RenderListEntry(Renderer renderer, bool alwaysEnabled)
    {
      this.renderer = renderer;
      this.alwaysEnabled = alwaysEnabled;
    }
  }

  private struct CubemapSkyboxVertex
  {
    public float x;
    public float y;
    public float z;
    public Color color;
    public float tu;
    public float tv;
    public float tw;
  }

  private struct CubemapFaceMatrices
  {
    public Matrix4x4 worldToView;
    public Matrix4x4 viewToWorld;

    public CubemapFaceMatrices(Vector3 x, Vector3 y, Vector3 z)
    {
      this.worldToView = Matrix4x4.get_identity();
      ((Matrix4x4) ref this.worldToView).set_Item(0, 0, ((Vector3) ref x).get_Item(0));
      ((Matrix4x4) ref this.worldToView).set_Item(0, 1, ((Vector3) ref x).get_Item(1));
      ((Matrix4x4) ref this.worldToView).set_Item(0, 2, ((Vector3) ref x).get_Item(2));
      ((Matrix4x4) ref this.worldToView).set_Item(1, 0, ((Vector3) ref y).get_Item(0));
      ((Matrix4x4) ref this.worldToView).set_Item(1, 1, ((Vector3) ref y).get_Item(1));
      ((Matrix4x4) ref this.worldToView).set_Item(1, 2, ((Vector3) ref y).get_Item(2));
      ((Matrix4x4) ref this.worldToView).set_Item(2, 0, ((Vector3) ref z).get_Item(0));
      ((Matrix4x4) ref this.worldToView).set_Item(2, 1, ((Vector3) ref z).get_Item(1));
      ((Matrix4x4) ref this.worldToView).set_Item(2, 2, ((Vector3) ref z).get_Item(2));
      this.viewToWorld = ((Matrix4x4) ref this.worldToView).get_inverse();
    }
  }
}
