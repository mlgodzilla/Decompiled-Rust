// Decompiled with JetBrains decompiler
// Type: AverageVelocity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class AverageVelocity
{
  private Vector3 pos;
  private float time;
  private float lastEntry;
  private float averageSpeed;
  private Vector3 averageVelocity;

  public void Record(Vector3 newPos)
  {
    float num = Time.get_time() - this.time;
    if ((double) num < 0.100000001490116)
      return;
    if ((double) ((Vector3) ref this.pos).get_sqrMagnitude() > 0.0)
    {
      this.averageVelocity = Vector3.op_Multiply(Vector3.op_Subtraction(newPos, this.pos), 1f / num);
      this.averageSpeed = ((Vector3) ref this.averageVelocity).get_magnitude();
    }
    this.time = Time.get_time();
    this.pos = newPos;
  }

  public float Speed
  {
    get
    {
      return this.averageSpeed;
    }
  }

  public Vector3 Average
  {
    get
    {
      return this.averageVelocity;
    }
  }
}
