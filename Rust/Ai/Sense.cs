// Decompiled with JetBrains decompiler
// Type: Rust.Ai.Sense
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Rust.Ai
{
  public static class Sense
  {
    private static BaseEntity[] query = new BaseEntity[512];

    public static void Stimulate(Sensation sensation)
    {
      int inSphere = BaseEntity.Query.Server.GetInSphere(sensation.Position, sensation.Radius, Sense.query, new Func<BaseEntity, bool>(Sense.IsAbleToBeStimulated));
      float num = sensation.Radius * sensation.Radius;
      for (int index = 0; index < inSphere; ++index)
      {
        Vector3 vector3 = Vector3.op_Subtraction(((Component) Sense.query[index]).get_transform().get_position(), sensation.Position);
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() <= (double) num)
          Sense.query[index].OnSensation(sensation);
      }
    }

    private static bool IsAbleToBeStimulated(BaseEntity ent)
    {
      return ent is BasePlayer || ent is BaseNpc;
    }
  }
}
