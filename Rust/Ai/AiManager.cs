// Decompiled with JetBrains decompiler
// Type: Rust.Ai.AiManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.LoadBalancing;
using Rust.Ai.HTN;
using Rust.Ai.HTN.ScientistJunkpile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Rust.Ai
{
  [DefaultExecutionOrder(-103)]
  public class AiManager : SingletonComponent<AiManager>, IServerComponent, ILoadBalanced
  {
    [ServerVar(Help = "If true we'll wait for the navmesh to generate before completely starting the server. This might cause your server to hitch and lag as it generates in the background.")]
    public static bool nav_wait = true;
    [ServerVar(Help = "If set to true the navmesh won't generate.. which means Ai that uses the navmesh won't be able to move")]
    public static bool nav_disable = false;
    [ServerVar(Help = "If ai_dormant is true, any npc outside the range of players will render itself dormant and take up less resources, but wildlife won't simulate as well.")]
    public static bool ai_dormant = true;
    [ServerVar(Help = "The maximum amount of nodes processed each frame in the asynchronous pathfinding process. Increasing this value will cause the paths to be processed faster, but can cause some hiccups in frame rate. Default value is 100, a good range for tuning is between 50 and 500.")]
    public static int pathfindingIterationsPerFrame = 100;
    [ServerVar(Help = "If an agent is beyond this distance to a player, it's flagged for becoming dormant.")]
    public static float ai_to_player_distance_wakeup_range = 160f;
    [ServerVar(Help = "nav_obstacles_carve_state defines which obstacles can carve the terrain. 0 - No carving, 1 - Only player construction carves, 2 - All obstacles carve.")]
    public static int nav_obstacles_carve_state = 2;
    [ServerVar(Help = "ai_dormant_max_wakeup_per_tick defines the maximum number of dormant agents we will wake up in a single tick. (default: 30)")]
    public static int ai_dormant_max_wakeup_per_tick = 30;
    [ServerVar(Help = "ai_htn_player_tick_budget defines the maximum amount of milliseconds ticking htn player agents are allowed to consume. (default: 4 ms)")]
    public static float ai_htn_player_tick_budget = 4f;
    [ServerVar(Help = "ai_htn_player_junkpile_tick_budget defines the maximum amount of milliseconds ticking htn player junkpile agents are allowed to consume. (default: 4 ms)")]
    public static float ai_htn_player_junkpile_tick_budget = 4f;
    [ServerVar(Help = "ai_htn_animal_tick_budget defines the maximum amount of milliseconds ticking htn animal agents are allowed to consume. (default: 4 ms)")]
    public static float ai_htn_animal_tick_budget = 4f;
    [ServerVar(Help = "If ai_htn_use_agency_tick is true, the ai manager's agency system will tick htn agents at the ms budgets defined in ai_htn_player_tick_budget and ai_htn_animal_tick_budget. If it's false, each agent registers with the invoke system individually, with no frame-budget restrictions. (default: true)")]
    public static bool ai_htn_use_agency_tick = true;
    private readonly HashSet<IAIAgent> activeAgents;
    private readonly List<IAIAgent> dormantAgents;
    private readonly HashSet<IAIAgent> pendingAddToActive;
    private readonly HashSet<IAIAgent> pendingAddToDormant;
    private readonly HashSet<IAIAgent> pendingRemoveFromActive;
    private readonly HashSet<IAIAgent> pendingRemoveFromDormant;
    private int lastWakeUpDormantIndex;
    [Header("Cover System")]
    [SerializeField]
    public bool UseCover;
    public float CoverPointVolumeCellSize;
    public float CoverPointVolumeCellHeight;
    public float CoverPointRayLength;
    public CoverPointVolume cpvPrefab;
    [SerializeField]
    public LayerMask DynamicCoverPointVolumeLayerMask;
    private WorldSpaceGrid<CoverPointVolume> coverPointVolumeGrid;
    private readonly BasePlayer[] playerVicinityQuery;
    private readonly Func<BasePlayer, bool> filter;

    internal void OnEnableAgency()
    {
    }

    internal void OnDisableAgency()
    {
    }

    public void Add(IAIAgent agent)
    {
      if (AiManager.ai_dormant)
      {
        if (this.IsAgentCloseToPlayers(agent))
          this.AddActiveAgency(agent);
        else
          this.AddDormantAgency(agent);
      }
      else
        this.AddActiveAgency(agent);
    }

    public void Remove(IAIAgent agent)
    {
      this.RemoveActiveAgency(agent);
      if (!AiManager.ai_dormant)
        return;
      this.RemoveDormantAgency(agent);
    }

    internal void AddActiveAgency(IAIAgent agent)
    {
      if (this.pendingAddToActive.Contains(agent))
        return;
      this.pendingAddToActive.Add(agent);
    }

    internal void AddDormantAgency(IAIAgent agent)
    {
      if (this.pendingAddToDormant.Contains(agent))
        return;
      this.pendingAddToDormant.Add(agent);
    }

    internal void RemoveActiveAgency(IAIAgent agent)
    {
      if (this.pendingRemoveFromActive.Contains(agent))
        return;
      this.pendingRemoveFromActive.Add(agent);
    }

    internal void RemoveDormantAgency(IAIAgent agent)
    {
      if (this.pendingRemoveFromDormant.Contains(agent))
        return;
      this.pendingRemoveFromDormant.Add(agent);
    }

    internal void UpdateAgency()
    {
      this.AgencyCleanup();
      this.AgencyAddPending();
      if (!AiManager.ai_dormant)
        return;
      this.TryWakeUpDormantAgents();
      this.TryMakeAgentsDormant();
    }

    private void AgencyCleanup()
    {
      if (AiManager.ai_dormant)
      {
        foreach (IAIAgent aiAgent in this.pendingRemoveFromDormant)
        {
          if (aiAgent != null)
            this.dormantAgents.Remove(aiAgent);
        }
        this.pendingRemoveFromDormant.Clear();
      }
      foreach (IAIAgent aiAgent in this.pendingRemoveFromActive)
      {
        if (aiAgent != null)
          this.activeAgents.Remove(aiAgent);
      }
      this.pendingRemoveFromActive.Clear();
    }

    private void AgencyAddPending()
    {
      if (AiManager.ai_dormant)
      {
        foreach (IAIAgent aiAgent in this.pendingAddToDormant)
        {
          if (aiAgent != null && !Object.op_Equality((Object) aiAgent.Entity, (Object) null) && !aiAgent.Entity.IsDestroyed)
          {
            this.dormantAgents.Add(aiAgent);
            aiAgent.IsDormant = true;
          }
        }
        this.pendingAddToDormant.Clear();
      }
      foreach (IAIAgent aiAgent in this.pendingAddToActive)
      {
        if (aiAgent != null && !Object.op_Equality((Object) aiAgent.Entity, (Object) null) && (!aiAgent.Entity.IsDestroyed && this.activeAgents.Add(aiAgent)))
          aiAgent.IsDormant = false;
      }
      this.pendingAddToActive.Clear();
    }

    private void TryWakeUpDormantAgents()
    {
      if (!AiManager.ai_dormant || this.dormantAgents.Count == 0)
        return;
      if (this.lastWakeUpDormantIndex >= this.dormantAgents.Count)
        this.lastWakeUpDormantIndex = 0;
      int wakeUpDormantIndex = this.lastWakeUpDormantIndex;
      int num = 0;
      while (num < AiManager.ai_dormant_max_wakeup_per_tick)
      {
        if (this.lastWakeUpDormantIndex >= this.dormantAgents.Count)
          this.lastWakeUpDormantIndex = 0;
        if (this.lastWakeUpDormantIndex == wakeUpDormantIndex && num > 0)
          break;
        IAIAgent dormantAgent = this.dormantAgents[this.lastWakeUpDormantIndex];
        ++this.lastWakeUpDormantIndex;
        ++num;
        if (dormantAgent.Entity.IsDestroyed)
          this.RemoveDormantAgency(dormantAgent);
        else if (this.IsAgentCloseToPlayers(dormantAgent))
        {
          this.AddActiveAgency(dormantAgent);
          this.RemoveDormantAgency(dormantAgent);
        }
      }
    }

    private void TryMakeAgentsDormant()
    {
      if (!AiManager.ai_dormant)
        return;
      foreach (IAIAgent activeAgent in this.activeAgents)
      {
        if (activeAgent.Entity.IsDestroyed)
          this.RemoveActiveAgency(activeAgent);
        else if (!this.IsAgentCloseToPlayers(activeAgent))
        {
          this.AddDormantAgency(activeAgent);
          this.RemoveActiveAgency(activeAgent);
        }
      }
    }

    public AiManager.AgencyHTN HTNAgency { get; }

    internal void OnEnableCover()
    {
      if (this.coverPointVolumeGrid != null)
        return;
      this.coverPointVolumeGrid = new WorldSpaceGrid<CoverPointVolume>((float) TerrainMeta.Size.x, this.CoverPointVolumeCellSize);
    }

    internal void OnDisableCover()
    {
      if (this.coverPointVolumeGrid == null || this.coverPointVolumeGrid.Cells == null)
        return;
      for (int index = 0; index < this.coverPointVolumeGrid.Cells.Length; ++index)
        Object.Destroy((Object) this.coverPointVolumeGrid.Cells[index]);
    }

    public static CoverPointVolume CreateNewCoverVolume(
      Vector3 point,
      Transform coverPointGroup)
    {
      if (!Object.op_Inequality((Object) SingletonComponent<AiManager>.Instance, (Object) null) || !((Behaviour) SingletonComponent<AiManager>.Instance).get_enabled() || !((AiManager) SingletonComponent<AiManager>.Instance).UseCover)
        return (CoverPointVolume) null;
      CoverPointVolume coverPointVolume = ((AiManager) SingletonComponent<AiManager>.Instance).GetCoverVolumeContaining(point);
      if (Object.op_Equality((Object) coverPointVolume, (Object) null))
      {
        Vector2i gridCoords = ((AiManager) SingletonComponent<AiManager>.Instance).coverPointVolumeGrid.WorldToGridCoords(point);
        coverPointVolume = !Object.op_Inequality((Object) ((AiManager) SingletonComponent<AiManager>.Instance).cpvPrefab, (Object) null) ? (CoverPointVolume) new GameObject("CoverPointVolume").AddComponent<CoverPointVolume>() : (CoverPointVolume) Object.Instantiate<CoverPointVolume>((M0) ((AiManager) SingletonComponent<AiManager>.Instance).cpvPrefab);
        ((Component) coverPointVolume).get_transform().set_localPosition((Vector3) null);
        ((Component) coverPointVolume).get_transform().set_position(Vector3.op_Addition(((AiManager) SingletonComponent<AiManager>.Instance).coverPointVolumeGrid.GridToWorldCoords(gridCoords), Vector3.op_Multiply(Vector3.get_up(), (float) point.y)));
        ((Component) coverPointVolume).get_transform().set_localScale(new Vector3(((AiManager) SingletonComponent<AiManager>.Instance).CoverPointVolumeCellSize, ((AiManager) SingletonComponent<AiManager>.Instance).CoverPointVolumeCellHeight, ((AiManager) SingletonComponent<AiManager>.Instance).CoverPointVolumeCellSize));
        coverPointVolume.CoverLayerMask = ((AiManager) SingletonComponent<AiManager>.Instance).DynamicCoverPointVolumeLayerMask;
        coverPointVolume.CoverPointRayLength = ((AiManager) SingletonComponent<AiManager>.Instance).CoverPointRayLength;
        ((AiManager) SingletonComponent<AiManager>.Instance).coverPointVolumeGrid.set_Item(gridCoords, coverPointVolume);
        coverPointVolume.GenerateCoverPoints(coverPointGroup);
      }
      return coverPointVolume;
    }

    public CoverPointVolume GetCoverVolumeContaining(Vector3 point)
    {
      if (this.coverPointVolumeGrid == null)
        return (CoverPointVolume) null;
      return this.coverPointVolumeGrid.get_Item(this.coverPointVolumeGrid.WorldToGridCoords(point));
    }

    public bool repeat
    {
      get
      {
        return true;
      }
    }

    public void Initialize()
    {
      this.OnEnableAgency();
      if (this.UseCover)
        this.OnEnableCover();
      AiManagerLoadBalancer.aiManagerLoadBalancer.Add((ILoadBalanced) this);
      if (this.HTNAgency == null)
        return;
      this.HTNAgency.OnEnableAgency();
      if (!AiManager.ai_htn_use_agency_tick)
        return;
      InvokeHandler.InvokeRepeating((Behaviour) this, new Action(this.HTNAgency.InvokedTick), 0.0f, 0.033f);
    }

    private void OnDisable()
    {
      if (Application.isQuitting != null)
        return;
      this.OnDisableAgency();
      if (this.UseCover)
        this.OnDisableCover();
      AiManagerLoadBalancer.aiManagerLoadBalancer.Remove((ILoadBalanced) this);
      if (this.HTNAgency == null)
        return;
      this.HTNAgency.OnDisableAgency();
      if (!AiManager.ai_htn_use_agency_tick)
        return;
      InvokeHandler.CancelInvoke((Behaviour) this, new Action(this.HTNAgency.InvokedTick));
    }

    public float? ExecuteUpdate(float deltaTime, float nextInterval)
    {
      if (AiManager.nav_disable)
        return new float?(nextInterval);
      this.UpdateAgency();
      this.HTNAgency?.UpdateAgency();
      return new float?(Random.get_value() + 1f);
    }

    private bool IsAgentCloseToPlayers(IAIAgent agent)
    {
      return BaseEntity.Query.Server.GetPlayersInSphere(((Component) agent.Entity).get_transform().get_position(), AiManager.ai_to_player_distance_wakeup_range, this.playerVicinityQuery, this.filter) > 0;
    }

    private static bool InterestedInPlayersOnly(BaseEntity entity)
    {
      BasePlayer basePlayer = entity as BasePlayer;
      return !Object.op_Equality((Object) basePlayer, (Object) null) && !(basePlayer is IAIAgent) && (!basePlayer.IsSleeping() && basePlayer.IsConnected);
    }

    public AiManager()
    {
      base.\u002Ector();
    }

    public class AgencyHTN
    {
      private readonly HashSet<IHTNAgent> activeAgents = new HashSet<IHTNAgent>();
      private readonly List<IHTNAgent> dormantAgents = new List<IHTNAgent>();
      private readonly HashSet<IHTNAgent> pendingAddToActive = new HashSet<IHTNAgent>();
      private readonly HashSet<IHTNAgent> pendingAddToDormant = new HashSet<IHTNAgent>();
      private readonly HashSet<IHTNAgent> pendingRemoveFromActive = new HashSet<IHTNAgent>();
      private readonly HashSet<IHTNAgent> pendingRemoveFromDormant = new HashSet<IHTNAgent>();
      private readonly List<HTNPlayer> tickingPlayers = new List<HTNPlayer>();
      private readonly List<HTNPlayer> tickingJunkpilePlayers = new List<HTNPlayer>();
      private readonly List<HTNAnimal> tickingAnimals = new List<HTNAnimal>();
      private Stopwatch watch = new Stopwatch();
      private readonly BasePlayer[] playerVicinityQuery = new BasePlayer[1];
      private readonly Func<BasePlayer, bool> filter = new Func<BasePlayer, bool>(AiManager.InterestedInPlayersOnly);
      private int playerTickIndex;
      private int junkpilePlayerTickIndex;
      private int animalTickIndex;
      private int lastWakeUpDormantIndex;

      internal void OnEnableAgency()
      {
      }

      internal void OnDisableAgency()
      {
      }

      public void InvokedTick()
      {
        this.watch.Reset();
        this.watch.Start();
        int playerTickIndex = this.playerTickIndex;
        while (this.tickingPlayers.Count > 0)
        {
          if (this.playerTickIndex >= this.tickingPlayers.Count)
            this.playerTickIndex = 0;
          HTNPlayer tickingPlayer = this.tickingPlayers[this.playerTickIndex];
          if (Object.op_Inequality((Object) tickingPlayer, (Object) null) && Object.op_Inequality((Object) ((Component) tickingPlayer).get_transform(), (Object) null) && !tickingPlayer.IsDestroyed)
            tickingPlayer.Tick();
          ++this.playerTickIndex;
          if (this.playerTickIndex >= this.tickingPlayers.Count)
            this.playerTickIndex = 0;
          if (this.playerTickIndex == playerTickIndex || this.watch.Elapsed.TotalMilliseconds > (double) AiManager.ai_htn_player_tick_budget)
            break;
        }
        this.watch.Reset();
        this.watch.Start();
        int junkpilePlayerTickIndex = this.junkpilePlayerTickIndex;
        while (this.tickingJunkpilePlayers.Count > 0)
        {
          if (this.junkpilePlayerTickIndex >= this.tickingJunkpilePlayers.Count)
            this.junkpilePlayerTickIndex = 0;
          HTNPlayer tickingJunkpilePlayer = this.tickingJunkpilePlayers[this.junkpilePlayerTickIndex];
          if (Object.op_Inequality((Object) tickingJunkpilePlayer, (Object) null) && Object.op_Inequality((Object) ((Component) tickingJunkpilePlayer).get_transform(), (Object) null) && !tickingJunkpilePlayer.IsDestroyed)
            tickingJunkpilePlayer.Tick();
          ++this.junkpilePlayerTickIndex;
          if (this.junkpilePlayerTickIndex >= this.tickingJunkpilePlayers.Count)
            this.junkpilePlayerTickIndex = 0;
          if (this.junkpilePlayerTickIndex == junkpilePlayerTickIndex || this.watch.Elapsed.TotalMilliseconds > (double) AiManager.ai_htn_player_junkpile_tick_budget)
            break;
        }
        this.watch.Reset();
        this.watch.Start();
        int animalTickIndex = this.animalTickIndex;
        while (this.tickingAnimals.Count > 0)
        {
          if (this.animalTickIndex >= this.tickingAnimals.Count)
            this.animalTickIndex = 0;
          HTNAnimal tickingAnimal = this.tickingAnimals[this.animalTickIndex];
          if (Object.op_Inequality((Object) tickingAnimal, (Object) null) && Object.op_Inequality((Object) ((Component) tickingAnimal).get_transform(), (Object) null) && !tickingAnimal.IsDestroyed)
            tickingAnimal.Tick();
          ++this.animalTickIndex;
          if (this.animalTickIndex >= this.tickingAnimals.Count)
            this.animalTickIndex = 0;
          if (this.animalTickIndex == animalTickIndex || this.watch.Elapsed.TotalMilliseconds > (double) AiManager.ai_htn_animal_tick_budget)
            break;
        }
      }

      public void Add(IHTNAgent agent)
      {
        if (AiManager.ai_dormant)
        {
          if (this.IsAgentCloseToPlayers(agent))
            this.AddActiveAgency(agent);
          else
            this.AddDormantAgency(agent);
        }
        else
          this.AddActiveAgency(agent);
      }

      public void Remove(IHTNAgent agent)
      {
        this.RemoveActiveAgency(agent);
        if (!AiManager.ai_dormant)
          return;
        this.RemoveDormantAgency(agent);
      }

      internal void AddActiveAgency(IHTNAgent agent)
      {
        if (this.pendingAddToActive.Contains(agent))
          return;
        this.pendingAddToActive.Add(agent);
      }

      internal void AddDormantAgency(IHTNAgent agent)
      {
        if (this.pendingAddToDormant.Contains(agent))
          return;
        this.pendingAddToDormant.Add(agent);
      }

      internal void RemoveActiveAgency(IHTNAgent agent)
      {
        if (this.pendingRemoveFromActive.Contains(agent))
          return;
        this.pendingRemoveFromActive.Add(agent);
      }

      internal void RemoveDormantAgency(IHTNAgent agent)
      {
        if (this.pendingRemoveFromDormant.Contains(agent))
          return;
        this.pendingRemoveFromDormant.Add(agent);
      }

      internal void UpdateAgency()
      {
        this.AgencyCleanup();
        this.AgencyAddPending();
        if (!AiManager.ai_dormant)
          return;
        this.TryWakeUpDormantAgents();
        this.TryMakeAgentsDormant();
      }

      private void AgencyCleanup()
      {
        if (AiManager.ai_dormant)
        {
          foreach (IHTNAgent htnAgent in this.pendingRemoveFromDormant)
          {
            if (htnAgent != null)
              this.dormantAgents.Remove(htnAgent);
          }
          this.pendingRemoveFromDormant.Clear();
        }
        foreach (IHTNAgent htnAgent in this.pendingRemoveFromActive)
        {
          if (htnAgent != null)
          {
            this.activeAgents.Remove(htnAgent);
            HTNPlayer htnPlayer = htnAgent as HTNPlayer;
            if (Object.op_Implicit((Object) htnPlayer))
            {
              if (htnPlayer.AiDomain is ScientistJunkpileDomain)
                this.tickingJunkpilePlayers.Remove(htnPlayer);
              else
                this.tickingPlayers.Remove(htnPlayer);
            }
            else
            {
              HTNAnimal htnAnimal = htnAgent as HTNAnimal;
              if (Object.op_Implicit((Object) htnAnimal))
                this.tickingAnimals.Remove(htnAnimal);
            }
          }
        }
        this.pendingRemoveFromActive.Clear();
      }

      private void AgencyAddPending()
      {
        if (AiManager.ai_dormant)
        {
          foreach (IHTNAgent htnAgent in this.pendingAddToDormant)
          {
            if (htnAgent != null && !htnAgent.IsDestroyed)
            {
              this.dormantAgents.Add(htnAgent);
              htnAgent.IsDormant = true;
            }
          }
          this.pendingAddToDormant.Clear();
        }
        foreach (IHTNAgent htnAgent in this.pendingAddToActive)
        {
          if (htnAgent != null && !htnAgent.IsDestroyed && this.activeAgents.Add(htnAgent))
          {
            htnAgent.IsDormant = false;
            HTNPlayer htnPlayer = htnAgent as HTNPlayer;
            if (Object.op_Implicit((Object) htnPlayer))
            {
              if (htnPlayer.AiDomain is ScientistJunkpileDomain)
                this.tickingJunkpilePlayers.Add(htnPlayer);
              else
                this.tickingPlayers.Add(htnPlayer);
            }
            else
            {
              HTNAnimal htnAnimal = htnAgent as HTNAnimal;
              if (Object.op_Implicit((Object) htnAnimal))
                this.tickingAnimals.Add(htnAnimal);
            }
          }
        }
        this.pendingAddToActive.Clear();
      }

      private void TryWakeUpDormantAgents()
      {
        if (!AiManager.ai_dormant || this.dormantAgents.Count == 0)
          return;
        if (this.lastWakeUpDormantIndex >= this.dormantAgents.Count)
          this.lastWakeUpDormantIndex = 0;
        int wakeUpDormantIndex = this.lastWakeUpDormantIndex;
        int num = 0;
        while (num < AiManager.ai_dormant_max_wakeup_per_tick)
        {
          if (this.lastWakeUpDormantIndex >= this.dormantAgents.Count)
            this.lastWakeUpDormantIndex = 0;
          if (this.lastWakeUpDormantIndex == wakeUpDormantIndex && num > 0)
            break;
          IHTNAgent dormantAgent = this.dormantAgents[this.lastWakeUpDormantIndex];
          ++this.lastWakeUpDormantIndex;
          ++num;
          if (dormantAgent.IsDestroyed)
            this.RemoveDormantAgency(dormantAgent);
          else if (this.IsAgentCloseToPlayers(dormantAgent))
          {
            this.AddActiveAgency(dormantAgent);
            this.RemoveDormantAgency(dormantAgent);
          }
        }
      }

      private void TryMakeAgentsDormant()
      {
        if (!AiManager.ai_dormant)
          return;
        foreach (IHTNAgent activeAgent in this.activeAgents)
        {
          if (activeAgent.IsDestroyed)
            this.RemoveActiveAgency(activeAgent);
          else if (!this.IsAgentCloseToPlayers(activeAgent))
          {
            this.AddDormantAgency(activeAgent);
            this.RemoveActiveAgency(activeAgent);
          }
        }
      }

      private bool IsAgentCloseToPlayers(IHTNAgent agent)
      {
        return BaseEntity.Query.Server.GetPlayersInSphere(agent.transform.get_position(), AiManager.ai_to_player_distance_wakeup_range, this.playerVicinityQuery, this.filter) > 0;
      }
    }
  }
}
