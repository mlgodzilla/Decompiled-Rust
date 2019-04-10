// Decompiled with JetBrains decompiler
// Type: TransformInterpolator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class TransformInterpolator
{
  public List<TransformInterpolator.Entry> list = new List<TransformInterpolator.Entry>(32);
  public TransformInterpolator.Entry last;

  public void Add(TransformInterpolator.Entry tick)
  {
    this.last = tick;
    this.list.Add(tick);
  }

  public void Cull(float beforeTime)
  {
    for (int index = 0; index < this.list.Count; ++index)
    {
      if ((double) this.list[index].time < (double) beforeTime)
      {
        this.list.RemoveAt(index);
        --index;
      }
    }
  }

  public void Clear()
  {
    this.list.Clear();
  }

  public TransformInterpolator.Segment Query(
    float time,
    float interpolation,
    float extrapolation,
    float smoothing)
  {
    TransformInterpolator.Segment segment = new TransformInterpolator.Segment();
    if (this.list.Count == 0)
    {
      segment.prev = this.last;
      segment.next = this.last;
      segment.tick = this.last;
      return segment;
    }
    float num1 = (float) ((double) time - (double) interpolation - (double) smoothing * 0.5);
    float num2 = Mathf.Min(time - interpolation, this.last.time);
    float num3 = num2 - smoothing;
    TransformInterpolator.Entry entry1 = this.list[0];
    TransformInterpolator.Entry entry2 = this.last;
    TransformInterpolator.Entry entry3 = this.list[0];
    TransformInterpolator.Entry entry4 = this.last;
    foreach (TransformInterpolator.Entry entry5 in this.list)
    {
      if ((double) entry5.time < (double) num3)
        entry1 = entry5;
      else if ((double) entry2.time >= (double) entry5.time)
        entry2 = entry5;
      if ((double) entry5.time < (double) num2)
        entry3 = entry5;
      else if ((double) entry4.time >= (double) entry5.time)
        entry4 = entry5;
    }
    TransformInterpolator.Entry entry6 = new TransformInterpolator.Entry();
    if ((double) entry2.time - (double) entry1.time < Mathf.Epsilon)
    {
      entry6.time = num3;
      entry6.pos = entry2.pos;
      entry6.rot = entry2.rot;
    }
    else
    {
      float num4 = (float) (((double) num3 - (double) entry1.time) / ((double) entry2.time - (double) entry1.time));
      entry6.time = num3;
      entry6.pos = Vector3.LerpUnclamped(entry1.pos, entry2.pos, num4);
      entry6.rot = Quaternion.SlerpUnclamped(entry1.rot, entry2.rot, num4);
    }
    segment.prev = entry6;
    TransformInterpolator.Entry entry7 = new TransformInterpolator.Entry();
    if ((double) entry4.time - (double) entry3.time < Mathf.Epsilon)
    {
      entry7.time = num2;
      entry7.pos = entry4.pos;
      entry7.rot = entry4.rot;
    }
    else
    {
      float num4 = (float) (((double) num2 - (double) entry3.time) / ((double) entry4.time - (double) entry3.time));
      entry7.time = num2;
      entry7.pos = Vector3.LerpUnclamped(entry3.pos, entry4.pos, num4);
      entry7.rot = Quaternion.SlerpUnclamped(entry3.rot, entry4.rot, num4);
    }
    segment.next = entry7;
    if ((double) entry7.time - (double) entry6.time < Mathf.Epsilon)
    {
      segment.prev = entry7;
      segment.tick = entry7;
      return segment;
    }
    if ((double) num1 - (double) entry7.time > (double) extrapolation)
    {
      segment.prev = entry7;
      segment.tick = entry7;
      return segment;
    }
    TransformInterpolator.Entry entry8 = new TransformInterpolator.Entry();
    float num5 = Mathf.Min(num1 - entry6.time, entry7.time + extrapolation - entry6.time) / (entry7.time - entry6.time);
    entry8.time = num1;
    entry8.pos = Vector3.LerpUnclamped(entry6.pos, entry7.pos, num5);
    entry8.rot = Quaternion.SlerpUnclamped(entry6.rot, entry7.rot, num5);
    segment.tick = entry8;
    return segment;
  }

  public struct Segment
  {
    public TransformInterpolator.Entry tick;
    public TransformInterpolator.Entry prev;
    public TransformInterpolator.Entry next;
  }

  public struct Entry
  {
    public float time;
    public Vector3 pos;
    public Quaternion rot;
  }
}
