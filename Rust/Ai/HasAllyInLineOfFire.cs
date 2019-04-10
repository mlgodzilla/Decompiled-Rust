// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HasAllyInLineOfFire
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
  public class HasAllyInLineOfFire : BaseScorer
  {
    public override float GetScore(BaseContext ctx)
    {
      NPCHumanContext npcHumanContext = ctx as NPCHumanContext;
      if (npcHumanContext != null)
      {
        Scientist human = npcHumanContext.Human as Scientist;
        List<Scientist> allies;
        if (Object.op_Inequality((Object) human, (Object) null) && human.GetAlliesInRange(out allies) > 0)
        {
          foreach (Scientist scientist in allies)
          {
            Vector3 vector3_1 = Vector3.op_Subtraction(npcHumanContext.EnemyPosition, npcHumanContext.Position);
            Vector3 vector3_2 = Vector3.op_Subtraction(scientist.Entity.ServerPosition, npcHumanContext.Position);
            if ((double) ((Vector3) ref vector3_2).get_sqrMagnitude() < (double) ((Vector3) ref vector3_1).get_sqrMagnitude() && (double) Vector3.Dot(((Vector3) ref vector3_1).get_normalized(), ((Vector3) ref vector3_2).get_normalized()) > 0.899999976158142)
              return 1f;
          }
        }
      }
      return 0.0f;
    }
  }
}
