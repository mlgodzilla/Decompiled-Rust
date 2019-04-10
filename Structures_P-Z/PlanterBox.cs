// Decompiled with JetBrains decompiler
// Type: PlanterBox
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using ProtoBuf;
using UnityEngine;

public class PlanterBox : BaseCombatEntity, ISplashable
{
  public int soilSaturationMax = 8000;
  public int soilSaturation;
  public MeshRenderer soilRenderer;

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.resource = (__Null) Pool.Get<BaseResource>();
    ((BaseResource) info.msg.resource).stage = (__Null) this.soilSaturation;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.resource == null)
      return;
    this.soilSaturation = (int) ((BaseResource) info.msg.resource).stage;
  }

  public float soilSaturationFraction
  {
    get
    {
      return (float) this.soilSaturation / (float) this.soilSaturationMax;
    }
  }

  public int UseWater(int amount)
  {
    int num = Mathf.Min(amount, this.soilSaturation);
    this.soilSaturation -= num;
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    return num;
  }

  public bool wantsSplash(ItemDefinition splashType, int amount)
  {
    if (!(splashType.shortname == "water.salt"))
      return this.soilSaturation < this.soilSaturationMax;
    return true;
  }

  public int DoSplash(ItemDefinition splashType, int amount)
  {
    if (splashType.shortname == "water.salt")
    {
      this.soilSaturation = 0;
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
      return amount;
    }
    int num = Mathf.Min(this.soilSaturationMax - this.soilSaturation, amount);
    this.soilSaturation += num;
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    return num;
  }
}
