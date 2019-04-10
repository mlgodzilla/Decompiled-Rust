// Decompiled with JetBrains decompiler
// Type: BaseBoat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseBoat : BaseVehicle
{
  [ServerVar]
  public static bool generate_paths = true;
  public float engineThrust = 10f;
  public float steeringScale = 0.1f;
  public float gasPedal;
  public float steering;
  public Rigidbody myRigidBody;
  public Transform thrustPoint;
  public Transform centerOfMass;
  public Buoyancy buoyancy;
  public GameObject clientCollider;
  public GameObject serverCollider;

  public bool InDryDock()
  {
    return Object.op_Inequality((Object) this.GetParentEntity(), (Object) null);
  }

  public override float MaxVelocity()
  {
    return 25f;
  }

  public override bool PhysicsDriven()
  {
    return true;
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.myRigidBody.set_isKinematic(false);
    if (Object.op_Equality((Object) this.myRigidBody, (Object) null))
      Debug.LogWarning((object) "Boat rigidbody null");
    else if (Object.op_Equality((Object) this.centerOfMass, (Object) null))
      Debug.LogWarning((object) "boat COM null");
    else
      this.myRigidBody.set_centerOfMass(this.centerOfMass.get_localPosition());
  }

  public override void PlayerServerInput(InputState inputState, BasePlayer player)
  {
    if (this.GetPlayerSeat(player) != 0)
      return;
    this.DriverInput(inputState, player);
  }

  public virtual void DriverInput(InputState inputState, BasePlayer player)
  {
    this.gasPedal = !inputState.IsDown(BUTTON.FORWARD) ? (!inputState.IsDown(BUTTON.BACKWARD) ? 0.0f : -0.5f) : 1f;
    if (inputState.IsDown(BUTTON.LEFT))
      this.steering = 1f;
    else if (inputState.IsDown(BUTTON.RIGHT))
      this.steering = -1f;
    else
      this.steering = 0.0f;
  }

  public virtual bool EngineOn()
  {
    if (this.HasDriver())
      return !this.IsFlipped();
    return false;
  }

  public override void VehicleFixedUpdate()
  {
    if (this.isClient)
      return;
    if (!this.EngineOn())
    {
      this.gasPedal = 0.0f;
      this.steering = 0.0f;
    }
    base.VehicleFixedUpdate();
    if (!((double) this.gasPedal != 0.0 & (double) WaterSystem.GetHeight(this.thrustPoint.get_position()) >= this.thrustPoint.get_position().y) || (double) this.buoyancy.submergedFraction <= 0.300000011920929)
      return;
    Vector3 vector3 = Vector3.op_Addition(((Component) this).get_transform().get_forward(), Vector3.op_Multiply(Vector3.op_Multiply(((Component) this).get_transform().get_right(), this.steering), this.steeringScale));
    this.myRigidBody.AddForceAtPosition(Vector3.op_Multiply(Vector3.op_Multiply(((Vector3) ref vector3).get_normalized(), this.gasPedal), this.engineThrust), this.thrustPoint.get_position(), (ForceMode) 0);
  }

  public virtual bool EngineInWater()
  {
    return (double) TerrainMeta.WaterMap.GetHeight(this.thrustPoint.get_position()) > this.thrustPoint.get_position().y;
  }

  public override float WaterFactorForPlayer(BasePlayer player)
  {
    return (double) TerrainMeta.WaterMap.GetHeight(player.eyes.position) >= player.eyes.position.y ? 1f : 0.0f;
  }

  public bool IsFlipped()
  {
    return (double) Vector3.Dot(Vector3.get_up(), ((Component) this).get_transform().get_up()) <= 0.0;
  }

  public static float GetWaterDepth(Vector3 pos)
  {
    if (Application.get_isPlaying() && !Object.op_Equality((Object) TerrainMeta.WaterMap, (Object) null))
      return TerrainMeta.WaterMap.GetDepth(pos);
    RaycastHit raycastHit;
    if (!Physics.Raycast(pos, Vector3.get_down(), ref raycastHit, 100f, 8388608))
      return 100f;
    return ((RaycastHit) ref raycastHit).get_distance();
  }

  public static List<Vector3> GenerateOceanPatrolPath(
    float minDistanceFromShore = 50f,
    float minWaterDepth = 8f)
  {
    // ISSUE: variable of the null type
    __Null x = TerrainMeta.Size.x;
    int num1 = Mathf.CeilToInt((float) (x * 2.0 * 3.14159274101257) / 30f);
    List<Vector3> vector3List1 = new List<Vector3>();
    float num2 = (float) x;
    float num3 = 0.0f;
    for (int index = 0; index < num1; ++index)
    {
      float num4 = (float) ((double) index / (double) num1 * 360.0);
      vector3List1.Add(new Vector3(Mathf.Sin(num4 * ((float) Math.PI / 180f)) * num2, num3, Mathf.Cos(num4 * ((float) Math.PI / 180f)) * num2));
    }
    float num5 = 4f;
    float num6 = 200f;
    bool flag1 = true;
    for (int index1 = 0; index1 < AI.ocean_patrol_path_iterations & flag1; ++index1)
    {
      flag1 = false;
      for (int index2 = 0; index2 < num1; ++index2)
      {
        Vector3 vector3_1 = vector3List1[index2];
        int index3 = index2 == 0 ? num1 - 1 : index2 - 1;
        int index4 = index2 == num1 - 1 ? 0 : index2 + 1;
        Vector3 vector3_2 = vector3List1[index4];
        Vector3 vector3_3 = vector3List1[index3];
        Vector3 vector3_4 = vector3_1;
        Vector3 vector3_5 = Vector3.op_Subtraction(Vector3.get_zero(), vector3_1);
        Vector3 normalized1 = ((Vector3) ref vector3_5).get_normalized();
        Vector3 vector3_6 = Vector3.op_Addition(vector3_1, Vector3.op_Multiply(normalized1, num5));
        if ((double) Vector3.Distance(vector3_6, vector3_2) <= (double) num6 && (double) Vector3.Distance(vector3_6, vector3_3) <= (double) num6)
        {
          bool flag2 = true;
          int num4 = 16;
          for (int index5 = 0; index5 < num4; ++index5)
          {
            float num7 = (float) ((double) index5 / (double) num4 * 360.0);
            vector3_5 = new Vector3(Mathf.Sin(num7 * ((float) Math.PI / 180f)), num3, Mathf.Cos(num7 * ((float) Math.PI / 180f)));
            Vector3 normalized2 = ((Vector3) ref vector3_5).get_normalized();
            Vector3 pos = Vector3.op_Addition(vector3_6, Vector3.op_Multiply(normalized2, 1f));
            double waterDepth = (double) BaseBoat.GetWaterDepth(pos);
            Vector3 vector3_7 = normalized1;
            if (Vector3.op_Inequality(pos, Vector3.get_zero()))
            {
              vector3_5 = Vector3.op_Subtraction(pos, vector3_6);
              vector3_7 = ((Vector3) ref vector3_5).get_normalized();
            }
            RaycastHit raycastHit;
            if (Physics.SphereCast(vector3_4, 3f, vector3_7, ref raycastHit, minDistanceFromShore, 1218511105))
            {
              flag2 = false;
              break;
            }
          }
          if (flag2)
          {
            flag1 = true;
            vector3List1[index2] = vector3_6;
          }
        }
      }
    }
    if (flag1)
    {
      Debug.LogWarning((object) "Failed to generate ocean patrol path");
      return (List<Vector3>) null;
    }
    List<int> intList = new List<int>();
    LineUtility.Simplify(vector3List1, 5f, intList);
    List<Vector3> vector3List2 = vector3List1;
    List<Vector3> vector3List3 = new List<Vector3>();
    foreach (int index in intList)
      vector3List3.Add(vector3List2[index]);
    Debug.Log((object) ("Generated ocean patrol path with node count: " + (object) vector3List3.Count));
    return vector3List3;
  }
}
