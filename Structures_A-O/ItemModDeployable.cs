// Decompiled with JetBrains decompiler
// Type: ItemModDeployable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ItemModDeployable : MonoBehaviour
{
  public GameObjectRef entityPrefab;
  [Header("Tooltips")]
  public bool showCrosshair;
  public string UnlockAchievement;

  public Deployable GetDeployable(BaseEntity entity)
  {
    if (Object.op_Equality((Object) entity.gameManager.FindPrefab(this.entityPrefab.resourcePath), (Object) null))
      return (Deployable) null;
    return entity.prefabAttribute.Find<Deployable>(this.entityPrefab.resourceID);
  }

  internal void OnDeployed(BaseEntity ent, BasePlayer player)
  {
    if (!player.IsValid() || string.IsNullOrEmpty(this.UnlockAchievement))
      return;
    player.GiveAchievement(this.UnlockAchievement);
  }

  public ItemModDeployable()
  {
    base.\u002Ector();
  }
}
