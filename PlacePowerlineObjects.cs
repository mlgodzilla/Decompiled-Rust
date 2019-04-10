// Decompiled with JetBrains decompiler
// Type: PlacePowerlineObjects
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine.Serialization;

public class PlacePowerlineObjects : ProceduralComponent
{
  public PathList.BasicObject[] Start;
  public PathList.BasicObject[] End;
  public PathList.SideObject[] Side;
  [FormerlySerializedAs("PowerlineObjects")]
  public PathList.PathObject[] Path;

  public override void Process(uint seed)
  {
    foreach (PathList powerline in TerrainMeta.Path.Powerlines)
    {
      foreach (PathList.BasicObject basicObject in this.Start)
        powerline.TrimStart(basicObject);
      foreach (PathList.BasicObject basicObject in this.End)
        powerline.TrimEnd(basicObject);
      foreach (PathList.BasicObject basicObject in this.Start)
        powerline.SpawnStart(ref seed, basicObject);
      foreach (PathList.BasicObject basicObject in this.End)
        powerline.SpawnEnd(ref seed, basicObject);
      foreach (PathList.PathObject pathObject in this.Path)
        powerline.SpawnAlong(ref seed, pathObject);
      foreach (PathList.SideObject sideObject in this.Side)
        powerline.SpawnSide(ref seed, sideObject);
      powerline.ResetTrims();
    }
  }
}
