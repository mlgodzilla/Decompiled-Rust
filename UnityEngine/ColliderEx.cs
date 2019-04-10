// Decompiled with JetBrains decompiler
// Type: UnityEngine.ColliderEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace UnityEngine
{
  public static class ColliderEx
  {
    public static PhysicMaterial GetMaterialAt(this Collider obj, Vector3 pos)
    {
      if (obj is TerrainCollider)
        return TerrainMeta.Physics.GetMaterial(pos);
      return obj.get_sharedMaterial();
    }
  }
}
