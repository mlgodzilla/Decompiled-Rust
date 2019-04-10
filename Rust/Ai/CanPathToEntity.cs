// Decompiled with JetBrains decompiler
// Type: Rust.Ai.CanPathToEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine.AI;

namespace Rust.Ai
{
  public sealed class CanPathToEntity : WeightedScorerBase<BaseEntity>
  {
    private static readonly NavMeshPath pathToEntity = new NavMeshPath();

    public override float GetScore(BaseContext c, BaseEntity target)
    {
      return c.AIAgent.IsNavRunning() && c.AIAgent.GetNavAgent.CalculatePath(target.ServerPosition, CanPathToEntity.pathToEntity) && CanPathToEntity.pathToEntity.get_status() == null ? 1f : 0.0f;
    }
  }
}
