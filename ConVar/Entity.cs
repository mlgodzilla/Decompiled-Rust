// Decompiled with JetBrains decompiler
// Type: ConVar.Entity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network.Visibility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("entity")]
  public class Entity : ConsoleSystem
  {
    private static TextTable GetEntityTable(Func<Entity.EntityInfo, bool> filter)
    {
      TextTable textTable1 = new TextTable();
      textTable1.AddColumn("realm");
      textTable1.AddColumn("entity");
      textTable1.AddColumn("group");
      textTable1.AddColumn("parent");
      textTable1.AddColumn("name");
      textTable1.AddColumn("position");
      textTable1.AddColumn("local");
      textTable1.AddColumn("rotation");
      textTable1.AddColumn("local");
      textTable1.AddColumn("status");
      textTable1.AddColumn("invokes");
      foreach (BaseNetworkable serverEntity in BaseNetworkable.serverEntities)
      {
        if (!Object.op_Equality((Object) serverEntity, (Object) null))
        {
          Entity.EntityInfo entityInfo = new Entity.EntityInfo(serverEntity);
          if (filter(entityInfo))
          {
            TextTable textTable2 = textTable1;
            string[] strArray = new string[11];
            strArray[0] = "sv";
            strArray[1] = entityInfo.entityID.ToString();
            strArray[2] = entityInfo.groupID.ToString();
            strArray[3] = entityInfo.parentID.ToString();
            strArray[4] = entityInfo.entity.ShortPrefabName;
            Vector3 vector3 = ((Component) entityInfo.entity).get_transform().get_position();
            strArray[5] = vector3.ToString();
            vector3 = ((Component) entityInfo.entity).get_transform().get_localPosition();
            strArray[6] = vector3.ToString();
            Quaternion rotation = ((Component) entityInfo.entity).get_transform().get_rotation();
            vector3 = ((Quaternion) ref rotation).get_eulerAngles();
            strArray[7] = vector3.ToString();
            Quaternion localRotation = ((Component) entityInfo.entity).get_transform().get_localRotation();
            vector3 = ((Quaternion) ref localRotation).get_eulerAngles();
            strArray[8] = vector3.ToString();
            strArray[9] = entityInfo.status;
            strArray[10] = entityInfo.entity.InvokeString();
            textTable2.AddRow(strArray);
          }
        }
      }
      return textTable1;
    }

    [ClientVar]
    [ServerVar]
    public static void find_entity(ConsoleSystem.Arg args)
    {
      string filter = args.GetString(0, "");
      TextTable entityTable = Entity.GetEntityTable((Func<Entity.EntityInfo, bool>) (info =>
      {
        if (!string.IsNullOrEmpty(filter))
          return info.entity.PrefabName.Contains(filter);
        return true;
      }));
      args.ReplyWith(((object) entityTable).ToString());
    }

    [ClientVar]
    [ServerVar]
    public static void find_id(ConsoleSystem.Arg args)
    {
      uint filter = args.GetUInt(0, 0U);
      TextTable entityTable = Entity.GetEntityTable((Func<Entity.EntityInfo, bool>) (info => (int) info.entityID == (int) filter));
      args.ReplyWith(((object) entityTable).ToString());
    }

    [ClientVar]
    [ServerVar]
    public static void find_group(ConsoleSystem.Arg args)
    {
      uint filter = args.GetUInt(0, 0U);
      TextTable entityTable = Entity.GetEntityTable((Func<Entity.EntityInfo, bool>) (info => (int) info.groupID == (int) filter));
      args.ReplyWith(((object) entityTable).ToString());
    }

    [ServerVar]
    [ClientVar]
    public static void find_parent(ConsoleSystem.Arg args)
    {
      uint filter = args.GetUInt(0, 0U);
      TextTable entityTable = Entity.GetEntityTable((Func<Entity.EntityInfo, bool>) (info => (int) info.parentID == (int) filter));
      args.ReplyWith(((object) entityTable).ToString());
    }

    [ClientVar]
    [ServerVar]
    public static void find_status(ConsoleSystem.Arg args)
    {
      string filter = args.GetString(0, "");
      TextTable entityTable = Entity.GetEntityTable((Func<Entity.EntityInfo, bool>) (info =>
      {
        if (!string.IsNullOrEmpty(filter))
          return info.status.Contains(filter);
        return true;
      }));
      args.ReplyWith(((object) entityTable).ToString());
    }

    [ServerVar]
    [ClientVar]
    public static void find_radius(ConsoleSystem.Arg args)
    {
      BasePlayer player = args.Player();
      if (Object.op_Equality((Object) player, (Object) null))
        return;
      uint filter = args.GetUInt(0, 10U);
      TextTable entityTable = Entity.GetEntityTable((Func<Entity.EntityInfo, bool>) (info => (double) Vector3.Distance(((Component) info.entity).get_transform().get_position(), ((Component) player).get_transform().get_position()) <= (double) filter));
      args.ReplyWith(((object) entityTable).ToString());
    }

    [ClientVar]
    [ServerVar]
    public static void find_self(ConsoleSystem.Arg args)
    {
      BasePlayer basePlayer = args.Player();
      if (Object.op_Equality((Object) basePlayer, (Object) null) || basePlayer.net == null)
        return;
      uint filter = (uint) basePlayer.net.ID;
      TextTable entityTable = Entity.GetEntityTable((Func<Entity.EntityInfo, bool>) (info => (int) info.entityID == (int) filter));
      args.ReplyWith(((object) entityTable).ToString());
    }

    [ServerVar]
    public static void debug_toggle(ConsoleSystem.Arg args)
    {
      int num = args.GetInt(0, 0);
      if (num == 0)
        return;
      BaseEntity baseEntity = BaseNetworkable.serverEntities.Find((uint) num) as BaseEntity;
      if (Object.op_Equality((Object) baseEntity, (Object) null))
        return;
      baseEntity.SetFlag(BaseEntity.Flags.Debugging, !baseEntity.IsDebugging(), false, true);
      if (baseEntity.IsDebugging())
        baseEntity.OnDebugStart();
      args.ReplyWith("Debugging for " + (object) (uint) baseEntity.net.ID + " " + (baseEntity.IsDebugging() ? (object) "enabled" : (object) "disabled"));
    }

    [ServerVar]
    public static void nudge(int entID)
    {
      if (entID == 0)
        return;
      BaseEntity baseEntity = BaseNetworkable.serverEntities.Find((uint) entID) as BaseEntity;
      if (Object.op_Equality((Object) baseEntity, (Object) null))
        return;
      ((Component) baseEntity).BroadcastMessage("DebugNudge", (SendMessageOptions) 1);
    }

    [ServerVar(Name = "spawn")]
    public static string svspawn(string name, Vector3 pos)
    {
      if (string.IsNullOrEmpty(name))
        return "No entity name provided";
      string[] array = ((IEnumerable<string>) GameManifest.Current.entities).Where<string>((Func<string, bool>) (x => StringEx.Contains(Path.GetFileNameWithoutExtension(x), name, CompareOptions.IgnoreCase))).Select<string, string>((Func<string, string>) (x => x.ToLower())).ToArray<string>();
      if (array.Length == 0)
        return "Entity type not found";
      if (array.Length > 1)
      {
        string str = ((IEnumerable<string>) array).FirstOrDefault<string>((Func<string, bool>) (x => string.Compare(Path.GetFileNameWithoutExtension(x), name, StringComparison.OrdinalIgnoreCase) == 0));
        if (str == null)
          return "Unknown entity - could be:\n\n" + string.Join("\n", ((IEnumerable<string>) array).Select<string, string>(new Func<string, string>(Path.GetFileNameWithoutExtension)).ToArray<string>());
        array[0] = str;
      }
      BaseEntity entity = GameManager.server.CreateEntity(array[0], pos, (Quaternion) null, true);
      if (Object.op_Equality((Object) entity, (Object) null))
        return "Couldn't spawn " + name;
      entity.Spawn();
      return "spawned " + (object) entity + " at " + (object) pos;
    }

    [ServerVar(Name = "spawnitem")]
    public static string svspawnitem(string name, Vector3 pos)
    {
      if (string.IsNullOrEmpty(name))
        return "No entity name provided";
      string[] array = ItemManager.itemList.Select<ItemDefinition, string>((Func<ItemDefinition, string>) (x => x.shortname)).Where<string>((Func<string, bool>) (x => StringEx.Contains(x, name, CompareOptions.IgnoreCase))).ToArray<string>();
      if (array.Length == 0)
        return "Entity type not found";
      if (array.Length > 1)
      {
        string str = ((IEnumerable<string>) array).FirstOrDefault<string>((Func<string, bool>) (x => string.Compare(x, name, StringComparison.OrdinalIgnoreCase) == 0));
        if (str == null)
          return "Unknown entity - could be:\n\n" + string.Join("\n", array);
        array[0] = str;
      }
      Item byName = ItemManager.CreateByName(array[0], 1, 0UL);
      if (byName == null)
        return "Couldn't spawn " + name;
      byName.CreateWorldObject(pos, (Quaternion) null, (BaseEntity) null, 0U);
      return "spawned " + (object) byName + " at " + (object) pos;
    }

    [ServerVar]
    public static void spawnlootfrom(ConsoleSystem.Arg args)
    {
      BasePlayer basePlayer = args.Player();
      string strPrefab = args.GetString(0, string.Empty);
      int num = args.GetInt(1, 1);
      Vector3 vector3 = args.GetVector3(1, Object.op_Implicit((Object) basePlayer) ? basePlayer.CenterPoint() : Vector3.get_zero());
      if (string.IsNullOrEmpty(strPrefab))
        return;
      BaseEntity entity = GameManager.server.CreateEntity(strPrefab, vector3, (Quaternion) null, true);
      if (Object.op_Equality((Object) entity, (Object) null))
        return;
      entity.Spawn();
      basePlayer.ChatMessage("Contents of " + strPrefab + " spawned " + (object) num + " times");
      LootContainer component = (LootContainer) ((Component) entity).GetComponent<LootContainer>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        for (int index = 0; index < num * component.maxDefinitionsToSpawn; ++index)
          component.lootDefinition.SpawnIntoContainer(basePlayer.inventory.containerMain);
      }
      entity.Kill(BaseNetworkable.DestroyMode.None);
    }

    [ServerVar(Help = "Destroy all entities created by this user")]
    public static int DeleteBy(ulong SteamId)
    {
      if (SteamId == 0UL)
        return 0;
      int num = 0;
      foreach (BaseEntity serverEntity in BaseNetworkable.serverEntities)
      {
        if (!Object.op_Equality((Object) serverEntity, (Object) null) && (long) serverEntity.OwnerID == (long) SteamId)
        {
          serverEntity.Invoke(new Action(((BaseNetworkable) serverEntity).KillMessage), (float) num * 0.2f);
          ++num;
        }
      }
      return num;
    }

    public Entity()
    {
      base.\u002Ector();
    }

    private struct EntityInfo
    {
      public BaseNetworkable entity;
      public uint entityID;
      public uint groupID;
      public uint parentID;
      public string status;

      public EntityInfo(BaseNetworkable src)
      {
        this.entity = src;
        BaseEntity entity = this.entity as BaseEntity;
        BaseEntity baseEntity = Object.op_Inequality((Object) entity, (Object) null) ? entity.GetParentEntity() : (BaseEntity) null;
        this.entityID = !Object.op_Inequality((Object) this.entity, (Object) null) || this.entity.net == null ? 0U : (uint) (int) this.entity.net.ID;
        this.groupID = !Object.op_Inequality((Object) this.entity, (Object) null) || this.entity.net == null || this.entity.net.group == null ? 0U : (uint) (int) ((Group) this.entity.net.group).ID;
        this.parentID = Object.op_Inequality((Object) entity, (Object) null) ? entity.parentEntity.uid : 0U;
        if (Object.op_Inequality((Object) entity, (Object) null) && entity.parentEntity.uid != 0U)
        {
          if (Object.op_Equality((Object) baseEntity, (Object) null))
            this.status = "orphan";
          else
            this.status = "child";
        }
        else
          this.status = string.Empty;
      }
    }
  }
}
