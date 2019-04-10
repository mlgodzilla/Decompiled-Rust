// Decompiled with JetBrains decompiler
// Type: PlantEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Facepunch.Extend;
using Network;
using Oxide.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlantEntity : BaseCombatEntity, IInstanceDataReceiver
{
  public int water = -1;
  public int consumedWater = -1;
  private float groundConditions = 1f;
  private int genetics = -1;
  public PlantProperties plantProperty;
  public PlantProperties.State state;
  public float realAge;
  public float growthAge;
  private float stageAge;
  private float lightExposure;
  private int seasons;
  private int harvests;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("PlantEntity.OnRpcMessage", 0.1f))
    {
      if (rpc == 598660365U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_PickFruit "));
        using (TimeWarning.New("RPC_PickFruit", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_PickFruit", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_PickFruit(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_PickFruit");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 2222960834U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_TakeClone "));
          using (TimeWarning.New("RPC_TakeClone", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_TakeClone", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_TakeClone(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_TakeClone");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  private PlantProperties.Stage currentStage
  {
    get
    {
      return this.plantProperty.stages[(int) this.state];
    }
  }

  public float stageAgeFraction
  {
    get
    {
      return this.stageAge / (this.currentStage.lifeLength * 60f);
    }
  }

  public override void ResetState()
  {
    base.ResetState();
    this.state = PlantProperties.State.Seed;
  }

  public bool CanPick()
  {
    return (double) this.currentStage.resources > 0.0;
  }

  public float GetGrowthAge()
  {
    return this.growthAge;
  }

  public float GetStageAge()
  {
    return this.stageAge;
  }

  public float GetRealAge()
  {
    return this.realAge;
  }

  public bool CanClone()
  {
    if ((double) this.currentStage.resources > 0.0)
      return Object.op_Inequality((Object) this.plantProperty.cloneItem, (Object) null);
    return false;
  }

  public void ReceiveInstanceData(Item.InstanceData data)
  {
    this.genetics = (int) data.dataInt;
  }

  public float YieldBonusScale()
  {
    return (float) this.consumedWater / (float) this.plantProperty.lifetimeWaterConsumption;
  }

  public override void ServerInit()
  {
    if (this.genetics == -1)
      this.genetics = Random.Range(0, 10000);
    this.groundConditions = PlantEntity.WorkoutGroundConditions(((Component) this).get_transform().get_position());
    base.ServerInit();
    this.InvokeRandomized(new Action(this.RunUpdate), this.thinkDeltaTime, this.thinkDeltaTime, this.thinkDeltaTime * 0.1f);
    this.health = 10f;
    this.ResetSeason();
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.plantEntity = (__Null) Pool.Get<PlantEntity>();
    ((PlantEntity) info.msg.plantEntity).state = (__Null) this.state;
    ((PlantEntity) info.msg.plantEntity).age = (__Null) (double) this.growthAge;
    ((PlantEntity) info.msg.plantEntity).genetics = (__Null) this.genetics;
    ((PlantEntity) info.msg.plantEntity).water = (__Null) this.water;
    ((PlantEntity) info.msg.plantEntity).totalAge = (__Null) (double) this.realAge;
    ((PlantEntity) info.msg.plantEntity).growthAge = (__Null) (double) this.growthAge;
    ((PlantEntity) info.msg.plantEntity).stageAge = (__Null) (double) this.stageAge;
    if (info.forDisk)
      return;
    ((PlantEntity) info.msg.plantEntity).healthy = (__Null) ((double) this.consumedWater / (double) this.plantProperty.lifetimeWaterConsumption);
    ((PlantEntity) info.msg.plantEntity).yieldFraction = (double) this.currentStage.resources == 0.0 ? (__Null) 0.0 : (__Null) ((double) this.YieldBonusScale() * (double) this.plantProperty.waterYieldBonus + (double) this.currentStage.resources);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.MaxDistance(3f)]
  public void RPC_TakeClone(BaseEntity.RPCMessage msg)
  {
    if (!this.CanClone())
      return;
    int num = 1 + Mathf.Clamp(Mathf.CeilToInt((float) ((double) this.currentStage.resources * (1.0 + (double) this.YieldBonusScale()) / 0.25)), 1, 4);
    for (int index = 0; index < num; ++index)
    {
      Item obj = ItemManager.Create(this.plantProperty.cloneItem, 1, 0UL);
      obj.instanceData = new Item.InstanceData();
      obj.instanceData.dataInt = (__Null) Mathf.CeilToInt((float) this.genetics * 0.9f);
      obj.instanceData.ShouldPool = (__Null) 0;
      msg.player.GiveItem(obj, BaseEntity.GiveItemReason.PickedUp);
    }
    if (this.plantProperty.pickEffect.isValid)
      Effect.server.Run(this.plantProperty.pickEffect.resourcePath, ((Component) this).get_transform().get_position(), Vector3.get_up(), (Connection) null, false);
    this.Die((HitInfo) null);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.MaxDistance(3f)]
  public void RPC_PickFruit(BaseEntity.RPCMessage msg)
  {
    if (!this.CanPick())
      return;
    ++this.harvests;
    int iAmount = Mathf.RoundToInt((this.currentStage.resources + this.YieldBonusScale() * (float) this.plantProperty.waterYieldBonus) * (float) this.plantProperty.pickupAmount);
    this.ResetSeason();
    if (this.plantProperty.pickupItem.condition.enabled)
    {
      for (int index = 0; index < iAmount; ++index)
      {
        Item obj = ItemManager.Create(this.plantProperty.pickupItem, 1, 0UL);
        obj.conditionNormalized = this.plantProperty.fruitCurve.Evaluate(this.stageAgeFraction);
        if (Interface.CallHook("OnCropGather", (object) this, (object) obj, (object) msg.player) != null)
          return;
        msg.player.GiveItem(obj, BaseEntity.GiveItemReason.PickedUp);
      }
    }
    else
    {
      Item obj = ItemManager.Create(this.plantProperty.pickupItem, iAmount, 0UL);
      if (Interface.CallHook("OnCropGather", (object) this, (object) obj, (object) msg.player) != null)
        return;
      msg.player.GiveItem(obj, BaseEntity.GiveItemReason.PickedUp);
    }
    if (this.plantProperty.pickEffect.isValid)
      Effect.server.Run(this.plantProperty.pickEffect.resourcePath, ((Component) this).get_transform().get_position(), Vector3.get_up(), (Connection) null, false);
    if (this.harvests >= this.plantProperty.maxHarvests)
    {
      if (this.plantProperty.disappearAfterHarvest)
        this.Die((HitInfo) null);
      else
        this.BecomeState(PlantProperties.State.Dying, true);
    }
    else
    {
      this.growthAge = this.plantProperty.waterConsumptionLifetime - this.plantProperty.stages[3].lifeLength;
      this.BecomeState(PlantProperties.State.Mature, true);
    }
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.plantEntity == null)
      return;
    this.genetics = (int) ((PlantEntity) info.msg.plantEntity).genetics;
    this.growthAge = (float) ((PlantEntity) info.msg.plantEntity).age;
    this.water = (int) ((PlantEntity) info.msg.plantEntity).water;
    this.realAge = (float) ((PlantEntity) info.msg.plantEntity).totalAge;
    this.growthAge = (float) ((PlantEntity) info.msg.plantEntity).growthAge;
    this.stageAge = (float) ((PlantEntity) info.msg.plantEntity).stageAge;
    this.BecomeState((PlantProperties.State) ((PlantEntity) info.msg.plantEntity).state, false);
  }

  private void BecomeState(PlantProperties.State state, bool resetAge = true)
  {
    if (this.isServer && this.state == state)
      return;
    this.state = state;
    if (!this.isServer)
      return;
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    if (!resetAge)
      return;
    this.stageAge = 0.0f;
  }

  protected float thinkDeltaTime
  {
    get
    {
      return Server.planttick;
    }
  }

  protected float growDeltaTime
  {
    get
    {
      return Server.planttick * Server.planttickscale;
    }
  }

  public void ResetSeason()
  {
    this.consumedWater = 0;
    if (this.water != -1)
      return;
    this.water = Mathf.CeilToInt((float) this.plantProperty.maxHeldWater * 0.5f);
  }

  public override string DebugText()
  {
    return string.Format("State: {0}\nGenetics: {1:0.00}\nHealth: {2:0.00}\nGroundCondition: {3:0.00}\nHappiness: {4:0.00}\nWater: {5:0.00}\nAge: {6}", (object) this.state, (object) this.genetics, (object) this.health, (object) this.groundConditions, (object) this.Happiness(), (object) this.water, (object) NumberExtensions.FormatSeconds((long) this.realAge));
  }

  public void RefreshLightExposure()
  {
    if (!Server.plantlightdetection)
    {
      this.lightExposure = this.plantProperty.timeOfDayHappiness.Evaluate((float) ((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).Hour);
    }
    else
    {
      this.lightExposure = this.CalculateSunExposure() * this.plantProperty.timeOfDayHappiness.Evaluate((float) ((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).Hour);
      if ((double) this.lightExposure > 0.0)
        return;
      this.lightExposure = this.CalculateArtificialLightExposure() * 2f;
    }
  }

  public float CalculateArtificialLightExposure()
  {
    float num = 0.0f;
    List<CeilingLight> list = (List<CeilingLight>) Pool.GetList<CeilingLight>();
    Vis.Entities<CeilingLight>(Vector3.op_Addition(((Component) this).get_transform().get_position(), new Vector3(0.0f, 2f, 0.0f)), 2f, list, 256, (QueryTriggerInteraction) 2);
    foreach (BaseEntity baseEntity in list)
    {
      if (baseEntity.IsOn())
      {
        ++num;
        break;
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<CeilingLight>((List<M0>&) ref list);
    return num;
  }

  public float CalculateSunExposure()
  {
    if (TOD_Sky.get_Instance().get_IsNight())
      return 0.0f;
    Vector3 vector3_1 = Vector3.op_Addition(((Component) this).get_transform().get_position(), new Vector3(0.0f, 1f, 0.0f));
    Vector3 vector3_2 = Vector3.op_Subtraction(((GameObject) TOD_Sky.get_Instance().get_Components().Sun).get_transform().get_position(), vector3_1);
    Vector3 normalized = ((Vector3) ref vector3_2).get_normalized();
    RaycastHit raycastHit;
    return Physics.Raycast(vector3_1, normalized, ref raycastHit, 100f, 10551297) ? 0.0f : 1f;
  }

  private void RunUpdate()
  {
    if (this.IsDead())
      return;
    this.RefreshLightExposure();
    float num1 = this.Happiness();
    this.realAge += this.thinkDeltaTime;
    this.stageAge += this.growDeltaTime * Mathf.Max(num1, 0.0f);
    this.growthAge += this.growDeltaTime * Mathf.Max(num1, 0.0f);
    this.growthAge = Mathf.Clamp(this.growthAge, 0.0f, this.plantProperty.waterConsumptionLifetime * 60f);
    this.health += num1 * this.currentStage.health * this.growDeltaTime;
    if ((double) this.health <= 0.0)
    {
      this.Die((HitInfo) null);
    }
    else
    {
      if ((double) this.stageAge > (double) this.currentStage.lifeLength * 60.0)
      {
        if (this.state == PlantProperties.State.Dying)
        {
          this.Die((HitInfo) null);
          return;
        }
        if (this.currentStage.nextState <= this.state)
          ++this.seasons;
        if (this.seasons >= this.plantProperty.maxSeasons)
          this.BecomeState(PlantProperties.State.Dying, true);
        else
          this.BecomeState(this.currentStage.nextState, true);
      }
      if (this.PlacedInPlanter() && this.consumedWater < this.plantProperty.lifetimeWaterConsumption && this.state < PlantProperties.State.Fruiting)
      {
        float num2 = this.thinkDeltaTime / (this.plantProperty.waterConsumptionLifetime * 60f) * (float) this.plantProperty.lifetimeWaterConsumption;
        int num3 = Mathf.CeilToInt(Mathf.Min((float) this.water, num2));
        this.water -= num3;
        this.consumedWater += num3;
        PlanterBox parentEntity = this.GetParentEntity() as PlanterBox;
        if (Object.op_Implicit((Object) parentEntity) && (double) parentEntity.soilSaturationFraction > 0.0)
        {
          int num4 = this.plantProperty.maxHeldWater - this.water;
          this.water += parentEntity.UseWater(Mathf.Min(Mathf.CeilToInt(num2 * 10f), num4));
        }
      }
      else
        this.water = this.plantProperty.maxHeldWater;
      this.water = Mathf.Clamp(this.water, 0, this.plantProperty.maxHeldWater);
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    }
  }

  private bool PlacedInPlanter()
  {
    return Object.op_Inequality((Object) this.GetParentEntity(), (Object) null) && this.GetParentEntity() is PlanterBox;
  }

  private float Happiness()
  {
    float num = Mathf.Clamp((0.0f + this.Energy_Light() + this.Energy_Temperature() + this.Energy_Water() + (this.PlacedInPlanter() ? 2f : this.groundConditions)) / 4f, -1f, (float) (0.25 + (double) ((float) this.genetics / 10000f) * 0.75));
    if ((double) num > -0.100000001490116 && (double) num < 0.100000001490116)
      num = Mathf.Sign(num) * 0.1f;
    return num;
  }

  private float Energy_Light()
  {
    return this.lightExposure;
  }

  private float Energy_Temperature()
  {
    float num = this.plantProperty.temperatureHappiness.Evaluate(this.GetTemperature());
    if ((double) num > 0.0)
      return num * 0.2f;
    return num;
  }

  private float Energy_Water()
  {
    return (float) this.water;
  }

  private float GetTemperature()
  {
    float num = Climate.GetTemperature(((Component) this).get_transform().get_position());
    if (this.PlacedInPlanter() && (double) num < 10.0)
      num = 10f;
    return num;
  }

  public static float WorkoutGroundConditions(Vector3 pos)
  {
    if (WaterLevel.Test(pos))
      return -1f;
    TerrainSplat.Enum splatMaxType = (TerrainSplat.Enum) TerrainMeta.SplatMap.GetSplatMaxType(pos, -1);
    if (splatMaxType <= 16)
    {
      switch (splatMaxType - 1)
      {
        case 0:
          return 0.5f;
        case 1:
          return -1f;
        case 2:
          break;
        case 3:
          return -0.3f;
        default:
          if (splatMaxType == 8)
            return -0.7f;
          if (splatMaxType == 16)
            return 0.5f;
          break;
      }
    }
    else
    {
      if (splatMaxType == 32)
        return 0.4f;
      if (splatMaxType == 64 || splatMaxType == 128)
        return -0.6f;
    }
    return 0.5f;
  }
}
