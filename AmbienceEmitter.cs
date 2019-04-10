// Decompiled with JetBrains decompiler
// Type: AmbienceEmitter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceEmitter : MonoBehaviour, IClientComponent, IComparable<AmbienceEmitter>
{
  public AmbienceDefinitionList baseAmbience;
  public AmbienceDefinitionList stings;
  public bool isStatic;
  public bool followCamera;
  public bool isBaseEmitter;
  public bool active;
  public float cameraDistance;
  public BoundingSphere boundingSphere;
  public float crossfadeTime;
  public Dictionary<AmbienceDefinition, float> nextStingTime;
  public float deactivateTime;

  public TerrainTopology.Enum currentTopology { get; private set; }

  public TerrainBiome.Enum currentBiome { get; private set; }

  public int CompareTo(AmbienceEmitter other)
  {
    return this.cameraDistance.CompareTo(other.cameraDistance);
  }

  public AmbienceEmitter()
  {
    base.\u002Ector();
  }
}
