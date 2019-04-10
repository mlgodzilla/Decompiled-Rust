// Decompiled with JetBrains decompiler
// Type: IAIAgent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Rust.Ai;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IAIAgent
{
  BaseNpc.AiStatistics GetStats { get; }

  NavMeshAgent GetNavAgent { get; }

  IAIContext GetContext(Guid aiId);

  bool AgencyUpdateRequired { get; set; }

  bool IsOnOffmeshLinkAndReachedNewCoord { get; set; }

  Vector3 Destination { get; set; }

  bool IsStopped { get; set; }

  bool AutoBraking { get; set; }

  bool HasPath { get; }

  bool IsNavRunning();

  void Pause();

  void Resume();

  bool IsDormant { get; set; }

  void SetTargetPathStatus(float pendingDelay = 0.05f);

  void UpdateDestination(Vector3 newDestination);

  void UpdateDestination(Transform tx);

  void StopMoving();

  float TimeAtDestination { get; }

  bool IsStuck { get; }

  float TargetSpeed { get; set; }

  bool WantsToEat(BaseEntity eatable);

  BaseEntity FoodTarget { get; set; }

  void Eat();

  float GetAttackRate { get; }

  float GetAttackRange { get; }

  Vector3 GetAttackOffset { get; }

  void StartAttack();

  void StartAttack(AttackOperator.AttackType type, BaseCombatEntity target);

  bool AttackReady();

  BaseEntity AttackTarget { get; set; }

  float AttackTargetVisibleFor { get; }

  Memory.SeenInfo AttackTargetMemory { get; set; }

  BaseCombatEntity CombatTarget { get; }

  Vector3 AttackPosition { get; }

  Vector3 CrouchedAttackPosition { get; }

  Vector3 CurrentAimAngles { get; }

  float GetWantsToAttack(BaseEntity target);

  float FearLevel(BaseEntity ent);

  Vector3 SpawnPosition { get; set; }

  float GetActiveAggressionRangeSqr();

  BaseCombatEntity Entity { get; }

  float GetAttackCost { get; }

  float GetStamina { get; }

  float GetEnergy { get; }

  float GetSleep { get; }

  bool BusyTimerActive();

  void SetBusyFor(float dur);

  float GetStuckDuration { get; }

  float GetLastStuckTime { get; }

  float currentBehaviorDuration { get; }

  BaseNpc.Behaviour CurrentBehaviour { get; set; }

  int AgentTypeIndex { get; set; }

  byte GetFact(BaseNpc.Facts fact);

  void SetFact(
    BaseNpc.Facts fact,
    byte value,
    bool triggerCallback = true,
    bool onlyTriggerCallbackOnDiffValue = true);

  float ToSpeed(BaseNpc.SpeedEnum speed);

  List<NavPointSample> RequestNavPointSamplesInCircle(
    NavPointSampler.SampleCount sampleCount,
    float radius,
    NavPointSampler.SampleFeatures features = NavPointSampler.SampleFeatures.None);

  List<NavPointSample> RequestNavPointSamplesInCircleWaterDepthOnly(
    NavPointSampler.SampleCount sampleCount,
    float radius,
    float waterDepth);

  byte GetFact(NPCPlayerApex.Facts fact);

  void SetFact(
    NPCPlayerApex.Facts fact,
    byte value,
    bool triggerCallback = true,
    bool onlyTriggerCallbackOnDiffValue = true);

  float ToSpeed(NPCPlayerApex.SpeedEnum speed);

  int TopologyPreference();
}
