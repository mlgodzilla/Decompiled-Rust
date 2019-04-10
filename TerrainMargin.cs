// Decompiled with JetBrains decompiler
// Type: TerrainMargin
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class TerrainMargin
{
  public static void Create()
  {
    Material materialTemplate = TerrainMeta.Terrain.get_materialTemplate();
    Vector3 center = TerrainMeta.Center;
    Vector3 size = TerrainMeta.Size;
    Vector3 vector3_1;
    ((Vector3) ref vector3_1).\u002Ector((float) size.x, 0.0f, 0.0f);
    Vector3 vector3_2;
    ((Vector3) ref vector3_2).\u002Ector(0.0f, 0.0f, (float) size.z);
    center.y = (__Null) (double) TerrainMeta.HeightMap.GetHeight(0, 0);
    TerrainMargin.Create(Vector3.op_Subtraction(center, vector3_2), size, materialTemplate);
    TerrainMargin.Create(Vector3.op_Subtraction(Vector3.op_Subtraction(center, vector3_2), vector3_1), size, materialTemplate);
    TerrainMargin.Create(Vector3.op_Addition(Vector3.op_Subtraction(center, vector3_2), vector3_1), size, materialTemplate);
    TerrainMargin.Create(Vector3.op_Subtraction(center, vector3_1), size, materialTemplate);
    TerrainMargin.Create(Vector3.op_Addition(center, vector3_1), size, materialTemplate);
    TerrainMargin.Create(Vector3.op_Addition(center, vector3_2), size, materialTemplate);
    TerrainMargin.Create(Vector3.op_Subtraction(Vector3.op_Addition(center, vector3_2), vector3_1), size, materialTemplate);
    TerrainMargin.Create(Vector3.op_Addition(Vector3.op_Addition(center, vector3_2), vector3_1), size, materialTemplate);
  }

  private static void Create(Vector3 position, Vector3 size, Material material)
  {
    GameObject primitive = GameObject.CreatePrimitive((PrimitiveType) 4);
    ((Object) primitive).set_name(nameof (TerrainMargin));
    primitive.set_layer(16);
    primitive.get_transform().set_position(position);
    primitive.get_transform().set_localScale(Vector3.op_Multiply(size, 0.1f));
    Object.Destroy((Object) primitive.GetComponent<MeshRenderer>());
    Object.Destroy((Object) primitive.GetComponent<MeshFilter>());
  }
}
