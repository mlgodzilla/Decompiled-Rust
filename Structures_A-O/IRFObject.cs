// Decompiled with JetBrains decompiler
// Type: IRFObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public interface IRFObject
{
  Vector3 GetPosition();

  float GetMaxRange();

  void RFSignalUpdate(bool on);

  int GetFrequency();
}
