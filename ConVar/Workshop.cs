// Decompiled with JetBrains decompiler
// Type: ConVar.Workshop
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch.Steamworks;
using Rust;

namespace ConVar
{
  [ConsoleSystem.Factory("workshop")]
  public class Workshop : ConsoleSystem
  {
    [ServerVar]
    public static void print_approved_skins(ConsoleSystem.Arg arg)
    {
      if (Global.get_SteamServer() == null || ((BaseSteamworks) Global.get_SteamServer()).get_Inventory().Definitions == null)
        return;
      TextTable textTable = new TextTable();
      textTable.AddColumn("name");
      textTable.AddColumn("itemshortname");
      textTable.AddColumn("workshopid");
      textTable.AddColumn("workshopdownload");
      foreach (Inventory.Definition definition in (Inventory.Definition[]) ((BaseSteamworks) Global.get_SteamServer()).get_Inventory().Definitions)
      {
        string name = definition.get_Name();
        string stringProperty1 = definition.GetStringProperty("itemshortname");
        string stringProperty2 = definition.GetStringProperty("workshopid");
        string stringProperty3 = definition.GetStringProperty("workshopdownload");
        textTable.AddRow(new string[4]
        {
          name,
          stringProperty1,
          stringProperty2,
          stringProperty3
        });
      }
      arg.ReplyWith(((object) textTable).ToString());
    }

    public Workshop()
    {
      base.\u002Ector();
    }
  }
}
