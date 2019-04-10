// Decompiled with JetBrains decompiler
// Type: ScaleRenderer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ScaleRenderer : MonoBehaviour
{
  public bool useRandomScale;
  public float scaleMin;
  public float scaleMax;
  private float lastScale;
  protected bool hasInitialValues;
  public Renderer myRenderer;

  private bool ScaleDifferent(float newScale)
  {
    return (double) newScale != (double) this.lastScale;
  }

  public void Start()
  {
    if (!this.useRandomScale)
      return;
    this.SetScale(Random.Range(this.scaleMin, this.scaleMax));
  }

  public void SetScale(float scale)
  {
    if (!this.hasInitialValues)
      this.GatherInitialValues();
    if (!this.ScaleDifferent(scale))
      return;
    this.SetRendererEnabled((double) scale != 0.0);
    this.SetScale_Internal(scale);
  }

  public virtual void SetScale_Internal(float scale)
  {
    this.lastScale = scale;
  }

  public virtual void SetRendererEnabled(bool isEnabled)
  {
    if (!Object.op_Implicit((Object) this.myRenderer) || this.myRenderer.get_enabled() == isEnabled)
      return;
    this.myRenderer.set_enabled(isEnabled);
  }

  public virtual void GatherInitialValues()
  {
    this.hasInitialValues = true;
  }

  public ScaleRenderer()
  {
    base.\u002Ector();
  }
}
