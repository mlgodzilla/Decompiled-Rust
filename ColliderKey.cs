// Decompiled with JetBrains decompiler
// Type: ColliderKey
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public struct ColliderKey : IEquatable<ColliderKey>
{
  public PhysicMaterial material;
  public int layer;

  public ColliderKey(PhysicMaterial material, int layer)
  {
    this.material = material;
    this.layer = layer;
  }

  public ColliderKey(Collider collider)
  {
    this.material = collider.get_sharedMaterial();
    this.layer = ((Component) collider).get_gameObject().get_layer();
  }

  public ColliderKey(ColliderBatch batch)
  {
    this.material = ((Collider) batch.BatchCollider).get_sharedMaterial();
    this.layer = ((Component) batch.BatchCollider).get_gameObject().get_layer();
  }

  public override int GetHashCode()
  {
    return ((object) this.material).GetHashCode() ^ this.layer.GetHashCode();
  }

  public override bool Equals(object other)
  {
    if (!(other is ColliderKey))
      return false;
    return this.Equals((ColliderKey) other);
  }

  public bool Equals(ColliderKey other)
  {
    if (Object.op_Equality((Object) this.material, (Object) other.material))
      return this.layer == other.layer;
    return false;
  }
}
