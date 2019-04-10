// Decompiled with JetBrains decompiler
// Type: LODComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public abstract class LODComponent : BaseMonoBehaviour, IClientComponent, ILOD
{
  public LODComponent.OccludeeParameters OccludeeParams = new LODComponent.OccludeeParameters()
  {
    isDynamic = false,
    dynamicUpdateInterval = 0.2f,
    shadowRangeScale = 3f,
    showBounds = false
  };
  public LODDistanceMode DistanceMode;

  [Serializable]
  public struct OccludeeParameters
  {
    [Tooltip("Is Occludee dynamic or static?")]
    public bool isDynamic;
    [Tooltip("Dynamic occludee update interval in seconds; 0 = every frame")]
    public float dynamicUpdateInterval;
    [Tooltip("Distance scale combined with occludee max bounds size at which culled occludee shadows are still visible")]
    public float shadowRangeScale;
    [Tooltip("Show culling bounds via gizmos; editor only")]
    public bool showBounds;
  }
}
