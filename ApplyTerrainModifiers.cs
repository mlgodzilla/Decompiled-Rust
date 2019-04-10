// Decompiled with JetBrains decompiler
// Type: ApplyTerrainModifiers
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ApplyTerrainModifiers : MonoBehaviour
{
  protected void Awake()
  {
    BaseEntity component = (BaseEntity) ((Component) this).GetComponent<BaseEntity>();
    TerrainModifier[] modifiers = (TerrainModifier[]) null;
    if (component.isServer)
      modifiers = PrefabAttribute.server.FindAll<TerrainModifier>(component.prefabID);
    ((Component) this).get_transform().ApplyTerrainModifiers(modifiers);
    GameManager.Destroy((Component) this, 0.0f);
  }

  public ApplyTerrainModifiers()
  {
    base.\u002Ector();
  }
}
