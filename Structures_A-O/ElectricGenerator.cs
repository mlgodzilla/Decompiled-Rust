// Decompiled with JetBrains decompiler
// Type: ElectricGenerator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using ProtoBuf;
using System;
using UnityEngine;

public class ElectricGenerator : IOEntity
{
  public float electricAmount = 8f;

  public override bool IsRootEntity()
  {
    return true;
  }

  public override int GetCurrentEnergy()
  {
    return (int) this.electricAmount;
  }

  public override int GetPassthroughAmount(int outputSlot = 0)
  {
    return this.GetCurrentEnergy();
  }

  public override void UpdateOutputs()
  {
    this.currentEnergy = this.GetCurrentEnergy();
    foreach (IOEntity.IOSlot output in this.outputs)
    {
      if (Object.op_Inequality((Object) output.connectedTo.Get(true), (Object) null))
        output.connectedTo.Get(true).UpdateFromInput(this.currentEnergy, output.connectedToSlot);
    }
  }

  public override void IOStateChanged(int inputAmount, int inputSlot)
  {
    base.IOStateChanged(inputAmount, inputSlot);
  }

  public override void PostServerLoad()
  {
    base.PostServerLoad();
    this.Invoke(new Action(this.ForcePuzzleReset), 1f);
  }

  private void ForcePuzzleReset()
  {
    PuzzleReset component = (PuzzleReset) ((Component) this).GetComponent<PuzzleReset>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.DoReset();
    component.ResetTimer();
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    if (!info.forDisk)
      return;
    PuzzleReset component = (PuzzleReset) ((Component) this).GetComponent<PuzzleReset>();
    if (!Object.op_Implicit((Object) component))
      return;
    info.msg.puzzleReset = (__Null) Pool.Get<PuzzleReset>();
    ((PuzzleReset) info.msg.puzzleReset).playerBlocksReset = (__Null) (component.playersBlockReset ? 1 : 0);
    if (Object.op_Inequality((Object) component.playerDetectionOrigin, (Object) null))
      ((PuzzleReset) info.msg.puzzleReset).playerDetectionOrigin = (__Null) component.playerDetectionOrigin.get_position();
    ((PuzzleReset) info.msg.puzzleReset).playerDetectionRadius = (__Null) (double) component.playerDetectionRadius;
    ((PuzzleReset) info.msg.puzzleReset).scaleWithServerPopulation = (__Null) (component.scaleWithServerPopulation ? 1 : 0);
    ((PuzzleReset) info.msg.puzzleReset).timeBetweenResets = (__Null) (double) component.timeBetweenResets;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (!info.fromDisk || info.msg.puzzleReset == null)
      return;
    PuzzleReset component = (PuzzleReset) ((Component) this).GetComponent<PuzzleReset>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.playersBlockReset = (bool) ((PuzzleReset) info.msg.puzzleReset).playerBlocksReset;
    if (Object.op_Inequality((Object) component.playerDetectionOrigin, (Object) null))
      component.playerDetectionOrigin.set_position((Vector3) ((PuzzleReset) info.msg.puzzleReset).playerDetectionOrigin);
    component.playerDetectionRadius = (float) ((PuzzleReset) info.msg.puzzleReset).playerDetectionRadius;
    component.scaleWithServerPopulation = (bool) ((PuzzleReset) info.msg.puzzleReset).scaleWithServerPopulation;
    component.timeBetweenResets = (float) ((PuzzleReset) info.msg.puzzleReset).timeBetweenResets;
    component.ResetTimer();
  }
}
