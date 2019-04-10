// Decompiled with JetBrains decompiler
// Type: LaserBeam
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class LaserBeam : MonoBehaviour
{
  public float scrollSpeed;
  public LineRenderer beamRenderer;
  public GameObject dotObject;
  public Renderer dotRenderer;
  public GameObject dotSpotlight;
  public Vector2 scrollDir;
  public float maxDistance;
  public float stillBlendFactor;
  public float movementBlendFactor;
  public float movementThreshhold;
  public bool isFirstPerson;
  public Transform emissionOverride;
  private MaterialPropertyBlock block;

  public LaserBeam()
  {
    base.\u002Ector();
  }
}
