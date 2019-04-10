// Decompiled with JetBrains decompiler
// Type: WaterLevel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public static class WaterLevel
{
  public static float Factor(Bounds bounds)
  {
    using (TimeWarning.New("WaterLevel.Factor", 0.1f))
    {
      if (Vector3.op_Equality(((Bounds) ref bounds).get_size(), Vector3.get_zero()))
        ((Bounds) ref bounds).set_size(new Vector3(0.1f, 0.1f, 0.1f));
      WaterLevel.WaterInfo waterInfo = WaterLevel.GetWaterInfo(bounds);
      return waterInfo.isValid ? Mathf.InverseLerp((float) ((Bounds) ref bounds).get_min().y, (float) ((Bounds) ref bounds).get_max().y, waterInfo.surfaceLevel) : 0.0f;
    }
  }

  public static bool Test(Vector3 pos)
  {
    using (TimeWarning.New("WaterLevel.Test", 0.1f))
      return WaterLevel.GetWaterInfo(pos).isValid;
  }

  public static float GetWaterDepth(Vector3 pos)
  {
    using (TimeWarning.New("WaterLevel.GetWaterDepth", 0.1f))
      return WaterLevel.GetWaterInfo(pos).currentDepth;
  }

  public static float GetOverallWaterDepth(Vector3 pos)
  {
    using (TimeWarning.New("WaterLevel.GetOverallWaterDepth", 0.1f))
      return WaterLevel.GetWaterInfo(pos).overallDepth;
  }

  public static WaterLevel.WaterInfo GetBuoyancyWaterInfo(
    Vector3 pos,
    Vector2 posUV,
    float terrainHeight,
    float waterHeight)
  {
    using (TimeWarning.New("WaterLevel.GetWaterInfo", 0.1f))
    {
      WaterLevel.WaterInfo waterInfo = new WaterLevel.WaterInfo();
      if (pos.y > (double) waterHeight)
        return waterInfo;
      bool flag = pos.y < (double) terrainHeight - 1.0;
      if (flag)
      {
        waterHeight = 0.0f;
        if (pos.y > (double) waterHeight)
          return waterInfo;
      }
      int num = Object.op_Implicit((Object) TerrainMeta.TopologyMap) ? TerrainMeta.TopologyMap.GetTopologyFast(posUV) : 0;
      if ((flag || (num & 246144) == 0) && (Object.op_Implicit((Object) WaterSystem.Collision) && WaterSystem.Collision.GetIgnore(pos, 0.01f)))
        return waterInfo;
      waterInfo.isValid = true;
      waterInfo.currentDepth = Mathf.Max(0.0f, waterHeight - (float) pos.y);
      waterInfo.overallDepth = Mathf.Max(0.0f, waterHeight - terrainHeight);
      waterInfo.surfaceLevel = waterHeight;
      return waterInfo;
    }
  }

  public static WaterLevel.WaterInfo GetWaterInfo(Vector3 pos)
  {
    using (TimeWarning.New("WaterLevel.GetWaterInfo", 0.1f))
    {
      WaterLevel.WaterInfo waterInfo = new WaterLevel.WaterInfo();
      float num1 = Object.op_Implicit((Object) TerrainMeta.WaterMap) ? TerrainMeta.WaterMap.GetHeight(pos) : 0.0f;
      if (pos.y > (double) num1)
        return waterInfo;
      float num2 = Object.op_Implicit((Object) TerrainMeta.HeightMap) ? TerrainMeta.HeightMap.GetHeight(pos) : 0.0f;
      if (pos.y < (double) num2 - 1.0)
      {
        num1 = 0.0f;
        if (pos.y > (double) num1)
          return waterInfo;
      }
      if (Object.op_Implicit((Object) WaterSystem.Collision) && WaterSystem.Collision.GetIgnore(pos, 0.01f))
        return waterInfo;
      waterInfo.isValid = true;
      waterInfo.currentDepth = Mathf.Max(0.0f, num1 - (float) pos.y);
      waterInfo.overallDepth = Mathf.Max(0.0f, num1 - num2);
      waterInfo.surfaceLevel = num1;
      return waterInfo;
    }
  }

  public static WaterLevel.WaterInfo GetWaterInfo(Bounds bounds)
  {
    using (TimeWarning.New("WaterLevel.GetWaterInfo", 0.1f))
    {
      WaterLevel.WaterInfo waterInfo = new WaterLevel.WaterInfo();
      float num1 = Object.op_Implicit((Object) TerrainMeta.WaterMap) ? TerrainMeta.WaterMap.GetHeight(((Bounds) ref bounds).get_center()) : 0.0f;
      if (((Bounds) ref bounds).get_min().y > (double) num1)
        return waterInfo;
      float num2 = Object.op_Implicit((Object) TerrainMeta.HeightMap) ? TerrainMeta.HeightMap.GetHeight(((Bounds) ref bounds).get_center()) : 0.0f;
      if (((Bounds) ref bounds).get_max().y < (double) num2 - 1.0)
      {
        num1 = 0.0f;
        if (((Bounds) ref bounds).get_min().y > (double) num1)
          return waterInfo;
      }
      if (Object.op_Implicit((Object) WaterSystem.Collision) && WaterSystem.Collision.GetIgnore(bounds))
        return waterInfo;
      waterInfo.isValid = true;
      waterInfo.currentDepth = Mathf.Max(0.0f, num1 - (float) ((Bounds) ref bounds).get_min().y);
      waterInfo.overallDepth = Mathf.Max(0.0f, num1 - num2);
      waterInfo.surfaceLevel = num1;
      return waterInfo;
    }
  }

  public struct WaterInfo
  {
    public bool isValid;
    public float currentDepth;
    public float overallDepth;
    public float surfaceLevel;
  }
}
