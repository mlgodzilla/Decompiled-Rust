// Decompiled with JetBrains decompiler
// Type: Recycler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using Oxide.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Recycler : StorageContainer
{
  public float recycleEfficiency = 0.5f;
  public SoundDefinition grindingLoopDef;
  public GameObjectRef startSound;
  public GameObjectRef stopSound;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("Recycler.OnRpcMessage", 0.1f))
    {
      if (rpc == 4167839872U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SVSwitch "));
          using (TimeWarning.New("SVSwitch", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.MaxDistance.Test("SVSwitch", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.SVSwitch(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in SVSwitch");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.MaxDistance(3f)]
  private void SVSwitch(BaseEntity.RPCMessage msg)
  {
    bool flag = msg.read.Bit();
    if (flag == this.IsOn() || Object.op_Equality((Object) msg.player, (Object) null) || Interface.CallHook("OnRecyclerToggle", (object) this, (object) msg.player) != null || flag && !this.HasRecyclable())
      return;
    if (flag)
    {
      foreach (Item obj in this.inventory.itemList)
        obj.CollectedForCrafting(msg.player);
      this.StartRecycling();
    }
    else
      this.StopRecycling();
  }

  public bool MoveItemToOutput(Item newItem)
  {
    int iTargetPos = -1;
    for (int slot1 = 6; slot1 < 12; ++slot1)
    {
      Item slot2 = this.inventory.GetSlot(slot1);
      if (slot2 == null)
      {
        iTargetPos = slot1;
        break;
      }
      if (slot2.CanStack(newItem))
      {
        if (slot2.amount + newItem.amount <= slot2.info.stackable)
        {
          iTargetPos = slot1;
          break;
        }
        int amountToConsume = Mathf.Min(slot2.info.stackable - slot2.amount, newItem.amount);
        newItem.UseItem(amountToConsume);
        slot2.amount += amountToConsume;
        slot2.MarkDirty();
        newItem.MarkDirty();
      }
      if (newItem.amount <= 0)
        return true;
    }
    if (iTargetPos != -1 && newItem.MoveToContainer(this.inventory, iTargetPos, true))
      return true;
    newItem.Drop(Vector3.op_Addition(((Component) this).get_transform().get_position(), new Vector3(0.0f, 2f, 0.0f)), Vector3.op_Addition(this.GetInheritedDropVelocity(), Vector3.op_Multiply(((Component) this).get_transform().get_forward(), 2f)), (Quaternion) null);
    return false;
  }

  public bool HasRecyclable()
  {
    for (int slot1 = 0; slot1 < 6; ++slot1)
    {
      Item slot2 = this.inventory.GetSlot(slot1);
      if (slot2 != null)
      {
        object obj = Interface.CallHook("CanRecycle", (object) this, (object) slot2);
        if (obj is bool)
          return (bool) obj;
        if (Object.op_Inequality((Object) slot2.info.Blueprint, (Object) null))
          return true;
      }
    }
    return false;
  }

  public void RecycleThink()
  {
    bool flag = false;
    float num1 = this.recycleEfficiency;
    for (int slot1 = 0; slot1 < 6; ++slot1)
    {
      Item slot2 = this.inventory.GetSlot(slot1);
      if (slot2 != null)
      {
        if (Interface.CallHook("OnRecycleItem", (object) this, (object) slot2) != null)
          return;
        if (Object.op_Inequality((Object) slot2.info.Blueprint, (Object) null))
        {
          if (slot2.hasCondition)
            num1 = Mathf.Clamp01(num1 * Mathf.Clamp(slot2.conditionNormalized * slot2.maxConditionNormalized, 0.1f, 1f));
          int amountToConsume = 1;
          if (slot2.amount > 1)
            amountToConsume = Mathf.CeilToInt(Mathf.Min((float) slot2.amount, (float) slot2.info.stackable * 0.1f));
          if (slot2.info.Blueprint.scrapFromRecycle > 0)
          {
            int iAmount = slot2.info.Blueprint.scrapFromRecycle * amountToConsume;
            if (slot2.info.stackable == 1 && slot2.hasCondition)
              iAmount = Mathf.CeilToInt((float) iAmount * slot2.conditionNormalized);
            if (iAmount >= 1)
              this.MoveItemToOutput(ItemManager.CreateByName("scrap", iAmount, 0UL));
          }
          slot2.UseItem(amountToConsume);
          using (List<ItemAmount>.Enumerator enumerator = slot2.info.Blueprint.ingredients.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              ItemAmount current = enumerator.Current;
              if (!(current.itemDef.shortname == "scrap"))
              {
                float num2 = current.amount / (float) slot2.info.Blueprint.amountToCreate;
                int num3 = 0;
                if ((double) num2 <= 1.0)
                {
                  for (int index = 0; index < amountToConsume; ++index)
                  {
                    if ((double) Random.Range(0.0f, 1f) <= (double) num2 * (double) num1)
                      ++num3;
                  }
                }
                else
                  num3 = Mathf.CeilToInt(Mathf.Clamp(num2 * num1 * Random.Range(1f, 1f), 0.0f, current.amount) * (float) amountToConsume);
                if (num3 > 0)
                {
                  int num4 = Mathf.CeilToInt((float) num3 / (float) current.itemDef.stackable);
                  for (int index = 0; index < num4; ++index)
                  {
                    int iAmount = num3 > current.itemDef.stackable ? current.itemDef.stackable : num3;
                    if (!this.MoveItemToOutput(ItemManager.Create(current.itemDef, iAmount, 0UL)))
                      flag = true;
                    num3 -= iAmount;
                    if (num3 <= 0)
                      break;
                  }
                }
              }
            }
            break;
          }
        }
      }
    }
    if (!flag && this.HasRecyclable())
      return;
    this.StopRecycling();
  }

  public void StartRecycling()
  {
    if (this.IsOn())
      return;
    this.InvokeRepeating(new Action(this.RecycleThink), 5f, 5f);
    Effect.server.Run(this.startSound.resourcePath, (BaseEntity) this, 0U, Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
    this.SetFlag(BaseEntity.Flags.On, true, false, true);
    this.SendNetworkUpdateImmediate(false);
  }

  public void StopRecycling()
  {
    this.CancelInvoke(new Action(this.RecycleThink));
    if (!this.IsOn())
      return;
    Effect.server.Run(this.stopSound.resourcePath, (BaseEntity) this, 0U, Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
    this.SendNetworkUpdateImmediate(false);
  }
}
