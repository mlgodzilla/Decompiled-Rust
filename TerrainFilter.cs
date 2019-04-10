// Decompiled with JetBrains decompiler
// Type: TerrainFilter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class TerrainFilter : PrefabAttribute
{
  public SpawnFilter Filter;

  protected void OnDrawGizmosSelected()
  {
    Gizmos.set_color(new Color(0.5f, 0.5f, 0.5f, 1f));
    Gizmos.DrawCube(Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(Vector3.op_Multiply(Vector3.get_up(), 50f), 0.5f)), new Vector3(0.5f, 50f, 0.5f));
    Gizmos.DrawSphere(Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_up(), 50f)), 2f);
  }

  public bool Check(Vector3 pos)
  {
    return (double) this.Filter.GetFactor(pos) > 0.0;
  }

  protected override System.Type GetIndexedType()
  {
    return typeof (TerrainFilter);
  }
}
