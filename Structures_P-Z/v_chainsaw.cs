// Decompiled with JetBrains decompiler
// Type: v_chainsaw
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class v_chainsaw : MonoBehaviour
{
  public bool bAttacking;
  public bool bHitMetal;
  public bool bHitWood;
  public bool bHitFlesh;
  public bool bEngineOn;
  public ParticleSystem[] hitMetalFX;
  public ParticleSystem[] hitWoodFX;
  public ParticleSystem[] hitFleshFX;
  public SoundDefinition hitMetalSoundDef;
  public SoundDefinition hitWoodSoundDef;
  public SoundDefinition hitFleshSoundDef;
  public Sound hitSound;
  public GameObject hitSoundTarget;
  public float hitSoundFadeTime;
  public ParticleSystem smokeEffect;
  public Animator chainsawAnimator;
  public Renderer chainRenderer;
  public Material chainlink;
  private MaterialPropertyBlock block;
  private Vector2 saveST;
  private float chainSpeed;
  private float chainAmount;
  public float temp1;
  public float temp2;

  public void OnEnable()
  {
    if (this.block == null)
      this.block = new MaterialPropertyBlock();
    this.saveST = Vector4.op_Implicit(this.chainRenderer.get_sharedMaterial().GetVector("_MainTex_ST"));
  }

  private void Awake()
  {
    this.chainlink = this.chainRenderer.get_sharedMaterial();
  }

  private void Start()
  {
  }

  private void ScrollChainTexture()
  {
    float num = this.chainAmount = (float) (((double) this.chainAmount + (double) Time.get_deltaTime() * (double) this.chainSpeed) % 1.0);
    this.block.Clear();
    this.block.SetVector("_MainTex_ST", new Vector4((float) this.saveST.x, (float) this.saveST.y, num, 0.0f));
    this.chainRenderer.SetPropertyBlock(this.block);
  }

  private void Update()
  {
    this.chainsawAnimator.SetBool("attacking", this.bAttacking);
    this.smokeEffect.set_enableEmission(this.bEngineOn);
    if (this.bHitMetal)
    {
      this.chainsawAnimator.SetBool("attackHit", true);
      foreach (ParticleSystem particleSystem in this.hitMetalFX)
        particleSystem.set_enableEmission(true);
      foreach (ParticleSystem particleSystem in this.hitWoodFX)
        particleSystem.set_enableEmission(false);
      foreach (ParticleSystem particleSystem in this.hitFleshFX)
        particleSystem.set_enableEmission(false);
      this.DoHitSound(this.hitMetalSoundDef);
    }
    else if (this.bHitWood)
    {
      this.chainsawAnimator.SetBool("attackHit", true);
      foreach (ParticleSystem particleSystem in this.hitMetalFX)
        particleSystem.set_enableEmission(false);
      foreach (ParticleSystem particleSystem in this.hitWoodFX)
        particleSystem.set_enableEmission(true);
      foreach (ParticleSystem particleSystem in this.hitFleshFX)
        particleSystem.set_enableEmission(false);
      this.DoHitSound(this.hitWoodSoundDef);
    }
    else if (this.bHitFlesh)
    {
      this.chainsawAnimator.SetBool("attackHit", true);
      foreach (ParticleSystem particleSystem in this.hitMetalFX)
        particleSystem.set_enableEmission(false);
      foreach (ParticleSystem particleSystem in this.hitWoodFX)
        particleSystem.set_enableEmission(false);
      foreach (ParticleSystem particleSystem in this.hitFleshFX)
        particleSystem.set_enableEmission(true);
      this.DoHitSound(this.hitFleshSoundDef);
    }
    else
    {
      this.chainsawAnimator.SetBool("attackHit", false);
      foreach (ParticleSystem particleSystem in this.hitMetalFX)
        particleSystem.set_enableEmission(false);
      foreach (ParticleSystem particleSystem in this.hitWoodFX)
        particleSystem.set_enableEmission(false);
      foreach (ParticleSystem particleSystem in this.hitFleshFX)
        particleSystem.set_enableEmission(false);
    }
  }

  private void DoHitSound(SoundDefinition soundDef)
  {
  }

  public v_chainsaw()
  {
    base.\u002Ector();
  }
}
