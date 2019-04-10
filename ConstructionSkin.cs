// Decompiled with JetBrains decompiler
// Type: ConstructionSkin
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class ConstructionSkin : BasePrefab
{
  private ColliderBatch[] colliderBatches;
  private List<GameObject> conditionals;

  private void RefreshColliderBatching()
  {
    if (this.colliderBatches == null)
      this.colliderBatches = (ColliderBatch[]) ((Component) this).GetComponentsInChildren<ColliderBatch>(true);
    for (int index = 0; index < this.colliderBatches.Length; ++index)
      this.colliderBatches[index].Refresh();
  }

  public int DetermineConditionalModelState(BuildingBlock parent)
  {
    ConditionalModel[] all = PrefabAttribute.server.FindAll<ConditionalModel>(this.prefabID);
    int num = 0;
    for (int index = 0; index < all.Length; ++index)
    {
      if (all[index].RunTests((BaseEntity) parent))
        num |= 1 << index;
    }
    return num;
  }

  private void CreateConditionalModels(BuildingBlock parent)
  {
    ConditionalModel[] all = PrefabAttribute.server.FindAll<ConditionalModel>(this.prefabID);
    for (int index = 0; index < all.Length; ++index)
    {
      if (parent.GetConditionalModel(index))
      {
        GameObject gameObject = all[index].InstantiateSkin((BaseEntity) parent);
        if (!Object.op_Equality((Object) gameObject, (Object) null))
        {
          if (this.conditionals == null)
            this.conditionals = new List<GameObject>();
          this.conditionals.Add(gameObject);
        }
      }
    }
  }

  private void DestroyConditionalModels(BuildingBlock parent)
  {
    if (this.conditionals == null)
      return;
    for (int index = 0; index < this.conditionals.Count; ++index)
      parent.gameManager.Retire(this.conditionals[index]);
    this.conditionals.Clear();
  }

  public void Refresh(BuildingBlock parent)
  {
    this.DestroyConditionalModels(parent);
    if (parent.isServer)
      this.RefreshColliderBatching();
    this.CreateConditionalModels(parent);
  }

  public void Destroy(BuildingBlock parent)
  {
    this.DestroyConditionalModels(parent);
    parent.gameManager.Retire(((Component) this).get_gameObject());
  }
}
