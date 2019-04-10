// Decompiled with JetBrains decompiler
// Type: SupplySignal
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class SupplySignal : TimedExplosive
{
  public GameObjectRef smokeEffectPrefab;
  public GameObjectRef EntityToCreate;
  [NonSerialized]
  public GameObject smokeEffect;

  public override void Explode()
  {
    BaseEntity entity = GameManager.server.CreateEntity(this.EntityToCreate.resourcePath, (Vector3) null, (Quaternion) null, true);
    if (Object.op_Implicit((Object) entity))
    {
      Vector3 vector3;
      ((Vector3) ref vector3).\u002Ector(Random.Range(-20f, 20f), 0.0f, Random.Range(-20f, 20f));
      ((Component) entity).SendMessage("InitDropPosition", (object) Vector3.op_Addition(((Component) this).get_transform().get_position(), vector3), (SendMessageOptions) 1);
      entity.Spawn();
    }
    this.Invoke(new Action(this.FinishUp), 210f);
    this.SetFlag(BaseEntity.Flags.On, true, false, true);
    this.SendNetworkUpdateImmediate(false);
  }

  public void FinishUp()
  {
    this.Kill(BaseNetworkable.DestroyMode.None);
  }
}
