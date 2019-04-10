// Decompiled with JetBrains decompiler
// Type: TerrainModifierEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public static class TerrainModifierEx
{
  public static void ApplyTerrainModifiers(
    this Transform transform,
    TerrainModifier[] modifiers,
    Vector3 pos,
    Quaternion rot,
    Vector3 scale)
  {
    for (int index = 0; index < modifiers.Length; ++index)
    {
      TerrainModifier modifier = modifiers[index];
      Vector3 vector3 = Vector3.Scale(modifier.worldPosition, scale);
      modifier.Apply(Vector3.op_Addition(pos, Quaternion.op_Multiply(rot, vector3)), (float) scale.y);
    }
  }

  public static void ApplyTerrainModifiers(this Transform transform, TerrainModifier[] modifiers)
  {
    transform.ApplyTerrainModifiers(modifiers, transform.get_position(), transform.get_rotation(), transform.get_lossyScale());
  }
}
