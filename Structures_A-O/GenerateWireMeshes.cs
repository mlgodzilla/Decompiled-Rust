// Decompiled with JetBrains decompiler
// Type: GenerateWireMeshes
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class GenerateWireMeshes : ProceduralComponent
{
  public override void Process(uint seed)
  {
    TerrainMeta.Path.CreateWires();
  }

  public override bool RunOnCache
  {
    get
    {
      return true;
    }
  }
}
