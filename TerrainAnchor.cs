// Decompiled with JetBrains decompiler
// Type: TerrainAnchor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class TerrainAnchor : PrefabAttribute
{
  public float Extents = 1f;
  public float Offset;

  protected void OnDrawGizmosSelected()
  {
    Gizmos.set_color(new Color(0.5f, 0.5f, 0.5f, 1f));
    Gizmos.DrawLine(Vector3.op_Subtraction(Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_up(), this.Offset)), Vector3.op_Multiply(Vector3.get_up(), this.Extents)), Vector3.op_Addition(Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_up(), this.Offset)), Vector3.op_Multiply(Vector3.get_up(), this.Extents)));
  }

  public void Apply(out float height, out float min, out float max, Vector3 pos)
  {
    float extents = this.Extents;
    float offset = this.Offset;
    height = TerrainMeta.HeightMap.GetHeight(pos);
    min = height - offset - extents;
    max = height - offset + extents;
  }

  protected override System.Type GetIndexedType()
  {
    return typeof (TerrainAnchor);
  }
}
