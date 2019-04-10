// Decompiled with JetBrains decompiler
// Type: Rust.Ai.AiLocationSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using UnityEngine;

namespace Rust.Ai
{
  public class AiLocationSpawner : SpawnGroup
  {
    public bool IsMainSpawner = true;
    public float chance = 1f;
    public AiLocationSpawner.SquadSpawnerLocation Location;
    public AiLocationManager Manager;
    public JunkPile Junkpile;
    private int defaultMaxPopulation;
    private int defaultNumToSpawnPerTickMax;
    private int defaultNumToSpawnPerTickMin;

    public override void SpawnInitial()
    {
      if (this.IsMainSpawner)
      {
        if (this.Location == AiLocationSpawner.SquadSpawnerLocation.MilitaryTunnels)
        {
          this.maxPopulation = AI.npc_max_population_military_tunnels;
          this.numToSpawnPerTickMax = AI.npc_spawn_per_tick_max_military_tunnels;
          this.numToSpawnPerTickMin = AI.npc_spawn_per_tick_min_military_tunnels;
          this.respawnDelayMax = AI.npc_respawn_delay_max_military_tunnels;
          this.respawnDelayMin = AI.npc_respawn_delay_min_military_tunnels;
        }
        else
        {
          this.defaultMaxPopulation = this.maxPopulation;
          this.defaultNumToSpawnPerTickMax = this.numToSpawnPerTickMax;
          this.defaultNumToSpawnPerTickMin = this.numToSpawnPerTickMin;
        }
      }
      else
      {
        this.defaultMaxPopulation = this.maxPopulation;
        this.defaultNumToSpawnPerTickMax = this.numToSpawnPerTickMax;
        this.defaultNumToSpawnPerTickMin = this.numToSpawnPerTickMin;
      }
      base.SpawnInitial();
    }

    protected override void Spawn(int numToSpawn)
    {
      if (!AI.npc_enable)
      {
        this.maxPopulation = 0;
        this.numToSpawnPerTickMax = 0;
        this.numToSpawnPerTickMin = 0;
      }
      else
      {
        if (numToSpawn == 0)
        {
          if (this.IsMainSpawner)
          {
            if (this.Location == AiLocationSpawner.SquadSpawnerLocation.MilitaryTunnels)
            {
              this.maxPopulation = AI.npc_max_population_military_tunnels;
              this.numToSpawnPerTickMax = AI.npc_spawn_per_tick_max_military_tunnels;
              this.numToSpawnPerTickMin = AI.npc_spawn_per_tick_min_military_tunnels;
              numToSpawn = Random.Range(this.numToSpawnPerTickMin, this.numToSpawnPerTickMax + 1);
            }
            else
            {
              this.maxPopulation = this.defaultMaxPopulation;
              this.numToSpawnPerTickMax = this.defaultNumToSpawnPerTickMax;
              this.numToSpawnPerTickMin = this.defaultNumToSpawnPerTickMin;
              numToSpawn = Random.Range(this.numToSpawnPerTickMin, this.numToSpawnPerTickMax + 1);
            }
          }
          else
          {
            this.maxPopulation = this.defaultMaxPopulation;
            this.numToSpawnPerTickMax = this.defaultNumToSpawnPerTickMax;
            this.numToSpawnPerTickMin = this.defaultNumToSpawnPerTickMin;
            numToSpawn = Random.Range(this.numToSpawnPerTickMin, this.numToSpawnPerTickMax + 1);
          }
        }
        float num = this.chance;
        switch (this.Location)
        {
          case AiLocationSpawner.SquadSpawnerLocation.JunkpileA:
            num = AI.npc_junkpile_a_spawn_chance;
            break;
          case AiLocationSpawner.SquadSpawnerLocation.JunkpileG:
            num = AI.npc_junkpile_g_spawn_chance;
            break;
        }
        if (numToSpawn == 0 || (double) Random.get_value() > (double) num || (this.Location == AiLocationSpawner.SquadSpawnerLocation.JunkpileA || this.Location == AiLocationSpawner.SquadSpawnerLocation.JunkpileG) && NPCPlayerApex.AllJunkpileNPCs.Count >= AI.npc_max_junkpile_count)
          return;
        numToSpawn = Mathf.Min(numToSpawn, this.maxPopulation - this.currentPopulation);
        for (int index = 0; index < numToSpawn; ++index)
        {
          Vector3 pos;
          Quaternion rot;
          BaseSpawnPoint spawnPoint = this.GetSpawnPoint(out pos, out rot);
          if (Object.op_Implicit((Object) spawnPoint))
          {
            BaseEntity entity = GameManager.server.CreateEntity(this.GetPrefab(), pos, rot, true);
            if (Object.op_Implicit((Object) entity))
            {
              if (Object.op_Inequality((Object) this.Manager, (Object) null))
              {
                NPCPlayerApex npc = entity as NPCPlayerApex;
                if (Object.op_Inequality((Object) npc, (Object) null))
                {
                  npc.AiContext.AiLocationManager = this.Manager;
                  if (Object.op_Inequality((Object) this.Junkpile, (Object) null))
                    this.Junkpile.AddNpc(npc);
                }
              }
              entity.Spawn();
              M0 m0 = ((Component) entity).get_gameObject().AddComponent<SpawnPointInstance>();
              ((SpawnPointInstance) m0).parentSpawnGroup = (SpawnGroup) this;
              ((SpawnPointInstance) m0).parentSpawnPoint = spawnPoint;
              ((SpawnPointInstance) m0).Notify();
            }
          }
        }
      }
    }

    protected override BaseSpawnPoint GetSpawnPoint(
      out Vector3 pos,
      out Quaternion rot)
    {
      return base.GetSpawnPoint(out pos, out rot);
    }

    public enum SquadSpawnerLocation
    {
      MilitaryTunnels,
      JunkpileA,
      JunkpileG,
      CH47,
      None,
      Compound,
      BanditTown,
      CargoShip,
    }
  }
}
