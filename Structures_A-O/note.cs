// Decompiled with JetBrains decompiler
// Type: note
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch.Extend;
using UnityEngine;

[ConsoleSystem.Factory("note")]
public class note : ConsoleSystem
{
  [ServerUserVar]
  public static void update(ConsoleSystem.Arg arg)
  {
    uint id = arg.GetUInt(0, 0U);
    string str = arg.GetString(1, "");
    Item itemUid = arg.Player().inventory.FindItemUID(id);
    if (itemUid == null)
      return;
    itemUid.text = StringExtensions.Truncate(str, 1024, (string) null);
    itemUid.MarkDirty();
  }

  public note()
  {
    base.\u002Ector();
  }
}
