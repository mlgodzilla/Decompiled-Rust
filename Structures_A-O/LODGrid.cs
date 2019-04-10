// Decompiled with JetBrains decompiler
// Type: LODGrid
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class LODGrid : SingletonComponent<LODGrid>, IClientComponent
{
  public static bool Paused = false;
  public static float TreeMeshDistance = 500f;
  public float CellSize;
  public float MaxMilliseconds;
  public const float MaxRefreshDistance = 500f;

  public LODGrid()
  {
    base.\u002Ector();
  }
}
