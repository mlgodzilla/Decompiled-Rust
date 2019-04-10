// Decompiled with JetBrains decompiler
// Type: ApplyTerrainAnchors
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ApplyTerrainAnchors : MonoBehaviour
{
  protected void Awake()
  {
    BaseEntity component = (BaseEntity) ((Component) this).GetComponent<BaseEntity>();
    TerrainAnchor[] anchors = (TerrainAnchor[]) null;
    if (component.isServer)
      anchors = PrefabAttribute.server.FindAll<TerrainAnchor>(component.prefabID);
    ((Component) this).get_transform().ApplyTerrainAnchors(anchors);
    GameManager.Destroy((Component) this, 0.0f);
  }

  public ApplyTerrainAnchors()
  {
    base.\u002Ector();
  }
}
