// Decompiled with JetBrains decompiler
// Type: Scientist
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai;
using System.Collections.Generic;
using UnityEngine;

public class Scientist : NPCPlayerApex
{
  private static readonly HashSet<Scientist> AllScientists = new HashSet<Scientist>();
  private static readonly List<Scientist> CommQueryCache = new List<Scientist>();
  private static readonly List<AiAnswer_ShareEnemyTarget> CommTargetCache = new List<AiAnswer_ShareEnemyTarget>(10);
  [Header("Loot")]
  public LootContainer.LootSpawnSlot[] LootSpawnSlots;

  public override string Categorize()
  {
    return "scientist";
  }

  public override float StartHealth()
  {
    return Random.Range(this.startHealth, this.startHealth);
  }

  public override float StartMaxHealth()
  {
    return this.startHealth;
  }

  public override float MaxHealth()
  {
    return this.startHealth;
  }

  public override BaseNpc.AiStatistics.FamilyEnum Family
  {
    get
    {
      return BaseNpc.AiStatistics.FamilyEnum.Scientist;
    }
  }

  public override void ServerInit()
  {
    if (this.isClient)
      return;
    base.ServerInit();
    Scientist.AllScientists.Add(this);
    this.InitComm();
  }

  internal override void DoServerDestroy()
  {
    base.DoServerDestroy();
    Scientist.AllScientists.Remove(this);
    this.OnDestroyComm();
  }

  public override bool ShouldDropActiveItem()
  {
    return false;
  }

  public override BaseCorpse CreateCorpse()
  {
    using (TimeWarning.New("Create corpse", 0.1f))
    {
      NPCPlayerCorpse npcPlayerCorpse = this.DropCorpse("assets/prefabs/npc/scientist/scientist_corpse.prefab") as NPCPlayerCorpse;
      if (Object.op_Implicit((Object) npcPlayerCorpse))
      {
        ((Component) npcPlayerCorpse).get_transform().set_position(Vector3.op_Addition(((Component) npcPlayerCorpse).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_down(), this.NavAgent.get_baseOffset())));
        npcPlayerCorpse.SetLootableIn(2f);
        npcPlayerCorpse.SetFlag(BaseEntity.Flags.Reserved5, this.HasPlayerFlag(BasePlayer.PlayerFlags.DisplaySash), false, true);
        npcPlayerCorpse.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
        npcPlayerCorpse.TakeFrom(this.inventory.containerMain, this.inventory.containerWear, this.inventory.containerBelt);
        npcPlayerCorpse.playerName = this.displayName;
        npcPlayerCorpse.playerSteamID = this.userID;
        npcPlayerCorpse.Spawn();
        npcPlayerCorpse.TakeChildren((BaseEntity) this);
        foreach (ItemContainer container in npcPlayerCorpse.containers)
          container.Clear();
        if (this.LootSpawnSlots.Length != 0)
        {
          foreach (LootContainer.LootSpawnSlot lootSpawnSlot in this.LootSpawnSlots)
          {
            for (int index = 0; index < lootSpawnSlot.numberToSpawn; ++index)
            {
              if ((double) Random.Range(0.0f, 1f) <= (double) lootSpawnSlot.probability)
                lootSpawnSlot.definition.SpawnIntoContainer(npcPlayerCorpse.containers[0]);
            }
          }
        }
      }
      return (BaseCorpse) npcPlayerCorpse;
    }
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    this._displayName = string.Format("Scientist {0}", (object) (this.net != null ? (int) this.net.ID : "scientist".GetHashCode()));
  }

  private void InitComm()
  {
    this.OnAggro = this.OnAggro + new NPCPlayerApex.ActionCallback(this.OnAggroComm);
  }

  private void OnDestroyComm()
  {
    this.OnAggro = this.OnAggro - new NPCPlayerApex.ActionCallback(this.OnAggroComm);
  }

  public override int GetAlliesInRange(out List<Scientist> allies)
  {
    Scientist.CommQueryCache.Clear();
    foreach (Scientist allScientist in Scientist.AllScientists)
    {
      if (!Object.op_Equality((Object) allScientist, (Object) this) && this.IsInCommunicationRange((NPCPlayerApex) allScientist))
        Scientist.CommQueryCache.Add(allScientist);
    }
    allies = Scientist.CommQueryCache;
    return Scientist.CommQueryCache.Count;
  }

  public override void SendStatement(AiStatement_EnemyEngaged statement)
  {
    foreach (Scientist allScientist in Scientist.AllScientists)
    {
      if (!Object.op_Equality((Object) allScientist, (Object) this) && this.IsInCommunicationRange((NPCPlayerApex) allScientist))
        allScientist.OnAiStatement((NPCPlayerApex) this, statement);
    }
  }

  public override void SendStatement(AiStatement_EnemySeen statement)
  {
    foreach (Scientist allScientist in Scientist.AllScientists)
    {
      if (!Object.op_Equality((Object) allScientist, (Object) this) && this.IsInCommunicationRange((NPCPlayerApex) allScientist))
        allScientist.OnAiStatement((NPCPlayerApex) this, statement);
    }
  }

  public override void OnAiStatement(NPCPlayerApex source, AiStatement_EnemyEngaged statement)
  {
    if (!Object.op_Inequality((Object) statement.Enemy, (Object) null) || !statement.LastKnownPosition.HasValue || !this.HostilityConsideration(statement.Enemy) || !Object.op_Equality((Object) this.AiContext.EnemyPlayer, (Object) null) && !Object.op_Equality((Object) this.AiContext.EnemyPlayer, (Object) statement.Enemy))
      return;
    if (source.GetFact(NPCPlayerApex.Facts.AttackedRecently) > (byte) 0)
    {
      this.SetFact(NPCPlayerApex.Facts.AllyAttackedRecently, (byte) 1, true, true);
      this.AllyAttackedRecentlyTimeout = Time.get_realtimeSinceStartup() + 7f;
    }
    if (this.GetFact(NPCPlayerApex.Facts.IsBandit) > (byte) 0)
    {
      this.AiContext.LastAttacker = (BaseEntity) statement.Enemy;
      this.lastAttackedTime = source.lastAttackedTime;
    }
    Memory.ExtendedInfo extendedInfo;
    this.UpdateTargetMemory((BaseEntity) statement.Enemy, 0.1f, statement.LastKnownPosition.Value, out extendedInfo);
  }

  public override void OnAiStatement(NPCPlayerApex source, AiStatement_EnemySeen statement)
  {
  }

  public override int AskQuestion(
    AiQuestion_ShareEnemyTarget question,
    out List<AiAnswer_ShareEnemyTarget> answers)
  {
    Scientist.CommTargetCache.Clear();
    List<Scientist> allies;
    if (this.GetAlliesInRange(out allies) > 0)
    {
      foreach (NPCPlayerApex npcPlayerApex in allies)
      {
        AiAnswer_ShareEnemyTarget shareEnemyTarget = npcPlayerApex.OnAiQuestion((NPCPlayerApex) this, question);
        if (Object.op_Inequality((Object) shareEnemyTarget.PlayerTarget, (Object) null))
          Scientist.CommTargetCache.Add(shareEnemyTarget);
      }
    }
    answers = Scientist.CommTargetCache;
    return Scientist.CommTargetCache.Count;
  }

  private void OnAggroComm()
  {
    AiStatement_EnemyEngaged statement = new AiStatement_EnemyEngaged()
    {
      Enemy = this.AiContext.EnemyPlayer,
      Score = this.AiContext.LastTargetScore
    };
    if (Object.op_Inequality((Object) this.AiContext.EnemyPlayer, (Object) null))
    {
      Memory.SeenInfo info = this.AiContext.Memory.GetInfo((BaseEntity) this.AiContext.EnemyPlayer);
      if (Object.op_Inequality((Object) info.Entity, (Object) null) && !info.Entity.IsDestroyed && !this.AiContext.EnemyPlayer.IsDead())
        statement.LastKnownPosition = new Vector3?(info.Position);
      else
        statement.Enemy = (BasePlayer) null;
    }
    this.SendStatement(statement);
  }
}
