// Decompiled with JetBrains decompiler
// Type: AttackEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class AttackEntity : HeldEntity
{
  [Header("Attack Entity")]
  public float deployDelay = 1f;
  public float repeatDelay = 0.5f;
  [Header("NPCUsage")]
  public float effectiveRange = 1f;
  public float npcDamageScale = 1f;
  public float attackLengthMin = -1f;
  public float attackLengthMax = -1f;
  public NPCPlayerApex.WeaponTypeEnum effectiveRangeType = NPCPlayerApex.WeaponTypeEnum.MediumRange;
  public bool CanUseAtMediumRange = true;
  public bool CanUseAtLongRange = true;
  private float nextAttackTime = float.NegativeInfinity;
  public float animationDelay;
  public float attackSpacing;
  public float aiAimSwayOffset;
  public float aiAimCone;
  public bool aiOnlyInRange;
  public float CloseRangeAddition;
  public float MediumRangeAddition;
  public float LongRangeAddition;
  public SoundDefinition[] reloadSounds;
  public SoundDefinition thirdPersonMeleeSound;

  public virtual float AmmoFraction()
  {
    return 0.0f;
  }

  public virtual bool CanReload()
  {
    return false;
  }

  public virtual bool ServerIsReloading()
  {
    return false;
  }

  public virtual void ServerReload()
  {
  }

  public virtual void TopUpAmmo()
  {
  }

  public virtual void ServerUse()
  {
  }

  public virtual void ServerUse(float damageModifier)
  {
    this.ServerUse();
  }

  public virtual Vector3 ModifyAIAim(Vector3 eulerInput, float swayModifier = 1f)
  {
    return eulerInput;
  }

  public float NextAttackTime
  {
    get
    {
      return this.nextAttackTime;
    }
  }

  public virtual void GetAttackStats(HitInfo info)
  {
  }

  protected void StartAttackCooldown(float cooldown)
  {
    this.nextAttackTime = this.CalculateCooldownTime(this.nextAttackTime, cooldown, true);
  }

  protected void ResetAttackCooldown()
  {
    this.nextAttackTime = float.NegativeInfinity;
  }

  public bool HasAttackCooldown()
  {
    return (double) Time.get_time() < (double) this.nextAttackTime;
  }

  protected float GetAttackCooldown()
  {
    return Mathf.Max(this.nextAttackTime - Time.get_time(), 0.0f);
  }

  protected float GetAttackIdle()
  {
    return Mathf.Max(Time.get_time() - this.nextAttackTime, 0.0f);
  }

  protected float CalculateCooldownTime(float nextTime, float cooldown, bool catchup)
  {
    float time = Time.get_time();
    float num = 0.0f;
    if (this.isServer)
    {
      BasePlayer ownerPlayer = this.GetOwnerPlayer();
      num = num + 0.1f + cooldown * 0.1f + (Object.op_Implicit((Object) ownerPlayer) ? ownerPlayer.desyncTime : 0.1f) + Mathf.Max(Time.get_deltaTime(), Time.get_smoothDeltaTime());
    }
    nextTime = (double) nextTime >= 0.0 ? ((double) time - (double) nextTime > (double) num ? Mathf.Max(nextTime + cooldown, time + cooldown - num) : Mathf.Min(nextTime + cooldown, time + cooldown)) : Mathf.Max(0.0f, time + cooldown - num);
    return nextTime;
  }

  protected bool VerifyClientRPC(BasePlayer player)
  {
    if (Object.op_Equality((Object) player, (Object) null))
    {
      Debug.LogWarning((object) "Received RPC from null player");
      return false;
    }
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (Object.op_Equality((Object) ownerPlayer, (Object) null))
    {
      AntiHack.Log(player, AntiHackType.AttackHack, "Owner not found (" + this.ShortPrefabName + ")");
      player.stats.combat.Log(this, "owner_missing");
      return false;
    }
    if (Object.op_Inequality((Object) ownerPlayer, (Object) player))
    {
      AntiHack.Log(player, AntiHackType.AttackHack, "Player mismatch (" + this.ShortPrefabName + ")");
      player.stats.combat.Log(this, "player_mismatch");
      return false;
    }
    if (player.IsDead())
    {
      AntiHack.Log(player, AntiHackType.AttackHack, "Player dead (" + this.ShortPrefabName + ")");
      player.stats.combat.Log(this, "player_dead");
      return false;
    }
    if (player.IsWounded())
    {
      AntiHack.Log(player, AntiHackType.AttackHack, "Player down (" + this.ShortPrefabName + ")");
      player.stats.combat.Log(this, "player_down");
      return false;
    }
    if (player.IsSleeping())
    {
      AntiHack.Log(player, AntiHackType.AttackHack, "Player sleeping (" + this.ShortPrefabName + ")");
      player.stats.combat.Log(this, "player_sleeping");
      return false;
    }
    if ((double) player.desyncTime > (double) ConVar.AntiHack.maxdesync)
    {
      AntiHack.Log(player, AntiHackType.AttackHack, "Player stalled (" + this.ShortPrefabName + " with " + (object) player.desyncTime + "s)");
      player.stats.combat.Log(this, "player_stalled");
      return false;
    }
    Item ownerItem = this.GetOwnerItem();
    if (ownerItem == null)
    {
      AntiHack.Log(player, AntiHackType.AttackHack, "Item not found (" + this.ShortPrefabName + ")");
      player.stats.combat.Log(this, "item_missing");
      return false;
    }
    if (!ownerItem.isBroken)
      return true;
    AntiHack.Log(player, AntiHackType.AttackHack, "Item broken (" + this.ShortPrefabName + ")");
    player.stats.combat.Log(this, "item_broken");
    return false;
  }

  protected virtual bool VerifyClientAttack(BasePlayer player)
  {
    if (!this.VerifyClientRPC(player))
      return false;
    if (!this.HasAttackCooldown())
      return true;
    AntiHack.Log(player, AntiHackType.CooldownHack, "T-" + (object) this.GetAttackCooldown() + "s (" + this.ShortPrefabName + ")");
    player.stats.combat.Log(this, "attack_cooldown");
    return false;
  }

  protected bool ValidateEyePos(BasePlayer player, Vector3 eyePos)
  {
    bool flag = true;
    if (Vector3Ex.IsNaNOrInfinity(eyePos))
    {
      string shortPrefabName = this.ShortPrefabName;
      AntiHack.Log(player, AntiHackType.EyeHack, "Contains NaN (" + shortPrefabName + ")");
      player.stats.combat.Log(this, "eye_nan");
      flag = false;
    }
    if (ConVar.AntiHack.eye_protection > 0)
    {
      float num1 = 1f + ConVar.AntiHack.eye_forgiveness;
      double eyeClientframes = (double) ConVar.AntiHack.eye_clientframes;
      float eyeServerframes = ConVar.AntiHack.eye_serverframes;
      float num2 = (float) (eyeClientframes / 60.0);
      float num3 = eyeServerframes * Mathx.Max(Time.get_deltaTime(), Time.get_smoothDeltaTime(), Time.get_fixedDeltaTime());
      float num4 = (player.desyncTime + num2 + num3) * num1;
      if (ConVar.AntiHack.eye_protection >= 1)
      {
        double num5 = (double) player.MaxVelocity();
        Vector3 parentVelocity = player.GetParentVelocity();
        double magnitude = (double) ((Vector3) ref parentVelocity).get_magnitude();
        float num6 = (float) (num5 + magnitude);
        float num7 = player.BoundsPadding() + num4 * num6;
        float num8 = Vector3.Distance(player.eyes.position, eyePos);
        if ((double) num8 > (double) num7)
        {
          string shortPrefabName = this.ShortPrefabName;
          AntiHack.Log(player, AntiHackType.EyeHack, "Distance (" + shortPrefabName + " on attack with " + (object) num8 + "m > " + (object) num7 + "m)");
          player.stats.combat.Log(this, "eye_distance");
          flag = false;
        }
      }
      if (ConVar.AntiHack.eye_protection >= 2)
      {
        Vector3 center = player.eyes.center;
        Vector3 position = player.eyes.position;
        Vector3 p2 = eyePos;
        if (!GamePhysics.LineOfSight(center, position, p2, 2162688, 0.0f))
        {
          string shortPrefabName = this.ShortPrefabName;
          AntiHack.Log(player, AntiHackType.EyeHack, "Line of sight (" + shortPrefabName + " on attack) " + (object) center + " " + (object) position + " " + (object) p2);
          player.stats.combat.Log(this, "eye_los");
          flag = false;
        }
      }
      if (!flag)
        AntiHack.AddViolation(player, AntiHackType.EyeHack, ConVar.AntiHack.eye_penalty);
    }
    return flag;
  }

  public override void OnHeldChanged()
  {
    base.OnHeldChanged();
    this.StartAttackCooldown(this.deployDelay * 0.9f);
  }
}
