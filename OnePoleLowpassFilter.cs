// Decompiled with JetBrains decompiler
// Type: OnePoleLowpassFilter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class OnePoleLowpassFilter : MonoBehaviour
{
  [Range(10f, 20000f)]
  public float frequency;

  public OnePoleLowpassFilter()
  {
    base.\u002Ector();
  }
}
