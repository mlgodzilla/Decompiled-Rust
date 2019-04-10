// Decompiled with JetBrains decompiler
// Type: StagedResourceEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Network;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StagedResourceEntity : ResourceEntity
{
  public List<StagedResourceEntity.ResourceStage> stages = new List<StagedResourceEntity.ResourceStage>();
  public int stage;
  public GameObjectRef changeStageEffect;
  public GameObject gibSourceTest;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("StagedResourceEntity.OnRpcMessage", 0.1f))
      ;
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.resource == null)
      return;
    int num = (int) ((BaseResource) info.msg.resource).stage;
    if ((!info.fromDisk ? 0 : (this.isServer ? 1 : 0)) != 0)
    {
      this.health = this.startHealth;
      num = 0;
    }
    if (num == this.stage)
      return;
    this.stage = num;
    this.UpdateStage();
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    if (info.msg.resource == null)
      info.msg.resource = (__Null) Pool.Get<BaseResource>();
    ((BaseResource) info.msg.resource).health = (__Null) (double) this.Health();
    ((BaseResource) info.msg.resource).stage = (__Null) this.stage;
  }

  protected override void OnHealthChanged()
  {
    this.Invoke(new Action(this.UpdateNetworkStage), 0.1f);
  }

  protected virtual void UpdateNetworkStage()
  {
    if (this.FindBestStage() == this.stage)
      return;
    this.stage = this.FindBestStage();
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    this.UpdateStage();
  }

  private int FindBestStage()
  {
    float num = Mathf.InverseLerp(0.0f, this.MaxHealth(), this.Health());
    for (int index = 0; index < this.stages.Count; ++index)
    {
      if ((double) num >= (double) this.stages[index].health)
        return index;
    }
    return this.stages.Count - 1;
  }

  public T GetStageComponent<T>() where T : Component
  {
    return this.stages[this.stage].instance.GetComponentInChildren<T>();
  }

  private void UpdateStage()
  {
    if (this.stages.Count == 0)
      return;
    for (int index = 0; index < this.stages.Count; ++index)
      this.stages[index].instance.SetActive(index == this.stage);
    GroundWatch.PhysicsChanged(((Component) this).get_gameObject());
  }

  [Serializable]
  public class ResourceStage
  {
    public float health;
    public GameObject instance;
  }
}
