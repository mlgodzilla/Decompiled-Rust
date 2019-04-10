// Decompiled with JetBrains decompiler
// Type: Upkeep
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class Upkeep : PrefabAttribute
{
  public float upkeepMultiplier = 1f;

  protected override System.Type GetIndexedType()
  {
    return typeof (Upkeep);
  }
}
