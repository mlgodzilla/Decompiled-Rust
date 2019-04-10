// Decompiled with JetBrains decompiler
// Type: ElectricWindmill
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using ProtoBuf;
using System;
using UnityEngine;

public class ElectricWindmill : IOEntity
{
  public int maxPowerGeneration = 100;
  public Animator animator;
  public Transform vaneRot;
  public SoundDefinition wooshSound;
  public Transform wooshOrigin;
  private float serverWindSpeed;

  public override bool IsRootEntity()
  {
    return true;
  }

  public float GetWindSpeedScale()
  {
    float num1 = Time.get_time() / 600f;
    double num2 = ((Component) this).get_transform().get_position().x / 512.0;
    float num3 = (float) (((Component) this).get_transform().get_position().z / 512.0);
    double num4 = (double) num1;
    float num5 = Mathf.PerlinNoise((float) (num2 + num4), num3 + num1 * 0.1f);
    float num6 = (float) ((Component) this).get_transform().get_position().y - TerrainMeta.HeightMap.GetHeight(((Component) this).get_transform().get_position());
    if ((double) num6 < 0.0)
      num6 = 0.0f;
    return Mathf.Clamp01(Mathf.InverseLerp(0.0f, 50f, num6) * 0.5f + num5);
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.InvokeRandomized(new Action(this.WindUpdate), 1f, 20f, 2f);
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    if (info.forDisk)
      return;
    if (info.msg.ioEntity == null)
      info.msg.ioEntity = (__Null) Pool.Get<IOEntity>();
    ((IOEntity) info.msg.ioEntity).genericFloat1 = (__Null) (double) Time.get_time();
    ((IOEntity) info.msg.ioEntity).genericFloat2 = (__Null) (double) this.serverWindSpeed;
  }

  public bool AmIVisible()
  {
    int num = 15;
    Vector3 vector3 = Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_up(), 6f));
    if (!this.IsVisible(Vector3.op_Addition(vector3, Vector3.op_Multiply(((Component) this).get_transform().get_up(), (float) num)), (float) (num + 1)))
      return false;
    Vector3 windAimDir = this.GetWindAimDir(Time.get_time());
    return this.IsVisible(Vector3.op_Addition(vector3, Vector3.op_Multiply(windAimDir, (float) num)), (float) (num + 1));
  }

  public void WindUpdate()
  {
    this.serverWindSpeed = this.GetWindSpeedScale();
    if (!this.AmIVisible())
      this.serverWindSpeed = 0.0f;
    int num1 = Mathf.FloorToInt((float) this.maxPowerGeneration * this.serverWindSpeed);
    int num2 = this.currentEnergy != num1 ? 1 : 0;
    this.currentEnergy = num1;
    if (num2 != 0)
      this.MarkDirty();
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public Vector3 GetWindAimDir(float time)
  {
    float num1 = (float) ((double) time / 3600.0 * 360.0);
    int num2 = 10;
    Vector3 vector3;
    ((Vector3) ref vector3).\u002Ector(Mathf.Sin(num1 * ((float) Math.PI / 180f)) * (float) num2, 0.0f, Mathf.Cos(num1 * ((float) Math.PI / 180f)) * (float) num2);
    return ((Vector3) ref vector3).get_normalized();
  }
}
