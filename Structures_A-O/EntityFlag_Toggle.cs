// Decompiled with JetBrains decompiler
// Type: EntityFlag_Toggle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;

public class EntityFlag_Toggle : EntityComponent<BaseEntity>, IOnPostNetworkUpdate, IOnSendNetworkUpdate, IPrefabPreProcess
{
  public bool runClientside = true;
  public bool runServerside = true;
  [SerializeField]
  private UnityEvent onFlagEnabled = new UnityEvent();
  [SerializeField]
  private UnityEvent onFlagDisabled = new UnityEvent();
  public BaseEntity.Flags flag;
  internal bool hasRunOnce;
  internal bool lastHasFlag;

  public void DoUpdate(BaseEntity entity)
  {
    bool flag = entity.HasFlag(this.flag);
    if (this.hasRunOnce && flag == this.lastHasFlag)
      return;
    this.hasRunOnce = true;
    this.lastHasFlag = flag;
    if (flag)
      this.onFlagEnabled.Invoke();
    else
      this.onFlagDisabled.Invoke();
  }

  public void OnPostNetworkUpdate(BaseEntity entity)
  {
    if (Object.op_Inequality((Object) this.baseEntity, (Object) entity) || !this.runClientside)
      return;
    this.DoUpdate(entity);
  }

  public void OnSendNetworkUpdate(BaseEntity entity)
  {
    if (!this.runServerside)
      return;
    this.DoUpdate(entity);
  }

  public void PreProcess(
    IPrefabProcessor process,
    GameObject rootObj,
    string name,
    bool serverside,
    bool clientside,
    bool bundling)
  {
    if ((!clientside || !this.runClientside ? (!serverside ? 0 : (this.runServerside ? 1 : 0)) : 1) != 0)
      return;
    process.RemoveComponent((Component) this);
  }
}
