// Decompiled with JetBrains decompiler
// Type: SurveyCharge
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System.Collections.Generic;
using UnityEngine;

public class SurveyCharge : TimedExplosive
{
  public GameObjectRef craterPrefab;
  public GameObjectRef craterPrefab_Oil;

  public override void Explode()
  {
    base.Explode();
    if (WaterLevel.Test(((Component) this).get_transform().get_position()))
      return;
    ResourceDepositManager.ResourceDeposit resourceDeposit = ResourceDepositManager.GetOrCreate(((Component) this).get_transform().get_position());
    if (resourceDeposit == null || (double) Time.get_realtimeSinceStartup() - (double) resourceDeposit.lastSurveyTime < 10.0)
      return;
    resourceDeposit.lastSurveyTime = Time.get_realtimeSinceStartup();
    RaycastHit hitOut;
    if (!TransformUtil.GetGroundInfo(((Component) this).get_transform().get_position(), out hitOut, 0.3f, LayerMask.op_Implicit(8388608), (Transform) null))
      return;
    Vector3 point = ((RaycastHit) ref hitOut).get_point();
    ((RaycastHit) ref hitOut).get_normal();
    List<SurveyCrater> list = (List<SurveyCrater>) Pool.GetList<SurveyCrater>();
    Vis.Entities<SurveyCrater>(((Component) this).get_transform().get_position(), 10f, list, 1, (QueryTriggerInteraction) 2);
    int num1 = list.Count > 0 ? 1 : 0;
    // ISSUE: cast to a reference type
    Pool.FreeList<SurveyCrater>((List<M0>&) ref list);
    if (num1 != 0)
      return;
    bool flag1 = false;
    bool flag2 = false;
    foreach (ResourceDepositManager.ResourceDeposit.ResourceDepositEntry resource in resourceDeposit._resources)
    {
      if (resource.spawnType == ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM && !resource.isLiquid && resource.amount >= 1000)
      {
        int num2 = Mathf.Clamp(Mathf.CeilToInt((float) (2.5 / (double) resource.workNeeded * 10.0)), 0, 5);
        int iAmount = 1;
        flag1 = true;
        if (resource.isLiquid)
          flag2 = true;
        for (int index = 0; index < num2; ++index)
        {
          Item obj = ItemManager.Create(resource.type, iAmount, 0UL);
          Vector3 aimConeDirection = AimConeUtil.GetModifiedAimConeDirection(20f, Vector3.get_up(), true);
          Vector3 vPos = Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_up(), 1f));
          Vector3 vVelocity = Vector3.op_Addition(this.GetInheritedDropVelocity(), Vector3.op_Multiply(aimConeDirection, Random.Range(5f, 10f)));
          Quaternion rotation1 = Random.get_rotation();
          BaseEntity baseEntity = obj.Drop(vPos, vVelocity, rotation1);
          Quaternion rotation2 = Random.get_rotation();
          Vector3 velocity = Vector3.op_Multiply(((Quaternion) ref rotation2).get_eulerAngles(), 5f);
          baseEntity.SetAngularVelocity(velocity);
        }
      }
    }
    if (!flag1)
      return;
    string strPrefab = flag2 ? this.craterPrefab_Oil.resourcePath : this.craterPrefab.resourcePath;
    BaseEntity entity = GameManager.server.CreateEntity(strPrefab, point, Quaternion.get_identity(), true);
    if (!Object.op_Implicit((Object) entity))
      return;
    entity.Spawn();
  }
}
