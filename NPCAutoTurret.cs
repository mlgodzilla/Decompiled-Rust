// Decompiled with JetBrains decompiler
// Type: NPCAutoTurret
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class NPCAutoTurret : AutoTurret
{
  [ServerVar(Help = "How many seconds until a sleeping player is considered hostile")]
  public static float sleeperhostiledelay = 1200f;
  public Transform centerMuzzle;
  public Transform muzzleLeft;
  public Transform muzzleRight;
  private bool useLeftMuzzle;

  public override void ServerInit()
  {
    base.ServerInit();
    this.SetOnline();
    this.SetPeacekeepermode(true);
  }

  public override bool HasAmmo()
  {
    return true;
  }

  public override bool CheckPeekers()
  {
    return false;
  }

  public override float TargetScanRate()
  {
    return 1.25f;
  }

  public override bool InFiringArc(BaseCombatEntity potentialtarget)
  {
    return true;
  }

  public override Transform GetCenterMuzzle()
  {
    return this.centerMuzzle;
  }

  public override void FireGun(
    Vector3 targetPos,
    float aimCone,
    Transform muzzleToUse = null,
    BaseCombatEntity target = null)
  {
    muzzleToUse = this.muzzleRight;
    base.FireGun(targetPos, aimCone, muzzleToUse, target);
  }

  protected override bool Ignore(BasePlayer player)
  {
    return Object.op_Implicit((Object) (player as Scientist));
  }

  public override bool IsEntityHostile(BaseCombatEntity ent)
  {
    BasePlayer basePlayer = ent as BasePlayer;
    if (Object.op_Inequality((Object) basePlayer, (Object) null))
    {
      if (basePlayer.IsNpc)
        return !(basePlayer is Scientist);
      if (basePlayer.IsSleeping() && (double) basePlayer.secondsSleeping >= (double) NPCAutoTurret.sleeperhostiledelay)
        return true;
    }
    return base.IsEntityHostile(ent);
  }
}
