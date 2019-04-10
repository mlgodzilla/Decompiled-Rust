// Decompiled with JetBrains decompiler
// Type: BuildingPrivlidge
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class BuildingPrivlidge : StorageContainer
{
  private static BuildingPrivlidge.UpkeepBracket[] upkeepBrackets = new BuildingPrivlidge.UpkeepBracket[4]
  {
    new BuildingPrivlidge.UpkeepBracket(ConVar.Decay.bracket_0_blockcount, ConVar.Decay.bracket_0_costfraction),
    new BuildingPrivlidge.UpkeepBracket(ConVar.Decay.bracket_1_blockcount, ConVar.Decay.bracket_1_costfraction),
    new BuildingPrivlidge.UpkeepBracket(ConVar.Decay.bracket_2_blockcount, ConVar.Decay.bracket_2_costfraction),
    new BuildingPrivlidge.UpkeepBracket(ConVar.Decay.bracket_3_blockcount, ConVar.Decay.bracket_3_costfraction)
  };
  public List<PlayerNameID> authorizedPlayers = new List<PlayerNameID>();
  private List<ItemAmount> upkeepBuffer = new List<ItemAmount>();
  private float cachedProtectedMinutes;
  private float nextProtectedCalcTime;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("BuildingPrivlidge.OnRpcMessage", 0.1f))
    {
      if (rpc == 1092560690U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - AddSelfAuthorize "));
        using (TimeWarning.New("AddSelfAuthorize", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("AddSelfAuthorize", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.AddSelfAuthorize(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in AddSelfAuthorize");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 253307592U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - ClearList "));
        using (TimeWarning.New("ClearList", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("ClearList", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.ClearList(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in ClearList");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 3617985969U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RemoveSelfAuthorize "));
        using (TimeWarning.New("RemoveSelfAuthorize", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("RemoveSelfAuthorize", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RemoveSelfAuthorize(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RemoveSelfAuthorize");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 2051750736U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_Rotate "));
          using (TimeWarning.New("RPC_Rotate", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_Rotate", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_Rotate(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_Rotate");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override void ResetState()
  {
    base.ResetState();
    this.authorizedPlayers.Clear();
  }

  public bool IsAuthed(BasePlayer player)
  {
    return ((IEnumerable<PlayerNameID>) this.authorizedPlayers).Any<PlayerNameID>((Func<PlayerNameID, bool>) (x => x.userid == (long) player.userID));
  }

  public bool AnyAuthed()
  {
    return this.authorizedPlayers.Count > 0;
  }

  public override bool ItemFilter(Item item, int targetSlot)
  {
    return base.ItemFilter(item, targetSlot);
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.buildingPrivilege = (__Null) Pool.Get<BuildingPrivilege>();
    ((BuildingPrivilege) info.msg.buildingPrivilege).users = (__Null) this.authorizedPlayers;
    if (info.forDisk)
      return;
    ((BuildingPrivilege) info.msg.buildingPrivilege).upkeepPeriodMinutes = (__Null) (double) this.CalculateUpkeepPeriodMinutes();
    ((BuildingPrivilege) info.msg.buildingPrivilege).costFraction = (__Null) (double) this.CalculateUpkeepCostFraction();
    ((BuildingPrivilege) info.msg.buildingPrivilege).protectedMinutes = (__Null) (double) this.GetProtectedMinutes(false);
  }

  public override void PostSave(BaseNetworkable.SaveInfo info)
  {
    ((BuildingPrivilege) info.msg.buildingPrivilege).users = null;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    this.authorizedPlayers.Clear();
    if (info.msg.buildingPrivilege == null || ((BuildingPrivilege) info.msg.buildingPrivilege).users == null)
      return;
    this.authorizedPlayers = (List<PlayerNameID>) ((BuildingPrivilege) info.msg.buildingPrivilege).users;
    if (!info.fromDisk)
      this.cachedProtectedMinutes = (float) ((BuildingPrivilege) info.msg.buildingPrivilege).protectedMinutes;
    ((BuildingPrivilege) info.msg.buildingPrivilege).users = null;
  }

  public void BuildingDirty()
  {
    if (!this.isServer)
      return;
    this.AddDelayedUpdate();
  }

  protected override void OnInventoryDirty()
  {
    base.OnInventoryDirty();
    this.AddDelayedUpdate();
  }

  public override void OnItemAddedOrRemoved(Item item, bool bAdded)
  {
    base.OnItemAddedOrRemoved(item, bAdded);
    this.AddDelayedUpdate();
  }

  public void AddDelayedUpdate()
  {
    if (this.IsInvoking(new Action(this.DelayedUpdate)))
      this.CancelInvoke(new Action(this.DelayedUpdate));
    this.Invoke(new Action(this.DelayedUpdate), 1f);
  }

  public void DelayedUpdate()
  {
    this.MarkProtectedMinutesDirty(0.0f);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  private bool CanAdministrate(BasePlayer player)
  {
    BaseLock slot = this.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
    if (Object.op_Equality((Object) slot, (Object) null))
      return true;
    return slot.OnTryToOpen(player);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void AddSelfAuthorize(BaseEntity.RPCMessage rpc)
  {
    if (!rpc.player.CanInteract() || !this.CanAdministrate(rpc.player) || Interface.CallHook("OnCupboardAuthorize", (object) this, (object) rpc.player) != null)
      return;
    this.authorizedPlayers.RemoveAll((Predicate<PlayerNameID>) (x => x.userid == (long) rpc.player.userID));
    this.authorizedPlayers.Add(new PlayerNameID()
    {
      userid = (__Null) (long) rpc.player.userID,
      username = (__Null) rpc.player.displayName
    });
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  [BaseEntity.RPC_Server.IsVisible(3f)]
  [BaseEntity.RPC_Server]
  public void RemoveSelfAuthorize(BaseEntity.RPCMessage rpc)
  {
    if (!rpc.player.CanInteract() || !this.CanAdministrate(rpc.player) || Interface.CallHook("OnCupboardDeauthorize", (object) this, (object) rpc.player) != null)
      return;
    this.authorizedPlayers.RemoveAll((Predicate<PlayerNameID>) (x => x.userid == (long) rpc.player.userID));
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void ClearList(BaseEntity.RPCMessage rpc)
  {
    if (!rpc.player.CanInteract() || !this.CanAdministrate(rpc.player) || Interface.CallHook("OnCupboardClearList", (object) this, (object) rpc.player) != null)
      return;
    this.authorizedPlayers.Clear();
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void RPC_Rotate(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    if (player.CanBuild() && Object.op_Implicit((Object) player.GetHeldEntity()) && Object.op_Inequality((Object) ((Component) player.GetHeldEntity()).GetComponent<Hammer>(), (Object) null) && (Object.op_Equality((Object) this.GetSlot(BaseEntity.Slot.Lock), (Object) null) || !this.GetSlot(BaseEntity.Slot.Lock).IsLocked()))
    {
      ((Component) this).get_transform().set_rotation(Quaternion.LookRotation(Vector3.op_UnaryNegation(((Component) this).get_transform().get_forward()), ((Component) this).get_transform().get_up()));
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
      Deployable component = (Deployable) ((Component) this).GetComponent<Deployable>();
      if ((PrefabAttribute) component != (PrefabAttribute) null && component.placeEffect.isValid)
        Effect.server.Run(component.placeEffect.resourcePath, ((Component) this).get_transform().get_position(), Vector3.get_up(), (Connection) null, false);
    }
    BaseEntity slot = this.GetSlot(BaseEntity.Slot.Lock);
    if (!Object.op_Inequality((Object) slot, (Object) null))
      return;
    slot.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public override bool HasSlot(BaseEntity.Slot slot)
  {
    if (slot == BaseEntity.Slot.Lock)
      return true;
    return base.HasSlot(slot);
  }

  public float CalculateUpkeepPeriodMinutes()
  {
    if (this.isServer)
      return ConVar.Decay.upkeep_period_minutes;
    return 0.0f;
  }

  public float CalculateUpkeepCostFraction()
  {
    if (this.isServer)
      return this.CalculateBuildingTaxRate();
    return 0.0f;
  }

  public void CalculateUpkeepCostAmounts(List<ItemAmount> itemAmounts)
  {
    BuildingManager.Building building = this.GetBuilding();
    if (building == null || !building.HasDecayEntities())
      return;
    float upkeepCostFraction = this.CalculateUpkeepCostFraction();
    using (IEnumerator<DecayEntity> enumerator = building.decayEntities.GetEnumerator())
    {
      while (enumerator.MoveNext())
        enumerator.Current.CalculateUpkeepCostAmounts(itemAmounts, upkeepCostFraction);
    }
  }

  public float GetProtectedMinutes(bool force = false)
  {
    if (!this.isServer)
      return 0.0f;
    if (!force && (double) Time.get_realtimeSinceStartup() < (double) this.nextProtectedCalcTime)
      return this.cachedProtectedMinutes;
    this.nextProtectedCalcTime = Time.get_realtimeSinceStartup() + 60f;
    List<ItemAmount> list = (List<ItemAmount>) Pool.GetList<ItemAmount>();
    this.CalculateUpkeepCostAmounts(list);
    float upkeepPeriodMinutes = this.CalculateUpkeepPeriodMinutes();
    float num1 = -1f;
    if (this.inventory != null)
    {
      foreach (ItemAmount itemAmount in list)
      {
        int num2 = this.inventory.FindItemsByItemID(itemAmount.itemid).Sum<Item>((Func<Item, int>) (x => x.amount));
        if (num2 > 0 && (double) itemAmount.amount > 0.0)
        {
          float num3 = (float) num2 / itemAmount.amount * upkeepPeriodMinutes;
          if ((double) num1 == -1.0 || (double) num3 < (double) num1)
            num1 = num3;
        }
        else
          num1 = 0.0f;
      }
      if ((double) num1 == -1.0)
        num1 = 0.0f;
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<ItemAmount>((List<M0>&) ref list);
    this.cachedProtectedMinutes = num1;
    return this.cachedProtectedMinutes;
  }

  public override void OnKilled(HitInfo info)
  {
    if ((double) ConVar.Decay.upkeep_grief_protection > 0.0)
      this.PurchaseUpkeepTime(ConVar.Decay.upkeep_grief_protection * 60f);
    base.OnKilled(info);
  }

  public override void DecayTick()
  {
    if (!this.EnsurePrimary())
      return;
    base.DecayTick();
  }

  private bool EnsurePrimary()
  {
    BuildingManager.Building building = this.GetBuilding();
    if (building != null)
    {
      BuildingPrivlidge buildingPrivilege = building.GetDominatingBuildingPrivilege();
      if (Object.op_Inequality((Object) buildingPrivilege, (Object) null) && Object.op_Inequality((Object) buildingPrivilege, (Object) this))
      {
        this.Kill(BaseNetworkable.DestroyMode.Gib);
        return false;
      }
    }
    return true;
  }

  public void MarkProtectedMinutesDirty(float delay = 0.0f)
  {
    this.nextProtectedCalcTime = Time.get_realtimeSinceStartup() + delay;
  }

  public float CalculateBuildingTaxRate()
  {
    BuildingManager.Building building = this.GetBuilding();
    if (building == null || !building.HasBuildingBlocks())
      return ConVar.Decay.bracket_0_costfraction;
    int count = building.buildingBlocks.get_Count();
    int num1 = count;
    for (int index = 0; index < BuildingPrivlidge.upkeepBrackets.Length; ++index)
    {
      BuildingPrivlidge.UpkeepBracket upkeepBracket = BuildingPrivlidge.upkeepBrackets[index];
      upkeepBracket.blocksTaxPaid = 0.0f;
      if (num1 > 0)
      {
        int num2 = index != BuildingPrivlidge.upkeepBrackets.Length - 1 ? Mathf.Min(num1, BuildingPrivlidge.upkeepBrackets[index].objectsUpTo) : num1;
        num1 -= num2;
        upkeepBracket.blocksTaxPaid = (float) num2 * upkeepBracket.fraction;
      }
    }
    float num3 = 0.0f;
    for (int index = 0; index < BuildingPrivlidge.upkeepBrackets.Length; ++index)
    {
      BuildingPrivlidge.UpkeepBracket upkeepBracket = BuildingPrivlidge.upkeepBrackets[index];
      if ((double) upkeepBracket.blocksTaxPaid > 0.0)
        num3 += upkeepBracket.blocksTaxPaid;
      else
        break;
    }
    return num3 / (float) count;
  }

  private void ApplyUpkeepPayment()
  {
    List<Item> list = (List<Item>) Pool.GetList<Item>();
    for (int index = 0; index < this.upkeepBuffer.Count; ++index)
    {
      ItemAmount itemAmount = this.upkeepBuffer[index];
      int amount = (int) itemAmount.amount;
      if (amount >= 1)
      {
        this.inventory.Take(list, itemAmount.itemid, amount);
        foreach (Item obj in list)
        {
          if (this.IsDebugging())
            Debug.Log((object) (((object) this).ToString() + ": Using " + (object) obj.amount + " of " + obj.info.shortname));
          obj.UseItem(obj.amount);
        }
        list.Clear();
        itemAmount.amount -= (float) amount;
        this.upkeepBuffer[index] = itemAmount;
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<Item>((List<M0>&) ref list);
  }

  private void QueueUpkeepPayment(List<ItemAmount> itemAmounts)
  {
    for (int index = 0; index < itemAmounts.Count; ++index)
    {
      ItemAmount itemAmount1 = itemAmounts[index];
      bool flag = false;
      foreach (ItemAmount itemAmount2 in this.upkeepBuffer)
      {
        if (Object.op_Equality((Object) itemAmount2.itemDef, (Object) itemAmount1.itemDef))
        {
          itemAmount2.amount += itemAmount1.amount;
          if (this.IsDebugging())
            Debug.Log((object) (((object) this).ToString() + ": Adding " + (object) itemAmount1.amount + " of " + itemAmount1.itemDef.shortname + " to " + (object) itemAmount2.amount));
          flag = true;
          break;
        }
      }
      if (!flag)
      {
        if (this.IsDebugging())
          Debug.Log((object) (((object) this).ToString() + ": Adding " + (object) itemAmount1.amount + " of " + itemAmount1.itemDef.shortname));
        this.upkeepBuffer.Add(new ItemAmount(itemAmount1.itemDef, itemAmount1.amount));
      }
    }
  }

  private bool CanAffordUpkeepPayment(List<ItemAmount> itemAmounts)
  {
    for (int index = 0; index < itemAmounts.Count; ++index)
    {
      ItemAmount itemAmount = itemAmounts[index];
      if ((double) this.inventory.GetAmount(itemAmount.itemid, true) < (double) itemAmount.amount)
      {
        if (this.IsDebugging())
          Debug.Log((object) (((object) this).ToString() + ": Can't afford " + (object) itemAmount.amount + " of " + itemAmount.itemDef.shortname));
        return false;
      }
    }
    return true;
  }

  public float PurchaseUpkeepTime(DecayEntity entity, float deltaTime)
  {
    double upkeepCostFraction = (double) this.CalculateUpkeepCostFraction();
    float num1 = this.CalculateUpkeepPeriodMinutes() * 60f;
    double num2 = (double) deltaTime;
    float multiplier = (float) (upkeepCostFraction * num2) / num1;
    List<ItemAmount> list = (List<ItemAmount>) Pool.GetList<ItemAmount>();
    entity.CalculateUpkeepCostAmounts(list, multiplier);
    int num3 = this.CanAffordUpkeepPayment(list) ? 1 : 0;
    this.QueueUpkeepPayment(list);
    // ISSUE: cast to a reference type
    Pool.FreeList<ItemAmount>((List<M0>&) ref list);
    this.ApplyUpkeepPayment();
    if (num3 == 0)
      return 0.0f;
    return deltaTime;
  }

  public void PurchaseUpkeepTime(float deltaTime)
  {
    BuildingManager.Building building = this.GetBuilding();
    if (building == null || !building.HasDecayEntities())
      return;
    float num = Mathf.Min(this.GetProtectedMinutes(true) * 60f, deltaTime);
    if ((double) num <= 0.0)
      return;
    using (IEnumerator<DecayEntity> enumerator = building.decayEntities.GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        DecayEntity current = enumerator.Current;
        float protectedSeconds = current.GetProtectedSeconds();
        if ((double) num > (double) protectedSeconds)
        {
          float time = this.PurchaseUpkeepTime(current, num - protectedSeconds);
          current.AddUpkeepTime(time);
          if (this.IsDebugging())
            Debug.Log((object) (((object) this).ToString() + " purchased upkeep time for " + ((object) current).ToString() + ": " + (object) protectedSeconds + " + " + (object) time + " = " + (object) current.GetProtectedSeconds()));
        }
      }
    }
  }

  public class UpkeepBracket
  {
    public int objectsUpTo;
    public float fraction;
    public float blocksTaxPaid;

    public UpkeepBracket(int numObjs, float frac)
    {
      this.objectsUpTo = numObjs;
      this.fraction = frac;
      this.blocksTaxPaid = 0.0f;
    }
  }
}
