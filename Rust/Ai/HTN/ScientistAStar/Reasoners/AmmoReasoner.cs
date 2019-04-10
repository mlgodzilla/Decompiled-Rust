// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.ScientistAStar.Reasoners.AmmoReasoner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai.HTN.Reasoning;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistAStar.Reasoners
{
  public class AmmoReasoner : INpcReasoner
  {
    public float TickFrequency { get; set; }

    public float LastTickTime { get; set; }

    public void Tick(IHTNAgent npc, float deltaTime, float time)
    {
      ScientistAStarContext npcContext = npc.AiDomain.NpcContext as ScientistAStarContext;
      if (npcContext == null)
        return;
      HTNPlayer htnPlayer = npc as HTNPlayer;
      if (Object.op_Equality((Object) htnPlayer, (Object) null))
        return;
      AttackEntity heldEntity = htnPlayer.GetHeldEntity() as AttackEntity;
      if (Object.op_Implicit((Object) heldEntity))
      {
        BaseProjectile baseProjectile = heldEntity as BaseProjectile;
        if (Object.op_Inequality((Object) baseProjectile, (Object) null))
        {
          float num = (float) baseProjectile.primaryMagazine.contents / (float) baseProjectile.primaryMagazine.capacity;
          if ((double) num > 0.899999976158142)
          {
            npcContext.SetFact(Rust.Ai.HTN.ScientistAStar.Facts.AmmoState, AmmoState.FullClip, true, true, true);
            return;
          }
          if ((double) num > 0.600000023841858)
          {
            npcContext.SetFact(Rust.Ai.HTN.ScientistAStar.Facts.AmmoState, AmmoState.HighClip, true, true, true);
            return;
          }
          if ((double) num > 0.170000001788139)
          {
            npcContext.SetFact(Rust.Ai.HTN.ScientistAStar.Facts.AmmoState, AmmoState.MediumClip, true, true, true);
            return;
          }
          if ((double) num > 0.0)
          {
            npcContext.SetFact(Rust.Ai.HTN.ScientistAStar.Facts.AmmoState, AmmoState.LowAmmo, true, true, true);
            return;
          }
          npcContext.SetFact(Rust.Ai.HTN.ScientistAStar.Facts.AmmoState, AmmoState.EmptyClip, true, true, true);
          return;
        }
      }
      npcContext.SetFact(Rust.Ai.HTN.ScientistAStar.Facts.AmmoState, AmmoState.DontRequireAmmo, true, true, true);
    }
  }
}
