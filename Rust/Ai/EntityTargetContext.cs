// Decompiled with JetBrains decompiler
// Type: Rust.Ai.EntityTargetContext
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;

namespace Rust.Ai
{
  public class EntityTargetContext : IAIContext
  {
    public IAIAgent Self;
    public BaseEntity[] Entities;
    public int EntityCount;
    public BaseNpc AnimalTarget;
    public float AnimalScore;
    public TimedExplosive ExplosiveTarget;
    public float ExplosiveScore;

    public void Refresh(IAIAgent self, BaseEntity[] entities, int entityCount)
    {
      this.Self = self;
      this.Entities = entities;
      this.EntityCount = entityCount;
      this.AnimalTarget = (BaseNpc) null;
      this.AnimalScore = 0.0f;
      this.ExplosiveTarget = (TimedExplosive) null;
      this.ExplosiveScore = 0.0f;
    }
  }
}
