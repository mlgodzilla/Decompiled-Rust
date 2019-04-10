// Decompiled with JetBrains decompiler
// Type: FoliageDisplacement
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class FoliageDisplacement : MonoBehaviour, IClientComponent, ILOD
{
  public bool moving;
  public bool billboard;
  public Mesh mesh;
  public Material material;

  public FoliageDisplacement()
  {
    base.\u002Ector();
  }
}
