// Decompiled with JetBrains decompiler
// Type: ConVar.XMas
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("xmas")]
  public class XMas : ConsoleSystem
  {
    [ServerVar]
    public static bool enabled = false;
    [ServerVar]
    public static float spawnRange = 40f;
    [ServerVar]
    public static int spawnAttempts = 5;
    [ServerVar]
    public static int giftsPerPlayer = 2;
    private const string path = "assets/prefabs/misc/xmas/xmasrefill.prefab";

    [ServerVar]
    public static void refill(ConsoleSystem.Arg arg)
    {
      BaseEntity entity = GameManager.server.CreateEntity("assets/prefabs/misc/xmas/xmasrefill.prefab", (Vector3) null, (Quaternion) null, true);
      if (!Object.op_Implicit((Object) entity))
        return;
      entity.Spawn();
    }

    public XMas()
    {
      base.\u002Ector();
    }
  }
}
