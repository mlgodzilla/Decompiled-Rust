// Decompiled with JetBrains decompiler
// Type: ObjWriter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.IO;
using System.Text;
using UnityEngine;

public static class ObjWriter
{
  public static string MeshToString(Mesh mesh)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("g ").Append(((Object) mesh).get_name()).Append("\n");
    foreach (Vector3 vertex in mesh.get_vertices())
      stringBuilder.Append(string.Format("v {0} {1} {2}\n", (object) (float) -vertex.x, (object) (float) vertex.y, (object) (float) vertex.z));
    stringBuilder.Append("\n");
    foreach (Vector3 normal in mesh.get_normals())
      stringBuilder.Append(string.Format("vn {0} {1} {2}\n", (object) (float) -normal.x, (object) (float) normal.y, (object) (float) normal.z));
    stringBuilder.Append("\n");
    foreach (Vector2 vector2 in mesh.get_uv())
    {
      Vector3 vector3 = Vector2.op_Implicit(vector2);
      stringBuilder.Append(string.Format("vt {0} {1}\n", (object) (float) vector3.x, (object) (float) vector3.y));
    }
    stringBuilder.Append("\n");
    int[] triangles = mesh.get_triangles();
    for (int index = 0; index < triangles.Length; index += 3)
    {
      int num1 = triangles[index] + 1;
      int num2 = triangles[index + 1] + 1;
      int num3 = triangles[index + 2] + 1;
      stringBuilder.Append(string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n", (object) num1, (object) num2, (object) num3));
    }
    return stringBuilder.ToString();
  }

  public static void Write(Mesh mesh, string path)
  {
    using (StreamWriter streamWriter = new StreamWriter(path))
      streamWriter.Write(ObjWriter.MeshToString(mesh));
  }
}
