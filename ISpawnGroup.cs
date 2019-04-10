// Decompiled with JetBrains decompiler
// Type: ISpawnGroup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public interface ISpawnGroup
{
  void Clear();

  void Fill();

  void SpawnInitial();

  void SpawnRepeating();

  int currentPopulation { get; }
}
