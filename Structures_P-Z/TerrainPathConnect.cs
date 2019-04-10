// Decompiled with JetBrains decompiler
// Type: TerrainPathConnect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class TerrainPathConnect : MonoBehaviour
{
  public InfrastructureType Type;

  public PathFinder.Point GetPoint(int res)
  {
    Vector3 position = ((Component) this).get_transform().get_position();
    float num1 = TerrainMeta.NormalizeX((float) position.x);
    float num2 = TerrainMeta.NormalizeZ((float) position.z);
    return new PathFinder.Point()
    {
      x = Mathf.Clamp((int) ((double) num1 * (double) res), 0, res - 1),
      y = Mathf.Clamp((int) ((double) num2 * (double) res), 0, res - 1)
    };
  }

  public TerrainPathConnect()
  {
    base.\u002Ector();
  }
}
