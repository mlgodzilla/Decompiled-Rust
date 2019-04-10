// Decompiled with JetBrains decompiler
// Type: PlaceRiverObjects
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine.Serialization;

public class PlaceRiverObjects : ProceduralComponent
{
  public PathList.BasicObject[] Start;
  public PathList.BasicObject[] End;
  [FormerlySerializedAs("RiversideObjects")]
  public PathList.SideObject[] Side;
  [FormerlySerializedAs("RiverObjects")]
  public PathList.PathObject[] Path;

  public override void Process(uint seed)
  {
    foreach (PathList river in TerrainMeta.Path.Rivers)
    {
      foreach (PathList.BasicObject basicObject in this.Start)
        river.TrimStart(basicObject);
      foreach (PathList.BasicObject basicObject in this.End)
        river.TrimEnd(basicObject);
      foreach (PathList.BasicObject basicObject in this.Start)
        river.SpawnStart(ref seed, basicObject);
      foreach (PathList.PathObject pathObject in this.Path)
        river.SpawnAlong(ref seed, pathObject);
      foreach (PathList.SideObject sideObject in this.Side)
        river.SpawnSide(ref seed, sideObject);
      foreach (PathList.BasicObject basicObject in this.End)
        river.SpawnEnd(ref seed, basicObject);
      river.ResetTrims();
    }
  }
}
