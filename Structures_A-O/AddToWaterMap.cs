// Decompiled with JetBrains decompiler
// Type: AddToWaterMap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class AddToWaterMap : ProceduralObject
{
  public override void Process()
  {
    Collider component = (Collider) ((Component) this).GetComponent<Collider>();
    Bounds bounds = component.get_bounds();
    int num1 = TerrainMeta.WaterMap.Index(TerrainMeta.NormalizeX((float) ((Bounds) ref bounds).get_min().x));
    int num2 = TerrainMeta.WaterMap.Index(TerrainMeta.NormalizeZ((float) ((Bounds) ref bounds).get_max().x));
    int num3 = TerrainMeta.WaterMap.Index(TerrainMeta.NormalizeX((float) ((Bounds) ref bounds).get_min().z));
    int num4 = TerrainMeta.WaterMap.Index(TerrainMeta.NormalizeZ((float) ((Bounds) ref bounds).get_max().z));
    if (component is BoxCollider && Quaternion.op_Equality(((Component) this).get_transform().get_rotation(), Quaternion.get_identity()))
    {
      float height = TerrainMeta.NormalizeY((float) ((Bounds) ref bounds).get_max().y);
      for (int z = num3; z <= num4; ++z)
      {
        for (int x = num1; x <= num2; ++x)
        {
          float height01 = TerrainMeta.WaterMap.GetHeight01(x, z);
          if ((double) height > (double) height01)
            TerrainMeta.WaterMap.SetHeight(x, z, height);
        }
      }
    }
    else
    {
      for (int index1 = num3; index1 <= num4; ++index1)
      {
        float normZ = TerrainMeta.WaterMap.Coordinate(index1);
        for (int index2 = num1; index2 <= num2; ++index2)
        {
          float normX = TerrainMeta.WaterMap.Coordinate(index2);
          Vector3 vector3;
          ((Vector3) ref vector3).\u002Ector(TerrainMeta.DenormalizeX(normX), (float) (((Bounds) ref bounds).get_max().y + 1.0), TerrainMeta.DenormalizeZ(normZ));
          Ray ray;
          ((Ray) ref ray).\u002Ector(vector3, Vector3.get_down());
          RaycastHit raycastHit;
          if (component.Raycast(ray, ref raycastHit, (float) (((Bounds) ref bounds).get_size().y + 1.0 + 1.0)))
          {
            float height = TerrainMeta.NormalizeY((float) ((RaycastHit) ref raycastHit).get_point().y);
            float height01 = TerrainMeta.WaterMap.GetHeight01(index2, index1);
            if ((double) height > (double) height01)
              TerrainMeta.WaterMap.SetHeight(index2, index1, height);
          }
        }
      }
    }
    GameManager.Destroy((Component) this, 0.0f);
  }
}
