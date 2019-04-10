// Decompiled with JetBrains decompiler
// Type: ConVar.PatrolHelicopter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("heli")]
  public class PatrolHelicopter : ConsoleSystem
  {
    [ServerVar]
    public static float lifetimeMinutes = 15f;
    [ServerVar]
    public static int guns = 1;
    [ServerVar]
    public static float bulletDamageScale = 1f;
    [ServerVar]
    public static float bulletAccuracy = 2f;
    private const string path = "assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab";

    [ServerVar]
    public static void drop(ConsoleSystem.Arg arg)
    {
      BasePlayer basePlayer = arg.Player();
      if (!Object.op_Implicit((Object) basePlayer))
        return;
      Debug.Log((object) ("heli called to : " + (object) ((Component) basePlayer).get_transform().get_position()));
      BaseEntity entity = GameManager.server.CreateEntity("assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab", (Vector3) null, (Quaternion) null, true);
      if (!Object.op_Implicit((Object) entity))
        return;
      ((PatrolHelicopterAI) ((Component) entity).GetComponent<PatrolHelicopterAI>()).SetInitialDestination(Vector3.op_Addition(((Component) basePlayer).get_transform().get_position(), new Vector3(0.0f, 10f, 0.0f)), 0.0f);
      entity.Spawn();
    }

    [ServerVar]
    public static void calltome(ConsoleSystem.Arg arg)
    {
      BasePlayer basePlayer = arg.Player();
      if (!Object.op_Implicit((Object) basePlayer))
        return;
      Debug.Log((object) ("heli called to : " + (object) ((Component) basePlayer).get_transform().get_position()));
      BaseEntity entity = GameManager.server.CreateEntity("assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab", (Vector3) null, (Quaternion) null, true);
      if (!Object.op_Implicit((Object) entity))
        return;
      ((PatrolHelicopterAI) ((Component) entity).GetComponent<PatrolHelicopterAI>()).SetInitialDestination(Vector3.op_Addition(((Component) basePlayer).get_transform().get_position(), new Vector3(0.0f, 10f, 0.0f)), 0.25f);
      entity.Spawn();
    }

    [ServerVar]
    public static void call(ConsoleSystem.Arg arg)
    {
      if (!Object.op_Implicit((Object) arg.Player()))
        return;
      Debug.Log((object) "Helicopter inbound");
      BaseEntity entity = GameManager.server.CreateEntity("assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab", (Vector3) null, (Quaternion) null, true);
      if (!Object.op_Implicit((Object) entity))
        return;
      entity.Spawn();
    }

    [ServerVar]
    public static void strafe(ConsoleSystem.Arg arg)
    {
      BasePlayer basePlayer = arg.Player();
      if (!Object.op_Implicit((Object) basePlayer))
        return;
      PatrolHelicopterAI heliInstance = PatrolHelicopterAI.heliInstance;
      if (Object.op_Equality((Object) heliInstance, (Object) null))
      {
        Debug.Log((object) "no heli instance");
      }
      else
      {
        RaycastHit raycastHit;
        if (Physics.Raycast(basePlayer.eyes.HeadRay(), ref raycastHit, 1000f, 1218652417))
        {
          Debug.Log((object) ("strafing :" + (object) ((RaycastHit) ref raycastHit).get_point()));
          heliInstance.interestZoneOrigin = ((RaycastHit) ref raycastHit).get_point();
          heliInstance.ExitCurrentState();
          heliInstance.State_Strafe_Enter(((RaycastHit) ref raycastHit).get_point(), false);
        }
        else
          Debug.Log((object) "strafe ray missed");
      }
    }

    [ServerVar]
    public static void testpuzzle(ConsoleSystem.Arg arg)
    {
      BasePlayer basePlayer = arg.Player();
      if (!Object.op_Implicit((Object) basePlayer))
        return;
      int num = basePlayer.IsDeveloper ? 1 : 0;
    }

    public PatrolHelicopter()
    {
      base.\u002Ector();
    }
  }
}
