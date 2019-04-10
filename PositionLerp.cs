// Decompiled with JetBrains decompiler
// Type: PositionLerp
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class PositionLerp : ListComponent<PositionLerp>
{
  public static bool DebugLog;
  public static bool DebugDraw;
  private Action idleDisable;
  private TransformInterpolator interpolator;
  private ILerpTarget target;
  private float timeOffset0;
  private float timeOffset1;
  private float timeOffset2;
  private float timeOffset3;
  private float lastClientTime;
  private float lastServerTime;
  private float extrapolatedTime;

  public void Initialize(ILerpTarget target)
  {
    this.target = target;
  }

  public void Snapshot(Vector3 position, Quaternion rotation, float serverTime)
  {
    float num1 = (float) ((double) this.target.GetInterpolationDelay() + (double) this.target.GetInterpolationSmoothing() + 1.0);
    float time = Time.get_time();
    this.timeOffset0 = this.timeOffset1;
    this.timeOffset1 = this.timeOffset2;
    this.timeOffset2 = this.timeOffset3;
    this.timeOffset3 = time - serverTime;
    float num2 = Mathx.Min(this.timeOffset0, this.timeOffset1, this.timeOffset2, this.timeOffset3);
    float num3 = serverTime + num2;
    if (PositionLerp.DebugLog && this.interpolator.list.Count > 0 && (double) serverTime < (double) this.lastServerTime)
      Debug.LogWarning((object) (this.target.ToString() + " adding tick from the past: server time " + (object) serverTime + " < " + (object) this.lastServerTime));
    else if (PositionLerp.DebugLog && this.interpolator.list.Count > 0 && (double) num3 < (double) this.lastClientTime)
    {
      Debug.LogWarning((object) (this.target.ToString() + " adding tick from the past: client time " + (object) num3 + " < " + (object) this.lastClientTime));
    }
    else
    {
      this.lastClientTime = num3;
      this.lastServerTime = serverTime;
      this.interpolator.Add(new TransformInterpolator.Entry()
      {
        time = num3,
        pos = position,
        rot = rotation
      });
    }
    this.interpolator.Cull(num3 - num1);
  }

  public void SnapTo(Vector3 position, Quaternion rotation, float serverTime)
  {
    this.interpolator.Clear();
    this.Snapshot(position, rotation, serverTime);
    this.target.SetNetworkPosition(position);
    this.target.SetNetworkRotation(rotation);
  }

  public void SnapToEnd()
  {
    TransformInterpolator.Segment segment = this.interpolator.Query(Time.get_time(), this.target.GetInterpolationDelay(), 0.0f, 0.0f);
    this.target.SetNetworkPosition(segment.tick.pos);
    this.target.SetNetworkRotation(segment.tick.rot);
    this.interpolator.Clear();
  }

  protected void DoCycle()
  {
    if (this.target == null)
      return;
    float extrapolationTime = this.target.GetExtrapolationTime();
    float interpolationDelay = this.target.GetInterpolationDelay();
    float interpolationSmoothing = this.target.GetInterpolationSmoothing();
    TransformInterpolator.Segment segment = this.interpolator.Query(Time.get_time(), interpolationDelay, extrapolationTime, interpolationSmoothing);
    this.extrapolatedTime = (double) segment.next.time < (double) this.interpolator.last.time ? Mathf.Max(this.extrapolatedTime - Time.get_deltaTime(), 0.0f) : Mathf.Min(this.extrapolatedTime + Time.get_deltaTime(), extrapolationTime);
    if ((double) this.extrapolatedTime > 0.0 && (double) extrapolationTime > 0.0 && (double) interpolationSmoothing > 0.0)
    {
      float num = Time.get_deltaTime() / (this.extrapolatedTime / extrapolationTime * interpolationSmoothing);
      segment.tick.pos = Vector3.Lerp(this.target.GetNetworkPosition(), segment.tick.pos, num);
      segment.tick.rot = Quaternion.Slerp(this.target.GetNetworkRotation(), segment.tick.rot, num);
    }
    this.target.SetNetworkPosition(segment.tick.pos);
    this.target.SetNetworkRotation(segment.tick.rot);
    if (PositionLerp.DebugDraw)
      this.target.DrawInterpolationState(segment, this.interpolator.list);
    if ((double) Time.get_time() - (double) this.lastClientTime <= 10.0)
      return;
    if (this.idleDisable == null)
      this.idleDisable = new Action(this.IdleDisable);
    InvokeHandler.Invoke((Behaviour) this, this.idleDisable, 0.0f);
  }

  private void IdleDisable()
  {
    ((Behaviour) this).set_enabled(false);
  }

  public void TransformEntries(Matrix4x4 matrix)
  {
    Quaternion rotation = ((Matrix4x4) ref matrix).get_rotation();
    for (int index = 0; index < this.interpolator.list.Count; ++index)
    {
      TransformInterpolator.Entry entry = this.interpolator.list[index];
      entry.pos = ((Matrix4x4) ref matrix).MultiplyPoint3x4(entry.pos);
      entry.rot = Quaternion.op_Multiply(rotation, entry.rot);
      this.interpolator.list[index] = entry;
    }
    this.interpolator.last.pos = ((Matrix4x4) ref matrix).MultiplyPoint3x4(this.interpolator.last.pos);
    this.interpolator.last.rot = Quaternion.op_Multiply(rotation, this.interpolator.last.rot);
  }

  public Quaternion GetEstimatedAngularVelocity()
  {
    if (this.target == null)
      return Quaternion.get_identity();
    float extrapolationTime = this.target.GetExtrapolationTime();
    float interpolationDelay = this.target.GetInterpolationDelay();
    float interpolationSmoothing = this.target.GetInterpolationSmoothing();
    TransformInterpolator.Segment segment = this.interpolator.Query(Time.get_time(), interpolationDelay, extrapolationTime, interpolationSmoothing);
    TransformInterpolator.Entry next = segment.next;
    TransformInterpolator.Entry prev = segment.prev;
    if ((double) next.time == (double) prev.time)
      return Quaternion.get_identity();
    return Quaternion.Euler(Vector3.op_Division(Vector3.op_Subtraction(((Quaternion) ref prev.rot).get_eulerAngles(), ((Quaternion) ref next.rot).get_eulerAngles()), prev.time - next.time));
  }

  public Vector3 GetEstimatedVelocity()
  {
    if (this.target == null)
      return Vector3.get_zero();
    float extrapolationTime = this.target.GetExtrapolationTime();
    float interpolationDelay = this.target.GetInterpolationDelay();
    float interpolationSmoothing = this.target.GetInterpolationSmoothing();
    TransformInterpolator.Segment segment = this.interpolator.Query(Time.get_time(), interpolationDelay, extrapolationTime, interpolationSmoothing);
    TransformInterpolator.Entry next = segment.next;
    TransformInterpolator.Entry prev = segment.prev;
    if ((double) next.time == (double) prev.time)
      return Vector3.get_zero();
    return Vector3.op_Division(Vector3.op_Subtraction(prev.pos, next.pos), prev.time - next.time);
  }

  public static void Cycle()
  {
    PositionLerp[] buffer = ((ListHashSet<PositionLerp>) ListComponent<PositionLerp>.InstanceList).get_Values().get_Buffer();
    int count = ((ListHashSet<PositionLerp>) ListComponent<PositionLerp>.InstanceList).get_Count();
    for (int index = 0; index < count; ++index)
      buffer[index].DoCycle();
  }

  public PositionLerp()
  {
    base.\u002Ector();
  }
}
