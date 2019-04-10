// Decompiled with JetBrains decompiler
// Type: ProtectionValue
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using UnityEngine;
using UnityEngine.UI;

public class ProtectionValue : MonoBehaviour, IClothingChanged
{
  public CanvasGroup group;
  public Text text;
  public DamageType damageType;
  public bool selectedItem;
  public bool displayBaseProtection;

  public ProtectionValue()
  {
    base.\u002Ector();
  }
}
