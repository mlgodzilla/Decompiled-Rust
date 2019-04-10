// Decompiled with JetBrains decompiler
// Type: HelicopterTurret
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Oxide.Core;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterTurret : MonoBehaviour
{
  public PatrolHelicopterAI _heliAI;
  public float fireRate;
  public float burstLength;
  public float timeBetweenBursts;
  public float maxTargetRange;
  public float loseTargetAfter;
  public Transform gun_yaw;
  public Transform gun_pitch;
  public Transform muzzleTransform;
  public bool left;
  public BaseCombatEntity _target;
  private float lastBurstTime;
  private float lastFireTime;
  private float lastSeenTargetTime;
  private bool targetVisible;

  public void SetTarget(BaseCombatEntity newTarget)
  {
    if (Interface.CallHook("OnHelicopterTarget", (object) this, (object) newTarget) != null)
      return;
    this._target = newTarget;
    this.UpdateTargetVisibility();
  }

  public bool NeedsNewTarget()
  {
    if (!this.HasTarget())
      return true;
    if (!this.targetVisible)
      return (double) this.TimeSinceTargetLastSeen() > (double) this.loseTargetAfter;
    return false;
  }

  public bool UpdateTargetFromList(List<PatrolHelicopterAI.targetinfo> newTargetList)
  {
    int index = Random.Range(0, newTargetList.Count);
    int count = newTargetList.Count;
    while (count >= 0)
    {
      --count;
      PatrolHelicopterAI.targetinfo newTarget = newTargetList[index];
      if (newTarget != null && Object.op_Inequality((Object) newTarget.ent, (Object) null) && (newTarget.IsVisible() && this.InFiringArc((BaseCombatEntity) newTarget.ply)))
      {
        this.SetTarget((BaseCombatEntity) newTarget.ply);
        return true;
      }
      ++index;
      if (index >= newTargetList.Count)
        index = 0;
    }
    return false;
  }

  public bool TargetVisible()
  {
    this.UpdateTargetVisibility();
    return this.targetVisible;
  }

  public float TimeSinceTargetLastSeen()
  {
    return Time.get_realtimeSinceStartup() - this.lastSeenTargetTime;
  }

  public bool HasTarget()
  {
    return Object.op_Inequality((Object) this._target, (Object) null);
  }

  public void ClearTarget()
  {
    this._target = (BaseCombatEntity) null;
    this.targetVisible = false;
  }

  public void TurretThink()
  {
    if (this.HasTarget() && (double) this.TimeSinceTargetLastSeen() > (double) this.loseTargetAfter * 2.0)
      this.ClearTarget();
    if (!this.HasTarget())
      return;
    if ((double) Time.get_time() - (double) this.lastBurstTime > (double) this.burstLength + (double) this.timeBetweenBursts && this.TargetVisible())
      this.lastBurstTime = Time.get_time();
    if ((double) Time.get_time() >= (double) this.lastBurstTime + (double) this.burstLength || (double) Time.get_time() - (double) this.lastFireTime < (double) this.fireRate || !this.InFiringArc(this._target))
      return;
    this.lastFireTime = Time.get_time();
    this.FireGun();
  }

  public void FireGun()
  {
    this._heliAI.FireGun(Vector3.op_Addition(((Component) this._target).get_transform().get_position(), new Vector3(0.0f, 0.25f, 0.0f)), PatrolHelicopter.bulletAccuracy, this.left);
  }

  public Vector3 GetPositionForEntity(BaseCombatEntity potentialtarget)
  {
    return ((Component) potentialtarget).get_transform().get_position();
  }

  public float AngleToTarget(BaseCombatEntity potentialtarget)
  {
    Vector3 vector3 = Vector3.op_Subtraction(this.GetPositionForEntity(potentialtarget), this.muzzleTransform.get_position());
    Vector3 normalized = ((Vector3) ref vector3).get_normalized();
    return Vector3.Angle(this.left ? Vector3.op_UnaryNegation(((Component) this._heliAI).get_transform().get_right()) : ((Component) this._heliAI).get_transform().get_right(), normalized);
  }

  public bool InFiringArc(BaseCombatEntity potentialtarget)
  {
    object obj = Interface.CallHook("CanBeTargeted", (object) potentialtarget, (object) this);
    if (obj is bool)
      return (bool) obj;
    return (double) this.AngleToTarget(potentialtarget) < 80.0;
  }

  public void UpdateTargetVisibility()
  {
    if (!this.HasTarget())
      return;
    Vector3 position = ((Component) this._target).get_transform().get_position();
    BasePlayer target = this._target as BasePlayer;
    if (Object.op_Implicit((Object) target))
      position = target.eyes.position;
    bool flag = false;
    float num = Vector3.Distance(position, this.muzzleTransform.get_position());
    Vector3 vector3 = Vector3.op_Subtraction(position, this.muzzleTransform.get_position());
    Vector3 normalized = ((Vector3) ref vector3).get_normalized();
    RaycastHit hitInfo;
    if ((double) num < (double) this.maxTargetRange && this.InFiringArc(this._target) && (GamePhysics.Trace(new Ray(Vector3.op_Addition(this.muzzleTransform.get_position(), Vector3.op_Multiply(normalized, 6f)), normalized), 0.0f, out hitInfo, num * 1.1f, 1218652417, (QueryTriggerInteraction) 0) && Object.op_Equality((Object) ((Component) ((RaycastHit) ref hitInfo).get_collider()).get_gameObject().ToBaseEntity(), (Object) this._target)))
      flag = true;
    if (flag)
      this.lastSeenTargetTime = Time.get_realtimeSinceStartup();
    this.targetVisible = flag;
  }

  public HelicopterTurret()
  {
    base.\u002Ector();
  }
}
