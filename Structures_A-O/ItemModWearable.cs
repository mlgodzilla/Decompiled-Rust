// Decompiled with JetBrains decompiler
// Type: ItemModWearable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using UnityEngine;

public class ItemModWearable : ItemMod
{
  public GameObjectRef entityPrefab = new GameObjectRef();
  public UIBlackoutOverlay.blackoutType occlusionType = UIBlackoutOverlay.blackoutType.NONE;
  public ProtectionProperties protectionProperties;
  public ArmorProperties armorProperties;
  public ClothingMovementProperties movementProperties;
  public bool blocksAiming;
  public bool emissive;
  public float accuracyBonus;
  public bool blocksEquipping;
  public GameObjectRef viewmodelAddition;

  public Wearable targetWearable
  {
    get
    {
      if (!this.entityPrefab.isValid)
        return (Wearable) null;
      return (Wearable) this.entityPrefab.Get().GetComponent<Wearable>();
    }
  }

  private void DoPrepare()
  {
    if (!this.entityPrefab.isValid)
      Debug.LogWarning((object) ("ItemModWearable: entityPrefab is null! " + (object) ((Component) this).get_gameObject()), (Object) ((Component) this).get_gameObject());
    if (!this.entityPrefab.isValid || !Object.op_Equality((Object) this.targetWearable, (Object) null))
      return;
    Debug.LogWarning((object) ("ItemModWearable: entityPrefab doesn't have a Wearable component! " + (object) ((Component) this).get_gameObject()), (Object) this.entityPrefab.Get());
  }

  public override void ModInit()
  {
    if (!string.IsNullOrEmpty(this.entityPrefab.resourcePath))
      return;
    Debug.LogWarning((object) (this.ToString() + " - entityPrefab is null or something.. - " + this.entityPrefab.guid));
  }

  public bool ProtectsArea(HitArea area)
  {
    if (Object.op_Equality((Object) this.armorProperties, (Object) null))
      return false;
    return this.armorProperties.Contains(area);
  }

  public bool HasProtections()
  {
    return Object.op_Inequality((Object) this.protectionProperties, (Object) null);
  }

  internal float GetProtection(Item item, DamageType damageType)
  {
    if (Object.op_Equality((Object) this.protectionProperties, (Object) null))
      return 0.0f;
    return this.protectionProperties.Get(damageType) * this.ConditionProtectionScale(item);
  }

  public float ConditionProtectionScale(Item item)
  {
    return !item.isBroken ? 1f : 0.25f;
  }

  public void CollectProtection(Item item, ProtectionProperties protection)
  {
    if (Object.op_Equality((Object) this.protectionProperties, (Object) null))
      return;
    protection.Add(this.protectionProperties, this.ConditionProtectionScale(item));
  }

  private bool IsHeadgear()
  {
    Wearable component = (Wearable) this.entityPrefab.Get().GetComponent<Wearable>();
    return Object.op_Inequality((Object) component, (Object) null) && (component.occupationOver & (Wearable.OccupationSlots.HeadTop | Wearable.OccupationSlots.Face | Wearable.OccupationSlots.HeadBack)) != (Wearable.OccupationSlots) 0;
  }

  public bool IsFootwear()
  {
    Wearable component = (Wearable) this.entityPrefab.Get().GetComponent<Wearable>();
    return Object.op_Inequality((Object) component, (Object) null) && (component.occupationOver & (Wearable.OccupationSlots.LeftFoot | Wearable.OccupationSlots.RightFoot)) != (Wearable.OccupationSlots) 0;
  }

  public override void OnAttacked(Item item, HitInfo info)
  {
    if (!item.hasCondition)
      return;
    float amount = 0.0f;
    for (int index = 0; index < 22; ++index)
    {
      DamageType damageType = (DamageType) index;
      if (info.damageTypes.Has(damageType))
      {
        amount += Mathf.Clamp(info.damageTypes.types[index] * this.GetProtection(item, damageType), 0.0f, item.condition);
        if ((double) amount >= (double) item.condition)
          break;
      }
    }
    item.LoseCondition(amount);
    if (item == null || !item.isBroken || (!Object.op_Implicit((Object) item.GetOwnerPlayer()) || !this.IsHeadgear()) || (double) info.damageTypes.Total() < (double) item.GetOwnerPlayer().health)
      return;
    BaseEntity baseEntity = item.Drop(Vector3.op_Addition(((Component) item.GetOwnerPlayer()).get_transform().get_position(), new Vector3(0.0f, 1.8f, 0.0f)), Vector3.op_Addition(item.GetOwnerPlayer().GetInheritedDropVelocity(), Vector3.op_Multiply(Vector3.get_up(), 3f)), (Quaternion) null);
    Quaternion rotation = Random.get_rotation();
    Vector3 velocity = Vector3.op_Multiply(((Quaternion) ref rotation).get_eulerAngles(), 5f);
    baseEntity.SetAngularVelocity(velocity);
  }

  public bool CanExistWith(ItemModWearable wearable)
  {
    if (Object.op_Equality((Object) wearable, (Object) null))
      return true;
    Wearable targetWearable1 = this.targetWearable;
    Wearable targetWearable2 = wearable.targetWearable;
    return (targetWearable1.occupationOver & targetWearable2.occupationOver) == (Wearable.OccupationSlots) 0 && (targetWearable1.occupationUnder & targetWearable2.occupationUnder) == (Wearable.OccupationSlots) 0;
  }
}
