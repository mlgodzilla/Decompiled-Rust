// Decompiled with JetBrains decompiler
// Type: EmissionToggle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class EmissionToggle : MonoBehaviour, IClientComponent
{
  private Color emissionColor;
  public Renderer[] targetRenderers;
  public int materialIndex;
  private static MaterialPropertyBlock block;

  public EmissionToggle()
  {
    base.\u002Ector();
  }
}
