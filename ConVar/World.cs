// Decompiled with JetBrains decompiler
// Type: ConVar.World
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("world")]
  public class World : ConsoleSystem
  {
    [ServerVar]
    [ClientVar]
    public static bool cache = true;

    [ClientVar]
    public static void monuments(ConsoleSystem.Arg arg)
    {
      if (!Object.op_Implicit((Object) TerrainMeta.Path))
        return;
      TextTable textTable = new TextTable();
      textTable.AddColumn("type");
      textTable.AddColumn("name");
      textTable.AddColumn("pos");
      foreach (MonumentInfo monument in TerrainMeta.Path.Monuments)
        textTable.AddRow(new string[3]
        {
          monument.Type.ToString(),
          ((Object) monument).get_name(),
          ((Component) monument).get_transform().get_position().ToString()
        });
      arg.ReplyWith(((object) textTable).ToString());
    }

    public World()
    {
      base.\u002Ector();
    }
  }
}
