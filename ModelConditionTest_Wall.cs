// Decompiled with JetBrains decompiler
// Type: ModelConditionTest_Wall
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class ModelConditionTest_Wall : ModelConditionTest
{
  public override bool DoTest(BaseEntity ent)
  {
    if (!ModelConditionTest_WallTriangleLeft.CheckCondition(ent))
      return !ModelConditionTest_WallTriangleRight.CheckCondition(ent);
    return false;
  }
}
