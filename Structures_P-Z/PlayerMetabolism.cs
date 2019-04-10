// Decompiled with JetBrains decompiler
// Type: PlayerMetabolism
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using UnityEngine;

public class PlayerMetabolism : BaseMetabolism<BasePlayer>
{
  public MetabolismAttribute temperature = new MetabolismAttribute();
  public MetabolismAttribute poison = new MetabolismAttribute();
  public MetabolismAttribute radiation_level = new MetabolismAttribute();
  public MetabolismAttribute radiation_poison = new MetabolismAttribute();
  public MetabolismAttribute wetness = new MetabolismAttribute();
  public MetabolismAttribute dirtyness = new MetabolismAttribute();
  public MetabolismAttribute oxygen = new MetabolismAttribute();
  public MetabolismAttribute bleeding = new MetabolismAttribute();
  public MetabolismAttribute comfort = new MetabolismAttribute();
  public MetabolismAttribute pending_health = new MetabolismAttribute();
  public const float HotThreshold = 40f;
  public const float ColdThreshold = 5f;
  public bool isDirty;
  private float lastConsumeTime;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("PlayerMetabolism.OnRpcMessage", 0.1f))
      ;
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override void Reset()
  {
    base.Reset();
    this.poison.Reset();
    this.radiation_level.Reset();
    this.radiation_poison.Reset();
    this.temperature.Reset();
    this.oxygen.Reset();
    this.bleeding.Reset();
    this.wetness.Reset();
    this.dirtyness.Reset();
    this.comfort.Reset();
    this.pending_health.Reset();
    this.lastConsumeTime = float.NegativeInfinity;
    this.isDirty = true;
  }

  public override void ServerUpdate(BaseCombatEntity ownerEntity, float delta)
  {
    base.ServerUpdate(ownerEntity, delta);
    Interface.CallHook("OnPlayerMetabolize", (object) this, (object) ownerEntity, (object) delta);
    this.SendChangesToClient();
  }

  internal bool HasChanged()
  {
    return this.pending_health.HasChanged() | (this.comfort.HasChanged() | (this.dirtyness.HasChanged() | (this.wetness.HasChanged() | (this.temperature.HasChanged() | (this.radiation_poison.HasChanged() | (this.radiation_level.HasChanged() | (this.poison.HasChanged() | (this.heartrate.HasChanged() | (this.hydration.HasChanged() | (this.calories.HasChanged() | this.isDirty))))))))));
  }

  protected override void DoMetabolismDamage(BaseCombatEntity ownerEntity, float delta)
  {
    base.DoMetabolismDamage(ownerEntity, delta);
    if ((double) this.temperature.value < -20.0)
      this.owner.Hurt((float) ((double) Mathf.InverseLerp(1f, -50f, this.temperature.value) * (double) delta * 1.0), DamageType.Cold, (BaseEntity) null, true);
    else if ((double) this.temperature.value < -10.0)
      this.owner.Hurt((float) ((double) Mathf.InverseLerp(1f, -50f, this.temperature.value) * (double) delta * 0.300000011920929), DamageType.Cold, (BaseEntity) null, true);
    else if ((double) this.temperature.value < 1.0)
      this.owner.Hurt((float) ((double) Mathf.InverseLerp(1f, -50f, this.temperature.value) * (double) delta * 0.100000001490116), DamageType.Cold, (BaseEntity) null, true);
    if ((double) this.temperature.value > 60.0)
      this.owner.Hurt((float) ((double) Mathf.InverseLerp(60f, 200f, this.temperature.value) * (double) delta * 5.0), DamageType.Heat, (BaseEntity) null, true);
    if ((double) this.oxygen.value < 0.5)
      this.owner.Hurt((float) ((double) Mathf.InverseLerp(0.5f, 0.0f, this.oxygen.value) * (double) delta * 20.0), DamageType.Drowned, (BaseEntity) null, false);
    if ((double) this.bleeding.value > 0.0)
    {
      float num = delta * 0.3333333f;
      this.owner.Hurt(num, DamageType.Bleeding, (BaseEntity) null, true);
      this.bleeding.Subtract(num);
    }
    if ((double) this.poison.value > 0.0)
      this.owner.Hurt((float) ((double) this.poison.value * (double) delta * 0.100000001490116), DamageType.Poison, (BaseEntity) null, true);
    if (!ConVar.Server.radiation || (double) this.radiation_poison.value <= 0.0)
      return;
    float num1 = (float) ((1.0 + (double) Mathf.Clamp01(this.radiation_poison.value / 25f) * 5.0) * ((double) delta / 5.0));
    this.owner.Hurt(num1, DamageType.Radiation, (BaseEntity) null, true);
    this.radiation_poison.Subtract(num1);
  }

  public bool SignificantBleeding()
  {
    return (double) this.bleeding.value > 0.0;
  }

  protected override void RunMetabolism(BaseCombatEntity ownerEntity, float delta)
  {
    if (Interface.CallHook("OnRunPlayerMetabolism", (object) this, (object) ownerEntity, (object) delta) != null)
      return;
    float currentTemperature = this.owner.currentTemperature;
    float fTarget = this.owner.currentComfort;
    float currentCraftLevel = this.owner.currentCraftLevel;
    this.owner.SetPlayerFlag(BasePlayer.PlayerFlags.Workbench1, (double) currentCraftLevel == 1.0);
    this.owner.SetPlayerFlag(BasePlayer.PlayerFlags.Workbench2, (double) currentCraftLevel == 2.0);
    this.owner.SetPlayerFlag(BasePlayer.PlayerFlags.Workbench3, (double) currentCraftLevel == 3.0);
    this.owner.SetPlayerFlag(BasePlayer.PlayerFlags.SafeZone, this.owner.InSafeZone());
    float num1 = currentTemperature - this.DeltaWet() * 34f;
    float num2 = Mathf.Clamp(this.owner.baseProtection.amounts[18] * 1.5f, -1f, 1f);
    float num3 = Mathf.InverseLerp(20f, -50f, currentTemperature);
    float num4 = Mathf.InverseLerp(20f, 30f, currentTemperature);
    this.temperature.MoveTowards(num1 + num3 * 70f * num2 + num4 * 10f * Mathf.Abs(num2) + this.heartrate.value * 5f, delta * 5f);
    if ((double) this.temperature.value >= 40.0)
      fTarget = 0.0f;
    this.comfort.MoveTowards(fTarget, delta / 5f);
    float num5 = (float) (0.600000023841858 + 0.400000005960464 * (double) this.comfort.value);
    if (((double) this.calories.value <= 100.0 || (double) this.owner.healthFraction >= (double) num5 || ((double) this.radiation_poison.Fraction() >= 0.25 || (double) this.owner.SecondsSinceAttacked <= 10.0) || (this.SignificantBleeding() || (double) this.temperature.value < 10.0) ? 0 : ((double) this.hydration.value > 40.0 ? 1 : 0)) != 0)
    {
      float num6 = Mathf.InverseLerp(this.calories.min, this.calories.max, this.calories.value);
      float num7 = 5f;
      float num8 = (float) ((double) num7 * (double) this.owner.MaxHealth() * 0.800000011920929 / 600.0);
      float num9 = num8 + (float) ((double) num8 * (double) num6 * 0.5);
      float num10 = num9 / num7;
      float num11 = num10 + (float) ((double) num10 * (double) this.comfort.value * 6.0);
      ownerEntity.Heal(num11 * delta);
      this.calories.Subtract(num9 * delta);
      this.hydration.Subtract((float) ((double) num9 * (double) delta * 0.200000002980232));
    }
    this.heartrate.MoveTowards(Mathf.Clamp(0.05f + (float) ((double) this.owner.estimatedSpeed2D / (double) this.owner.GetMaxSpeed() * 0.75), 0.0f, 1f), delta * 0.1f);
    float num12 = this.heartrate.Fraction() * 0.375f;
    this.calories.MoveTowards(0.0f, delta * num12);
    float num13 = 0.008333334f + Mathf.InverseLerp(40f, 60f, this.temperature.value) * 0.08333334f + this.heartrate.value * 0.06666667f;
    this.hydration.MoveTowards(0.0f, delta * num13);
    this.owner.SetPlayerFlag(BasePlayer.PlayerFlags.NoSprint, (double) this.hydration.Fraction() <= 0.0 || (double) this.radiation_poison.value >= 100.0);
    if ((double) this.temperature.value > 40.0)
      this.hydration.Add((float) ((double) Mathf.InverseLerp(40f, 200f, this.temperature.value) * (double) delta * -1.0));
    if ((double) this.temperature.value < 10.0)
    {
      float num6 = Mathf.InverseLerp(20f, -100f, this.temperature.value);
      this.heartrate.MoveTowards(Mathf.Lerp(0.2f, 1f, num6), delta * 2f * num6);
    }
    float num14 = this.owner.WaterFactor();
    if ((double) num14 > 0.850000023841858)
      this.oxygen.MoveTowards(0.0f, delta * 0.1f);
    else
      this.oxygen.MoveTowards(1f, delta * 1f);
    float num15 = 0.0f;
    float num16 = 0.0f;
    if (this.owner.IsOutside(this.owner.eyes.position))
    {
      num15 = Climate.GetRain(this.owner.eyes.position) * 0.6f;
      num16 = Climate.GetSnow(this.owner.eyes.position) * 0.2f;
    }
    bool flag = (double) this.owner.baseProtection.amounts[4] > 0.0;
    if (!flag)
      this.wetness.value = Mathf.Max(this.wetness.value, num14);
    float num17 = Mathx.Max(this.wetness.value, num15, num16);
    this.wetness.MoveTowards(Mathf.Min(num17, flag ? 0.0f : num17), delta * 0.05f);
    if ((double) num14 < (double) this.wetness.value)
      this.wetness.MoveTowards(0.0f, delta * 0.2f * Mathf.InverseLerp(0.0f, 100f, currentTemperature));
    this.poison.MoveTowards(0.0f, delta * 0.5555556f);
    if ((double) this.wetness.Fraction() > 0.400000005960464 && (double) this.owner.estimatedSpeed > 0.25 && (double) this.radiation_level.Fraction() == 0.0)
      this.radiation_poison.Subtract((float) ((double) this.radiation_poison.value * 0.200000002980232 * (double) this.wetness.Fraction() * (double) delta * 0.200000002980232));
    if (ConVar.Server.radiation)
    {
      this.radiation_level.value = this.owner.radiationLevel;
      if ((double) this.radiation_level.value > 0.0)
        this.radiation_poison.Add(this.radiation_level.value * delta);
    }
    if ((double) this.pending_health.value <= 0.0)
      return;
    float num18 = Mathf.Min(1f * delta, this.pending_health.value);
    ownerEntity.Heal(num18);
    if ((double) ownerEntity.healthFraction == 1.0)
      this.pending_health.value = 0.0f;
    else
      this.pending_health.Subtract(num18);
  }

  private float DeltaHot()
  {
    return Mathf.InverseLerp(20f, 100f, this.temperature.value);
  }

  private float DeltaCold()
  {
    return Mathf.InverseLerp(20f, -50f, this.temperature.value);
  }

  private float DeltaWet()
  {
    return this.wetness.value;
  }

  public void UseHeart(float frate)
  {
    if ((double) this.heartrate.value > (double) frate)
      this.heartrate.Add(frate);
    else
      this.heartrate.value = frate;
  }

  public void SendChangesToClient()
  {
    if (!this.HasChanged())
      return;
    this.isDirty = false;
    using (PlayerMetabolism playerMetabolism = this.Save())
      this.baseEntity.ClientRPCPlayer<PlayerMetabolism>((Connection) null, this.baseEntity, "UpdateMetabolism", playerMetabolism);
  }

  public bool CanConsume()
  {
    if (Object.op_Implicit((Object) this.owner) && this.owner.IsHeadUnderwater())
      return false;
    return (double) Time.get_time() - (double) this.lastConsumeTime > 1.0;
  }

  public void MarkConsumption()
  {
    this.lastConsumeTime = Time.get_time();
  }

  public PlayerMetabolism Save()
  {
    PlayerMetabolism playerMetabolism = (PlayerMetabolism) Pool.Get<PlayerMetabolism>();
    playerMetabolism.calories = (__Null) (double) this.calories.value;
    playerMetabolism.hydration = (__Null) (double) this.hydration.value;
    playerMetabolism.heartrate = (__Null) (double) this.heartrate.value;
    playerMetabolism.temperature = (__Null) (double) this.temperature.value;
    playerMetabolism.radiation_level = (__Null) (double) this.radiation_level.value;
    playerMetabolism.radiation_poisoning = (__Null) (double) this.radiation_poison.value;
    playerMetabolism.wetness = (__Null) (double) this.wetness.value;
    playerMetabolism.dirtyness = (__Null) (double) this.dirtyness.value;
    playerMetabolism.oxygen = (__Null) (double) this.oxygen.value;
    playerMetabolism.bleeding = (__Null) (double) this.bleeding.value;
    playerMetabolism.comfort = (__Null) (double) this.comfort.value;
    playerMetabolism.pending_health = (__Null) (double) this.pending_health.value;
    if (Object.op_Implicit((Object) this.owner))
      playerMetabolism.health = (__Null) (double) this.owner.Health();
    return playerMetabolism;
  }

  public void Load(PlayerMetabolism s)
  {
    this.calories.SetValue((float) s.calories);
    this.hydration.SetValue((float) s.hydration);
    this.comfort.SetValue((float) s.comfort);
    this.heartrate.value = (float) s.heartrate;
    this.temperature.value = (float) s.temperature;
    this.radiation_level.value = (float) s.radiation_level;
    this.radiation_poison.value = (float) s.radiation_poisoning;
    this.wetness.value = (float) s.wetness;
    this.dirtyness.value = (float) s.dirtyness;
    this.oxygen.value = (float) s.oxygen;
    this.bleeding.value = (float) s.bleeding;
    this.pending_health.value = (float) s.pending_health;
    if (!Object.op_Implicit((Object) this.owner))
      return;
    this.owner.health = (float) s.health;
  }

  public override MetabolismAttribute FindAttribute(MetabolismAttribute.Type type)
  {
    switch (type)
    {
      case MetabolismAttribute.Type.Poison:
        return this.poison;
      case MetabolismAttribute.Type.Radiation:
        return this.radiation_poison;
      case MetabolismAttribute.Type.Bleeding:
        return this.bleeding;
      case MetabolismAttribute.Type.HealthOverTime:
        return this.pending_health;
      default:
        return base.FindAttribute(type);
    }
  }
}
