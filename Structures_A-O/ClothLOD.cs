// Decompiled with JetBrains decompiler
// Type: ClothLOD
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ClothLOD : FacepunchBehaviour
{
  [ServerVar(Help = "distance cloth will simulate until")]
  public static float clothLODDist = 20f;
  public Cloth cloth;

  public ClothLOD()
  {
    base.\u002Ector();
  }
}
