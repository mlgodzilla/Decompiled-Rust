// Decompiled with JetBrains decompiler
// Type: DamageUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Rust;
using System.Collections.Generic;
using UnityEngine;

public static class DamageUtil
{
  public static void RadiusDamage(
    BaseEntity attackingPlayer,
    BaseEntity weaponPrefab,
    Vector3 pos,
    float minradius,
    float radius,
    List<DamageTypeEntry> damage,
    int layers,
    bool useLineOfSight)
  {
    using (TimeWarning.New("DamageUtil.RadiusDamage", 0.1f))
    {
      List<HitInfo> list1 = (List<HitInfo>) Pool.GetList<HitInfo>();
      List<BaseEntity> list2 = (List<BaseEntity>) Pool.GetList<BaseEntity>();
      List<BaseEntity> list3 = (List<BaseEntity>) Pool.GetList<BaseEntity>();
      Vis.Entities<BaseEntity>(pos, radius, list3, layers, (QueryTriggerInteraction) 2);
      for (int index = 0; index < list3.Count; ++index)
      {
        BaseEntity baseEntity = list3[index];
        if (baseEntity.isServer && !list2.Contains(baseEntity))
        {
          Vector3 vector3_1 = baseEntity.ClosestPoint(pos);
          float num = Mathf.Clamp01((float) (((double) Vector3.Distance(vector3_1, pos) - (double) minradius) / ((double) radius - (double) minradius)));
          if ((double) num <= 1.0)
          {
            float amount = 1f - num;
            if (!useLineOfSight || baseEntity.IsVisible(pos, float.PositiveInfinity))
            {
              HitInfo hitInfo1 = new HitInfo();
              hitInfo1.Initiator = attackingPlayer;
              hitInfo1.WeaponPrefab = weaponPrefab;
              hitInfo1.damageTypes.Add(damage);
              hitInfo1.damageTypes.ScaleAll(amount);
              hitInfo1.HitPositionWorld = vector3_1;
              HitInfo hitInfo2 = hitInfo1;
              Vector3 vector3_2 = Vector3.op_Subtraction(pos, vector3_1);
              Vector3 normalized = ((Vector3) ref vector3_2).get_normalized();
              hitInfo2.HitNormalWorld = normalized;
              hitInfo1.PointStart = pos;
              hitInfo1.PointEnd = hitInfo1.HitPositionWorld;
              list1.Add(hitInfo1);
              list2.Add(baseEntity);
            }
          }
        }
      }
      for (int index = 0; index < list2.Count; ++index)
        list2[index].OnAttacked(list1[index]);
      // ISSUE: cast to a reference type
      Pool.FreeList<BaseEntity>((List<M0>&) ref list2);
      // ISSUE: cast to a reference type
      Pool.FreeList<BaseEntity>((List<M0>&) ref list3);
    }
  }
}
