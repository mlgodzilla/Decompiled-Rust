// Decompiled with JetBrains decompiler
// Type: ConVar.Inventory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("inventory")]
  public class Inventory : ConsoleSystem
  {
    [ServerUserVar]
    public static void lighttoggle(ConsoleSystem.Arg arg)
    {
      BasePlayer basePlayer = arg.Player();
      if (!Object.op_Implicit((Object) basePlayer) || basePlayer.IsDead() || basePlayer.IsSleeping())
        return;
      basePlayer.LightToggle();
    }

    [ServerUserVar]
    public static void endloot(ConsoleSystem.Arg arg)
    {
      BasePlayer basePlayer = arg.Player();
      if (!Object.op_Implicit((Object) basePlayer) || basePlayer.IsDead() || basePlayer.IsSleeping())
        return;
      basePlayer.inventory.loot.Clear();
    }

    [ServerVar]
    public static void give(ConsoleSystem.Arg arg)
    {
      BasePlayer basePlayer = arg.Player();
      if (!Object.op_Implicit((Object) basePlayer))
        return;
      Item byPartialName = ItemManager.CreateByPartialName(arg.GetString(0, ""), 1);
      if (byPartialName == null)
      {
        arg.ReplyWith("Invalid Item!");
      }
      else
      {
        int num1 = arg.GetInt(1, 1);
        byPartialName.amount = num1;
        float num2 = arg.GetFloat(2, 1f);
        byPartialName.conditionNormalized = num2;
        byPartialName.OnVirginSpawn();
        if (!basePlayer.inventory.GiveItem(byPartialName, (ItemContainer) null))
        {
          byPartialName.Remove(0.0f);
          arg.ReplyWith("Couldn't give item (inventory full?)");
        }
        else
        {
          basePlayer.Command("note.inv", (object) byPartialName.info.itemid, (object) num1);
          Debug.Log((object) ("giving " + basePlayer.displayName + " " + (object) num1 + " x " + byPartialName.info.displayName.english));
          Chat.Broadcast(basePlayer.displayName + " gave themselves " + (object) num1 + " x " + byPartialName.info.displayName.english, "SERVER", "#eee", 0UL);
        }
      }
    }

    [ServerVar]
    public static void resetbp(ConsoleSystem.Arg arg)
    {
      BasePlayer basePlayer = arg.Player();
      if (!Object.op_Implicit((Object) basePlayer))
        return;
      basePlayer.blueprints.Reset();
    }

    [ServerVar]
    public static void unlockall(ConsoleSystem.Arg arg)
    {
      BasePlayer basePlayer = arg.Player();
      if (!Object.op_Implicit((Object) basePlayer))
        return;
      basePlayer.blueprints.UnlockAll();
    }

    [ServerVar]
    public static void giveall(ConsoleSystem.Arg arg)
    {
      Item obj = (Item) null;
      string str = "SERVER";
      if (Object.op_Inequality((Object) arg.Player(), (Object) null))
        str = arg.Player().displayName;
      foreach (BasePlayer activePlayer in BasePlayer.activePlayerList)
      {
        obj = ItemManager.CreateByPartialName(arg.GetString(0, ""), 1);
        if (obj == null)
        {
          arg.ReplyWith("Invalid Item!");
          return;
        }
        obj.amount = arg.GetInt(1, 1);
        obj.OnVirginSpawn();
        if (!activePlayer.inventory.GiveItem(obj, (ItemContainer) null))
        {
          obj.Remove(0.0f);
          arg.ReplyWith("Couldn't give item (inventory full?)");
        }
        else
        {
          activePlayer.Command("note.inv", (object) obj.info.itemid, (object) obj.amount);
          Debug.Log((object) (" [ServerVar] giving " + activePlayer.displayName + " " + (object) obj.amount + " x " + obj.info.displayName.english));
        }
      }
      if (obj == null)
        return;
      Chat.Broadcast(str + " gave everyone " + (object) obj.amount + " x " + obj.info.displayName.english, "SERVER", "#eee", 0UL);
    }

    [ServerVar]
    public static void giveto(ConsoleSystem.Arg arg)
    {
      string str = "SERVER";
      if (Object.op_Inequality((Object) arg.Player(), (Object) null))
        str = arg.Player().displayName;
      BasePlayer basePlayer = BasePlayer.Find(arg.GetString(0, ""));
      if (Object.op_Equality((Object) basePlayer, (Object) null))
      {
        arg.ReplyWith("Couldn't find player!");
      }
      else
      {
        Item byPartialName = ItemManager.CreateByPartialName(arg.GetString(1, ""), 1);
        if (byPartialName == null)
        {
          arg.ReplyWith("Invalid Item!");
        }
        else
        {
          byPartialName.amount = arg.GetInt(2, 1);
          byPartialName.OnVirginSpawn();
          if (!basePlayer.inventory.GiveItem(byPartialName, (ItemContainer) null))
          {
            byPartialName.Remove(0.0f);
            arg.ReplyWith("Couldn't give item (inventory full?)");
          }
          else
          {
            basePlayer.Command("note.inv", (object) byPartialName.info.itemid, (object) byPartialName.amount);
            Debug.Log((object) (" [ServerVar] giving " + basePlayer.displayName + " " + (object) byPartialName.amount + " x " + byPartialName.info.displayName.english));
            Chat.Broadcast(str + " gave " + basePlayer.displayName + " " + (object) byPartialName.amount + " x " + byPartialName.info.displayName.english, "SERVER", "#eee", 0UL);
          }
        }
      }
    }

    [ServerVar]
    public static void giveid(ConsoleSystem.Arg arg)
    {
      string str = "SERVER";
      if (Object.op_Inequality((Object) arg.Player(), (Object) null))
        str = arg.Player().displayName;
      BasePlayer basePlayer = arg.Player();
      if (!Object.op_Implicit((Object) basePlayer))
        return;
      Item byItemId = ItemManager.CreateByItemID(arg.GetInt(0, 0), 1, 0UL);
      if (byItemId == null)
      {
        arg.ReplyWith("Invalid Item!");
      }
      else
      {
        byItemId.amount = arg.GetInt(1, 1);
        byItemId.OnVirginSpawn();
        if (!basePlayer.inventory.GiveItem(byItemId, (ItemContainer) null))
        {
          byItemId.Remove(0.0f);
          arg.ReplyWith("Couldn't give item (inventory full?)");
        }
        else
        {
          basePlayer.Command("note.inv", (object) byItemId.info.itemid, (object) byItemId.amount);
          Debug.Log((object) (" [ServerVar] giving " + basePlayer.displayName + " " + (object) byItemId.amount + " x " + byItemId.info.displayName.english));
          Chat.Broadcast(str + " gave " + basePlayer.displayName + " " + (object) byItemId.amount + " x " + byItemId.info.displayName.english, "SERVER", "#eee", 0UL);
        }
      }
    }

    [ServerVar]
    public static void givearm(ConsoleSystem.Arg arg)
    {
      BasePlayer basePlayer = arg.Player();
      if (!Object.op_Implicit((Object) basePlayer))
        return;
      Item byItemId = ItemManager.CreateByItemID(arg.GetInt(0, 0), 1, 0UL);
      if (byItemId == null)
      {
        arg.ReplyWith("Invalid Item!");
      }
      else
      {
        byItemId.amount = arg.GetInt(1, 1);
        byItemId.OnVirginSpawn();
        if (!basePlayer.inventory.GiveItem(byItemId, basePlayer.inventory.containerBelt))
        {
          byItemId.Remove(0.0f);
          arg.ReplyWith("Couldn't give item (inventory full?)");
        }
        else
        {
          basePlayer.Command("note.inv", (object) byItemId.info.itemid, (object) byItemId.amount);
          Debug.Log((object) (" [ServerVar] giving " + basePlayer.displayName + " " + (object) byItemId.amount + " x " + byItemId.info.displayName.english));
          Chat.Broadcast(basePlayer.displayName + " gave themselves " + (object) byItemId.amount + " x " + byItemId.info.displayName.english, "SERVER", "#eee", 0UL);
        }
      }
    }

    public Inventory()
    {
      base.\u002Ector();
    }
  }
}
