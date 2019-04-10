// Decompiled with JetBrains decompiler
// Type: ConVar.Batching
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Text;
using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("batching")]
  public class Batching : ConsoleSystem
  {
    [ClientVar]
    [ServerVar]
    public static bool colliders = false;
    [ServerVar]
    [ClientVar]
    public static bool collider_threading = true;
    [ServerVar]
    [ClientVar]
    public static int collider_capacity = 30000;
    [ServerVar]
    [ClientVar]
    public static int collider_vertices = 1000;
    [ClientVar]
    [ServerVar]
    public static int collider_submeshes = 1;
    [ClientVar]
    public static bool renderers = true;
    [ClientVar]
    public static bool renderer_threading = true;
    [ClientVar]
    public static int renderer_capacity = 30000;
    [ClientVar]
    public static int renderer_vertices = 1000;
    [ClientVar]
    public static int renderer_submeshes = 1;
    [ServerVar]
    [ClientVar]
    public static int verbose = 0;

    [ServerVar]
    public static void refresh_colliders(ConsoleSystem.Arg args)
    {
      foreach (ColliderBatch colliderBatch in (ColliderBatch[]) Object.FindObjectsOfType<ColliderBatch>())
        colliderBatch.Refresh();
      if (!Object.op_Implicit((Object) SingletonComponent<ColliderGrid>.Instance))
        return;
      ((ColliderGrid) SingletonComponent<ColliderGrid>.Instance).Refresh();
    }

    [ServerVar]
    public static void print_colliders(ConsoleSystem.Arg args)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (Object.op_Implicit((Object) SingletonComponent<ColliderGrid>.Instance))
      {
        stringBuilder.AppendFormat("Mesh Collider Batching: {0:N0}/{0:N0}", (object) ((ColliderGrid) SingletonComponent<ColliderGrid>.Instance).BatchedMeshCount(), (object) ((ColliderGrid) SingletonComponent<ColliderGrid>.Instance).MeshCount());
        stringBuilder.AppendLine();
      }
      args.ReplyWith(stringBuilder.ToString());
    }

    public Batching()
    {
      base.\u002Ector();
    }
  }
}
