// Decompiled with JetBrains decompiler
// Type: WaterCheck
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class WaterCheck : PrefabAttribute
{
  public bool Rotate = true;

  protected void OnDrawGizmosSelected()
  {
    Gizmos.set_color(new Color(0.0f, 0.0f, 0.5f, 1f));
    Gizmos.DrawSphere(((Component) this).get_transform().get_position(), 1f);
  }

  public bool Check(Vector3 pos)
  {
    return pos.y <= (double) TerrainMeta.WaterMap.GetHeight(pos);
  }

  protected override System.Type GetIndexedType()
  {
    return typeof (WaterCheck);
  }
}
