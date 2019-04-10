// Decompiled with JetBrains decompiler
// Type: ArmorProperties
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Armor Properties")]
public class ArmorProperties : ScriptableObject
{
  [InspectorFlags]
  public HitArea area;

  public bool Contains(HitArea hitArea)
  {
    return (uint) (this.area & hitArea) > 0U;
  }

  public ArmorProperties()
  {
    base.\u002Ector();
  }
}
