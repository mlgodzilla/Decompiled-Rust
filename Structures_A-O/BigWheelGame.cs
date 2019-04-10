// Decompiled with JetBrains decompiler
// Type: BigWheelGame
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BigWheelGame : SpinnerWheel
{
  [ServerVar]
  public static float spinFrequencySeconds = 45f;
  protected int lastPaidSpinNumber = -1;
  protected List<BigWheelBettingTerminal> terminals = new List<BigWheelBettingTerminal>();
  public HitNumber[] hitNumbers;
  public GameObject indicator;
  public GameObjectRef winEffect;
  protected int spinNumber;

  public override bool AllowPlayerSpins()
  {
    return false;
  }

  public override bool CanUpdateSign(BasePlayer player)
  {
    return false;
  }

  public override float GetMaxSpinSpeed()
  {
    return 180f;
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.Invoke(new Action(this.InitBettingTerminals), 3f);
    this.Invoke(new Action(this.DoSpin), 10f);
  }

  public void DoSpin()
  {
    if ((double) this.velocity > 0.0)
      return;
    this.velocity += Random.Range(7f, 10f);
    ++this.spinNumber;
    this.SetTerminalsLocked(true);
  }

  public void SetTerminalsLocked(bool isLocked)
  {
    foreach (StorageContainer terminal in this.terminals)
      terminal.inventory.SetLocked(isLocked);
  }

  protected void InitBettingTerminals()
  {
    this.terminals.Clear();
    Vis.Entities<BigWheelBettingTerminal>(((Component) this).get_transform().get_position(), 30f, this.terminals, 256, (QueryTriggerInteraction) 2);
    foreach (BigWheelBettingTerminal terminal in this.terminals)
      ;
  }

  public override void Update_Server()
  {
    double velocity1 = (double) this.velocity;
    base.Update_Server();
    float velocity2 = this.velocity;
    if (velocity1 <= 0.0 || (double) velocity2 != 0.0 || this.spinNumber <= this.lastPaidSpinNumber)
      return;
    this.Payout();
    this.lastPaidSpinNumber = this.spinNumber;
    this.QueueSpin();
  }

  public float SpinSpacing()
  {
    return BigWheelGame.spinFrequencySeconds;
  }

  public void QueueSpin()
  {
    foreach (BaseEntity terminal in this.terminals)
      terminal.ClientRPC<float>((Connection) null, "SetTimeUntilNextSpin", this.SpinSpacing());
    this.Invoke(new Action(this.DoSpin), this.SpinSpacing());
  }

  public void Payout()
  {
    HitNumber currentHitType = this.GetCurrentHitType();
    foreach (BigWheelBettingTerminal terminal in this.terminals)
    {
      if (!terminal.isClient)
      {
        bool flag1 = false;
        bool flag2 = false;
        Item slot1 = terminal.inventory.GetSlot((int) currentHitType.hitType);
        if (slot1 != null)
        {
          int multiplier = currentHitType.ColorToMultiplier(currentHitType.hitType);
          slot1.amount += slot1.amount * multiplier;
          slot1.RemoveFromContainer();
          slot1.MoveToContainer(terminal.inventory, 5, true);
          flag1 = true;
        }
        for (int slot2 = 0; slot2 < 5; ++slot2)
        {
          Item slot3 = terminal.inventory.GetSlot(slot2);
          if (slot3 != null)
          {
            slot3.Remove(0.0f);
            flag2 = true;
          }
        }
        if (flag1 | flag2)
          terminal.ClientRPC<bool>((Connection) null, "WinOrLoseSound", flag1);
      }
    }
    ItemManager.DoRemoves();
    this.SetTerminalsLocked(false);
  }

  public HitNumber GetCurrentHitType()
  {
    HitNumber hitNumber1 = (HitNumber) null;
    float num1 = float.PositiveInfinity;
    foreach (HitNumber hitNumber2 in this.hitNumbers)
    {
      float num2 = Vector3.Distance(this.indicator.get_transform().get_position(), ((Component) hitNumber2).get_transform().get_position());
      if ((double) num2 < (double) num1)
      {
        hitNumber1 = hitNumber2;
        num1 = num2;
      }
    }
    return hitNumber1;
  }

  [ContextMenu("LoadHitNumbers")]
  private void LoadHitNumbers()
  {
    this.hitNumbers = (HitNumber[]) ((Component) this).GetComponentsInChildren<HitNumber>();
  }
}
