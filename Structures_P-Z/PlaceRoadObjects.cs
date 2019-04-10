// Decompiled with JetBrains decompiler
// Type: PlaceRoadObjects
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine.Serialization;

public class PlaceRoadObjects : ProceduralComponent
{
  public PathList.BasicObject[] Start;
  public PathList.BasicObject[] End;
  [FormerlySerializedAs("RoadsideObjects")]
  public PathList.SideObject[] Side;
  [FormerlySerializedAs("RoadObjects")]
  public PathList.PathObject[] Path;

  public override void Process(uint seed)
  {
    foreach (PathList road in TerrainMeta.Path.Roads)
    {
      foreach (PathList.BasicObject basicObject in this.Start)
        road.TrimStart(basicObject);
      foreach (PathList.BasicObject basicObject in this.End)
        road.TrimEnd(basicObject);
      foreach (PathList.BasicObject basicObject in this.Start)
        road.SpawnStart(ref seed, basicObject);
      foreach (PathList.BasicObject basicObject in this.End)
        road.SpawnEnd(ref seed, basicObject);
      foreach (PathList.PathObject pathObject in this.Path)
        road.SpawnAlong(ref seed, pathObject);
      foreach (PathList.SideObject sideObject in this.Side)
        road.SpawnSide(ref seed, sideObject);
      road.ResetTrims();
    }
  }
}
