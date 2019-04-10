// Decompiled with JetBrains decompiler
// Type: MorphCache
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class MorphCache : MonoBehaviour
{
  public bool fallback;
  public int blendShape;
  [Range(0.0f, 1f)]
  public float[] blendWeights;

  public MorphCache()
  {
    base.\u002Ector();
  }
}
