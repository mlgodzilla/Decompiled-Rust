// Decompiled with JetBrains decompiler
// Type: ScientistNPC
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using System;
using UnityEngine;

public class ScientistNPC : HumanNPC
{
  public Vector2 IdleChatterRepeatRange = new Vector2(10f, 15f);
  protected float lastAlertedTime = -100f;
  public GameObjectRef[] RadioChatterEffects;
  public GameObjectRef[] DeathEffects;
  public ScientistNPC.RadioChatterType radioChatterType;

  public void SetChatterType(ScientistNPC.RadioChatterType newType)
  {
    if (newType == this.radioChatterType)
      return;
    if (newType == ScientistNPC.RadioChatterType.Idle)
      this.QueueRadioChatter();
    else
      this.CancelInvoke(new Action(this.PlayRadioChatter));
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.SetChatterType(ScientistNPC.RadioChatterType.Idle);
    this.InvokeRandomized(new Action(this.IdleCheck), 0.0f, 20f, 1f);
  }

  public void IdleCheck()
  {
    if ((double) Time.get_time() <= (double) this.lastAlertedTime + 20.0)
      return;
    this.SetChatterType(ScientistNPC.RadioChatterType.Idle);
  }

  public void QueueRadioChatter()
  {
    if (!this.IsAlive() || this.IsDestroyed)
      return;
    this.Invoke(new Action(this.PlayRadioChatter), Random.Range((float) this.IdleChatterRepeatRange.x, (float) this.IdleChatterRepeatRange.y));
  }

  public override bool ShotTest()
  {
    int num = base.ShotTest() ? 1 : 0;
    if ((double) Time.get_time() - (double) this.lastGunShotTime >= 5.0)
      return num != 0;
    this.Alert();
    return num != 0;
  }

  public void Alert()
  {
    this.lastAlertedTime = Time.get_time();
    this.SetChatterType(ScientistNPC.RadioChatterType.Alert);
  }

  public override void OnAttacked(HitInfo info)
  {
    base.OnAttacked(info);
    this.Alert();
  }

  public override void OnKilled(HitInfo info)
  {
    base.OnKilled(info);
    this.SetChatterType(ScientistNPC.RadioChatterType.NONE);
    if (this.DeathEffects.Length == 0)
      return;
    Effect.server.Run(this.DeathEffects[Random.Range(0, this.DeathEffects.Length)].resourcePath, this.ServerPosition, Vector3.get_up(), (Connection) null, false);
  }

  public void PlayRadioChatter()
  {
    if (this.RadioChatterEffects.Length == 0)
      return;
    if (this.IsDestroyed || Object.op_Equality((Object) ((Component) this).get_transform(), (Object) null))
    {
      this.CancelInvoke(new Action(this.PlayRadioChatter));
    }
    else
    {
      Effect.server.Run(this.RadioChatterEffects[Random.Range(0, this.RadioChatterEffects.Length)].resourcePath, (BaseEntity) this, StringPool.Get("head"), Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
      this.QueueRadioChatter();
    }
  }

  public override void EquipWeapon()
  {
    base.EquipWeapon();
    HeldEntity heldEntity = this.GetHeldEntity();
    if (!Object.op_Inequality((Object) heldEntity, (Object) null))
      return;
    Item obj = heldEntity.GetItem();
    if (obj == null || obj.contents == null)
      return;
    if (Random.Range(0, 3) == 0)
    {
      Item byName = ItemManager.CreateByName("weapon.mod.flashlight", 1, 0UL);
      if (!byName.MoveToContainer(obj.contents, -1, true))
      {
        byName.Remove(0.0f);
      }
      else
      {
        this.lightsOn = false;
        this.InvokeRandomized(new Action(((HumanNPC) this).LightCheck), 0.0f, 30f, 5f);
        this.LightCheck();
      }
    }
    else
    {
      Item byName = ItemManager.CreateByName("weapon.mod.lasersight", 1, 0UL);
      if (!byName.MoveToContainer(obj.contents, -1, true))
        byName.Remove(0.0f);
      this.SetLightsOn(true);
      this.lightsOn = true;
    }
  }

  public enum RadioChatterType
  {
    NONE,
    Idle,
    Alert,
  }
}
