// Decompiled with JetBrains decompiler
// Type: RendererGrid
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class RendererGrid : SingletonComponent<RendererGrid>, IClientComponent
{
  public static bool Paused;
  public GameObjectRef BatchPrefab;
  public float CellSize;
  public float MaxMilliseconds;

  public RendererGrid()
  {
    base.\u002Ector();
  }
}
