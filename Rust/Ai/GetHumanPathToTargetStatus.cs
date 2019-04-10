// Decompiled with JetBrains decompiler
// Type: Rust.Ai.GetHumanPathToTargetStatus
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine.AI;

namespace Rust.Ai
{
  public class GetHumanPathToTargetStatus : BaseScorer
  {
    [ApexSerialization]
    public NavMeshPathStatus Status;

    public override float GetScore(BaseContext c)
    {
      return GetHumanPathToTargetStatus.Evaluate(c as NPCHumanContext, this.Status) ? 1f : 0.0f;
    }

    public static bool Evaluate(NPCHumanContext c, NavMeshPathStatus s)
    {
      byte fact = c.GetFact(NPCPlayerApex.Facts.PathToTargetStatus);
      return c.Human.ToPathStatus(fact) == s;
    }
  }
}
