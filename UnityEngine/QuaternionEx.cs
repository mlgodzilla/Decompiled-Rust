// Decompiled with JetBrains decompiler
// Type: UnityEngine.QuaternionEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace UnityEngine
{
  public static class QuaternionEx
  {
    public static Quaternion AlignToNormal(this Quaternion rot, Vector3 normal)
    {
      return Quaternion.op_Multiply(Quaternion.FromToRotation(Vector3.get_up(), normal), rot);
    }

    public static Quaternion LookRotationWithOffset(
      Vector3 offset,
      Vector3 forward,
      Vector3 up)
    {
      return Quaternion.op_Multiply(Quaternion.LookRotation(forward, Vector3.get_up()), Quaternion.Inverse(Quaternion.LookRotation(offset, Vector3.get_up())));
    }

    public static Quaternion LookRotationForcedUp(Vector3 forward, Vector3 up)
    {
      if (Vector3.op_Equality(forward, up))
        return Quaternion.LookRotation(up);
      Vector3 vector3 = Vector3.Cross(forward, up);
      forward = Vector3.Cross(up, vector3);
      return Quaternion.LookRotation(forward, up);
    }

    public static Quaternion LookRotationGradient(Vector3 normal, Vector3 up)
    {
      Vector3 vector3 = Vector3.op_Equality(normal, Vector3.get_up()) ? Vector3.get_forward() : Vector3.Cross(normal, Vector3.get_up());
      return QuaternionEx.LookRotationForcedUp(Vector3.Cross(normal, vector3), up);
    }

    public static Quaternion LookRotationNormal(Vector3 normal, Vector3 up = null)
    {
      if (Vector3.op_Inequality(up, Vector3.get_zero()))
        return QuaternionEx.LookRotationForcedUp(up, normal);
      if (Vector3.op_Equality(normal, Vector3.get_up()))
        return QuaternionEx.LookRotationForcedUp(Vector3.get_forward(), normal);
      if (Vector3.op_Equality(normal, Vector3.get_down()))
        return QuaternionEx.LookRotationForcedUp(Vector3.get_back(), normal);
      if (normal.y == 0.0)
        return QuaternionEx.LookRotationForcedUp(Vector3.get_up(), normal);
      Vector3 vector3 = Vector3.Cross(normal, Vector3.get_up());
      return QuaternionEx.LookRotationForcedUp(Vector3.op_UnaryNegation(Vector3.Cross(normal, vector3)), normal);
    }

    public static Quaternion EnsureValid(this Quaternion rot, float epsilon = 1.401298E-45f)
    {
      if ((double) Quaternion.Dot(rot, rot) < (double) epsilon)
        return Quaternion.get_identity();
      return rot;
    }
  }
}
