// Decompiled with JetBrains decompiler
// Type: FlameExplosive
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class FlameExplosive : TimedExplosive
{
  public float numToCreate = 10f;
  public float minVelocity = 2f;
  public float maxVelocity = 5f;
  public float spreadAngle = 90f;
  public GameObjectRef createOnExplode;

  public override void Explode()
  {
    this.Explode(Vector3.op_UnaryNegation(((Component) this).get_transform().get_forward()));
  }

  public void Explode(Vector3 surfaceNormal)
  {
    if (!this.isServer)
      return;
    for (int index = 0; (double) index < (double) this.numToCreate; ++index)
    {
      BaseEntity entity = GameManager.server.CreateEntity(this.createOnExplode.resourcePath, ((Component) this).get_transform().get_position(), (Quaternion) null, true);
      if (Object.op_Implicit((Object) entity))
      {
        ((Component) entity).get_transform().set_position(((Component) this).get_transform().get_position());
        Vector3 aimConeDirection = AimConeUtil.GetModifiedAimConeDirection(this.spreadAngle, surfaceNormal, true);
        ((Component) entity).get_transform().set_rotation(Quaternion.LookRotation(aimConeDirection));
        entity.creatorEntity = Object.op_Equality((Object) this.creatorEntity, (Object) null) ? entity : this.creatorEntity;
        entity.Spawn();
        entity.SetVelocity(Vector3.op_Multiply(aimConeDirection, Random.Range(this.minVelocity, this.maxVelocity)));
      }
    }
    base.Explode();
  }

  public override void ProjectileImpact(RaycastHit info)
  {
    this.Explode(((RaycastHit) ref info).get_normal());
  }
}
