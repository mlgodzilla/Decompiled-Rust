// Decompiled with JetBrains decompiler
// Type: FoliageGrid
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine.Rendering;

public class FoliageGrid : SingletonComponent<FoliageGrid>, IClientComponent
{
  public static bool Paused;
  public GameObjectRef BatchPrefab;
  public float CellSize;
  public float MaxMilliseconds;
  public LayerSelect FoliageLayer;
  public ShadowCastingMode FoliageShadows;
  public const float MaxRefreshDistance = 100f;

  public FoliageGrid()
  {
    base.\u002Ector();
  }
}
