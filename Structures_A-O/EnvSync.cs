// Decompiled with JetBrains decompiler
// Type: EnvSync
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using ProtoBuf;
using System;
using UnityEngine;

public class EnvSync : PointEntity
{
  private const float syncInterval = 5f;
  private const float syncIntervalInv = 0.2f;

  public override void ServerInit()
  {
    base.ServerInit();
    this.InvokeRepeating(new Action(this.UpdateNetwork), 5f, 5f);
  }

  private void UpdateNetwork()
  {
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.environment = (__Null) Pool.Get<Environment>();
    if (Object.op_Implicit((Object) TOD_Sky.get_Instance()))
      ((Environment) info.msg.environment).dateTime = (__Null) ((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).get_DateTime().ToBinary();
    if (Object.op_Implicit((Object) SingletonComponent<Climate>.Instance))
    {
      ((Environment) info.msg.environment).clouds = (__Null) (double) ((Climate) SingletonComponent<Climate>.Instance).Overrides.Clouds;
      ((Environment) info.msg.environment).fog = (__Null) (double) ((Climate) SingletonComponent<Climate>.Instance).Overrides.Fog;
      ((Environment) info.msg.environment).wind = (__Null) (double) ((Climate) SingletonComponent<Climate>.Instance).Overrides.Wind;
      ((Environment) info.msg.environment).rain = (__Null) (double) ((Climate) SingletonComponent<Climate>.Instance).Overrides.Rain;
    }
    ((Environment) info.msg.environment).engineTime = (__Null) (double) Time.get_realtimeSinceStartup();
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.environment == null || !Object.op_Implicit((Object) TOD_Sky.get_Instance()))
      return;
    ((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).set_DateTime(DateTime.FromBinary((long) ((Environment) info.msg.environment).dateTime));
  }
}
