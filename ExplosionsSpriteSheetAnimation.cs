// Decompiled with JetBrains decompiler
// Type: ExplosionsSpriteSheetAnimation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections;
using UnityEngine;

internal class ExplosionsSpriteSheetAnimation : MonoBehaviour
{
  public int TilesX;
  public int TilesY;
  public float AnimationFPS;
  public bool IsInterpolateFrames;
  public int StartFrameOffset;
  public bool IsLoop;
  public float StartDelay;
  public AnimationCurve FrameOverTime;
  private bool isInizialised;
  private int index;
  private int count;
  private int allCount;
  private float animationLifeTime;
  private bool isVisible;
  private bool isCorutineStarted;
  private Renderer currentRenderer;
  private Material instanceMaterial;
  private float currentInterpolatedTime;
  private float animationStartTime;
  private bool animationStoped;

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
    this.animationStoped = false;
    this.animationLifeTime = (float) (this.TilesX * this.TilesY) / this.AnimationFPS;
    this.count = this.TilesY * this.TilesX;
    this.index = this.TilesX - 1;
    Vector3 zero = Vector3.get_zero();
    this.StartFrameOffset -= this.StartFrameOffset / this.count * this.count;
    Vector2 vector2;
    ((Vector2) ref vector2).\u002Ector(1f / (float) this.TilesX, 1f / (float) this.TilesY);
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
    this.animationStartTime = Time.get_time();
    while (this.isVisible && (this.IsLoop || !this.animationStoped))
    {
      this.UpdateFrame();
      if (this.IsLoop || !this.animationStoped)
        yield return (object) new WaitForSeconds((float) (1.0 / ((double) this.AnimationFPS * (double) this.FrameOverTime.Evaluate(Mathf.Clamp01((Time.get_time() - this.animationStartTime) / this.animationLifeTime)))));
      else
        break;
    }
    this.isCorutineStarted = false;
    this.currentRenderer.set_enabled(false);
  }

  private void UpdateFrame()
  {
    ++this.allCount;
    ++this.index;
    if (this.index >= this.count)
      this.index = 0;
    if (this.count == this.allCount)
    {
      this.animationStartTime = Time.get_time();
      this.allCount = 0;
      this.animationStoped = true;
    }
    Vector2 vector2;
    ((Vector2) ref vector2).\u002Ector((float) this.index / (float) this.TilesX - (float) (this.index / this.TilesX), (float) (1.0 - (double) (this.index / this.TilesX) / (double) this.TilesY));
    if (Object.op_Inequality((Object) this.currentRenderer, (Object) null))
      this.instanceMaterial.SetTextureOffset("_MainTex", vector2);
    if (!this.IsInterpolateFrames)
      return;
    this.currentInterpolatedTime = 0.0f;
  }

  private void Update()
  {
    if (!this.IsInterpolateFrames)
      return;
    this.currentInterpolatedTime += Time.get_deltaTime();
    int num = this.index + 1;
    if (this.allCount == 0)
      num = this.index;
    Vector4 vector4;
    ((Vector4) ref vector4).\u002Ector(1f / (float) this.TilesX, 1f / (float) this.TilesY, (float) num / (float) this.TilesX - (float) (num / this.TilesX), (float) (1.0 - (double) (num / this.TilesX) / (double) this.TilesY));
    if (!Object.op_Inequality((Object) this.currentRenderer, (Object) null))
      return;
    this.instanceMaterial.SetVector("_MainTex_NextFrame", vector4);
    this.instanceMaterial.SetFloat("InterpolationValue", Mathf.Clamp01(this.currentInterpolatedTime * this.AnimationFPS * this.FrameOverTime.Evaluate(Mathf.Clamp01((Time.get_time() - this.animationStartTime) / this.animationLifeTime))));
  }

  private void OnDestroy()
  {
    if (!Object.op_Inequality((Object) this.instanceMaterial, (Object) null))
      return;
    Object.Destroy((Object) this.instanceMaterial);
    this.instanceMaterial = (Material) null;
  }

  public ExplosionsSpriteSheetAnimation()
  {
    base.\u002Ector();
  }
}
