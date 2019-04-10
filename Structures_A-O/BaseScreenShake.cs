// Decompiled with JetBrains decompiler
// Type: BaseScreenShake
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseScreenShake : MonoBehaviour
{
  public static List<BaseScreenShake> list = new List<BaseScreenShake>();
  public float length;
  internal float timeTaken;
  private int currentFrame;

  public static void Apply(Camera cam, BaseViewModel vm)
  {
    CachedTransform<Camera> cam1 = new CachedTransform<Camera>(cam);
    CachedTransform<BaseViewModel> vm1 = new CachedTransform<BaseViewModel>(vm);
    for (int index = 0; index < BaseScreenShake.list.Count; ++index)
      BaseScreenShake.list[index].Run(ref cam1, ref vm1);
    cam1.Apply();
    vm1.Apply();
  }

  protected void OnEnable()
  {
    BaseScreenShake.list.Add(this);
    this.timeTaken = 0.0f;
    this.Setup();
  }

  protected void OnDisable()
  {
    if (Application.isQuitting != null)
      return;
    BaseScreenShake.list.Remove(this);
  }

  public void Run(ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
  {
    if ((double) this.timeTaken > (double) this.length)
      return;
    if (Time.get_frameCount() != this.currentFrame)
    {
      this.timeTaken += Time.get_deltaTime();
      this.currentFrame = Time.get_frameCount();
    }
    this.Run(Mathf.InverseLerp(0.0f, this.length, this.timeTaken), ref cam, ref vm);
  }

  public abstract void Setup();

  public abstract void Run(
    float delta,
    ref CachedTransform<Camera> cam,
    ref CachedTransform<BaseViewModel> vm);

  protected BaseScreenShake()
  {
    base.\u002Ector();
  }
}
