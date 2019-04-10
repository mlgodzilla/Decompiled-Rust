// Decompiled with JetBrains decompiler
// Type: GeneratePowerlineTopology
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class GeneratePowerlineTopology : ProceduralComponent
{
  public override void Process(uint seed)
  {
    foreach (PathList powerline in TerrainMeta.Path.Powerlines)
      powerline.Path.RecalculateTangents();
  }
}
