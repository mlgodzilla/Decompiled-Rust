// Decompiled with JetBrains decompiler
// Type: ConVar.Debugging
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Facepunch.Unity;
using Rust;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("debug")]
  public class Debugging : ConsoleSystem
  {
    [ClientVar]
    [ServerVar]
    public static bool checktriggers;
    [ServerVar(Help = "Do not damage any items")]
    public static bool disablecondition;
    [ClientVar]
    [ServerVar]
    public static bool callbacks;

    [ServerVar]
    [ClientVar]
    public static void renderinfo(ConsoleSystem.Arg arg)
    {
      RenderInfo.GenerateReport();
    }

    [ClientVar]
    [ServerVar]
    public static bool log
    {
      set
      {
        Debug.get_unityLogger().set_logEnabled(value);
      }
      get
      {
        return Debug.get_unityLogger().get_logEnabled();
      }
    }

    [ClientVar]
    [ServerVar]
    public static void stall(ConsoleSystem.Arg arg)
    {
      float num = Mathf.Clamp(arg.GetFloat(0, 0.0f), 0.0f, 1f);
      arg.ReplyWith("Stalling for " + (object) num + " seconds...");
      Thread.Sleep(Mathf.RoundToInt(num * 1000f));
    }

    [ServerVar(Help = "Takes you in and out of your current network group, causing you to delete and then download all entities in your PVS again")]
    public static void flushgroup(ConsoleSystem.Arg arg)
    {
      BasePlayer basePlayer = arg.Player();
      if (Object.op_Equality((Object) basePlayer, (Object) null))
        return;
      basePlayer.net.SwitchGroup(BaseNetworkable.LimboNetworkGroup);
      basePlayer.UpdateNetworkGroup();
    }

    [ServerVar(Help = "Break the current held object")]
    public static void breakheld(ConsoleSystem.Arg arg)
    {
      Item activeItem = arg.Player().GetActiveItem();
      activeItem?.LoseCondition(activeItem.condition * 2f);
    }

    [ServerVar(Help = "reset all puzzles")]
    public static void puzzlereset(ConsoleSystem.Arg arg)
    {
      if (Object.op_Equality((Object) arg.Player(), (Object) null))
        return;
      M0[] objectsOfType = Object.FindObjectsOfType<PuzzleReset>();
      Debug.Log((object) "iterating...");
      foreach (PuzzleReset puzzleReset in (PuzzleReset[]) objectsOfType)
      {
        Debug.Log((object) ("resetting puzzle at :" + (object) ((Component) puzzleReset).get_transform().get_position()));
        puzzleReset.DoReset();
        puzzleReset.ResetTimer();
      }
    }

    [ServerVar(Help = "respawn all puzzles from their prefabs")]
    public static void puzzleprefabrespawn(ConsoleSystem.Arg arg)
    {
      foreach (BaseNetworkable baseNetworkable in BaseNetworkable.serverEntities.Where<BaseNetworkable>((Func<BaseNetworkable, bool>) (x =>
      {
        if (x is IOEntity)
          return (PrefabAttribute) PrefabAttribute.server.Find<Construction>(x.prefabID) == (PrefabAttribute) null;
        return false;
      })).ToList<BaseNetworkable>())
        baseNetworkable.Kill(BaseNetworkable.DestroyMode.None);
      foreach (MonumentInfo monument in TerrainMeta.Path.Monuments)
      {
        GameObject prefab = GameManager.server.FindPrefab(((Object) ((Component) monument).get_gameObject()).get_name());
        if (!Object.op_Equality((Object) prefab, (Object) null))
        {
          Dictionary<IOEntity, IOEntity> dictionary = new Dictionary<IOEntity, IOEntity>();
          foreach (IOEntity componentsInChild in (IOEntity[]) prefab.GetComponentsInChildren<IOEntity>(true))
          {
            Quaternion rot = Quaternion.op_Multiply(((Component) monument).get_transform().get_rotation(), ((Component) componentsInChild).get_transform().get_rotation());
            Vector3 pos = ((Component) monument).get_transform().TransformPoint(((Component) componentsInChild).get_transform().get_position());
            BaseEntity newEntity = GameManager.server.CreateEntity(componentsInChild.PrefabName, pos, rot, true);
            IOEntity ioEntity = newEntity as IOEntity;
            if (Object.op_Inequality((Object) ioEntity, (Object) null))
            {
              dictionary.Add(componentsInChild, ioEntity);
              DoorManipulator doorManipulator = newEntity as DoorManipulator;
              if (Object.op_Inequality((Object) doorManipulator, (Object) null))
              {
                List<Door> list = (List<Door>) Pool.GetList<Door>();
                global::Vis.Entities<Door>(((Component) newEntity).get_transform().get_position(), 10f, list, -1, (QueryTriggerInteraction) 2);
                Door door = list.OrderBy<Door, float>((Func<Door, float>) (x => x.Distance(((Component) newEntity).get_transform().get_position()))).FirstOrDefault<Door>();
                if (Object.op_Inequality((Object) door, (Object) null))
                  doorManipulator.targetDoor = door;
                // ISSUE: cast to a reference type
                Pool.FreeList<Door>((List<M0>&) ref list);
              }
              CardReader cardReader1 = newEntity as CardReader;
              if (Object.op_Inequality((Object) cardReader1, (Object) null))
              {
                CardReader cardReader2 = componentsInChild as CardReader;
                if (Object.op_Inequality((Object) cardReader2, (Object) null))
                {
                  cardReader1.accessLevel = cardReader2.accessLevel;
                  cardReader1.accessDuration = cardReader2.accessDuration;
                }
              }
              TimerSwitch timerSwitch1 = newEntity as TimerSwitch;
              if (Object.op_Inequality((Object) timerSwitch1, (Object) null))
              {
                TimerSwitch timerSwitch2 = componentsInChild as TimerSwitch;
                if (Object.op_Inequality((Object) timerSwitch2, (Object) null))
                  timerSwitch1.timerLength = timerSwitch2.timerLength;
              }
            }
          }
          foreach (KeyValuePair<IOEntity, IOEntity> keyValuePair in dictionary)
          {
            IOEntity key = keyValuePair.Key;
            IOEntity ioEntity = keyValuePair.Value;
            for (int index = 0; index < key.outputs.Length; ++index)
            {
              if (!Object.op_Equality((Object) key.outputs[index].connectedTo.ioEnt, (Object) null))
              {
                ioEntity.outputs[index].connectedTo.ioEnt = dictionary[key.outputs[index].connectedTo.ioEnt];
                ioEntity.outputs[index].connectedToSlot = key.outputs[index].connectedToSlot;
              }
            }
          }
          foreach (BaseNetworkable baseNetworkable in dictionary.Values)
            baseNetworkable.Spawn();
        }
      }
    }

    [ServerVar(Help = "Break all the items in your inventory whose name match the passed string")]
    public static void breakitem(ConsoleSystem.Arg arg)
    {
      string str = arg.GetString(0, "");
      foreach (Item obj in arg.Player().inventory.containerMain.itemList)
      {
        if (StringEx.Contains(obj.info.shortname, str, CompareOptions.IgnoreCase) && obj.hasCondition)
          obj.LoseCondition(obj.condition * 2f);
      }
    }

    [ServerVar]
    public static void hurt(ConsoleSystem.Arg arg)
    {
      BasePlayer basePlayer = arg.Player();
      HitInfo info = new HitInfo((BaseEntity) basePlayer, (BaseEntity) basePlayer, DamageType.Bullet, (float) arg.GetInt(0, 1));
      string str = arg.GetString(1, string.Empty);
      if (!string.IsNullOrEmpty(str))
        info.HitBone = StringPool.Get(str);
      basePlayer.OnAttacked(info);
    }

    [ServerVar]
    public static void eat(ConsoleSystem.Arg arg)
    {
      arg.Player().metabolism.ApplyChange(MetabolismAttribute.Type.Calories, (float) arg.GetInt(0, 1), (float) arg.GetInt(1, 1));
    }

    [ServerVar]
    public static void drink(ConsoleSystem.Arg arg)
    {
      arg.Player().metabolism.ApplyChange(MetabolismAttribute.Type.Hydration, (float) arg.GetInt(0, 1), (float) arg.GetInt(1, 1));
    }

    public Debugging()
    {
      base.\u002Ector();
    }
  }
}
