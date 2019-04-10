// Decompiled with JetBrains decompiler
// Type: RainSurfaceAmbience
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class RainSurfaceAmbience : MonoBehaviour
{
  public float tickRate;
  public float gridSize;
  public float gridSamples;
  public float startHeight;
  public float rayLength;
  public LayerMask layerMask;
  public float spreadScale;
  public float maxDistance;
  public float lerpSpeed;
  public List<RainSurfaceAmbience.SurfaceSound> surfaces;

  public RainSurfaceAmbience()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class SurfaceSound
  {
    public List<PhysicMaterial> materials = new List<PhysicMaterial>();
    [HideInInspector]
    public Vector3 position = Vector3.get_zero();
    [HideInInspector]
    public List<Vector3> points = new List<Vector3>();
    public SoundDefinition soundDef;
    [HideInInspector]
    public Sound sound;
    [HideInInspector]
    public float amount;
    [HideInInspector]
    public Bounds bounds;
    [HideInInspector]
    public SoundModulation.Modulator gainMod;
    [HideInInspector]
    public SoundModulation.Modulator spreadMod;
  }
}
