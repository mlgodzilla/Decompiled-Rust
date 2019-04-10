// Decompiled with JetBrains decompiler
// Type: CargoPlane
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Oxide.Core;
using UnityEngine;

public class CargoPlane : BaseEntity
{
  public Vector3 dropPosition = Vector3.get_zero();
  public GameObjectRef prefabDrop;
  public SpawnFilter filter;
  public Vector3 startPos;
  public Vector3 endPos;
  public float secondsToTake;
  public float secondsTaken;
  public bool dropped;

  public void InitDropPosition(Vector3 newDropPosition)
  {
    this.dropPosition = newDropPosition;
    this.dropPosition.y = (__Null) 0.0;
  }

  public override void ServerInit()
  {
    base.ServerInit();
    if (Vector3.op_Equality(this.dropPosition, Vector3.get_zero()))
      this.dropPosition = this.RandomDropPosition();
    this.UpdateDropPosition(this.dropPosition);
  }

  public Vector3 RandomDropPosition()
  {
    Vector3.get_zero();
    float num = 100f;
    float x = (float) TerrainMeta.Size.x;
    Vector3 worldPos;
    do
    {
      worldPos = Vector3Ex.Range((float) -((double) x / 3.0), x / 3f);
    }
    while ((double) this.filter.GetFactor(worldPos) == 0.0 && (double) --num > 0.0);
    worldPos.y = (__Null) 0.0;
    return worldPos;
  }

  public void UpdateDropPosition(Vector3 newDropPosition)
  {
    float x = (float) TerrainMeta.Size.x;
    float num = (float) (TerrainMeta.HighestPoint.y + 250.0);
    this.startPos = Vector3Ex.Range(-1f, 1f);
    this.startPos.y = (__Null) 0.0;
    ((Vector3) ref this.startPos).Normalize();
    this.startPos = Vector3.op_Multiply(this.startPos, x * 2f);
    this.startPos.y = (__Null) (double) num;
    this.endPos = Vector3.op_Multiply(this.startPos, -1f);
    this.endPos.y = this.startPos.y;
    this.startPos = Vector3.op_Addition(this.startPos, newDropPosition);
    this.endPos = Vector3.op_Addition(this.endPos, newDropPosition);
    this.secondsToTake = Vector3.Distance(this.startPos, this.endPos) / 50f;
    this.secondsToTake *= Random.Range(0.95f, 1.05f);
    ((Component) this).get_transform().set_position(this.startPos);
    ((Component) this).get_transform().set_rotation(Quaternion.LookRotation(Vector3.op_Subtraction(this.endPos, this.startPos)));
    this.dropPosition = newDropPosition;
    Interface.CallHook("OnAirdrop", (object) this, (object) newDropPosition);
  }

  private void Update()
  {
    if (!this.isServer)
      return;
    this.secondsTaken += Time.get_deltaTime();
    float num = Mathf.InverseLerp(0.0f, this.secondsToTake, this.secondsTaken);
    if (!this.dropped && (double) num >= 0.5)
    {
      this.dropped = true;
      BaseEntity entity = GameManager.server.CreateEntity(this.prefabDrop.resourcePath, ((Component) this).get_transform().get_position(), (Quaternion) null, true);
      if (Object.op_Implicit((Object) entity))
      {
        entity.globalBroadcast = true;
        entity.Spawn();
      }
    }
    ((Component) this).get_transform().set_position(Vector3.Lerp(this.startPos, this.endPos, num));
    ((Component) this).get_transform().set_hasChanged(true);
    if ((double) num < 1.0)
      return;
    this.Kill(BaseNetworkable.DestroyMode.None);
  }
}
