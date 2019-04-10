// Decompiled with JetBrains decompiler
// Type: TreadAnimator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class TreadAnimator : MonoBehaviour, IClientComponent
{
  public Animator mainBodyAnimator;
  public Transform[] wheelBones;
  public Vector3[] vecShocksOffsetPosition;
  public Vector3[] wheelBoneOrigin;
  public float wheelBoneDistMax;
  public Renderer treadRenderer;
  public Material leftTread;
  public Material rightTread;
  public TreadEffects treadEffects;

  public TreadAnimator()
  {
    base.\u002Ector();
  }
}
