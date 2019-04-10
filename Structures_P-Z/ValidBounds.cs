// Decompiled with JetBrains decompiler
// Type: ValidBounds
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ValidBounds : SingletonComponent<ValidBounds>
{
  public Bounds worldBounds;

  public static bool Test(Vector3 vPos)
  {
    if (!Object.op_Implicit((Object) SingletonComponent<ValidBounds>.Instance))
      return true;
    return ((ValidBounds) SingletonComponent<ValidBounds>.Instance).IsInside(vPos);
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.set_color(Color.get_blue());
    Gizmos.DrawWireCube(((Bounds) ref this.worldBounds).get_center(), ((Bounds) ref this.worldBounds).get_size());
  }

  internal bool IsInside(Vector3 vPos)
  {
    return !Vector3Ex.IsNaNOrInfinity(vPos) && ((Bounds) ref this.worldBounds).Contains(vPos) && (!Object.op_Inequality((Object) TerrainMeta.Terrain, (Object) null) || vPos.y >= TerrainMeta.Position.y && !TerrainMeta.OutOfMargin(vPos));
  }

  public ValidBounds()
  {
    base.\u002Ector();
  }
}
