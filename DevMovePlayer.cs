// Decompiled with JetBrains decompiler
// Type: DevMovePlayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class DevMovePlayer : BaseMonoBehaviour
{
  public Vector3 destination = Vector3.get_zero();
  public Vector3 lookPoint = Vector3.get_zero();
  public BasePlayer player;
  public Transform[] Waypoints;
  public bool moveRandomly;
  private int waypointIndex;
  private float randRun;

  public void Awake()
  {
    this.randRun = Random.Range(5f, 10f);
    this.player = (BasePlayer) ((Component) this).GetComponent<BasePlayer>();
    this.destination = this.Waypoints.Length == 0 ? ((Component) this).get_transform().get_position() : this.Waypoints[0].get_position();
    if (this.player.isClient)
      return;
    if (Object.op_Equality((Object) this.player.eyes, (Object) null))
      this.player.eyes = (PlayerEyes) ((Component) this.player).GetComponent<PlayerEyes>();
    this.Invoke(new Action(this.LateSpawn), 1f);
  }

  public void LateSpawn()
  {
    Item byName = ItemManager.CreateByName("rifle.semiauto", 1, 0UL);
    this.player.inventory.GiveItem(byName, this.player.inventory.containerBelt);
    this.player.UpdateActiveItem(byName.uid);
    this.player.health = 100f;
  }

  public void SetWaypoints(Transform[] wps)
  {
    this.Waypoints = wps;
    this.destination = wps[0].get_position();
  }

  public void Update()
  {
    if (this.player.isClient || !this.player.IsAlive() || this.player.IsWounded())
      return;
    if ((double) Vector3.Distance(this.destination, ((Component) this).get_transform().get_position()) < 0.25)
    {
      if (this.moveRandomly)
        this.waypointIndex = Random.Range(0, this.Waypoints.Length);
      else
        ++this.waypointIndex;
      if (this.waypointIndex >= this.Waypoints.Length)
        this.waypointIndex = 0;
    }
    this.destination = this.Waypoints[this.waypointIndex].get_position();
    Vector3 vector3_1 = Vector3.op_Subtraction(this.destination, ((Component) this).get_transform().get_position());
    Vector3 normalized = ((Vector3) ref vector3_1).get_normalized();
    float speed = this.player.GetSpeed(Mathf.Sin(Time.get_time() + this.randRun), 0.0f);
    Vector3 vector3_2 = ((Component) this).get_transform().get_position();
    float range = 1f;
    LayerMask mask = LayerMask.op_Implicit(1537286401);
    RaycastHit hitOut;
    if (TransformUtil.GetGroundInfo(Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(Vector3.op_Multiply(normalized, speed), Time.get_deltaTime())), out hitOut, range, mask, ((Component) this.player).get_transform()))
      vector3_2 = ((RaycastHit) ref hitOut).get_point();
    ((Component) this).get_transform().set_position(vector3_2);
    Vector3 vector3_3 = Vector3.op_Subtraction(new Vector3((float) this.destination.x, 0.0f, (float) this.destination.z), new Vector3((float) ((Component) this.player).get_transform().get_position().x, 0.0f, (float) ((Component) this.player).get_transform().get_position().z));
    ((Vector3) ref vector3_3).get_normalized();
    this.player.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }
}
