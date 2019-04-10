// Decompiled with JetBrains decompiler
// Type: TerrainCheck
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class TerrainCheck : PrefabAttribute
{
  public bool Rotate = true;
  public float Extents = 1f;

  protected void OnDrawGizmosSelected()
  {
    Gizmos.set_color(new Color(0.5f, 0.5f, 0.5f, 1f));
    Gizmos.DrawLine(Vector3.op_Subtraction(((Component) this).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_up(), this.Extents)), Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_up(), this.Extents)));
  }

  public bool Check(Vector3 pos)
  {
    float extents = this.Extents;
    float height = TerrainMeta.HeightMap.GetHeight(pos);
    double num1 = pos.y - (double) extents;
    float num2 = (float) pos.y + extents;
    double num3 = (double) height;
    return num1 <= num3 && (double) num2 >= (double) height;
  }

  protected override System.Type GetIndexedType()
  {
    return typeof (TerrainCheck);
  }
}
