// Decompiled with JetBrains decompiler
// Type: ItemSkin
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Workshop;
using UnityEngine;

public class ItemSkin : SteamInventoryItem
{
  public Skinnable Skinnable;
  public Material[] Materials;

  public void ApplySkin(GameObject obj)
  {
    if (Object.op_Equality((Object) this.Skinnable, (Object) null))
      return;
    Skin.Apply(obj, this.Skinnable, this.Materials);
  }
}
