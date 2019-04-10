// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HasPathStatus
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine.AI;

namespace Rust.Ai
{
  public class HasPathStatus : BaseScorer
  {
    [ApexSerialization]
    private NavMeshPathStatus Status;

    public override float GetScore(BaseContext c)
    {
      return c.AIAgent.IsNavRunning() && c.AIAgent.GetNavAgent.get_pathStatus() == this.Status ? 1f : 0.0f;
    }
  }
}
