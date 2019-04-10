// Decompiled with JetBrains decompiler
// Type: ConVar.Supply
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("supply")]
  public class Supply : ConsoleSystem
  {
    private const string path = "assets/prefabs/npc/cargo plane/cargo_plane.prefab";

    [ServerVar]
    public static void drop(ConsoleSystem.Arg arg)
    {
      BasePlayer basePlayer = arg.Player();
      if (!Object.op_Implicit((Object) basePlayer))
        return;
      Debug.Log((object) "Supply Drop Inbound");
      BaseEntity entity = GameManager.server.CreateEntity("assets/prefabs/npc/cargo plane/cargo_plane.prefab", (Vector3) null, (Quaternion) null, true);
      if (!Object.op_Implicit((Object) entity))
        return;
      ((CargoPlane) ((Component) entity).GetComponent<CargoPlane>()).InitDropPosition(Vector3.op_Addition(((Component) basePlayer).get_transform().get_position(), new Vector3(0.0f, 10f, 0.0f)));
      entity.Spawn();
    }

    [ServerVar]
    public static void call(ConsoleSystem.Arg arg)
    {
      if (!Object.op_Implicit((Object) arg.Player()))
        return;
      Debug.Log((object) "Supply Drop Inbound");
      BaseEntity entity = GameManager.server.CreateEntity("assets/prefabs/npc/cargo plane/cargo_plane.prefab", (Vector3) null, (Quaternion) null, true);
      if (!Object.op_Implicit((Object) entity))
        return;
      entity.Spawn();
    }

    public Supply()
    {
      base.\u002Ector();
    }
  }
}
