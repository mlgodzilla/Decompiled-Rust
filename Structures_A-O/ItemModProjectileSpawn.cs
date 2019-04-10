// Decompiled with JetBrains decompiler
// Type: ItemModProjectileSpawn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ItemModProjectileSpawn : ItemModProjectile
{
  public GameObjectRef createOnImpact = new GameObjectRef();
  public float spreadAngle = 30f;
  public float spreadVelocityMin = 1f;
  public float spreadVelocityMax = 3f;
  public int numToCreateChances = 1;
  public float createOnImpactChance;

  public override void ServerProjectileHit(HitInfo info)
  {
    for (int index = 0; index < this.numToCreateChances; ++index)
    {
      if (this.createOnImpact.isValid && (double) Random.Range(0.0f, 1f) < (double) this.createOnImpactChance)
      {
        BaseEntity entity = GameManager.server.CreateEntity(this.createOnImpact.resourcePath, (Vector3) null, (Quaternion) null, true);
        if (Object.op_Implicit((Object) entity))
        {
          ((Component) entity).get_transform().set_position(Vector3.op_Addition(info.HitPositionWorld, Vector3.op_Multiply(info.HitNormalWorld, 0.1f)));
          ((Component) entity).get_transform().set_rotation(Quaternion.LookRotation(info.HitNormalWorld));
          if (!GamePhysics.LineOfSight(info.HitPositionWorld, ((Component) entity).get_transform().get_position(), 2162688, 0.0f))
            ((Component) entity).get_transform().set_position(info.HitPositionWorld);
          entity.Spawn();
          if ((double) this.spreadAngle > 0.0)
          {
            Vector3 aimConeDirection = AimConeUtil.GetModifiedAimConeDirection(this.spreadAngle, info.HitNormalWorld, true);
            entity.SetVelocity(Vector3.op_Multiply(aimConeDirection, Random.Range(1f, 3f)));
          }
        }
      }
    }
    base.ServerProjectileHit(info);
  }
}
