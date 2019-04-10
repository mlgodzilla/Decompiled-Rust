// Decompiled with JetBrains decompiler
// Type: PhysicsEffects
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using UnityEngine;

public class PhysicsEffects : MonoBehaviour
{
  public BaseEntity entity;
  public SoundDefinition physImpactSoundDef;
  public float minTimeBetweenEffects;
  public float minDistBetweenEffects;
  public float hardnessScale;
  public float lowMedThreshold;
  public float medHardThreshold;
  public float enableDelay;
  public LayerMask ignoreLayers;
  private float lastEffectPlayed;
  private float enabledAt;
  private float ignoreImpactThreshold;
  private Vector3 lastCollisionPos;

  public void OnEnable()
  {
    this.enabledAt = Time.get_time();
  }

  public void OnCollisionEnter(Collision collision)
  {
    if (!Physics.sendeffects || (double) Time.get_time() < (double) this.enabledAt + (double) this.enableDelay || ((double) Time.get_time() < (double) this.lastEffectPlayed + (double) this.minTimeBetweenEffects || (1 << collision.get_gameObject().get_layer() & LayerMask.op_Implicit(this.ignoreLayers)) != 0))
      return;
    Vector3 relativeVelocity = collision.get_relativeVelocity();
    float num = ((Vector3) ref relativeVelocity).get_magnitude() * 0.055f * this.hardnessScale;
    if ((double) num <= (double) this.ignoreImpactThreshold || (double) Vector3.Distance(((Component) this).get_transform().get_position(), this.lastCollisionPos) < (double) this.minDistBetweenEffects && (double) this.lastEffectPlayed != 0.0)
      return;
    if (Object.op_Inequality((Object) this.entity, (Object) null))
      this.entity.SignalBroadcast(BaseEntity.Signal.PhysImpact, num.ToString(), (Connection) null);
    this.lastEffectPlayed = Time.get_time();
    this.lastCollisionPos = ((Component) this).get_transform().get_position();
  }

  public PhysicsEffects()
  {
    base.\u002Ector();
  }
}
