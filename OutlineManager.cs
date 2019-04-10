// Decompiled with JetBrains decompiler
// Type: OutlineManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class OutlineManager : MonoBehaviour, IClientComponent
{
  public static Material blurMat;
  public List<OutlineObject> objectsToRender;
  public float blurAmount;
  public Material glowSolidMaterial;
  public Material blendGlowMaterial;

  public OutlineManager()
  {
    base.\u002Ector();
  }
}
