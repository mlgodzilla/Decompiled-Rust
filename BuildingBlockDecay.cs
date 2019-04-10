// Decompiled with JetBrains decompiler
// Type: BuildingBlockDecay
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class BuildingBlockDecay : Decay
{
  private bool isFoundation;

  public override float GetDecayDelay(BaseEntity entity)
  {
    BuildingBlock buildingBlock = entity as BuildingBlock;
    return this.GetDecayDelay(Object.op_Implicit((Object) buildingBlock) ? buildingBlock.grade : BuildingGrade.Enum.Twigs);
  }

  public override float GetDecayDuration(BaseEntity entity)
  {
    BuildingBlock buildingBlock = entity as BuildingBlock;
    return this.GetDecayDuration(Object.op_Implicit((Object) buildingBlock) ? buildingBlock.grade : BuildingGrade.Enum.Twigs);
  }

  public override bool ShouldDecay(BaseEntity entity)
  {
    if (ConVar.Decay.upkeep || this.isFoundation)
      return true;
    BuildingBlock buildingBlock = entity as BuildingBlock;
    return (Object.op_Implicit((Object) buildingBlock) ? (int) buildingBlock.grade : 0) == 0;
  }

  protected override void AttributeSetup(
    GameObject rootObj,
    string name,
    bool serverside,
    bool clientside,
    bool bundling)
  {
    this.isFoundation = name.Contains("foundation");
  }
}
