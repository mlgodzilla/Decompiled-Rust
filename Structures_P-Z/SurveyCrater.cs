// Decompiled with JetBrains decompiler
// Type: SurveyCrater
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class SurveyCrater : BaseCombatEntity
{
  private ResourceDispenser resourceDispenser;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("SurveyCrater.OnRpcMessage", 0.1f))
    {
      if (rpc == 3491246334U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - AnalysisComplete "));
          using (TimeWarning.New("AnalysisComplete", 0.1f))
          {
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.AnalysisComplete(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in AnalysisComplete");
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
    this.Invoke(new Action(this.RemoveMe), 1800f);
  }

  public override void OnAttacked(HitInfo info)
  {
    int num = this.isServer ? 1 : 0;
    base.OnAttacked(info);
  }

  public void RemoveMe()
  {
    this.Kill(BaseNetworkable.DestroyMode.None);
  }

  [BaseEntity.RPC_Server]
  public void AnalysisComplete(BaseEntity.RPCMessage msg)
  {
    ResourceDepositManager.ResourceDeposit resourceDeposit = ResourceDepositManager.GetOrCreate(((Component) this).get_transform().get_position());
    if (resourceDeposit == null)
      return;
    Item byName = ItemManager.CreateByName("note", 1, 0UL);
    byName.text = "-Mineral Analysis-\n\n";
    float num1 = 10f;
    float num2 = 7.5f;
    foreach (ResourceDepositManager.ResourceDeposit.ResourceDepositEntry resource in resourceDeposit._resources)
    {
      float num3 = (float) (60.0 / (double) num1 * ((double) num2 / (double) resource.workNeeded));
      Item obj = byName;
      obj.text = obj.text + resource.type.displayName.english + " : " + num3.ToString("0.0") + " pM\n";
    }
    byName.MarkDirty();
    msg.player.GiveItem(byName, BaseEntity.GiveItemReason.PickedUp);
  }

  public override float BoundsPadding()
  {
    return 2f;
  }
}
