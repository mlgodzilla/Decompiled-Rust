// Decompiled with JetBrains decompiler
// Type: HTNPlayerSpawnGroup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN;
using UnityEngine;

public class HTNPlayerSpawnGroup : SpawnGroup
{
  [Header("HTN Player Spawn Group")]
  public HTNDomain.MovementRule Movement = HTNDomain.MovementRule.FreeMove;
  public float MovementRadius = -1f;

  protected override void PostSpawnProcess(BaseEntity entity, BaseSpawnPoint spawnPoint)
  {
    HTNPlayer htnPlayer = entity as HTNPlayer;
    if (!Object.op_Inequality((Object) htnPlayer, (Object) null) || !Object.op_Inequality((Object) htnPlayer.AiDomain, (Object) null))
      return;
    htnPlayer.AiDomain.Movement = this.Movement;
    htnPlayer.AiDomain.MovementRadius = this.MovementRadius;
  }
}
