// Decompiled with JetBrains decompiler
// Type: VertexColorStream
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
public class VertexColorStream : MonoBehaviour
{
  [HideInInspector]
  public Mesh originalMesh;
  [HideInInspector]
  public Mesh paintedMesh;
  [HideInInspector]
  public MeshHolder meshHold;
  [HideInInspector]
  public Vector3[] _vertices;
  [HideInInspector]
  public Vector3[] _normals;
  [HideInInspector]
  public int[] _triangles;
  [HideInInspector]
  public int[][] _Subtriangles;
  [HideInInspector]
  public Matrix4x4[] _bindPoses;
  [HideInInspector]
  public BoneWeight[] _boneWeights;
  [HideInInspector]
  public Bounds _bounds;
  [HideInInspector]
  public int _subMeshCount;
  [HideInInspector]
  public Vector4[] _tangents;
  [HideInInspector]
  public Vector2[] _uv;
  [HideInInspector]
  public Vector2[] _uv2;
  [HideInInspector]
  public Vector2[] _uv3;
  [HideInInspector]
  public Color[] _colors;
  [HideInInspector]
  public Vector2[] _uv4;

  private void OnDidApplyAnimationProperties()
  {
  }

  public void init(Mesh origMesh, bool destroyOld)
  {
    this.originalMesh = origMesh;
    this.paintedMesh = (Mesh) Object.Instantiate<Mesh>((M0) origMesh);
    if (destroyOld)
      Object.DestroyImmediate((Object) origMesh);
    ((Object) this.paintedMesh).set_hideFlags((HideFlags) 0);
    ((Object) this.paintedMesh).set_name("vpp_" + ((Object) ((Component) this).get_gameObject()).get_name());
    this.meshHold = new MeshHolder();
    this.meshHold._vertices = this.paintedMesh.get_vertices();
    this.meshHold._normals = this.paintedMesh.get_normals();
    this.meshHold._triangles = this.paintedMesh.get_triangles();
    this.meshHold._TrianglesOfSubs = new trisPerSubmesh[this.paintedMesh.get_subMeshCount()];
    for (int index = 0; index < this.paintedMesh.get_subMeshCount(); ++index)
    {
      this.meshHold._TrianglesOfSubs[index] = new trisPerSubmesh();
      this.meshHold._TrianglesOfSubs[index].triangles = this.paintedMesh.GetTriangles(index);
    }
    this.meshHold._bindPoses = this.paintedMesh.get_bindposes();
    this.meshHold._boneWeights = this.paintedMesh.get_boneWeights();
    this.meshHold._bounds = this.paintedMesh.get_bounds();
    this.meshHold._subMeshCount = this.paintedMesh.get_subMeshCount();
    this.meshHold._tangents = this.paintedMesh.get_tangents();
    this.meshHold._uv = this.paintedMesh.get_uv();
    this.meshHold._uv2 = this.paintedMesh.get_uv2();
    this.meshHold._uv3 = this.paintedMesh.get_uv3();
    this.meshHold._colors = this.paintedMesh.get_colors();
    this.meshHold._uv4 = this.paintedMesh.get_uv4();
    ((MeshFilter) ((Component) this).GetComponent<MeshFilter>()).set_sharedMesh(this.paintedMesh);
    if (!Object.op_Implicit((Object) ((Component) this).GetComponent<MeshCollider>()))
      return;
    ((MeshCollider) ((Component) this).GetComponent<MeshCollider>()).set_sharedMesh(this.paintedMesh);
  }

  public void setWholeMesh(Mesh tmpMesh)
  {
    this.paintedMesh.set_vertices(tmpMesh.get_vertices());
    this.paintedMesh.set_triangles(tmpMesh.get_triangles());
    this.paintedMesh.set_normals(tmpMesh.get_normals());
    this.paintedMesh.set_colors(tmpMesh.get_colors());
    this.paintedMesh.set_uv(tmpMesh.get_uv());
    this.paintedMesh.set_uv2(tmpMesh.get_uv2());
    this.paintedMesh.set_uv3(tmpMesh.get_uv3());
    this.meshHold._vertices = tmpMesh.get_vertices();
    this.meshHold._triangles = tmpMesh.get_triangles();
    this.meshHold._normals = tmpMesh.get_normals();
    this.meshHold._colors = tmpMesh.get_colors();
    this.meshHold._uv = tmpMesh.get_uv();
    this.meshHold._uv2 = tmpMesh.get_uv2();
    this.meshHold._uv3 = tmpMesh.get_uv3();
  }

  public Vector3[] setVertices(Vector3[] _deformedVertices)
  {
    this.paintedMesh.set_vertices(_deformedVertices);
    this.meshHold._vertices = _deformedVertices;
    this.paintedMesh.RecalculateNormals();
    this.paintedMesh.RecalculateBounds();
    this.meshHold._normals = this.paintedMesh.get_normals();
    this.meshHold._bounds = this.paintedMesh.get_bounds();
    ((MeshCollider) ((Component) this).GetComponent<MeshCollider>()).set_sharedMesh((Mesh) null);
    if (Object.op_Implicit((Object) ((Component) this).GetComponent<MeshCollider>()))
      ((MeshCollider) ((Component) this).GetComponent<MeshCollider>()).set_sharedMesh(this.paintedMesh);
    return this.meshHold._normals;
  }

  public Vector3[] getVertices()
  {
    return this.paintedMesh.get_vertices();
  }

  public Vector3[] getNormals()
  {
    return this.paintedMesh.get_normals();
  }

  public int[] getTriangles()
  {
    return this.paintedMesh.get_triangles();
  }

  public void setTangents(Vector4[] _meshTangents)
  {
    this.paintedMesh.set_tangents(_meshTangents);
    this.meshHold._tangents = _meshTangents;
  }

  public Vector4[] getTangents()
  {
    return this.paintedMesh.get_tangents();
  }

  public void setColors(Color[] _vertexColors)
  {
    this.paintedMesh.set_colors(_vertexColors);
    this.meshHold._colors = _vertexColors;
  }

  public Color[] getColors()
  {
    return this.paintedMesh.get_colors();
  }

  public Vector2[] getUVs()
  {
    return this.paintedMesh.get_uv();
  }

  public void setUV4s(Vector2[] _uv4s)
  {
    this.paintedMesh.set_uv4(_uv4s);
    this.meshHold._uv4 = _uv4s;
  }

  public Vector2[] getUV4s()
  {
    return this.paintedMesh.get_uv4();
  }

  public void unlink()
  {
    this.init(this.paintedMesh, false);
  }

  public void rebuild()
  {
    if (!Object.op_Implicit((Object) ((Component) this).GetComponent<MeshFilter>()))
      return;
    this.paintedMesh = new Mesh();
    ((Object) this.paintedMesh).set_hideFlags((HideFlags) 61);
    ((Object) this.paintedMesh).set_name("vpp_" + ((Object) ((Component) this).get_gameObject()).get_name());
    if (this.meshHold == null || this.meshHold._vertices.Length == 0 || this.meshHold._TrianglesOfSubs.Length == 0)
    {
      this.paintedMesh.set_subMeshCount(this._subMeshCount);
      this.paintedMesh.set_vertices(this._vertices);
      this.paintedMesh.set_normals(this._normals);
      this.paintedMesh.set_triangles(this._triangles);
      this.meshHold._TrianglesOfSubs = new trisPerSubmesh[this.paintedMesh.get_subMeshCount()];
      for (int index = 0; index < this.paintedMesh.get_subMeshCount(); ++index)
      {
        this.meshHold._TrianglesOfSubs[index] = new trisPerSubmesh();
        this.meshHold._TrianglesOfSubs[index].triangles = this.paintedMesh.GetTriangles(index);
      }
      this.paintedMesh.set_bindposes(this._bindPoses);
      this.paintedMesh.set_boneWeights(this._boneWeights);
      this.paintedMesh.set_bounds(this._bounds);
      this.paintedMesh.set_tangents(this._tangents);
      this.paintedMesh.set_uv(this._uv);
      this.paintedMesh.set_uv2(this._uv2);
      this.paintedMesh.set_uv3(this._uv3);
      this.paintedMesh.set_colors(this._colors);
      this.paintedMesh.set_uv4(this._uv4);
      this.init(this.paintedMesh, true);
    }
    else
    {
      this.paintedMesh.set_subMeshCount(this.meshHold._subMeshCount);
      this.paintedMesh.set_vertices(this.meshHold._vertices);
      this.paintedMesh.set_normals(this.meshHold._normals);
      for (int index = 0; index < this.meshHold._subMeshCount; ++index)
        this.paintedMesh.SetTriangles(this.meshHold._TrianglesOfSubs[index].triangles, index);
      this.paintedMesh.set_bindposes(this.meshHold._bindPoses);
      this.paintedMesh.set_boneWeights(this.meshHold._boneWeights);
      this.paintedMesh.set_bounds(this.meshHold._bounds);
      this.paintedMesh.set_tangents(this.meshHold._tangents);
      this.paintedMesh.set_uv(this.meshHold._uv);
      this.paintedMesh.set_uv2(this.meshHold._uv2);
      this.paintedMesh.set_uv3(this.meshHold._uv3);
      this.paintedMesh.set_colors(this.meshHold._colors);
      this.paintedMesh.set_uv4(this.meshHold._uv4);
      this.init(this.paintedMesh, true);
    }
  }

  private void Start()
  {
    if (Object.op_Implicit((Object) this.paintedMesh) && this.meshHold != null)
      return;
    this.rebuild();
  }

  public VertexColorStream()
  {
    base.\u002Ector();
  }
}
