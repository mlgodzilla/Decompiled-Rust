// Decompiled with JetBrains decompiler
// Type: ILerpTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public interface ILerpTarget
{
  float GetExtrapolationTime();

  float GetInterpolationDelay();

  float GetInterpolationSmoothing();

  Vector3 GetNetworkPosition();

  Quaternion GetNetworkRotation();

  void SetNetworkPosition(Vector3 pos);

  void SetNetworkRotation(Quaternion rot);

  void DrawInterpolationState(
    TransformInterpolator.Segment segment,
    List<TransformInterpolator.Entry> entries);
}
