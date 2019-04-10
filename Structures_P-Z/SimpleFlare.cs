// Decompiled with JetBrains decompiler
// Type: SimpleFlare
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class SimpleFlare : BaseMonoBehaviour, IClientComponent
{
  public float fadeSpeed = 35f;
  public float maxVisibleDistance = 30f;
  protected float tickRate = 0.33f;
  public bool faceCameraPos = true;
  public float dotMin = -1f;
  public float dotMax = -1f;
  public bool timeShimmer;
  public bool positionalShimmer;
  public bool rotate;
  public Collider checkCollider;
  public bool lightScaled;
  public bool alignToCameraViaScript;
  private Vector3 fullSize;
  public bool billboardViaShader;
  private float artificialLightExposure;
  private float privateRand;
  private List<BasePlayer> players;
  private Renderer myRenderer;
  private static MaterialPropertyBlock block;
}
