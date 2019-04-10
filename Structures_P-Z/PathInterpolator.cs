// Decompiled with JetBrains decompiler
// Type: PathInterpolator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class PathInterpolator
{
  public Vector3[] Points;
  public Vector3[] Tangents;
  private bool initialized;

  public int MinIndex { get; set; }

  public int MaxIndex { get; set; }

  public float Length { get; private set; }

  public float StepSize { get; private set; }

  public int DefaultMinIndex
  {
    get
    {
      return 0;
    }
  }

  public int DefaultMaxIndex
  {
    get
    {
      return this.Points.Length - 1;
    }
  }

  public float StartOffset
  {
    get
    {
      return this.Length * (float) (this.MinIndex - this.DefaultMinIndex) / (float) (this.DefaultMaxIndex - this.DefaultMinIndex);
    }
  }

  public float EndOffset
  {
    get
    {
      return this.Length * (float) (this.DefaultMaxIndex - this.MaxIndex) / (float) (this.DefaultMaxIndex - this.DefaultMinIndex);
    }
  }

  public PathInterpolator(Vector3[] points)
  {
    if (points.Length < 2)
      throw new ArgumentException("Point list too short.");
    this.Points = points;
    this.MinIndex = this.DefaultMinIndex;
    this.MaxIndex = this.DefaultMaxIndex;
  }

  public void RecalculateTangents()
  {
    if (this.Tangents == null || this.Tangents.Length != this.Points.Length)
      this.Tangents = new Vector3[this.Points.Length];
    float num = 0.0f;
    for (int index = 0; index < this.Tangents.Length; ++index)
    {
      Vector3 point = this.Points[Mathf.Max(index - 1, 0)];
      Vector3 vector3 = Vector3.op_Subtraction(this.Points[Mathf.Min(index + 1, this.Tangents.Length - 1)], point);
      float magnitude = ((Vector3) ref vector3).get_magnitude();
      num += magnitude;
      this.Tangents[index] = Vector3.op_Division(vector3, magnitude);
    }
    this.Length = num;
    this.StepSize = num / (float) this.Points.Length;
    this.initialized = true;
  }

  public void Resample(float distance)
  {
    if (!this.initialized)
      throw new Exception("Tangents have not been calculated yet or are outdated.");
    Vector3[] vector3Array = new Vector3[Mathf.RoundToInt(this.Length / distance)];
    for (int index = 0; index < vector3Array.Length; ++index)
      vector3Array[index] = this.GetPointCubicHermite((float) index * distance);
    this.Points = vector3Array;
    this.initialized = false;
  }

  public void Smoothen(int iterations = 1)
  {
    float num = 0.25f;
    for (int index1 = 0; index1 < iterations; ++index1)
    {
      Vector3 vector3 = this.Points[0];
      for (int index2 = 1; index2 < this.Points.Length - 1; ++index2)
      {
        Vector3 point1 = this.Points[index2];
        Vector3 point2 = this.Points[index2 + 1];
        this.Points[index2] = Vector3.op_Multiply(Vector3.op_Addition(Vector3.op_Addition(Vector3.op_Addition(vector3, point1), point1), point2), num);
        vector3 = point1;
      }
    }
    this.initialized = false;
  }

  public Vector3 GetStartPoint()
  {
    return this.Points[this.MinIndex];
  }

  public Vector3 GetEndPoint()
  {
    return this.Points[this.MaxIndex];
  }

  public Vector3 GetStartTangent()
  {
    if (!this.initialized)
      throw new Exception("Tangents have not been calculated yet or are outdated.");
    return this.Tangents[this.MinIndex];
  }

  public Vector3 GetEndTangent()
  {
    if (!this.initialized)
      throw new Exception("Tangents have not been calculated yet or are outdated.");
    return this.Tangents[this.MaxIndex];
  }

  public Vector3 GetPoint(float distance)
  {
    float num1 = distance / this.Length * (float) this.Points.Length;
    int index = (int) num1;
    if ((double) num1 <= (double) this.MinIndex)
      return this.GetStartPoint();
    if ((double) num1 >= (double) this.MaxIndex)
      return this.GetEndPoint();
    Vector3 point1 = this.Points[index];
    Vector3 point2 = this.Points[index + 1];
    float num2 = num1 - (float) index;
    Vector3 vector3 = point2;
    double num3 = (double) num2;
    return Vector3.Lerp(point1, vector3, (float) num3);
  }

  public Vector3 GetTangent(float distance)
  {
    if (!this.initialized)
      throw new Exception("Tangents have not been calculated yet or are outdated.");
    float num1 = distance / this.Length * (float) this.Tangents.Length;
    int index = (int) num1;
    if ((double) num1 <= (double) this.MinIndex)
      return this.GetStartTangent();
    if ((double) num1 >= (double) this.MaxIndex)
      return this.GetEndTangent();
    Vector3 tangent1 = this.Tangents[index];
    Vector3 tangent2 = this.Tangents[index + 1];
    float num2 = num1 - (float) index;
    Vector3 vector3 = tangent2;
    double num3 = (double) num2;
    return Vector3.Lerp(tangent1, vector3, (float) num3);
  }

  public Vector3 GetPointCubicHermite(float distance)
  {
    if (!this.initialized)
      throw new Exception("Tangents have not been calculated yet or are outdated.");
    float num1 = distance / this.Length * (float) this.Points.Length;
    int index = (int) num1;
    if ((double) num1 <= (double) this.MinIndex)
      return this.GetStartPoint();
    if ((double) num1 >= (double) this.MaxIndex)
      return this.GetEndPoint();
    Vector3 point1 = this.Points[index];
    Vector3 point2 = this.Points[index + 1];
    Vector3 vector3_1 = Vector3.op_Multiply(this.Tangents[index], this.StepSize);
    Vector3 vector3_2 = Vector3.op_Multiply(this.Tangents[index + 1], this.StepSize);
    float num2 = num1 - (float) index;
    float num3 = num2 * num2;
    float num4 = num2 * num3;
    return Vector3.op_Addition(Vector3.op_Addition(Vector3.op_Addition(Vector3.op_Multiply((float) (2.0 * (double) num4 - 3.0 * (double) num3 + 1.0), point1), Vector3.op_Multiply(num4 - 2f * num3 + num2, vector3_1)), Vector3.op_Multiply((float) (-2.0 * (double) num4 + 3.0 * (double) num3), point2)), Vector3.op_Multiply(num4 - num3, vector3_2));
  }
}
