// Decompiled with JetBrains decompiler
// Type: BaseLiquidVessel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseLiquidVessel : AttackEntity
{
  public float throwScale = 10f;
  public float fillMlPerSec = 500f;
  [Header("Liquid Vessel")]
  public GameObjectRef thrownWaterObject;
  public GameObjectRef ThrowEffect3P;
  public SoundDefinition throwSound3P;
  public GameObjectRef fillFromContainer;
  public GameObjectRef fillFromWorld;
  public bool hasLid;
  public bool canDrinkFrom;
  public bool updateVMWater;
  public float minThrowFrac;
  public bool useThrowAnim;
  private float lastFillTime;
  private float nextFreeTime;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("BaseLiquidVessel.OnRpcMessage", 0.1f))
    {
      if (rpc == 4013436649U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - DoDrink "));
        using (TimeWarning.New("DoDrink", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsActiveItem.Test("DoDrink", (BaseEntity) this, player))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.DoDrink(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in DoDrink");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 2781345828U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SendFilling "));
        using (TimeWarning.New("SendFilling", 0.1f))
        {
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.SendFilling(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in SendFilling");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 3038767821U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - ThrowContents "));
          using (TimeWarning.New("ThrowContents", 0.1f))
          {
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.ThrowContents(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in ThrowContents");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.InvokeRepeating(new Action(this.FillCheck), 1f, 1f);
  }

  public override void OnHeldChanged()
  {
    base.OnHeldChanged();
    if (this.IsDisabled())
      this.StopFilling();
    if (this.hasLid)
      return;
    this.DoThrow(((Component) this).get_transform().get_position(), Vector3.get_zero());
    Item obj = this.GetItem();
    if (obj == null)
      return;
    obj.contents.SetLocked(this.IsDisabled());
    this.SendNetworkUpdateImmediate(false);
  }

  public void SetFilling(bool isFilling)
  {
    this.SetFlag(BaseEntity.Flags.Open, isFilling, false, true);
    if (isFilling)
      this.StartFilling();
    else
      this.StopFilling();
  }

  public void StartFilling()
  {
    double num = (double) Time.get_realtimeSinceStartup() - (double) this.lastFillTime;
    this.StopFilling();
    this.InvokeRepeating(new Action(this.FillCheck), 0.0f, 0.3f);
    if (num > 1.0)
    {
      LiquidContainer facingLiquidContainer = this.GetFacingLiquidContainer();
      if (Object.op_Inequality((Object) facingLiquidContainer, (Object) null) && facingLiquidContainer.GetLiquidItem() != null)
        Effect.server.Run(this.fillFromContainer.resourcePath, ((Component) facingLiquidContainer).get_transform().get_position(), Vector3.get_up(), (Connection) null, false);
      else if (this.CanFillFromWorld())
        Effect.server.Run(this.fillFromWorld.resourcePath, (BaseEntity) this.GetOwnerPlayer(), 0U, Vector3.get_zero(), Vector3.get_up(), (Connection) null, false);
    }
    this.lastFillTime = Time.get_realtimeSinceStartup();
  }

  public void StopFilling()
  {
    this.CancelInvoke(new Action(this.FillCheck));
  }

  public void FillCheck()
  {
    if (this.isClient)
      return;
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (!Object.op_Implicit((Object) ownerPlayer))
      return;
    float num1 = Time.get_realtimeSinceStartup() - this.lastFillTime;
    Vector3 pos = Vector3.op_Subtraction(((Component) ownerPlayer).get_transform().get_position(), new Vector3(0.0f, 1f, 0.0f));
    if (this.CanFillFromWorld())
    {
      this.AddLiquid(WaterResource.GetAtPoint(pos), Mathf.FloorToInt(num1 * this.fillMlPerSec));
    }
    else
    {
      LiquidContainer facingLiquidContainer = this.GetFacingLiquidContainer();
      if (!Object.op_Inequality((Object) facingLiquidContainer, (Object) null) || !facingLiquidContainer.HasLiquidItem())
        return;
      int num2 = Mathf.CeilToInt((1f - this.HeldFraction()) * (float) this.MaxHoldable());
      if (num2 <= 0)
        return;
      Item liquidItem = facingLiquidContainer.GetLiquidItem();
      int num3 = Mathf.Min(Mathf.CeilToInt(num1 * this.fillMlPerSec), Mathf.Min(liquidItem.amount, num2));
      this.AddLiquid(liquidItem.info, num3);
      liquidItem.UseItem(num3);
      facingLiquidContainer.OpenTap(2f);
    }
  }

  public void LoseWater(int amount)
  {
    Item slot = this.GetItem().contents.GetSlot(0);
    if (slot == null)
      return;
    slot.UseItem(amount);
    slot.MarkDirty();
    this.SendNetworkUpdateImmediate(false);
  }

  public void AddLiquid(ItemDefinition liquidType, int amount)
  {
    if (amount <= 0)
      return;
    Item obj = this.GetItem();
    Item slot = obj.contents.GetSlot(0);
    ItemModContainer component = (ItemModContainer) ((Component) obj.info).GetComponent<ItemModContainer>();
    if (slot == null)
    {
      ItemManager.Create(liquidType, amount, 0UL)?.MoveToContainer(obj.contents, -1, true);
    }
    else
    {
      int iAmount = Mathf.Clamp(slot.amount + amount, 0, component.maxStackSize);
      ItemDefinition template = WaterResource.Merge(slot.info, liquidType);
      if (Object.op_Inequality((Object) template, (Object) slot.info))
      {
        slot.Remove(0.0f);
        slot = ItemManager.Create(template, iAmount, 0UL);
        slot.MoveToContainer(obj.contents, -1, true);
      }
      else
        slot.amount = iAmount;
      slot.MarkDirty();
      this.SendNetworkUpdateImmediate(false);
    }
  }

  public int AmountHeld()
  {
    Item slot = this.GetItem().contents.GetSlot(0);
    if (slot == null)
      return 0;
    return slot.amount;
  }

  public float HeldFraction()
  {
    return (float) this.AmountHeld() / (float) this.MaxHoldable();
  }

  public int MaxHoldable()
  {
    return ((ItemModContainer) ((Component) this.GetItem().info).GetComponent<ItemModContainer>()).maxStackSize;
  }

  public bool CanDrink()
  {
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (!Object.op_Implicit((Object) ownerPlayer) || !ownerPlayer.metabolism.CanConsume() || !this.canDrinkFrom)
      return false;
    Item obj = this.GetItem();
    return obj != null && obj.contents != null && (obj.contents.itemList != null && obj.contents.itemList.Count != 0);
  }

  private bool IsWeaponBusy()
  {
    return (double) Time.get_realtimeSinceStartup() < (double) this.nextFreeTime;
  }

  private void SetBusyFor(float dur)
  {
    this.nextFreeTime = Time.get_realtimeSinceStartup() + dur;
  }

  private void ClearBusy()
  {
    this.nextFreeTime = Time.get_realtimeSinceStartup() - 1f;
  }

  [BaseEntity.RPC_Server.IsActiveItem]
  [BaseEntity.RPC_Server]
  private void DoDrink(BaseEntity.RPCMessage msg)
  {
    if (!msg.player.CanInteract())
      return;
    Item obj1 = this.GetItem();
    if (obj1 == null || obj1.contents == null || !msg.player.metabolism.CanConsume())
      return;
    foreach (Item obj2 in obj1.contents.itemList)
    {
      ItemModConsume component = (ItemModConsume) ((Component) obj2.info).GetComponent<ItemModConsume>();
      if (!Object.op_Equality((Object) component, (Object) null) && component.CanDoAction(obj2, msg.player))
      {
        component.DoAction(obj2, msg.player);
        break;
      }
    }
  }

  [BaseEntity.RPC_Server]
  private void ThrowContents(BaseEntity.RPCMessage msg)
  {
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (Object.op_Equality((Object) ownerPlayer, (Object) null))
      return;
    this.DoThrow(Vector3.op_Addition(ownerPlayer.eyes.position, Vector3.op_Multiply(ownerPlayer.eyes.BodyForward(), 1f)), Vector3.op_Addition(ownerPlayer.estimatedVelocity, Vector3.op_Multiply(ownerPlayer.eyes.BodyForward(), this.throwScale)));
    Effect.server.Run(this.ThrowEffect3P.resourcePath, ((Component) ownerPlayer).get_transform().get_position(), ownerPlayer.eyes.BodyForward(), ownerPlayer.net.get_connection(), false);
  }

  public void DoThrow(Vector3 pos, Vector3 velocity)
  {
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (Object.op_Equality((Object) ownerPlayer, (Object) null))
      return;
    Item obj = this.GetItem();
    if (obj == null || obj.contents == null)
      return;
    Item slot = obj.contents.GetSlot(0);
    if (slot == null || slot.amount <= 0)
      return;
    Vector3 pos1 = Vector3.op_Addition(ownerPlayer.eyes.position, Vector3.op_Multiply(ownerPlayer.eyes.BodyForward(), 1f));
    WaterBall entity = GameManager.server.CreateEntity(this.thrownWaterObject.resourcePath, pos1, Quaternion.get_identity(), true) as WaterBall;
    if (Object.op_Implicit((Object) entity))
    {
      entity.liquidType = slot.info;
      entity.waterAmount = slot.amount;
      ((Component) entity).get_transform().set_position(pos1);
      entity.SetVelocity(velocity);
      entity.Spawn();
    }
    slot.UseItem(slot.amount);
    slot.MarkDirty();
    this.SendNetworkUpdateImmediate(false);
  }

  [BaseEntity.RPC_Server]
  private void SendFilling(BaseEntity.RPCMessage msg)
  {
    this.SetFilling(msg.read.Bit());
  }

  public bool CanFillFromWorld()
  {
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (!Object.op_Implicit((Object) ownerPlayer))
      return false;
    return (double) ownerPlayer.WaterFactor() >= 0.0500000007450581;
  }

  public bool CanThrow()
  {
    return (double) this.HeldFraction() > (double) this.minThrowFrac;
  }

  public LiquidContainer GetFacingLiquidContainer()
  {
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (!Object.op_Implicit((Object) ownerPlayer))
      return (LiquidContainer) null;
    RaycastHit hit;
    if (Physics.Raycast(ownerPlayer.eyes.HeadRay(), ref hit, 2f, 1236478737))
    {
      BaseEntity entity = hit.GetEntity();
      if (Object.op_Implicit((Object) entity) && !((Component) ((RaycastHit) ref hit).get_collider()).get_gameObject().CompareTag("Not Player Usable") && !((Component) ((RaycastHit) ref hit).get_collider()).get_gameObject().CompareTag("Usable Primary"))
        return (LiquidContainer) ((Component) entity.ToServer<BaseEntity>()).GetComponent<LiquidContainer>();
    }
    return (LiquidContainer) null;
  }
}
