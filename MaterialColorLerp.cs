// Decompiled with JetBrains decompiler
// Type: MaterialColorLerp
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class MaterialColorLerp : MonoBehaviour, IClientComponent
{
  public Color startColor;
  public Color endColor;
  public Color currentColor;
  public float delta;

  public MaterialColorLerp()
  {
    base.\u002Ector();
  }
}
