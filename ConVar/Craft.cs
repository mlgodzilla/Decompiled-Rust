// Decompiled with JetBrains decompiler
// Type: ConVar.Craft
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ProtoBuf;
using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("craft")]
  public class Craft : ConsoleSystem
  {
    [ServerVar]
    public static bool instant;

    [ServerUserVar]
    public static void add(ConsoleSystem.Arg args)
    {
      BasePlayer owner = args.Player();
      if (!Object.op_Implicit((Object) owner) || owner.IsDead())
        return;
      int num1 = args.GetInt(0, 0);
      int amount = args.GetInt(1, 1);
      int num2 = (int) args.GetUInt64(2, 0UL);
      if (amount < 1)
        return;
      ItemDefinition itemDefinition = ItemManager.FindItemDefinition(num1);
      if (Object.op_Equality((Object) itemDefinition, (Object) null))
      {
        args.ReplyWith("Item not found");
      }
      else
      {
        ItemBlueprint blueprint = ItemManager.FindBlueprint(itemDefinition);
        if (!Object.op_Implicit((Object) blueprint))
          args.ReplyWith("Blueprint not found");
        else if (!blueprint.userCraftable)
        {
          args.ReplyWith("Item is not craftable");
        }
        else
        {
          if (!owner.blueprints.CanCraft(num1, num2))
          {
            num2 = 0;
            if (!owner.blueprints.CanCraft(num1, num2))
            {
              args.ReplyWith("You can't craft this item");
              return;
            }
            args.ReplyWith("You don't have permission to use this skin, so crafting unskinned");
          }
          if (owner.inventory.crafting.CraftItem(blueprint, owner, (Item.InstanceData) null, amount, num2, (Item) null))
            return;
          args.ReplyWith("Couldn't craft!");
        }
      }
    }

    [ServerUserVar]
    public static void canceltask(ConsoleSystem.Arg args)
    {
      BasePlayer basePlayer = args.Player();
      if (!Object.op_Implicit((Object) basePlayer) || basePlayer.IsDead())
        return;
      int iID = args.GetInt(0, 0);
      if (basePlayer.inventory.crafting.CancelTask(iID, true))
        return;
      args.ReplyWith("Couldn't cancel task!");
    }

    [ServerUserVar]
    public static void cancel(ConsoleSystem.Arg args)
    {
      BasePlayer basePlayer = args.Player();
      if (!Object.op_Implicit((Object) basePlayer) || basePlayer.IsDead())
        return;
      int itemid = args.GetInt(0, 0);
      basePlayer.inventory.crafting.CancelBlueprint(itemid);
    }

    public Craft()
    {
      base.\u002Ector();
    }
  }
}
