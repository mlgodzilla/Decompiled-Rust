// Decompiled with JetBrains decompiler
// Type: DecorComponentEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public static class DecorComponentEx
{
  public static void ApplyDecorComponents(
    this Transform transform,
    DecorComponent[] components,
    ref Vector3 pos,
    ref Quaternion rot,
    ref Vector3 scale)
  {
    for (int index = 0; index < components.Length; ++index)
      components[index].Apply(ref pos, ref rot, ref scale);
  }

  public static void ApplyDecorComponents(this Transform transform, DecorComponent[] components)
  {
    Vector3 position = transform.get_position();
    Quaternion rotation = transform.get_rotation();
    Vector3 localScale = transform.get_localScale();
    transform.ApplyDecorComponents(components, ref position, ref rotation, ref localScale);
    transform.set_position(position);
    transform.set_rotation(rotation);
    transform.set_localScale(localScale);
  }

  public static void ApplyDecorComponentsScaleOnly(
    this Transform transform,
    DecorComponent[] components)
  {
    Vector3 position = transform.get_position();
    Quaternion rotation = transform.get_rotation();
    Vector3 localScale = transform.get_localScale();
    transform.ApplyDecorComponents(components, ref position, ref rotation, ref localScale);
    transform.set_localScale(localScale);
  }
}
