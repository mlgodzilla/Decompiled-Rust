// Decompiled with JetBrains decompiler
// Type: Rust.Ai.ScientistSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using UnityEngine;

namespace Rust.Ai
{
  public class ScientistSpawner : SpawnGroup
  {
    [Header("Scientist Spawner")]
    public bool Mobile = true;
    public bool OnlyAggroMarkedTargets = true;
    public bool IsPeacekeeper = true;
    public NPCPlayerApex.EnemyRangeEnum MaxRangeToSpawnLoc = NPCPlayerApex.EnemyRangeEnum.LongAttackRange;
    public Vector2 RadioEffectRepeatRange = new Vector2(10f, 15f);
    private float _nextForcedRespawn = float.PositiveInfinity;
    public bool NeverMove;
    public bool SpawnHostile;
    public bool IsBandit;
    public bool IsMilitaryTunnelLab;
    public WaypointSet Waypoints;
    public Transform[] LookAtInterestPointsStationary;
    public Model Model;
    [SerializeField]
    private AiLocationManager _mgr;
    private bool _lastSpawnCallHadAliveMembers;
    private bool _lastSpawnCallHadMaxAliveMembers;

    protected override void Spawn(int numToSpawn)
    {
      if (!AI.npc_enable)
        return;
      if (this.currentPopulation == this.maxPopulation)
      {
        this._lastSpawnCallHadMaxAliveMembers = true;
        this._lastSpawnCallHadAliveMembers = true;
      }
      else
      {
        if (this._lastSpawnCallHadMaxAliveMembers)
          this._nextForcedRespawn = Time.get_time() + 2200f;
        if ((double) Time.get_time() < (double) this._nextForcedRespawn)
        {
          if (this.currentPopulation == 0 && this._lastSpawnCallHadAliveMembers)
          {
            this._lastSpawnCallHadMaxAliveMembers = false;
            this._lastSpawnCallHadAliveMembers = false;
            return;
          }
          if (this.currentPopulation > 0)
          {
            this._lastSpawnCallHadMaxAliveMembers = false;
            this._lastSpawnCallHadAliveMembers = this.currentPopulation > 0;
            return;
          }
        }
        this._lastSpawnCallHadMaxAliveMembers = false;
        this._lastSpawnCallHadAliveMembers = this.currentPopulation > 0;
        base.Spawn(numToSpawn);
      }
    }

    protected override void PostSpawnProcess(BaseEntity entity, BaseSpawnPoint spawnPoint)
    {
      Scientist component = (Scientist) ((Component) entity).GetComponent<Scientist>();
      if (!Object.op_Implicit((Object) component))
        return;
      component.Stats.Hostility = this.SpawnHostile ? 1f : 0.0f;
      component.Stats.Defensiveness = this.SpawnHostile ? 1f : (this.IsBandit ? 1f : 0.0f);
      component.Stats.OnlyAggroMarkedTargets = this.OnlyAggroMarkedTargets;
      component.Stats.IsMobile = this.Mobile;
      component.NeverMove = this.NeverMove;
      component.WaypointSet = this.Waypoints;
      if (this.LookAtInterestPointsStationary != null && this.LookAtInterestPointsStationary.Length != 0)
        component.LookAtInterestPointsStationary = this.LookAtInterestPointsStationary;
      component.RadioEffectRepeatRange = this.RadioEffectRepeatRange;
      component.SetFact(NPCPlayerApex.Facts.IsPeacekeeper, this.IsPeacekeeper ? (byte) 1 : (byte) 0, true, true);
      component.SetFact(NPCPlayerApex.Facts.IsBandit, this.IsBandit ? (byte) 1 : (byte) 0, true, true);
      component.SetFact(NPCPlayerApex.Facts.IsMilitaryTunnelLab, this.IsMilitaryTunnelLab ? (byte) 1 : (byte) 0, true, true);
      component.Stats.MaxRangeToSpawnLoc = this.MaxRangeToSpawnLoc;
      if (!this.SpawnHostile)
      {
        component.SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, true);
        component.SetFact(NPCPlayerApex.Facts.Speed, (byte) 0, true, true);
      }
      if (Object.op_Equality((Object) this._mgr, (Object) null))
        this._mgr = (AiLocationManager) ((Component) this).GetComponentInParent<AiLocationManager>();
      if (!Object.op_Inequality((Object) this._mgr, (Object) null))
        return;
      component.AiContext.AiLocationManager = this._mgr;
    }

    protected override void OnDrawGizmos()
    {
      base.OnDrawGizmos();
      if (this.LookAtInterestPointsStationary == null || this.LookAtInterestPointsStationary.Length == 0)
        return;
      Gizmos.set_color(Color.op_Subtraction(Color.get_magenta(), new Color(0.0f, 0.0f, 0.0f, 0.5f)));
      foreach (Transform transform in this.LookAtInterestPointsStationary)
      {
        if (Object.op_Inequality((Object) transform, (Object) null))
        {
          Gizmos.DrawSphere(transform.get_position(), 0.1f);
          Gizmos.DrawLine(((Component) this).get_transform().get_position(), transform.get_position());
        }
      }
    }
  }
}
