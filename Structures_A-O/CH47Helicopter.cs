// Decompiled with JetBrains decompiler
// Type: CH47Helicopter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class CH47Helicopter : BaseHelicopterVehicle
{
  public GameObjectRef mapMarkerEntityPrefab;
  private BaseEntity mapMarkerInstance;

  public override void ServerInit()
  {
    this.rigidBody.set_isKinematic(false);
    base.ServerInit();
    this.CreateMapMarker();
  }

  public override void PlayerServerInput(InputState inputState, BasePlayer player)
  {
    base.PlayerServerInput(inputState, player);
  }

  public void CreateMapMarker()
  {
    if (Object.op_Implicit((Object) this.mapMarkerInstance))
      this.mapMarkerInstance.Kill(BaseNetworkable.DestroyMode.None);
    BaseEntity entity = GameManager.server.CreateEntity(this.mapMarkerEntityPrefab.resourcePath, Vector3.get_zero(), Quaternion.get_identity(), true);
    entity.Spawn();
    entity.SetParent((BaseEntity) this, false, false);
    this.mapMarkerInstance = entity;
  }
}
