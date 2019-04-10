// Decompiled with JetBrains decompiler
// Type: UVTextureAnimator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections;
using UnityEngine;

internal class UVTextureAnimator : MonoBehaviour
{
  public int Rows;
  public int Columns;
  public float Fps;
  public int OffsetMat;
  public bool IsLoop;
  public float StartDelay;
  private bool isInizialised;
  private int index;
  private int count;
  private int allCount;
  private float deltaFps;
  private bool isVisible;
  private bool isCorutineStarted;
  private Renderer currentRenderer;
  private Material instanceMaterial;

  private void Start()
  {
    this.currentRenderer = (Renderer) ((Component) this).GetComponent<Renderer>();
    this.InitDefaultVariables();
    this.isInizialised = true;
    this.isVisible = true;
    this.Play();
  }

  private void InitDefaultVariables()
  {
    this.currentRenderer = (Renderer) ((Component) this).GetComponent<Renderer>();
    if (Object.op_Equality((Object) this.currentRenderer, (Object) null))
      throw new Exception("UvTextureAnimator can't get renderer");
    if (!this.currentRenderer.get_enabled())
      this.currentRenderer.set_enabled(true);
    this.allCount = 0;
    this.deltaFps = 1f / this.Fps;
    this.count = this.Rows * this.Columns;
    this.index = this.Columns - 1;
    Vector3 zero = Vector3.get_zero();
    this.OffsetMat -= this.OffsetMat / this.count * this.count;
    Vector2 vector2;
    ((Vector2) ref vector2).\u002Ector(1f / (float) this.Columns, 1f / (float) this.Rows);
    if (!Object.op_Inequality((Object) this.currentRenderer, (Object) null))
      return;
    this.instanceMaterial = this.currentRenderer.get_material();
    this.instanceMaterial.SetTextureScale("_MainTex", vector2);
    this.instanceMaterial.SetTextureOffset("_MainTex", Vector2.op_Implicit(zero));
  }

  private void Play()
  {
    if (this.isCorutineStarted)
      return;
    if ((double) this.StartDelay > 9.99999974737875E-05)
      this.Invoke("PlayDelay", this.StartDelay);
    else
      this.StartCoroutine(this.UpdateCorutine());
    this.isCorutineStarted = true;
  }

  private void PlayDelay()
  {
    this.StartCoroutine(this.UpdateCorutine());
  }

  private void OnEnable()
  {
    if (!this.isInizialised)
      return;
    this.InitDefaultVariables();
    this.isVisible = true;
    this.Play();
  }

  private void OnDisable()
  {
    this.isCorutineStarted = false;
    this.isVisible = false;
    this.StopAllCoroutines();
    this.CancelInvoke("PlayDelay");
  }

  private IEnumerator UpdateCorutine()
  {
    while (this.isVisible && (this.IsLoop || this.allCount != this.count))
    {
      this.UpdateCorutineFrame();
      if (this.IsLoop || this.allCount != this.count)
        yield return (object) new WaitForSeconds(this.deltaFps);
      else
        break;
    }
    this.isCorutineStarted = false;
    this.currentRenderer.set_enabled(false);
  }

  private void UpdateCorutineFrame()
  {
    ++this.allCount;
    ++this.index;
    if (this.index >= this.count)
      this.index = 0;
    Vector2 vector2;
    ((Vector2) ref vector2).\u002Ector((float) this.index / (float) this.Columns - (float) (this.index / this.Columns), (float) (1.0 - (double) (this.index / this.Columns) / (double) this.Rows));
    if (!Object.op_Inequality((Object) this.currentRenderer, (Object) null))
      return;
    this.instanceMaterial.SetTextureOffset("_MainTex", vector2);
  }

  private void OnDestroy()
  {
    if (!Object.op_Inequality((Object) this.instanceMaterial, (Object) null))
      return;
    Object.Destroy((Object) this.instanceMaterial);
    this.instanceMaterial = (Material) null;
  }

  public UVTextureAnimator()
  {
    base.\u002Ector();
  }
}
