// Decompiled with JetBrains decompiler
// Type: WearableHolsterOffset
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class WearableHolsterOffset : MonoBehaviour
{
  public WearableHolsterOffset.offsetInfo[] Offsets;

  public WearableHolsterOffset()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class offsetInfo
  {
    public HeldEntity.HolsterInfo.HolsterSlot type;
    public Vector3 offset;
    public Vector3 rotationOffset;
    public int priority;
  }
}
