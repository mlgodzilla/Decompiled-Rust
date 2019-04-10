// Decompiled with JetBrains decompiler
// Type: AimConeUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class AimConeUtil
{
  public static Vector3 GetModifiedAimConeDirection(
    float aimCone,
    Vector3 inputVec,
    bool anywhereInside = true)
  {
    Quaternion quaternion1 = Quaternion.LookRotation(inputVec);
    Vector2 vector2_1;
    if (!anywhereInside)
    {
      Vector2 insideUnitCircle = Random.get_insideUnitCircle();
      vector2_1 = ((Vector2) ref insideUnitCircle).get_normalized();
    }
    else
      vector2_1 = Random.get_insideUnitCircle();
    Vector2 vector2_2 = vector2_1;
    Quaternion quaternion2 = Quaternion.Euler((float) (vector2_2.x * (double) aimCone * 0.5), (float) (vector2_2.y * (double) aimCone * 0.5), 0.0f);
    return Quaternion.op_Multiply(Quaternion.op_Multiply(quaternion1, quaternion2), Vector3.get_forward());
  }

  public static Quaternion GetAimConeQuat(float aimCone)
  {
    Vector3 insideUnitSphere = Random.get_insideUnitSphere();
    return Quaternion.Euler((float) (insideUnitSphere.x * (double) aimCone * 0.5), (float) (insideUnitSphere.y * (double) aimCone * 0.5), 0.0f);
  }
}
