// Decompiled with JetBrains decompiler
// Type: HeldEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class HeldEntity : BaseEntity
{
  [Header("Held Entity")]
  public string handBone = "r_prop";
  public Animator worldModelAnimator;
  public SoundDefinition thirdPersonDeploySound;
  public SoundDefinition thirdPersonAimSound;
  public SoundDefinition thirdPersonAimEndSound;
  public AnimatorOverrideController HoldAnimationOverride;
  public NPCPlayerApex.ToolTypeEnum toolType;
  [Header("Hostility")]
  public float hostileScore;
  public HeldEntity.HolsterInfo holsterInfo;
  private bool holsterVisible;
  private HeldEntity.heldEntityVisState currentVisState;
  internal uint ownerItemUID;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("HeldEntity.OnRpcMessage", 0.1f))
      ;
    return base.OnRpcMessage(player, rpc, msg);
  }

  public bool hostile
  {
    get
    {
      return (double) this.hostileScore > 0.0;
    }
  }

  public bool LightsOn()
  {
    return this.HasFlag(BaseEntity.Flags.Reserved5);
  }

  public bool IsDeployed()
  {
    return this.HasFlag(BaseEntity.Flags.Reserved4);
  }

  public BasePlayer GetOwnerPlayer()
  {
    BaseEntity parentEntity = this.GetParentEntity();
    if (!parentEntity.IsValid())
      return (BasePlayer) null;
    BasePlayer player = parentEntity.ToPlayer();
    if (Object.op_Equality((Object) player, (Object) null))
      return (BasePlayer) null;
    if (player.IsDead())
      return (BasePlayer) null;
    return player;
  }

  public Connection GetOwnerConnection()
  {
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (Object.op_Equality((Object) ownerPlayer, (Object) null))
      return (Connection) null;
    if (ownerPlayer.net == null)
      return (Connection) null;
    return ownerPlayer.net.get_connection();
  }

  public virtual void SetOwnerPlayer(BasePlayer player)
  {
    Assert.IsTrue(this.isServer, "Should be server!");
    Assert.IsTrue(player.isServer, "Player should be serverside!");
    ((Component) this).get_gameObject().Identity();
    this.SetParent((BaseEntity) player, this.handBone, false, false);
    this.SetHeld(false);
  }

  public virtual void ClearOwnerPlayer()
  {
    Assert.IsTrue(this.isServer, "Should be server!");
    this.SetParent((BaseEntity) null, false, false);
    this.SetHeld(false);
  }

  public virtual void SetVisibleWhileHolstered(bool visible)
  {
    if (!this.holsterInfo.displayWhenHolstered)
      return;
    this.holsterVisible = visible;
    this.UpdateHeldItemVisibility();
  }

  public uint GetBone(string bone)
  {
    return StringPool.Get(bone);
  }

  public virtual void SetLightsOn(bool isOn)
  {
    this.SetFlag(BaseEntity.Flags.Reserved5, isOn, false, true);
  }

  public void UpdateHeldItemVisibility()
  {
    if (!Object.op_Implicit((Object) this.GetOwnerPlayer()))
      return;
    bool flag = Object.op_Equality((Object) this.GetOwnerPlayer().GetHeldEntity(), (Object) this);
    if (!(Server.showHolsteredItems || flag ? (!flag ? (!this.holsterVisible ? this.UpdateVisiblity_Invis() : this.UpdateVisiblity_Holster()) : this.UpdateVisibility_Hand()) : this.UpdateVisiblity_Invis()))
      return;
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public bool UpdateVisibility_Hand()
  {
    if (this.currentVisState == HeldEntity.heldEntityVisState.Hand)
      return false;
    this.currentVisState = HeldEntity.heldEntityVisState.Hand;
    this.limitNetworking = false;
    this.SetFlag(BaseEntity.Flags.Disabled, false, false, true);
    this.SetParent((BaseEntity) this.GetOwnerPlayer(), this.GetBone(this.handBone), false, false);
    return true;
  }

  public bool UpdateVisiblity_Holster()
  {
    if (this.currentVisState == HeldEntity.heldEntityVisState.Holster)
      return false;
    this.currentVisState = HeldEntity.heldEntityVisState.Holster;
    this.limitNetworking = false;
    this.SetFlag(BaseEntity.Flags.Disabled, false, false, true);
    this.SetParent((BaseEntity) this.GetOwnerPlayer(), this.GetBone(this.holsterInfo.holsterBone), false, false);
    this.SetLightsOn(false);
    return true;
  }

  public bool UpdateVisiblity_Invis()
  {
    if (this.currentVisState == HeldEntity.heldEntityVisState.Invis)
      return false;
    this.currentVisState = HeldEntity.heldEntityVisState.Invis;
    this.SetParent((BaseEntity) this.GetOwnerPlayer(), this.GetBone(this.handBone), false, false);
    this.limitNetworking = true;
    this.SetFlag(BaseEntity.Flags.Disabled, true, false, true);
    return true;
  }

  public virtual void SetHeld(bool bHeld)
  {
    Assert.IsTrue(this.isServer, "Should be server!");
    this.SetFlag(BaseEntity.Flags.Reserved4, bHeld, false, true);
    if (!bHeld)
      this.UpdateVisiblity_Invis();
    this.limitNetworking = !bHeld;
    this.SetFlag(BaseEntity.Flags.Disabled, !bHeld, false, true);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    this.OnHeldChanged();
  }

  public virtual void OnHeldChanged()
  {
  }

  public virtual bool CanBeUsedInWater()
  {
    return false;
  }

  protected Item GetOwnerItem()
  {
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (Object.op_Equality((Object) ownerPlayer, (Object) null) || Object.op_Equality((Object) ownerPlayer.inventory, (Object) null))
      return (Item) null;
    return ownerPlayer.inventory.FindItemUID(this.ownerItemUID);
  }

  public override Item GetItem()
  {
    return this.GetOwnerItem();
  }

  public ItemDefinition GetOwnerItemDefinition()
  {
    Item ownerItem = this.GetOwnerItem();
    if (ownerItem != null)
      return ownerItem.info;
    Debug.LogWarning((object) "GetOwnerItem - null!", (Object) this);
    return (ItemDefinition) null;
  }

  public virtual void CollectedForCrafting(Item item, BasePlayer crafter)
  {
  }

  public virtual void ReturnedFromCancelledCraft(Item item, BasePlayer crafter)
  {
  }

  public virtual void ServerCommand(Item item, string command, BasePlayer player)
  {
  }

  public virtual void SetupHeldEntity(Item item)
  {
    this.ownerItemUID = item.uid;
    this.InitOwnerPlayer();
  }

  public override void PostServerLoad()
  {
    this.InitOwnerPlayer();
  }

  private void InitOwnerPlayer()
  {
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (Object.op_Inequality((Object) ownerPlayer, (Object) null))
      this.SetOwnerPlayer(ownerPlayer);
    else
      this.ClearOwnerPlayer();
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.heldEntity = (__Null) Pool.Get<HeldEntity>();
    ((HeldEntity) info.msg.heldEntity).itemUID = (__Null) (int) this.ownerItemUID;
  }

  public void DestroyThis()
  {
    this.GetOwnerItem()?.Remove(0.0f);
  }

  protected bool HasItemAmount()
  {
    Item ownerItem = this.GetOwnerItem();
    if (ownerItem != null)
      return ownerItem.amount > 0;
    return false;
  }

  protected bool UseItemAmount(int iAmount)
  {
    if (iAmount <= 0)
      return true;
    Item ownerItem = this.GetOwnerItem();
    if (ownerItem == null)
    {
      this.DestroyThis();
      return true;
    }
    ownerItem.amount -= iAmount;
    ownerItem.MarkDirty();
    if (ownerItem.amount > 0)
      return false;
    this.DestroyThis();
    return true;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.heldEntity == null)
      return;
    this.ownerItemUID = (uint) ((HeldEntity) info.msg.heldEntity).itemUID;
  }

  public void SendPunch(Vector3 amount, float duration)
  {
    this.ClientRPCPlayer<Vector3, float>((Connection) null, this.GetOwnerPlayer(), "CL_Punch", amount, duration);
  }

  [Serializable]
  public class HolsterInfo
  {
    public string holsterBone = "spine3";
    public HeldEntity.HolsterInfo.HolsterSlot slot;
    public bool displayWhenHolstered;
    public Vector3 holsterOffset;
    public Vector3 holsterRotationOffset;

    public enum HolsterSlot
    {
      BACK,
      RIGHT_THIGH,
      LEFT_THIGH,
    }
  }

  public static class HeldEntityFlags
  {
    public const BaseEntity.Flags Deployed = BaseEntity.Flags.Reserved4;
    public const BaseEntity.Flags LightsOn = BaseEntity.Flags.Reserved5;
  }

  public enum heldEntityVisState
  {
    UNSET,
    Invis,
    Hand,
    Holster,
  }
}
