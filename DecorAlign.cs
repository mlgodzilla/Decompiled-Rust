// Decompiled with JetBrains decompiler
// Type: DecorAlign
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class DecorAlign : DecorComponent
{
  public float NormalAlignment = 1f;
  public float GradientAlignment = 1f;
  public Vector3 SlopeOffset = Vector3.get_zero();
  public Vector3 SlopeScale = Vector3.get_one();

  public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
  {
    Vector3 normal = TerrainMeta.HeightMap.GetNormal(pos);
    Vector3 vector3_1 = Vector3.op_Equality(normal, Vector3.get_up()) ? Vector3.get_forward() : Vector3.Cross(normal, Vector3.get_up());
    Vector3 vector3_2 = Vector3.Cross(normal, vector3_1);
    if (Vector3.op_Inequality(this.SlopeOffset, Vector3.get_zero()) || Vector3.op_Inequality(this.SlopeScale, Vector3.get_one()))
    {
      float slope01 = TerrainMeta.HeightMap.GetSlope01(pos);
      if (Vector3.op_Inequality(this.SlopeOffset, Vector3.get_zero()))
      {
        Vector3 vector3_3 = Vector3.op_Multiply(this.SlopeOffset, slope01);
        pos = Vector3.op_Addition(pos, Vector3.op_Multiply((float) vector3_3.x, vector3_1));
        pos = Vector3.op_Addition(pos, Vector3.op_Multiply((float) vector3_3.y, normal));
        pos = Vector3.op_Subtraction(pos, Vector3.op_Multiply((float) vector3_3.z, vector3_2));
      }
      if (Vector3.op_Inequality(this.SlopeScale, Vector3.get_one()))
      {
        Vector3 vector3_3 = Vector3.Lerp(Vector3.get_one(), Vector3.op_Addition(Vector3.get_one(), Quaternion.op_Multiply(Quaternion.Inverse(rot), Vector3.op_Subtraction(this.SlopeScale, Vector3.get_one()))), slope01);
        ref __Null local1 = ref scale.x;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local1 = ^(float&) ref local1 * (float) vector3_3.x;
        ref __Null local2 = ref scale.y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local2 = ^(float&) ref local2 * (float) vector3_3.y;
        ref __Null local3 = ref scale.z;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local3 = ^(float&) ref local3 * (float) vector3_3.z;
      }
    }
    Vector3 up = Vector3.Lerp(Quaternion.op_Multiply(rot, Vector3.get_up()), normal, this.NormalAlignment);
    Quaternion quaternion = QuaternionEx.LookRotationForcedUp(Vector3.Lerp(Quaternion.op_Multiply(rot, Vector3.get_forward()), vector3_2, this.GradientAlignment), up);
    rot = Quaternion.op_Multiply(quaternion, rot);
  }
}
