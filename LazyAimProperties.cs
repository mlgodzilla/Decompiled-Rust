// Decompiled with JetBrains decompiler
// Type: LazyAimProperties
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[CreateAssetMenu(menuName = "Rust/LazyAim Properties")]
public class LazyAimProperties : ScriptableObject
{
  [Range(0.0f, 10f)]
  public float snapStrength;
  [Range(0.0f, 45f)]
  public float deadzoneAngle;

  public LazyAimProperties()
  {
    base.\u002Ector();
  }
}
