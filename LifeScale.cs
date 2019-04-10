// Decompiled with JetBrains decompiler
// Type: LifeScale
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class LifeScale : BaseMonoBehaviour
{
  public Vector3 finalScale = Vector3.get_one();
  private Vector3 targetLerpScale = Vector3.get_zero();
  [NonSerialized]
  private bool initialized;
  [NonSerialized]
  private Vector3 initialScale;
  private Action updateScaleAction;

  protected void Awake()
  {
    this.updateScaleAction = new Action(this.UpdateScale);
  }

  public void OnEnable()
  {
    this.Init();
    ((Component) this).get_transform().set_localScale(this.initialScale);
  }

  public void SetProgress(float progress)
  {
    this.Init();
    this.targetLerpScale = Vector3.Lerp(this.initialScale, this.finalScale, progress);
    this.InvokeRepeating(this.updateScaleAction, 0.0f, 0.015f);
  }

  public void Init()
  {
    if (this.initialized)
      return;
    this.initialScale = ((Component) this).get_transform().get_localScale();
    this.initialized = true;
  }

  public void UpdateScale()
  {
    ((Component) this).get_transform().set_localScale(Vector3.Lerp(((Component) this).get_transform().get_localScale(), this.targetLerpScale, Time.get_deltaTime()));
    if (!Vector3.op_Equality(((Component) this).get_transform().get_localScale(), this.targetLerpScale))
      return;
    this.targetLerpScale = Vector3.get_zero();
    this.CancelInvoke(this.updateScaleAction);
  }
}
