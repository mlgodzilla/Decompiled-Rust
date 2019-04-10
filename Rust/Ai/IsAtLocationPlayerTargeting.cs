// Decompiled with JetBrains decompiler
// Type: Rust.Ai.IsAtLocationPlayerTargeting
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class IsAtLocationPlayerTargeting : ContextualScorerBase<PlayerTargetContext>
  {
    [ApexSerialization]
    public AiLocationSpawner.SquadSpawnerLocation Location;

    public virtual float Score(PlayerTargetContext c)
    {
      if (!IsAtLocationPlayerTargeting.Test(c, this.Location))
        return 0.0f;
      return (float) this.score;
    }

    public static bool Test(PlayerTargetContext c, AiLocationSpawner.SquadSpawnerLocation location)
    {
      NPCPlayerApex self = c.Self as NPCPlayerApex;
      if (Object.op_Inequality((Object) self, (Object) null) && Object.op_Inequality((Object) self.AiContext.AiLocationManager, (Object) null))
        return self.AiContext.AiLocationManager.LocationType == location;
      return false;
    }

    public IsAtLocationPlayerTargeting()
    {
      base.\u002Ector();
    }
  }
}
