// Decompiled with JetBrains decompiler
// Type: AddToHeightMap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class AddToHeightMap : ProceduralObject
{
  public override void Process()
  {
    Collider component = (Collider) ((Component) this).GetComponent<Collider>();
    Bounds bounds = component.get_bounds();
    int num1 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeX((float) ((Bounds) ref bounds).get_min().x));
    int num2 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeZ((float) ((Bounds) ref bounds).get_max().x));
    int num3 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeX((float) ((Bounds) ref bounds).get_min().z));
    int num4 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeZ((float) ((Bounds) ref bounds).get_max().z));
    for (int index1 = num3; index1 <= num4; ++index1)
    {
      float normZ = TerrainMeta.HeightMap.Coordinate(index1);
      for (int index2 = num1; index2 <= num2; ++index2)
      {
        float normX = TerrainMeta.HeightMap.Coordinate(index2);
        Vector3 vector3;
        ((Vector3) ref vector3).\u002Ector(TerrainMeta.DenormalizeX(normX), (float) ((Bounds) ref bounds).get_max().y, TerrainMeta.DenormalizeZ(normZ));
        Ray ray;
        ((Ray) ref ray).\u002Ector(vector3, Vector3.get_down());
        RaycastHit raycastHit;
        if (component.Raycast(ray, ref raycastHit, (float) ((Bounds) ref bounds).get_size().y))
        {
          float height = TerrainMeta.NormalizeY((float) ((RaycastHit) ref raycastHit).get_point().y);
          float height01 = TerrainMeta.HeightMap.GetHeight01(index2, index1);
          if ((double) height > (double) height01)
            TerrainMeta.HeightMap.SetHeight(index2, index1, height);
        }
      }
    }
    GameManager.Destroy((Component) this, 0.0f);
  }
}
