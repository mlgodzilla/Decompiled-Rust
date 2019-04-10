// Decompiled with JetBrains decompiler
// Type: TickInterpolator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class TickInterpolator
{
  private List<TickInterpolator.Segment> points = new List<TickInterpolator.Segment>();
  private int index;
  public float Length;
  public Vector3 CurrentPoint;
  public Vector3 StartPoint;
  public Vector3 EndPoint;

  public void Reset()
  {
    this.index = 0;
    this.CurrentPoint = this.StartPoint;
  }

  public void Reset(Vector3 point)
  {
    this.points.Clear();
    this.index = 0;
    this.Length = 0.0f;
    this.CurrentPoint = this.StartPoint = this.EndPoint = point;
  }

  public void AddPoint(Vector3 point)
  {
    TickInterpolator.Segment segment = new TickInterpolator.Segment(this.EndPoint, point);
    this.points.Add(segment);
    this.Length += segment.length;
    this.EndPoint = segment.point;
  }

  public bool MoveNext(float distance)
  {
    float num;
    for (num = 0.0f; (double) num < (double) distance && this.index < this.points.Count; ++this.index)
    {
      TickInterpolator.Segment point = this.points[this.index];
      this.CurrentPoint = point.point;
      num += point.length;
    }
    return (double) num > 0.0;
  }

  public bool HasNext()
  {
    return this.index < this.points.Count;
  }

  public void TransformEntries(Matrix4x4 matrix)
  {
    for (int index = 0; index < this.points.Count; ++index)
    {
      TickInterpolator.Segment point = this.points[index];
      point.point = ((Matrix4x4) ref matrix).MultiplyPoint3x4(point.point);
      this.points[index] = point;
    }
    this.CurrentPoint = ((Matrix4x4) ref matrix).MultiplyPoint3x4(this.CurrentPoint);
    this.StartPoint = ((Matrix4x4) ref matrix).MultiplyPoint3x4(this.StartPoint);
    this.EndPoint = ((Matrix4x4) ref matrix).MultiplyPoint3x4(this.EndPoint);
  }

  private struct Segment
  {
    public Vector3 point;
    public float length;

    public Segment(Vector3 a, Vector3 b)
    {
      this.point = b;
      this.length = Vector3.Distance(a, b);
    }
  }
}
