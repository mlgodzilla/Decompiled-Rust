// Decompiled with JetBrains decompiler
// Type: CeilingLight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Network;
using Rust;
using System.Collections.Generic;
using UnityEngine;

public class CeilingLight : IOEntity
{
  public float pushScale = 2f;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("CeilingLight.OnRpcMessage", 0.1f))
      ;
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override void Hurt(HitInfo info)
  {
    if (!this.isServer)
      return;
    if (info.damageTypes.Has(DamageType.Explosion))
      this.ClientRPC<int, Vector3, Vector3>((Connection) null, "ClientPhysPush", 0, Vector3.op_Multiply(Vector3.op_Multiply(info.attackNormal, 3f), info.damageTypes.Total() / 50f), info.HitPositionWorld);
    base.Hurt(info);
  }

  public void RefreshPlants()
  {
    List<PlantEntity> list = (List<PlantEntity>) Pool.GetList<PlantEntity>();
    Vis.Entities<PlantEntity>(Vector3.op_Addition(((Component) this).get_transform().get_position(), new Vector3(0.0f, -2f, 0.0f)), 5f, list, 512, (QueryTriggerInteraction) 2);
    foreach (PlantEntity plantEntity in list)
      plantEntity.RefreshLightExposure();
    // ISSUE: cast to a reference type
    Pool.FreeList<PlantEntity>((List<M0>&) ref list);
  }

  public override int ConsumptionAmount()
  {
    if (this.IsOn())
      return 2;
    return base.ConsumptionAmount();
  }

  public override void IOStateChanged(int inputAmount, int inputSlot)
  {
    base.IOStateChanged(inputAmount, inputSlot);
    int num1 = this.IsOn() ? 1 : 0;
    this.SetFlag(BaseEntity.Flags.On, this.IsPowered(), false, true);
    int num2 = this.IsOn() ? 1 : 0;
    if (num1 == num2)
      return;
    if (this.IsOn())
      this.LightsOn();
    else
      this.LightsOff();
  }

  public void LightsOn()
  {
    this.RefreshPlants();
  }

  public void LightsOff()
  {
    this.RefreshPlants();
  }

  public override void OnKilled(HitInfo info)
  {
    base.OnKilled(info);
    this.RefreshPlants();
  }

  public override void OnAttacked(HitInfo info)
  {
    float num = (float) (3.0 * ((double) info.damageTypes.Total() / 50.0));
    this.ClientRPC<uint, Vector3, Vector3>((Connection) null, "ClientPhysPush", !Object.op_Inequality((Object) info.Initiator, (Object) null) || !(info.Initiator is BasePlayer) || info.IsPredicting ? 0U : (uint) (int) info.Initiator.net.ID, Vector3.op_Multiply(info.attackNormal, num), info.HitPositionWorld);
    base.OnAttacked(info);
  }
}
