﻿// Decompiled with JetBrains decompiler
// Type: RendererLOD
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Rendering;

public class RendererLOD : LODComponent, IBatchingHandler
{
  [Horizontal(1, 0)]
  public RendererLOD.State[] States;

  [Serializable]
  public class State
  {
    public float distance;
    public Renderer renderer;
    [NonSerialized]
    public MeshFilter filter;
    [NonSerialized]
    public ShadowCastingMode shadowMode;
    [NonSerialized]
    public bool isImpostor;
  }
}
