// Decompiled with JetBrains decompiler
// Type: SeparableSSS
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class SeparableSSS
{
  private static Vector3 Gaussian(float variance, float r, Color falloffColor)
  {
    Vector3 zero = Vector3.get_zero();
    for (int index = 0; index < 3; ++index)
    {
      float num = r / (1f / 1000f + ((Color) ref falloffColor).get_Item(index));
      ((Vector3) ref zero).set_Item(index, Mathf.Exp((float) (-((double) num * (double) num) / (2.0 * (double) variance))) / (6.28f * variance));
    }
    return zero;
  }

  private static Vector3 Profile(float r, Color falloffColor)
  {
    return Vector3.op_Addition(Vector3.op_Addition(Vector3.op_Addition(Vector3.op_Addition(Vector3.op_Multiply(0.1f, SeparableSSS.Gaussian(0.0484f, r, falloffColor)), Vector3.op_Multiply(0.118f, SeparableSSS.Gaussian(0.187f, r, falloffColor))), Vector3.op_Multiply(0.113f, SeparableSSS.Gaussian(0.567f, r, falloffColor))), Vector3.op_Multiply(0.358f, SeparableSSS.Gaussian(1.99f, r, falloffColor))), Vector3.op_Multiply(0.078f, SeparableSSS.Gaussian(7.41f, r, falloffColor)));
  }

  public static void CalculateKernel(
    Color[] target,
    int targetStart,
    int targetSize,
    Color subsurfaceColor,
    Color falloffColor)
  {
    int num1 = targetSize;
    int length = num1 * 2 - 1;
    float num2 = length > 20 ? 3f : 2f;
    float num3 = 2f;
    Color[] colorArray = new Color[length];
    float num4 = 2f * num2 / (float) (length - 1);
    for (int index = 0; index < length; ++index)
    {
      float num5 = (float) (-(double) num2 + (double) index * (double) num4);
      float num6 = (double) num5 < 0.0 ? -1f : 1f;
      colorArray[index].a = (__Null) ((double) num2 * (double) num6 * (double) Mathf.Abs(Mathf.Pow(num5, num3)) / (double) Mathf.Pow(num2, num3));
    }
    for (int index = 0; index < length; ++index)
    {
      Vector3 vector3 = Vector3.op_Multiply((float) (((index > 0 ? (double) Mathf.Abs((float) (colorArray[index].a - colorArray[index - 1].a)) : 0.0) + (index < length - 1 ? (double) Mathf.Abs((float) (colorArray[index].a - colorArray[index + 1].a)) : 0.0)) / 2.0), SeparableSSS.Profile((float) colorArray[index].a, falloffColor));
      colorArray[index].r = vector3.x;
      colorArray[index].g = vector3.y;
      colorArray[index].b = vector3.z;
    }
    Color color = colorArray[length / 2];
    for (int index = length / 2; index > 0; --index)
      colorArray[index] = colorArray[index - 1];
    colorArray[0] = color;
    Vector3 zero = Vector3.get_zero();
    for (int index = 0; index < length; ++index)
    {
      ref __Null local1 = ref zero.x;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local1 = ^(float&) ref local1 + (float) colorArray[index].r;
      ref __Null local2 = ref zero.y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local2 = ^(float&) ref local2 + (float) colorArray[index].g;
      ref __Null local3 = ref zero.z;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local3 = ^(float&) ref local3 + (float) colorArray[index].b;
    }
    for (int index = 0; index < length; ++index)
    {
      ref __Null local1 = ref colorArray[index].r;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local1 = ^(float&) ref local1 / (float) zero.x;
      ref __Null local2 = ref colorArray[index].g;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local2 = ^(float&) ref local2 / (float) zero.y;
      ref __Null local3 = ref colorArray[index].b;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local3 = ^(float&) ref local3 / (float) zero.z;
    }
    target[targetStart] = colorArray[0];
    for (uint index = 0; (long) index < (long) (num1 - 1); ++index)
      target[(long) targetStart + (long) index + 1L] = colorArray[(long) num1 + (long) index];
  }
}
