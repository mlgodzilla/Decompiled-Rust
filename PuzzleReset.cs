// Decompiled with JetBrains decompiler
// Type: PuzzleReset
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleReset : FacepunchBehaviour
{
  public SpawnGroup[] respawnGroups;
  public IOEntity[] resetEnts;
  public bool playersBlockReset;
  public float playerDetectionRadius;
  public Transform playerDetectionOrigin;
  public float timeBetweenResets;
  public bool scaleWithServerPopulation;
  [HideInInspector]
  public Vector3[] resetPositions;

  public float GetResetSpacing()
  {
    return this.timeBetweenResets * (this.scaleWithServerPopulation ? 1f - SpawnHandler.PlayerLerp(Spawn.min_rate, Spawn.max_rate) : 1f);
  }

  public void Start()
  {
    if ((double) this.timeBetweenResets == double.PositiveInfinity)
      return;
    this.ResetTimer();
  }

  public void ResetTimer()
  {
    this.CancelInvoke(new Action(this.TimedReset));
    this.Invoke(new Action(this.TimedReset), this.GetResetSpacing());
  }

  public bool PassesResetCheck()
  {
    if (!this.playersBlockReset)
      return true;
    List<BasePlayer> list = (List<BasePlayer>) Pool.GetList<BasePlayer>();
    Vis.Entities<BasePlayer>(this.playerDetectionOrigin.get_position(), this.playerDetectionRadius, list, 131072, (QueryTriggerInteraction) 2);
    bool flag = true;
    foreach (BasePlayer basePlayer in list)
    {
      if (!basePlayer.IsSleeping() && basePlayer.IsAlive() && !basePlayer.IsNpc)
      {
        flag = false;
        break;
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<BasePlayer>((List<M0>&) ref list);
    return flag;
  }

  public void TimedReset()
  {
    if (this.PassesResetCheck())
    {
      this.DoReset();
      this.Invoke(new Action(this.TimedReset), this.GetResetSpacing());
    }
    else
      this.Invoke(new Action(this.TimedReset), 30f);
  }

  public void DoReset()
  {
    IOEntity component = (IOEntity) ((Component) this).GetComponent<IOEntity>();
    if (Object.op_Inequality((Object) component, (Object) null))
    {
      PuzzleReset.ResetIOEntRecursive(component, Time.get_frameCount());
      component.MarkDirty();
    }
    else if (this.resetPositions != null)
    {
      foreach (Vector3 resetPosition in this.resetPositions)
      {
        Vector3 position = ((Component) this).get_transform().TransformPoint(resetPosition);
        List<IOEntity> list1 = (List<IOEntity>) Pool.GetList<IOEntity>();
        List<IOEntity> list2 = list1;
        Vis.Entities<IOEntity>(position, 0.5f, list2, 1235288065, (QueryTriggerInteraction) 1);
        foreach (IOEntity target in list1)
        {
          if (target.IsRootEntity())
          {
            PuzzleReset.ResetIOEntRecursive(target, Time.get_frameCount());
            target.MarkDirty();
          }
        }
        // ISSUE: cast to a reference type
        Pool.FreeList<IOEntity>((List<M0>&) ref list1);
      }
    }
    List<SpawnGroup> list = (List<SpawnGroup>) Pool.GetList<SpawnGroup>();
    Vis.Components<SpawnGroup>(((Component) this).get_transform().get_position(), 1f, list, 262144, (QueryTriggerInteraction) 2);
    foreach (SpawnGroup spawnGroup in list)
    {
      if (!Object.op_Equality((Object) spawnGroup, (Object) null))
        spawnGroup.Spawn();
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<SpawnGroup>((List<M0>&) ref list);
  }

  public static void ResetIOEntRecursive(IOEntity target, int resetIndex)
  {
    if (target.lastResetIndex == resetIndex)
      return;
    target.lastResetIndex = resetIndex;
    target.ResetIOState();
    foreach (IOEntity.IOSlot output in target.outputs)
    {
      if (Object.op_Inequality((Object) output.connectedTo.Get(true), (Object) null) && Object.op_Inequality((Object) output.connectedTo.Get(true), (Object) target))
        PuzzleReset.ResetIOEntRecursive(output.connectedTo.Get(true), resetIndex);
    }
  }

  public PuzzleReset()
  {
    base.\u002Ector();
  }
}
