// Decompiled with JetBrains decompiler
// Type: LocalPositionAnimation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class LocalPositionAnimation : MonoBehaviour, IClientComponent
{
  public Vector3 centerPosition;
  public bool worldSpace;
  public float scaleX;
  public float timeScaleX;
  public AnimationCurve movementX;
  public float scaleY;
  public float timeScaleY;
  public AnimationCurve movementY;
  public float scaleZ;
  public float timeScaleZ;
  public AnimationCurve movementZ;

  public LocalPositionAnimation()
  {
    base.\u002Ector();
  }
}
