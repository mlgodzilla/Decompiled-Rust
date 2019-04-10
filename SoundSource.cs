// Decompiled with JetBrains decompiler
// Type: SoundSource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundSource : MonoBehaviour, IClientComponentEx, ILOD
{
  [Header("Occlusion")]
  public bool handleOcclusionChecks;
  public LayerMask occlusionLayerMask;
  public List<SoundSource.OcclusionPoint> occlusionPoints;
  public bool isOccluded;
  public float occlusionAmount;
  public float lodDistance;
  public bool inRange;

  public virtual void PreClientComponentCull(IPrefabProcessor p)
  {
    p.RemoveComponent((Component) this);
  }

  public SoundSource()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class OcclusionPoint
  {
    public Vector3 offset = Vector3.get_zero();
    public bool isOccluded;
  }
}
