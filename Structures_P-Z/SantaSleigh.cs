// Decompiled with JetBrains decompiler
// Type: SantaSleigh
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using System;
using UnityEngine;

public class SantaSleigh : BaseEntity
{
  [ServerVar]
  public static float altitudeAboveTerrain = 50f;
  [ServerVar]
  public static float desiredAltitude = 60f;
  public float hohohospacing = 4f;
  public float hohoho_additional_spacing = 2f;
  private Vector3 dropPosition = Vector3.get_zero();
  public float appliedSwimScale = 1f;
  public float appliedSwimRotation = 20f;
  public GameObjectRef prefabDrop;
  public SpawnFilter filter;
  public Transform dropOrigin;
  public Light bigLight;
  public SoundPlayer hohoho;
  private Vector3 startPos;
  private Vector3 endPos;
  private float secondsToTake;
  private float secondsTaken;
  private bool dropped;
  public Vector3 swimScale;
  public Vector3 swimSpeed;
  private float swimRandom;
  private const string path = "assets/prefabs/misc/xmas/sleigh/santasleigh.prefab";

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("SantaSleigh.OnRpcMessage", 0.1f))
      ;
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override bool PhysicsDriven()
  {
    return true;
  }

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
    this.Invoke(new Action(this.SendHoHoHo), 0.0f);
  }

  public void SendHoHoHo()
  {
    this.Invoke(new Action(this.SendHoHoHo), this.hohohospacing + Random.Range(0.0f, this.hohoho_additional_spacing));
    this.ClientRPC((Connection) null, "ClientPlayHoHoHo");
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
    float altitudeAboveTerrain = SantaSleigh.altitudeAboveTerrain;
    this.startPos = Vector3Ex.Range(-1f, 1f);
    this.startPos.y = (__Null) 0.0;
    ((Vector3) ref this.startPos).Normalize();
    this.startPos = Vector3.op_Multiply(this.startPos, x * 1.25f);
    this.startPos.y = (__Null) (double) altitudeAboveTerrain;
    this.endPos = Vector3.op_Multiply(this.startPos, -1f);
    this.endPos.y = this.startPos.y;
    this.startPos = Vector3.op_Addition(this.startPos, newDropPosition);
    this.endPos = Vector3.op_Addition(this.endPos, newDropPosition);
    this.secondsToTake = Vector3.Distance(this.startPos, this.endPos) / 25f;
    this.secondsToTake *= Random.Range(0.95f, 1.05f);
    ((Component) this).get_transform().set_position(this.startPos);
    ((Component) this).get_transform().set_rotation(Quaternion.LookRotation(Vector3.op_Subtraction(this.endPos, this.startPos)));
    this.dropPosition = newDropPosition;
  }

  private void FixedUpdate()
  {
    if (!this.isServer)
      return;
    this.secondsTaken += Time.get_deltaTime();
    float num1 = Mathf.InverseLerp(0.0f, this.secondsToTake, this.secondsTaken);
    if (!this.dropped && (double) num1 >= 0.5)
    {
      this.dropped = true;
      BaseEntity entity = GameManager.server.CreateEntity(this.prefabDrop.resourcePath, ((Component) this.dropOrigin).get_transform().get_position(), (Quaternion) null, true);
      if (Object.op_Implicit((Object) entity))
      {
        entity.globalBroadcast = true;
        entity.Spawn();
      }
    }
    ((Component) this).get_transform().set_position(Vector3.Lerp(this.startPos, this.endPos, num1));
    Vector3 vector3_1 = Vector3.op_Subtraction(this.endPos, this.startPos);
    Vector3 normalized = ((Vector3) ref vector3_1).get_normalized();
    Vector3 zero = Vector3.get_zero();
    if (Vector3.op_Inequality(this.swimScale, Vector3.get_zero()))
    {
      if ((double) this.swimRandom == 0.0)
        this.swimRandom = Random.Range(0.0f, 20f);
      float num2 = Time.get_time() + this.swimRandom;
      ((Vector3) ref zero).\u002Ector(Mathf.Sin(num2 * (float) this.swimSpeed.x) * (float) this.swimScale.x, Mathf.Cos(num2 * (float) this.swimSpeed.y) * (float) this.swimScale.y, Mathf.Sin(num2 * (float) this.swimSpeed.z) * (float) this.swimScale.z);
      Vector3 vector3_2 = ((Component) this).get_transform().InverseTransformDirection(zero);
      Transform transform = ((Component) this).get_transform();
      transform.set_position(Vector3.op_Addition(transform.get_position(), Vector3.op_Multiply(vector3_2, this.appliedSwimScale)));
    }
    ((Component) this).get_transform().set_rotation(Quaternion.op_Multiply(Quaternion.LookRotation(normalized), Quaternion.Euler(Mathf.Cos(Time.get_time() * (float) this.swimSpeed.y) * this.appliedSwimRotation, 0.0f, Mathf.Sin(Time.get_time() * (float) this.swimSpeed.x) * this.appliedSwimRotation)));
    Vector3 position = ((Component) this).get_transform().get_position();
    float num3 = Mathf.Max(TerrainMeta.HeightMap.GetHeight(Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(((Component) this).get_transform().get_forward(), 30f))), TerrainMeta.HeightMap.GetHeight(((Component) this).get_transform().get_position()));
    position.y = (__Null) (double) Mathf.Max(SantaSleigh.desiredAltitude, num3 + SantaSleigh.altitudeAboveTerrain);
    ((Component) this).get_transform().set_position(position);
    ((Component) this).get_transform().set_hasChanged(true);
    if ((double) num1 < 1.0)
      return;
    this.Kill(BaseNetworkable.DestroyMode.None);
  }

  [ServerVar]
  public static void drop(ConsoleSystem.Arg arg)
  {
    BasePlayer basePlayer = arg.Player();
    if (!Object.op_Implicit((Object) basePlayer))
      return;
    Debug.Log((object) "Santa Inbound");
    BaseEntity entity = GameManager.server.CreateEntity("assets/prefabs/misc/xmas/sleigh/santasleigh.prefab", (Vector3) null, (Quaternion) null, true);
    if (!Object.op_Implicit((Object) entity))
      return;
    ((SantaSleigh) ((Component) entity).GetComponent<SantaSleigh>()).InitDropPosition(Vector3.op_Addition(((Component) basePlayer).get_transform().get_position(), new Vector3(0.0f, 10f, 0.0f)));
    entity.Spawn();
  }
}
