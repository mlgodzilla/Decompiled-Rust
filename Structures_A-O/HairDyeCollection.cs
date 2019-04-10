// Decompiled with JetBrains decompiler
// Type: HairDyeCollection
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Hair Dye Collection")]
public class HairDyeCollection : ScriptableObject
{
  public Texture capMask;
  public bool applyCap;
  public HairDye[] Variations;

  public HairDye Get(float seed)
  {
    if (this.Variations.Length != 0)
      return this.Variations[Mathf.Clamp(Mathf.FloorToInt(seed * (float) this.Variations.Length), 0, this.Variations.Length - 1)];
    return (HairDye) null;
  }

  public HairDyeCollection()
  {
    base.\u002Ector();
  }
}
