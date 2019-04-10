// Decompiled with JetBrains decompiler
// Type: JunkPile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Network;
using Rust.Ai.HTN.ScientistJunkpile;
using System;
using System.Collections.Generic;
using UnityEngine;

public class JunkPile : BaseEntity
{
  public GameObjectRef sinkEffect;
  public SpawnGroup[] spawngroups;
  public ScientistJunkpileSpawner npcSpawnGroup;
  private const float lifetimeMinutes = 30f;
  private List<NPCPlayerApex> _npcs;
  private List<HTNPlayer> _htnPlayers;
  protected bool isSinking;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("JunkPile.OnRpcMessage", 0.1f))
      ;
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.Invoke(new Action(this.TimeOut), 1800f);
    this.InvokeRepeating(new Action(this.CheckEmpty), 10f, 30f);
    this.Invoke(new Action(this.SpawnInitial), 1f);
    this.isSinking = false;
  }

  private void SpawnInitial()
  {
    foreach (SpawnGroup spawngroup in this.spawngroups)
      spawngroup.SpawnInitial();
    this.npcSpawnGroup?.SpawnInitial();
  }

  public bool SpawnGroupsEmpty()
  {
    foreach (SpawnGroup spawngroup in this.spawngroups)
    {
      if (spawngroup.currentPopulation > 0)
        return false;
    }
    ScientistJunkpileSpawner npcSpawnGroup = this.npcSpawnGroup;
    // ISSUE: explicit non-virtual call
    return (npcSpawnGroup != null ? (__nonvirtual (npcSpawnGroup.currentPopulation) > 0 ? 1 : 0) : 0) == 0;
  }

  public void CheckEmpty()
  {
    if (!this.SpawnGroupsEmpty())
      return;
    this.CancelInvoke(new Action(this.CheckEmpty));
    this.SinkAndDestroy();
  }

  public virtual float TimeoutPlayerCheckRadius()
  {
    return 15f;
  }

  public void TimeOut()
  {
    if (this.SpawnGroupsEmpty())
    {
      this.SinkAndDestroy();
    }
    else
    {
      List<BasePlayer> list = (List<BasePlayer>) Pool.GetList<BasePlayer>();
      Vis.Entities<BasePlayer>(((Component) this).get_transform().get_position(), this.TimeoutPlayerCheckRadius(), list, 131072, (QueryTriggerInteraction) 2);
      bool flag = false;
      foreach (BasePlayer basePlayer in list)
      {
        if (!basePlayer.IsSleeping() && basePlayer.IsAlive())
        {
          flag = true;
          break;
        }
      }
      if (flag)
      {
        this.Invoke(new Action(this.TimeOut), 300f);
      }
      else
      {
        if (this._npcs != null)
        {
          foreach (NPCPlayerApex npc in this._npcs)
          {
            if (!Object.op_Equality((Object) npc, (Object) null) && !Object.op_Equality((Object) ((Component) npc).get_transform(), (Object) null) && (!npc.IsDestroyed && !npc.IsDead()))
              npc.Kill(BaseNetworkable.DestroyMode.None);
          }
          this._npcs.Clear();
        }
        if (this._htnPlayers != null)
        {
          foreach (HTNPlayer htnPlayer in this._htnPlayers)
          {
            if (!Object.op_Equality((Object) htnPlayer, (Object) null) && !Object.op_Equality((Object) ((Component) htnPlayer).get_transform(), (Object) null) && (!htnPlayer.IsDestroyed && !htnPlayer.IsDead()))
              htnPlayer.Kill(BaseNetworkable.DestroyMode.None);
          }
          this._htnPlayers.Clear();
        }
        this.SinkAndDestroy();
      }
      // ISSUE: cast to a reference type
      Pool.FreeList<BasePlayer>((List<M0>&) ref list);
    }
  }

  public void SinkAndDestroy()
  {
    this.CancelInvoke(new Action(this.SinkAndDestroy));
    foreach (SpawnGroup spawngroup in this.spawngroups)
      spawngroup.Clear();
    this.npcSpawnGroup?.Clear();
    this.ClientRPC((Connection) null, "CLIENT_StartSink");
    Transform transform = ((Component) this).get_transform();
    transform.set_position(Vector3.op_Subtraction(transform.get_position(), new Vector3(0.0f, 5f, 0.0f)));
    this.isSinking = true;
    this.Invoke(new Action(this.KillMe), 22f);
  }

  public void KillMe()
  {
    this.Kill(BaseNetworkable.DestroyMode.None);
  }

  public void AddNpc(NPCPlayerApex npc)
  {
    if (this._npcs == null)
      this._npcs = new List<NPCPlayerApex>(1);
    this._npcs.Add(npc);
  }

  public void AddNpc(HTNPlayer npc)
  {
    if (this._htnPlayers == null)
      this._htnPlayers = new List<HTNPlayer>();
    this._htnPlayers.Add(npc);
  }
}
