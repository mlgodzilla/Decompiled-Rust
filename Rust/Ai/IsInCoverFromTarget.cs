// Decompiled with JetBrains decompiler
// Type: Rust.Ai.IsInCoverFromTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class IsInCoverFromTarget : BaseScorer
  {
    [ApexSerialization]
    public float CoverArcThreshold = -0.75f;

    public override float GetScore(BaseContext ctx)
    {
      if (Object.op_Equality((Object) SingletonComponent<AiManager>.Instance, (Object) null) || !((Behaviour) SingletonComponent<AiManager>.Instance).get_enabled() || (!((AiManager) SingletonComponent<AiManager>.Instance).UseCover || Object.op_Equality((Object) ctx.AIAgent.AttackTarget, (Object) null)))
        return 0.0f;
      NPCHumanContext npcHumanContext = ctx as NPCHumanContext;
      return 0.0f;
    }
  }
}
