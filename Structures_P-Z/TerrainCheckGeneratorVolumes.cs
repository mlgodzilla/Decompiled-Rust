// Decompiled with JetBrains decompiler
// Type: TerrainCheckGeneratorVolumes
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class TerrainCheckGeneratorVolumes : MonoBehaviour, IEditorComponent
{
  public float PlacementRadius;

  protected void OnDrawGizmosSelected()
  {
    Gizmos.set_color(new Color(0.5f, 0.5f, 0.5f, 1f));
    GizmosUtil.DrawWireCircleY(((Component) this).get_transform().get_position(), this.PlacementRadius);
  }

  public TerrainCheckGeneratorVolumes()
  {
    base.\u002Ector();
  }
}
