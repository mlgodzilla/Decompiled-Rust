// Decompiled with JetBrains decompiler
// Type: SkinnedMultiMesh
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedMultiMesh : MonoBehaviour
{
  public bool shadowOnly;
  public List<SkinnedMultiMesh.Part> parts;
  public BoneDictionary boneDict;
  [NonSerialized]
  public List<SkinnedMultiMesh.Part> createdParts;
  [NonSerialized]
  public long lastBuildHash;
  [NonSerialized]
  public MaterialPropertyBlock sharedPropertyBlock;
  [NonSerialized]
  public MaterialPropertyBlock hairPropertyBlock;
  public float skinNumber;
  public float meshNumber;
  public float hairNumber;
  public int skinType;
  public SkinSetCollection SkinCollection;
  private ArticulatedOccludee articulatedOccludee;
  private LODGroup lodGroup;
  private List<Renderer> renderers;
  private List<Animator> animators;

  public List<Renderer> Renderers
  {
    get
    {
      return this.renderers;
    }
  }

  public List<Animator> Animators
  {
    get
    {
      return this.animators;
    }
  }

  public SkinnedMultiMesh()
  {
    base.\u002Ector();
  }

  public struct Part
  {
    public GameObject gameObject;
    public string name;
    public Item item;
  }
}
