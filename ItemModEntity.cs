// Decompiled with JetBrains decompiler
// Type: ItemModEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ItemModEntity : ItemMod
{
  public GameObjectRef entityPrefab = new GameObjectRef();
  public string defaultBone;

  public override void OnItemCreated(Item item)
  {
    if (!Object.op_Equality((Object) item.GetHeldEntity(), (Object) null))
      return;
    BaseEntity entity = GameManager.server.CreateEntity(this.entityPrefab.resourcePath, (Vector3) null, (Quaternion) null, true);
    if (Object.op_Equality((Object) entity, (Object) null))
    {
      Debug.LogWarning((object) ("Couldn't create item entity " + item.info.displayName.english + " (" + this.entityPrefab.resourcePath + ")"));
    }
    else
    {
      entity.skinID = item.skin;
      entity.Spawn();
      item.SetHeldEntity(entity);
    }
  }

  public override void OnRemove(Item item)
  {
    BaseEntity heldEntity = item.GetHeldEntity();
    if (Object.op_Equality((Object) heldEntity, (Object) null))
      return;
    heldEntity.Kill(BaseNetworkable.DestroyMode.None);
    item.SetHeldEntity((BaseEntity) null);
  }

  private bool ParentToParent(Item item, BaseEntity ourEntity)
  {
    if (item.parentItem == null)
      return false;
    BaseEntity entity = item.parentItem.GetWorldEntity();
    if (Object.op_Equality((Object) entity, (Object) null))
      entity = item.parentItem.GetHeldEntity();
    ourEntity.SetFlag(BaseEntity.Flags.Disabled, false, false, true);
    ourEntity.limitNetworking = false;
    ourEntity.SetParent(entity, this.defaultBone, false, false);
    return true;
  }

  private bool ParentToPlayer(Item item, BaseEntity ourEntity)
  {
    HeldEntity heldEntity = ourEntity as HeldEntity;
    if (Object.op_Equality((Object) heldEntity, (Object) null))
      return false;
    BasePlayer ownerPlayer = item.GetOwnerPlayer();
    if (Object.op_Implicit((Object) ownerPlayer))
    {
      heldEntity.SetOwnerPlayer(ownerPlayer);
      return true;
    }
    heldEntity.ClearOwnerPlayer();
    return true;
  }

  public override void OnParentChanged(Item item)
  {
    BaseEntity heldEntity = item.GetHeldEntity();
    if (Object.op_Equality((Object) heldEntity, (Object) null) || this.ParentToParent(item, heldEntity) || this.ParentToPlayer(item, heldEntity))
      return;
    heldEntity.SetParent((BaseEntity) null, false, false);
    heldEntity.limitNetworking = true;
    heldEntity.SetFlag(BaseEntity.Flags.Disabled, true, false, true);
  }

  public override void CollectedForCrafting(Item item, BasePlayer crafter)
  {
    BaseEntity heldEntity1 = item.GetHeldEntity();
    if (Object.op_Equality((Object) heldEntity1, (Object) null))
      return;
    HeldEntity heldEntity2 = heldEntity1 as HeldEntity;
    if (Object.op_Equality((Object) heldEntity2, (Object) null))
      return;
    heldEntity2.CollectedForCrafting(item, crafter);
  }

  public override void ReturnedFromCancelledCraft(Item item, BasePlayer crafter)
  {
    BaseEntity heldEntity1 = item.GetHeldEntity();
    if (Object.op_Equality((Object) heldEntity1, (Object) null))
      return;
    HeldEntity heldEntity2 = heldEntity1 as HeldEntity;
    if (Object.op_Equality((Object) heldEntity2, (Object) null))
      return;
    heldEntity2.ReturnedFromCancelledCraft(item, crafter);
  }
}
