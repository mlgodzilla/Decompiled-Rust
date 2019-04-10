// Decompiled with JetBrains decompiler
// Type: ModelConditionTest
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public abstract class ModelConditionTest : PrefabAttribute
{
  public abstract bool DoTest(BaseEntity ent);

  protected override System.Type GetIndexedType()
  {
    return typeof (ModelConditionTest);
  }
}
