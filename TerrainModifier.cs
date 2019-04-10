// Decompiled with JetBrains decompiler
// Type: TerrainModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public abstract class TerrainModifier : PrefabAttribute
{
  public float Opacity = 1f;
  public float Radius;
  public float Fade;

  protected void OnDrawGizmosSelected()
  {
    Gizmos.set_color(new Color(0.5f, 0.5f, 0.5f, this.Opacity));
    GizmosUtil.DrawWireCircleY(((Component) this).get_transform().get_position(), (float) ((Component) this).get_transform().get_lossyScale().y * this.Radius);
  }

  public void Apply(Vector3 pos, float scale)
  {
    float opacity = this.Opacity;
    float radius = scale * this.Radius;
    float fade = scale * this.Fade;
    this.Apply(pos, opacity, radius, fade);
  }

  protected abstract void Apply(Vector3 position, float opacity, float radius, float fade);

  protected override System.Type GetIndexedType()
  {
    return typeof (TerrainModifier);
  }
}
