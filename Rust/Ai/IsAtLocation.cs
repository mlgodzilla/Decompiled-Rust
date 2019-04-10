// Decompiled with JetBrains decompiler
// Type: Rust.Ai.IsAtLocation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class IsAtLocation : BaseScorer
  {
    [ApexSerialization]
    public AiLocationSpawner.SquadSpawnerLocation Location;

    public override float GetScore(BaseContext ctx)
    {
      NPCHumanContext c = ctx as NPCHumanContext;
      return c != null && IsAtLocation.Test(c, this.Location) ? 1f : 0.0f;
    }

    public static bool Test(NPCHumanContext c, AiLocationSpawner.SquadSpawnerLocation location)
    {
      if (Object.op_Inequality((Object) c.AiLocationManager, (Object) null))
        return c.AiLocationManager.LocationType == location;
      return false;
    }
  }
}
