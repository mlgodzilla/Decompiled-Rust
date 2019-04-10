// Decompiled with JetBrains decompiler
// Type: OreResourceEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using System;
using UnityEngine;

public class OreResourceEntity : StagedResourceEntity
{
  private Vector3 lastNodeDir = Vector3.get_zero();
  public GameObjectRef bonusPrefab;
  public GameObjectRef finishEffect;
  public GameObjectRef bonusFailEffect;
  public OreHotSpot _hotSpot;
  private int bonusesKilled;
  private int bonusesSpawned;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("OreResourceEntity.OnRpcMessage", 0.1f))
      ;
    return base.OnRpcMessage(player, rpc, msg);
  }

  protected override void UpdateNetworkStage()
  {
    int stage = this.stage;
    base.UpdateNetworkStage();
    if (this.stage == stage || !Object.op_Implicit((Object) this._hotSpot))
      return;
    this.DelayedBonusSpawn();
  }

  public void CleanupBonus()
  {
    if (Object.op_Implicit((Object) this._hotSpot))
      this._hotSpot.Kill(BaseNetworkable.DestroyMode.None);
    this._hotSpot = (OreHotSpot) null;
  }

  public override void OnKilled(HitInfo info)
  {
    this.CleanupBonus();
    base.OnKilled(info);
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this._hotSpot = this.SpawnBonusSpot(Vector3.get_zero());
  }

  public void FinishBonusAssigned()
  {
    Effect.server.Run(this.finishEffect.resourcePath, ((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_up(), (Connection) null, false);
  }

  public override void OnAttacked(HitInfo info)
  {
    if (this.isClient)
    {
      base.OnAttacked(info);
    }
    else
    {
      if (!info.DidGather && (double) info.gatherScale > 0.0 && Object.op_Implicit((Object) this._hotSpot))
      {
        if ((double) Vector3.Distance(info.HitPositionWorld, ((Component) this._hotSpot).get_transform().get_position()) <= (double) ((SphereCollider) ((Component) this._hotSpot).GetComponent<SphereCollider>()).get_radius() * 1.5 || info.Weapon is Jackhammer)
        {
          ++this.bonusesKilled;
          info.gatherScale = 1f + Mathf.Clamp((float) this.bonusesKilled * 0.5f, 0.0f, 2f);
          this._hotSpot.FireFinishEffect();
          this.ClientRPC<int, Vector3>((Connection) null, "PlayBonusLevelSound", this.bonusesKilled, ((Component) this._hotSpot).get_transform().get_position());
        }
        else if (this.bonusesKilled > 0)
        {
          this.bonusesKilled = 0;
          Effect.server.Run(this.bonusFailEffect.resourcePath, ((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_up(), (Connection) null, false);
        }
        if (this.bonusesKilled > 0)
          this.CleanupBonus();
      }
      if (Object.op_Equality((Object) this._hotSpot, (Object) null))
        this.DelayedBonusSpawn();
      base.OnAttacked(info);
    }
  }

  public void DelayedBonusSpawn()
  {
    this.CancelInvoke(new Action(this.RespawnBonus));
    this.Invoke(new Action(this.RespawnBonus), 0.25f);
  }

  public void RespawnBonus()
  {
    this.CleanupBonus();
    this._hotSpot = this.SpawnBonusSpot(this.lastNodeDir);
  }

  public OreHotSpot SpawnBonusSpot(Vector3 lastDirection)
  {
    if (this.isClient)
      return (OreHotSpot) null;
    if (!this.bonusPrefab.isValid)
      return (OreHotSpot) null;
    Vector2 insideUnitCircle = Random.get_insideUnitCircle();
    ((Vector2) ref insideUnitCircle).get_normalized();
    Vector3.get_zero();
    MeshCollider stageComponent = this.GetStageComponent<MeshCollider>();
    Transform transform = ((Component) this).get_transform();
    Bounds bounds = ((Collider) stageComponent).get_bounds();
    Vector3 center = ((Bounds) ref bounds).get_center();
    Vector3 vector3_1 = transform.InverseTransformPoint(center);
    Vector3 vector3_2;
    Vector3 vector3_3;
    if (Vector3.op_Equality(lastDirection, Vector3.get_zero()))
    {
      Vector3 vector3_4 = this.RandomCircle(1f, false);
      this.lastNodeDir = ((Vector3) ref vector3_4).get_normalized();
      Vector3 vector3_5 = ((Component) this).get_transform().TransformDirection(((Vector3) ref vector3_4).get_normalized());
      vector3_2 = Vector3.op_Addition(Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(((Component) this).get_transform().get_up(), (float) (vector3_1.y + 0.5))), Vector3.op_Multiply(((Vector3) ref vector3_5).get_normalized(), 2.5f));
    }
    else
    {
      vector3_3 = Vector3.op_Addition(this.lastNodeDir, Vector3.op_Multiply(Vector3.op_Multiply(Vector3.Cross(this.lastNodeDir, Vector3.get_up()), Random.Range(0.25f, 0.5f)), Random.Range(0, 2) == 0 ? -1f : 1f));
      Vector3 normalized = ((Vector3) ref vector3_3).get_normalized();
      this.lastNodeDir = normalized;
      Vector3 vector3_4 = Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(((Component) this).get_transform().TransformDirection(normalized), 2f));
      float num = Random.Range(1f, 1.5f);
      vector3_2 = Vector3.op_Addition(vector3_4, Vector3.op_Multiply(((Component) this).get_transform().get_up(), (float) vector3_1.y + num));
    }
    ++this.bonusesSpawned;
    bounds = ((Collider) stageComponent).get_bounds();
    vector3_3 = Vector3.op_Subtraction(((Bounds) ref bounds).get_center(), vector3_2);
    Vector3 normalized1 = ((Vector3) ref vector3_3).get_normalized();
    RaycastHit raycastHit;
    if (!((Collider) stageComponent).Raycast(new Ray(vector3_2, normalized1), ref raycastHit, 10f))
      return (OreHotSpot) null;
    OreHotSpot entity = GameManager.server.CreateEntity(this.bonusPrefab.resourcePath, Vector3.op_Subtraction(((RaycastHit) ref raycastHit).get_point(), Vector3.op_Multiply(normalized1, 0.025f)), Quaternion.LookRotation(((RaycastHit) ref raycastHit).get_normal(), Vector3.get_up()), true) as OreHotSpot;
    entity.Spawn();
    ((Component) entity).SendMessage("OreOwner", (object) this);
    return entity;
  }

  public Vector3 RandomCircle(float distance = 1f, bool allowInside = false)
  {
    Vector2 vector2_1;
    if (!allowInside)
    {
      Vector2 insideUnitCircle = Random.get_insideUnitCircle();
      vector2_1 = ((Vector2) ref insideUnitCircle).get_normalized();
    }
    else
      vector2_1 = Random.get_insideUnitCircle();
    Vector2 vector2_2 = vector2_1;
    return new Vector3((float) vector2_2.x, 0.0f, (float) vector2_2.y);
  }

  public Vector3 RandomHemisphereDirection(
    Vector3 input,
    float degreesOffset,
    bool allowInside = true,
    bool changeHeight = true)
  {
    degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
    Vector2 vector2_1;
    if (!allowInside)
    {
      Vector2 insideUnitCircle = Random.get_insideUnitCircle();
      vector2_1 = ((Vector2) ref insideUnitCircle).get_normalized();
    }
    else
      vector2_1 = Random.get_insideUnitCircle();
    Vector2 vector2_2 = vector2_1;
    Vector3 vector3_1;
    ((Vector3) ref vector3_1).\u002Ector((float) vector2_2.x * degreesOffset, changeHeight ? Random.Range(-1f, 1f) * degreesOffset : 0.0f, (float) vector2_2.y * degreesOffset);
    Vector3 vector3_2 = Vector3.op_Addition(input, vector3_1);
    return ((Vector3) ref vector3_2).get_normalized();
  }

  public Vector3 ClampToHemisphere(Vector3 hemiInput, float degreesOffset, Vector3 inputVec)
  {
    degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
    Vector3 vector3_1 = Vector3.op_Addition(hemiInput, Vector3.op_Multiply(Vector3.get_one(), degreesOffset));
    Vector3 normalized1 = ((Vector3) ref vector3_1).get_normalized();
    Vector3 vector3_2 = Vector3.op_Addition(hemiInput, Vector3.op_Multiply(Vector3.get_one(), -degreesOffset));
    Vector3 normalized2 = ((Vector3) ref vector3_2).get_normalized();
    for (int index = 0; index < 3; ++index)
      ((Vector3) ref inputVec).set_Item(index, Mathf.Clamp(((Vector3) ref inputVec).get_Item(index), ((Vector3) ref normalized2).get_Item(index), ((Vector3) ref normalized1).get_Item(index)));
    return inputVec;
  }

  public static Vector3 RandomCylinderPointAroundVector(
    Vector3 input,
    float distance,
    float minHeight = 0.0f,
    float maxHeight = 0.0f,
    bool allowInside = false)
  {
    Vector2 vector2_1;
    if (!allowInside)
    {
      Vector2 insideUnitCircle = Random.get_insideUnitCircle();
      vector2_1 = ((Vector2) ref insideUnitCircle).get_normalized();
    }
    else
      vector2_1 = Random.get_insideUnitCircle();
    Vector2 vector2_2 = vector2_1;
    Vector3 vector3_1 = new Vector3((float) vector2_2.x, 0.0f, (float) vector2_2.y);
    Vector3 vector3_2 = Vector3.op_Multiply(((Vector3) ref vector3_1).get_normalized(), distance);
    vector3_2.y = (__Null) (double) Random.Range(minHeight, maxHeight);
    return vector3_2;
  }

  public Vector3 ClampToCylinder(
    Vector3 localPos,
    Vector3 cylinderAxis,
    float cylinderDistance,
    float minHeight = 0.0f,
    float maxHeight = 0.0f)
  {
    return Vector3.get_zero();
  }
}
