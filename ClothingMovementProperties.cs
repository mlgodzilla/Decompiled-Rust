// Decompiled with JetBrains decompiler
// Type: ClothingMovementProperties
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Clothing Movement Properties")]
public class ClothingMovementProperties : ScriptableObject
{
  public float speedReduction;
  [Tooltip("If this piece of clothing is worn movement speed will be reduced by atleast this much")]
  public float minSpeedReduction;
  public float waterSpeedBonus;

  public ClothingMovementProperties()
  {
    base.\u002Ector();
  }
}
