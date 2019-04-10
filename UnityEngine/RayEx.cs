// Decompiled with JetBrains decompiler
// Type: UnityEngine.RayEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace UnityEngine
{
  public static class RayEx
  {
    public static Vector3 ClosestPoint(this Ray ray, Vector3 pos)
    {
      return Vector3.op_Addition(((Ray) ref ray).get_origin(), Vector3.op_Multiply(Vector3.Dot(Vector3.op_Subtraction(pos, ((Ray) ref ray).get_origin()), ((Ray) ref ray).get_direction()), ((Ray) ref ray).get_direction()));
    }

    public static float Distance(this Ray ray, Vector3 pos)
    {
      Vector3 vector3 = Vector3.Cross(((Ray) ref ray).get_direction(), Vector3.op_Subtraction(pos, ((Ray) ref ray).get_origin()));
      return ((Vector3) ref vector3).get_magnitude();
    }

    public static float SqrDistance(this Ray ray, Vector3 pos)
    {
      Vector3 vector3 = Vector3.Cross(((Ray) ref ray).get_direction(), Vector3.op_Subtraction(pos, ((Ray) ref ray).get_origin()));
      return ((Vector3) ref vector3).get_sqrMagnitude();
    }

    public static bool IsNaNOrInfinity(this Ray r)
    {
      if (!Vector3Ex.IsNaNOrInfinity(((Ray) ref r).get_origin()))
        return Vector3Ex.IsNaNOrInfinity(((Ray) ref r).get_direction());
      return true;
    }
  }
}
