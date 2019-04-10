// Decompiled with JetBrains decompiler
// Type: TerrainExtension
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch.Extend;
using System;
using UnityEngine;

[RequireComponent(typeof (TerrainMeta))]
public abstract class TerrainExtension : MonoBehaviour
{
  [NonSerialized]
  public bool isInitialized;
  internal Terrain terrain;
  internal TerrainConfig config;

  public void Init(Terrain terrain, TerrainConfig config)
  {
    this.terrain = terrain;
    this.config = config;
  }

  public virtual void Setup()
  {
  }

  public virtual void PostSetup()
  {
  }

  public void LogSize(object obj, ulong size)
  {
    Debug.Log((object) (obj.GetType().ToString() + " allocated: " + NumberExtensions.FormatBytes<ulong>((M0) (long) size, false)));
  }

  protected TerrainExtension()
  {
    base.\u002Ector();
  }
}
