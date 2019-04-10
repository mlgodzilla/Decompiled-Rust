// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.ScientistJunkpile.ScientistJunkpileMemory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistJunkpile
{
  [Serializable]
  public class ScientistJunkpileMemory : BaseNpcMemory
  {
    public List<BasePlayer> MarkedEnemies = new List<BasePlayer>();
    [NonSerialized]
    public ScientistJunkpileContext ScientistJunkpileContext;
    public Vector3 CachedPreferredDistanceDestination;
    public float CachedPreferredDistanceDestinationTime;
    public Vector3 CachedCoverDestination;
    public float CachedCoverDestinationTime;

    public override BaseNpcDefinition Definition
    {
      get
      {
        return this.ScientistJunkpileContext.Body.AiDefinition;
      }
    }

    public ScientistJunkpileMemory(ScientistJunkpileContext context)
      : base((BaseNpcContext) context)
    {
      this.ScientistJunkpileContext = context;
    }

    public override void ResetState()
    {
      base.ResetState();
      this.MarkedEnemies.Clear();
    }

    protected override void OnSetPrimaryKnownEnemyPlayer(ref BaseNpcMemory.EnemyPlayerInfo info)
    {
      if (!this.MarkedEnemies.Contains(info.PlayerInfo.Player))
        return;
      base.OnSetPrimaryKnownEnemyPlayer(ref info);
      Vector3 vector3 = Vector3.op_Subtraction(info.LastKnownPosition, this.ScientistJunkpileContext.BodyPosition);
      if ((double) ((Vector3) ref vector3).get_sqrMagnitude() <= 1.0)
        return;
      this.ScientistJunkpileContext.HasVisitedLastKnownEnemyPlayerLocation = false;
    }

    public void MarkEnemy(BasePlayer player)
    {
      if (!Object.op_Inequality((Object) player, (Object) null) || this.MarkedEnemies.Contains(player))
        return;
      this.MarkedEnemies.Add(player);
    }

    protected override void OnForget(BasePlayer player)
    {
      this.MarkedEnemies.Remove(player);
    }

    public override bool ShouldRemoveOnPlayerForgetTimeout(float time, NpcPlayerInfo player)
    {
      return Object.op_Equality((Object) player.Player, (Object) null) || Object.op_Equality((Object) ((Component) player.Player).get_transform(), (Object) null) || (player.Player.IsDestroyed || player.Player.IsDead()) || player.Player.IsWounded() || (double) time > (double) player.Time + (double) this.Definition.Memory.ForgetInRangeTime && (!this.MarkedEnemies.Contains(player.Player) || (double) player.SqrDistance > 0.0 && (double) player.SqrDistance <= (double) this.Definition.Sensory.SqrVisionRange);
    }
  }
}
