// Decompiled with JetBrains decompiler
// Type: EntityCollisionMessage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class EntityCollisionMessage : EntityComponent<BaseEntity>
{
  private void OnCollisionEnter(Collision collision)
  {
    if (Object.op_Equality((Object) this.baseEntity, (Object) null) || this.baseEntity.IsDestroyed)
      return;
    BaseEntity hitEntity = collision.GetEntity();
    if (Object.op_Equality((Object) hitEntity, (Object) this.baseEntity))
      return;
    if (Object.op_Inequality((Object) hitEntity, (Object) null))
    {
      if (hitEntity.IsDestroyed)
        return;
      if (this.baseEntity.isServer)
        hitEntity = hitEntity.ToServer<BaseEntity>();
    }
    this.baseEntity.OnCollision(collision, hitEntity);
  }
}
