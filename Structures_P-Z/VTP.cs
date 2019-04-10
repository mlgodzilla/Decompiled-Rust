// Decompiled with JetBrains decompiler
// Type: VTP
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class VTP : MonoBehaviour
{
  public static Color getSingleVertexColorAtHit(Transform transform, RaycastHit hit)
  {
    Vector3[] vertices = ((MeshFilter) ((Component) transform).GetComponent<MeshFilter>()).get_sharedMesh().get_vertices();
    int[] triangles = ((MeshFilter) ((Component) transform).GetComponent<MeshFilter>()).get_sharedMesh().get_triangles();
    Color[] colors = ((MeshFilter) ((Component) transform).GetComponent<MeshFilter>()).get_sharedMesh().get_colors();
    int triangleIndex = ((RaycastHit) ref hit).get_triangleIndex();
    float num1 = float.PositiveInfinity;
    int index1 = 0;
    for (int index2 = 0; index2 < 3; ++index2)
    {
      float num2 = Vector3.Distance(transform.TransformPoint(vertices[triangles[triangleIndex * 3 + index2]]), ((RaycastHit) ref hit).get_point());
      if ((double) num2 < (double) num1)
      {
        index1 = triangles[triangleIndex * 3 + index2];
        num1 = num2;
      }
    }
    return colors[index1];
  }

  public static Color getFaceVerticesColorAtHit(Transform transform, RaycastHit hit)
  {
    int[] triangles = ((MeshFilter) ((Component) transform).GetComponent<MeshFilter>()).get_sharedMesh().get_triangles();
    Color[] colors = ((MeshFilter) ((Component) transform).GetComponent<MeshFilter>()).get_sharedMesh().get_colors();
    int index1 = ((RaycastHit) ref hit).get_triangleIndex() * 3;
    int index2 = triangles[index1];
    return Color.op_Division(Color.op_Addition(Color.op_Addition(colors[index2], colors[index2 + 1]), colors[index2 + 2]), 3f);
  }

  public static void paintSingleVertexOnHit(
    Transform transform,
    RaycastHit hit,
    Color color,
    float strength)
  {
    Vector3[] vertices = ((MeshFilter) ((Component) transform).GetComponent<MeshFilter>()).get_sharedMesh().get_vertices();
    int[] triangles = ((MeshFilter) ((Component) transform).GetComponent<MeshFilter>()).get_sharedMesh().get_triangles();
    Color[] colors = ((MeshFilter) ((Component) transform).GetComponent<MeshFilter>()).get_sharedMesh().get_colors();
    int triangleIndex = ((RaycastHit) ref hit).get_triangleIndex();
    float num1 = float.PositiveInfinity;
    int index1 = 0;
    for (int index2 = 0; index2 < 3; index2 += 3)
    {
      float num2 = Vector3.Distance(transform.TransformPoint(vertices[triangles[triangleIndex * 3 + index2]]), ((RaycastHit) ref hit).get_point());
      if ((double) num2 < (double) num1)
      {
        index1 = triangles[triangleIndex * 3 + index2];
        num1 = num2;
      }
    }
    Color color1 = VTP.VertexColorLerp(colors[index1], color, strength);
    colors[index1] = color1;
    ((MeshFilter) ((Component) transform).GetComponent<MeshFilter>()).get_sharedMesh().set_colors(colors);
  }

  public static void paintFaceVerticesOnHit(
    Transform transform,
    RaycastHit hit,
    Color color,
    float strength)
  {
    int[] triangles = ((MeshFilter) ((Component) transform).GetComponent<MeshFilter>()).get_sharedMesh().get_triangles();
    Color[] colors = ((MeshFilter) ((Component) transform).GetComponent<MeshFilter>()).get_sharedMesh().get_colors();
    int triangleIndex = ((RaycastHit) ref hit).get_triangleIndex();
    for (int index1 = 0; index1 < 3; ++index1)
    {
      int index2 = triangles[triangleIndex * 3 + index1];
      Color color1 = VTP.VertexColorLerp(colors[index2], color, strength);
      colors[index2] = color1;
    }
    ((MeshFilter) ((Component) transform).GetComponent<MeshFilter>()).get_sharedMesh().set_colors(colors);
  }

  public static void deformSingleVertexOnHit(
    Transform transform,
    RaycastHit hit,
    bool up,
    float strength,
    bool recalculateNormals,
    bool recalculateCollider,
    bool recalculateFlow)
  {
    Vector3[] vertices = ((MeshFilter) ((Component) transform).GetComponent<MeshFilter>()).get_sharedMesh().get_vertices();
    int[] triangles = ((MeshFilter) ((Component) transform).GetComponent<MeshFilter>()).get_sharedMesh().get_triangles();
    Vector3[] normals = ((MeshFilter) ((Component) transform).GetComponent<MeshFilter>()).get_sharedMesh().get_normals();
    int triangleIndex = ((RaycastHit) ref hit).get_triangleIndex();
    float num1 = float.PositiveInfinity;
    int index1 = 0;
    for (int index2 = 0; index2 < 3; ++index2)
    {
      float num2 = Vector3.Distance(transform.TransformPoint(vertices[triangles[triangleIndex * 3 + index2]]), ((RaycastHit) ref hit).get_point());
      if ((double) num2 < (double) num1)
      {
        index1 = triangles[triangleIndex * 3 + index2];
        num1 = num2;
      }
    }
    int num3 = 1;
    if (!up)
      num3 = -1;
    ref Vector3 local = ref vertices[index1];
    local = Vector3.op_Addition(local, Vector3.op_Multiply((float) num3 * 0.1f * strength, normals[index1]));
    ((MeshFilter) ((Component) transform).GetComponent<MeshFilter>()).get_sharedMesh().set_vertices(vertices);
    if (recalculateNormals)
      ((MeshFilter) ((Component) transform).GetComponent<MeshFilter>()).get_sharedMesh().RecalculateNormals();
    if (recalculateCollider)
      ((MeshCollider) ((Component) transform).GetComponent<MeshCollider>()).set_sharedMesh(((MeshFilter) ((Component) transform).GetComponent<MeshFilter>()).get_sharedMesh());
    if (!recalculateFlow)
      return;
    Vector4[] meshTangents = VTP.calculateMeshTangents(triangles, vertices, ((MeshCollider) ((Component) transform).GetComponent<MeshCollider>()).get_sharedMesh().get_uv(), normals);
    ((MeshCollider) ((Component) transform).GetComponent<MeshCollider>()).get_sharedMesh().set_tangents(meshTangents);
    VTP.recalculateMeshForFlow(transform, vertices, normals, meshTangents);
  }

  public static void deformFaceVerticesOnHit(
    Transform transform,
    RaycastHit hit,
    bool up,
    float strength,
    bool recalculateNormals,
    bool recalculateCollider,
    bool recalculateFlow)
  {
    Vector3[] vertices = ((MeshFilter) ((Component) transform).GetComponent<MeshFilter>()).get_sharedMesh().get_vertices();
    int[] triangles = ((MeshFilter) ((Component) transform).GetComponent<MeshFilter>()).get_sharedMesh().get_triangles();
    Vector3[] normals = ((MeshFilter) ((Component) transform).GetComponent<MeshFilter>()).get_sharedMesh().get_normals();
    int triangleIndex = ((RaycastHit) ref hit).get_triangleIndex();
    int num = 1;
    if (!up)
      num = -1;
    for (int index1 = 0; index1 < 3; ++index1)
    {
      int index2 = triangles[triangleIndex * 3 + index1];
      ref Vector3 local = ref vertices[index2];
      local = Vector3.op_Addition(local, Vector3.op_Multiply((float) num * 0.1f * strength, normals[index2]));
    }
    ((MeshFilter) ((Component) transform).GetComponent<MeshFilter>()).get_sharedMesh().set_vertices(vertices);
    if (recalculateNormals)
      ((MeshFilter) ((Component) transform).GetComponent<MeshFilter>()).get_sharedMesh().RecalculateNormals();
    if (recalculateCollider)
      ((MeshCollider) ((Component) transform).GetComponent<MeshCollider>()).set_sharedMesh(((MeshFilter) ((Component) transform).GetComponent<MeshFilter>()).get_sharedMesh());
    if (!recalculateFlow)
      return;
    Vector4[] meshTangents = VTP.calculateMeshTangents(triangles, vertices, ((MeshCollider) ((Component) transform).GetComponent<MeshCollider>()).get_sharedMesh().get_uv(), normals);
    ((MeshCollider) ((Component) transform).GetComponent<MeshCollider>()).get_sharedMesh().set_tangents(meshTangents);
    VTP.recalculateMeshForFlow(transform, vertices, normals, meshTangents);
  }

  private static void recalculateMeshForFlow(
    Transform transform,
    Vector3[] currentVertices,
    Vector3[] currentNormals,
    Vector4[] currentTangents)
  {
    Vector2[] uv4 = ((MeshFilter) ((Component) transform).GetComponent<MeshFilter>()).get_sharedMesh().get_uv4();
    for (int index = 0; index < currentVertices.Length; ++index)
    {
      Transform transform1 = transform;
      Vector3 vector3_1 = Vector3.Cross(currentNormals[index], new Vector3((float) currentTangents[index].x, (float) currentTangents[index].y, (float) currentTangents[index].z));
      Vector3 vector3_2 = Vector3.op_Multiply(((Vector3) ref vector3_1).get_normalized(), (float) currentTangents[index].w);
      Vector3 vector3_3 = transform1.TransformDirection(vector3_2);
      float num1 = (float) (0.5 + 0.5 * transform.TransformDirection(Vector4.op_Implicit(((Vector4) ref currentTangents[index]).get_normalized())).y);
      float num2 = (float) (0.5 + 0.5 * vector3_3.y);
      uv4[index] = new Vector2(num1, num2);
    }
    ((MeshFilter) ((Component) transform).GetComponent<MeshFilter>()).get_sharedMesh().set_uv4(uv4);
  }

  private static Vector4[] calculateMeshTangents(
    int[] triangles,
    Vector3[] vertices,
    Vector2[] uv,
    Vector3[] normals)
  {
    int length1 = triangles.Length;
    int length2 = vertices.Length;
    Vector3[] vector3Array1 = new Vector3[length2];
    Vector3[] vector3Array2 = new Vector3[length2];
    Vector4[] vector4Array = new Vector4[length2];
    for (long index = 0; index < (long) length1; index += 3L)
    {
      long triangle1 = (long) triangles[index];
      long triangle2 = (long) triangles[index + 1L];
      long triangle3 = (long) triangles[index + 2L];
      Vector3 vertex1 = vertices[triangle1];
      Vector3 vertex2 = vertices[triangle2];
      Vector3 vertex3 = vertices[triangle3];
      Vector2 vector2_1 = uv[triangle1];
      Vector2 vector2_2 = uv[triangle2];
      Vector2 vector2_3 = uv[triangle3];
      float num1 = (float) (vertex2.x - vertex1.x);
      float num2 = (float) (vertex3.x - vertex1.x);
      float num3 = (float) (vertex2.y - vertex1.y);
      float num4 = (float) (vertex3.y - vertex1.y);
      float num5 = (float) (vertex2.z - vertex1.z);
      float num6 = (float) (vertex3.z - vertex1.z);
      float num7 = (float) (vector2_2.x - vector2_1.x);
      float num8 = (float) (vector2_3.x - vector2_1.x);
      float num9 = (float) (vector2_2.y - vector2_1.y);
      float num10 = (float) (vector2_3.y - vector2_1.y);
      float num11 = (float) ((double) num7 * (double) num10 - (double) num8 * (double) num9);
      float num12 = (double) num11 == 0.0 ? 0.0f : 1f / num11;
      Vector3 vector3_1;
      ((Vector3) ref vector3_1).\u002Ector((float) ((double) num10 * (double) num1 - (double) num9 * (double) num2) * num12, (float) ((double) num10 * (double) num3 - (double) num9 * (double) num4) * num12, (float) ((double) num10 * (double) num5 - (double) num9 * (double) num6) * num12);
      Vector3 vector3_2;
      ((Vector3) ref vector3_2).\u002Ector((float) ((double) num7 * (double) num2 - (double) num8 * (double) num1) * num12, (float) ((double) num7 * (double) num4 - (double) num8 * (double) num3) * num12, (float) ((double) num7 * (double) num6 - (double) num8 * (double) num5) * num12);
      ref Vector3 local1 = ref vector3Array1[triangle1];
      local1 = Vector3.op_Addition(local1, vector3_1);
      ref Vector3 local2 = ref vector3Array1[triangle2];
      local2 = Vector3.op_Addition(local2, vector3_1);
      ref Vector3 local3 = ref vector3Array1[triangle3];
      local3 = Vector3.op_Addition(local3, vector3_1);
      ref Vector3 local4 = ref vector3Array2[triangle1];
      local4 = Vector3.op_Addition(local4, vector3_2);
      ref Vector3 local5 = ref vector3Array2[triangle2];
      local5 = Vector3.op_Addition(local5, vector3_2);
      ref Vector3 local6 = ref vector3Array2[triangle3];
      local6 = Vector3.op_Addition(local6, vector3_2);
    }
    for (long index = 0; index < (long) length2; ++index)
    {
      Vector3 normal = normals[index];
      Vector3 vector3 = vector3Array1[index];
      Vector3.OrthoNormalize(ref normal, ref vector3);
      vector4Array[index].x = vector3.x;
      vector4Array[index].y = vector3.y;
      vector4Array[index].z = vector3.z;
      vector4Array[index].w = (double) Vector3.Dot(Vector3.Cross(normal, vector3), vector3Array2[index]) < 0.0 ? (__Null) -1.0 : (__Null) 1.0;
    }
    return vector4Array;
  }

  public static Color VertexColorLerp(Color colorA, Color colorB, float value)
  {
    if ((double) value >= 1.0)
      return colorB;
    if ((double) value <= 0.0)
      return colorA;
    return new Color((float) (colorA.r + (colorB.r - colorA.r) * (double) value), (float) (colorA.g + (colorB.g - colorA.g) * (double) value), (float) (colorA.b + (colorB.b - colorA.b) * (double) value), (float) (colorA.a + (colorB.a - colorA.a) * (double) value));
  }

  public VTP()
  {
    base.\u002Ector();
  }
}
