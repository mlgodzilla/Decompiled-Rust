// Decompiled with JetBrains decompiler
// Type: WaterRadialMesh
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WaterRadialMesh
{
  private const float AlignmentGranularity = 1f;
  private const float MaxHorizontalDisplacement = 1f;
  private Mesh[] meshes;
  private bool initialized;

  public Mesh[] Meshes
  {
    get
    {
      return this.meshes;
    }
  }

  public bool IsInitialized
  {
    get
    {
      return this.initialized;
    }
  }

  public void Initialize(int vertexCount)
  {
    this.meshes = this.GenerateMeshes(vertexCount, false);
    this.initialized = true;
  }

  public void Destroy()
  {
    if (!this.initialized)
      return;
    foreach (Object mesh in this.meshes)
      Object.DestroyImmediate(mesh);
    this.meshes = (Mesh[]) null;
    this.initialized = false;
  }

  private Mesh CreateMesh(string name, Vector3[] vertices, int[] indices)
  {
    Mesh mesh = new Mesh();
    ((Object) mesh).set_hideFlags((HideFlags) 52);
    ((Object) mesh).set_name(name);
    mesh.set_vertices(vertices);
    mesh.SetIndices(indices, (MeshTopology) 2, 0);
    mesh.RecalculateBounds();
    mesh.UploadMeshData(true);
    return mesh;
  }

  private Mesh[] GenerateMeshes(int vertexCount, bool volume = false)
  {
    int length = Mathf.RoundToInt((float) Mathf.RoundToInt(Mathf.Sqrt((float) vertexCount)) * 0.4f);
    int num1 = Mathf.RoundToInt((float) vertexCount / (float) length);
    int num2 = volume ? num1 / 2 : num1;
    List<Mesh> meshList = new List<Mesh>();
    List<Vector3> vector3List = new List<Vector3>();
    List<int> intList = new List<int>();
    Vector2[] vector2Array1 = new Vector2[length];
    int num3 = 0;
    int num4 = 0;
    for (int index1 = 0; index1 < length; ++index1)
    {
      float num5 = (float) (((double) index1 / (double) (length - 1) * 2.0 - 1.0) * 3.14159274101257 * 0.25);
      Vector2[] vector2Array2 = vector2Array1;
      int index2 = index1;
      Vector2 vector2 = new Vector2(Mathf.Sin(num5), Mathf.Cos(num5));
      Vector2 normalized = ((Vector2) ref vector2).get_normalized();
      vector2Array2[index2] = normalized;
    }
    for (int index1 = 0; index1 < num2; ++index1)
    {
      float num5 = 1f - Mathf.Cos((float) ((double) ((float) index1 / (float) (num1 - 1)) * 3.14159274101257 * 0.5));
      for (int index2 = 0; index2 < length; ++index2)
      {
        Vector2 vector2 = Vector2.op_Multiply(vector2Array1[index2], num5);
        if (index1 < num2 - 2 || !volume)
          vector3List.Add(new Vector3((float) vector2.x, 0.0f, (float) vector2.y));
        else if (index1 == num2 - 2)
          vector3List.Add(Vector3.op_Multiply(new Vector3((float) (vector2.x * 10.0), -0.9f, (float) vector2.y), 0.5f));
        else
          vector3List.Add(Vector3.op_Multiply(new Vector3((float) (vector2.x * 10.0), -0.9f, (float) (vector2.y * -10.0)), 0.5f));
        if (index2 != 0 && index1 != 0 && num3 > length)
        {
          intList.Add(num3);
          intList.Add(num3 - length);
          intList.Add(num3 - length - 1);
          intList.Add(num3 - 1);
        }
        ++num3;
        if (num3 >= 65000)
        {
          meshList.Add(this.CreateMesh("WaterMesh_" + (object) length + "x" + (object) num1 + "_" + (object) num4, vector3List.ToArray(), intList.ToArray()));
          --index2;
          --index1;
          num5 = 1f - Mathf.Cos((float) ((double) index1 / (double) (num1 - 1) * 3.14159274101257 * 0.5));
          num3 = 0;
          vector3List.Clear();
          intList.Clear();
          ++num4;
        }
      }
    }
    if (num3 != 0)
      meshList.Add(this.CreateMesh("WaterMesh_" + (object) length + "x" + (object) num1 + "_" + (object) num4, vector3List.ToArray(), intList.ToArray()));
    return meshList.ToArray();
  }

  private Vector3 RaycastPlane(Camera camera, float planeHeight, Vector3 pos)
  {
    Ray ray = camera.ViewportPointToRay(pos);
    if (((Component) camera).get_transform().get_position().y > (double) planeHeight)
    {
      if (((Ray) ref ray).get_direction().y > -0.00999999977648258)
        ((Ray) ref ray).set_direction(new Vector3((float) ((Ray) ref ray).get_direction().x, (float) (-((Ray) ref ray).get_direction().y - 0.0199999995529652), (float) ((Ray) ref ray).get_direction().z));
    }
    else if (((Ray) ref ray).get_direction().y < 0.00999999977648258)
      ((Ray) ref ray).set_direction(new Vector3((float) ((Ray) ref ray).get_direction().x, (float) (-((Ray) ref ray).get_direction().y + 0.0199999995529652), (float) ((Ray) ref ray).get_direction().z));
    float num = (float) (-(((Ray) ref ray).get_origin().y - (double) planeHeight) / ((Ray) ref ray).get_direction().y);
    return Quaternion.op_Multiply(Quaternion.AngleAxis((float) -((Component) camera).get_transform().get_eulerAngles().y, Vector3.get_up()), Vector3.op_Multiply(((Ray) ref ray).get_direction(), num));
  }

  public Matrix4x4 ComputeLocalToWorldMatrix(Camera camera, float oceanWaterLevel)
  {
    if (Object.op_Equality((Object) camera, (Object) null))
      return Matrix4x4.get_identity();
    Matrix4x4 worldToCameraMatrix = camera.get_worldToCameraMatrix();
    Vector3 vector3_1 = ((Matrix4x4) ref worldToCameraMatrix).MultiplyVector(Vector3.get_up());
    worldToCameraMatrix = camera.get_worldToCameraMatrix();
    Vector3 vector3_2 = ((Matrix4x4) ref worldToCameraMatrix).MultiplyVector(Vector3.Cross(((Component) camera).get_transform().get_forward(), Vector3.get_up()));
    Vector3 vector3_3 = new Vector3((float) vector3_1.x, (float) vector3_1.y, 0.0f);
    Vector3 vector3_4 = Vector3.op_Addition(Vector3.op_Multiply(((Vector3) ref vector3_3).get_normalized(), 0.5f), new Vector3(0.5f, 0.0f, 0.5f));
    Vector3 vector3_5 = new Vector3((float) vector3_2.x, (float) vector3_2.y, 0.0f);
    Vector3 vector3_6 = Vector3.op_Multiply(((Vector3) ref vector3_5).get_normalized(), 0.5f);
    Vector3 vector3_7 = this.RaycastPlane(camera, oceanWaterLevel, Vector3.op_Subtraction(vector3_4, vector3_6));
    Vector3 vector3_8 = this.RaycastPlane(camera, oceanWaterLevel, Vector3.op_Addition(vector3_4, vector3_6));
    float num1 = Mathf.Min(camera.get_farClipPlane(), 5000f);
    Vector3 position = ((Component) camera).get_transform().get_position();
    Vector3 vector3_9 = (Vector3) null;
    vector3_9.x = (__Null) ((double) num1 * (double) Mathf.Tan((float) ((double) camera.get_fieldOfView() * 0.5 * (Math.PI / 180.0))) * (double) camera.get_aspect() + 2.0);
    vector3_9.y = (__Null) (double) num1;
    vector3_9.z = (__Null) (double) num1;
    float num2 = Mathf.Abs((float) (vector3_8.x - vector3_7.x));
    float num3 = Mathf.Min((float) vector3_7.z, (float) vector3_8.z) - (float) (((double) num2 + 2.0) * vector3_9.z / vector3_9.x);
    Vector3 forward = ((Component) camera).get_transform().get_forward();
    forward.y = (__Null) 0.0;
    ((Vector3) ref forward).Normalize();
    ref __Null local = ref vector3_9.z;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local = ^(float&) ref local - num3;
    return Matrix4x4.TRS(Vector3.op_Addition(new Vector3((float) position.x, oceanWaterLevel, (float) position.z), Vector3.op_Multiply(forward, num3)), Quaternion.AngleAxis(Mathf.Atan2((float) forward.x, (float) forward.z) * 57.29578f, Vector3.get_up()), vector3_9);
  }
}
