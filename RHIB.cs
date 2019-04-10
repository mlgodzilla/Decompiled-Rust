// Decompiled with JetBrains decompiler
// Type: RHIB
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class RHIB : MotorRowboat
{
  [ServerVar(Help = "Population active on the server")]
  public static float rhibpopulation = 1f;
  public GameObject steeringWheel;
  private float targetGasPedal;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("RHIB.OnRpcMessage", 0.1f))
    {
      if (rpc == 1382282393U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - Server_Release "));
          using (TimeWarning.New("Server_Release", 0.1f))
          {
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.Server_Release(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in Server_Release");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  [BaseEntity.RPC_Server]
  public void Server_Release(BaseEntity.RPCMessage msg)
  {
    Debug.Log((object) "Rhib server release!");
    if (Object.op_Equality((Object) this.GetParentEntity(), (Object) null))
      return;
    this.SetParent((BaseEntity) null, true, true);
    this.myRigidBody.set_isKinematic(false);
  }

  public override void VehicleFixedUpdate()
  {
    this.gasPedal = Mathf.MoveTowards(this.gasPedal, this.targetGasPedal, Time.get_fixedDeltaTime() * 1f);
    base.VehicleFixedUpdate();
  }

  public override bool EngineOn()
  {
    return base.EngineOn();
  }

  public override bool HasFuel(bool forceCheck = false)
  {
    return base.HasFuel(forceCheck);
  }

  public override bool UseFuel(float seconds)
  {
    return base.UseFuel(seconds);
  }

  public override void DriverInput(InputState inputState, BasePlayer player)
  {
    base.DriverInput(inputState, player);
    this.targetGasPedal = !inputState.IsDown(BUTTON.FORWARD) ? (!inputState.IsDown(BUTTON.BACKWARD) ? 0.0f : -0.5f) : 1f;
    if (inputState.IsDown(BUTTON.LEFT))
      this.steering = 1f;
    else if (inputState.IsDown(BUTTON.RIGHT))
      this.steering = -1f;
    else
      this.steering = 0.0f;
  }
}
