// Decompiled with JetBrains decompiler
// Type: ServerPerformance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class ServerPerformance : BaseMonoBehaviour
{
  public static ulong deaths;
  public static ulong spawns;
  public static ulong position_changes;
  private string fileName;
  private int lastFrame;

  private void Start()
  {
    if (!Profiler.get_supported() || !CommandLine.HasSwitch("-perf"))
      return;
    this.fileName = "perfdata." + DateTime.Now.ToString() + ".txt";
    this.fileName = this.fileName.Replace('\\', '-');
    this.fileName = this.fileName.Replace('/', '-');
    this.fileName = this.fileName.Replace(' ', '_');
    this.fileName = this.fileName.Replace(':', '.');
    this.lastFrame = Time.get_frameCount();
    File.WriteAllText(this.fileName, "MemMono,MemUnity,Frame,PlayerCount,Sleepers,CollidersDisabled,BehavioursDisabled,GameObjects,Colliders,RigidBodies,BuildingBlocks,nwSend,nwRcv,cnInit,cnApp,cnRej,deaths,spawns,poschange\r\n");
    this.InvokeRepeating(new Action(this.WriteLine), 1f, 60f);
  }

  private void WriteLine()
  {
    Rust.GC.Collect();
    uint monoUsedSize = Profiler.GetMonoUsedSize();
    uint usedHeapSize = Profiler.get_usedHeapSize();
    int count1 = BasePlayer.activePlayerList.Count;
    int count2 = BasePlayer.sleepingPlayerList.Count;
    int length = Object.FindObjectsOfType<GameObject>().Length;
    int num1 = 0;
    int num2 = 0;
    int num3 = 0;
    int num4 = 0;
    int num5 = 0;
    int num6 = Time.get_frameCount() - this.lastFrame;
    File.AppendAllText(this.fileName, ((int) monoUsedSize).ToString() + "," + (object) usedHeapSize + "," + (object) num6 + "," + (object) count1 + "," + (object) count2 + "," + (object) NetworkSleep.totalCollidersDisabled + "," + (object) NetworkSleep.totalBehavioursDisabled + "," + (object) length + "," + (object) Object.FindObjectsOfType<Collider>().Length + "," + (object) Object.FindObjectsOfType<Rigidbody>().Length + "," + (object) Object.FindObjectsOfType<BuildingBlock>().Length + "," + (object) num1 + "," + (object) num2 + "," + (object) num3 + "," + (object) num4 + "," + (object) num5 + "," + (object) ServerPerformance.deaths + "," + (object) ServerPerformance.spawns + "," + (object) ServerPerformance.position_changes + "\r\n");
    this.lastFrame = Time.get_frameCount();
    ServerPerformance.deaths = 0UL;
    ServerPerformance.spawns = 0UL;
    ServerPerformance.position_changes = 0UL;
  }

  public static void DoReport()
  {
    string str = ("report." + DateTime.Now.ToString() + ".txt").Replace('\\', '-').Replace('/', '-').Replace(' ', '_').Replace(':', '.');
    File.WriteAllText(str, "Report Generated " + DateTime.Now.ToString() + "\r\n");
    ServerPerformance.ComponentReport(str, "All Objects", (Object[]) Object.FindObjectsOfType<Transform>());
    ServerPerformance.ComponentReport(str, "Entities", (Object[]) Object.FindObjectsOfType<BaseEntity>());
    ServerPerformance.ComponentReport(str, "Rigidbodies", (Object[]) Object.FindObjectsOfType<Rigidbody>());
    ServerPerformance.ComponentReport(str, "Disabled Colliders", (Object[]) ((IEnumerable<Collider>) Object.FindObjectsOfType<Collider>()).Where<Collider>((Func<Collider, bool>) (x => !x.get_enabled())).ToArray<Collider>());
    ServerPerformance.ComponentReport(str, "Enabled Colliders", (Object[]) ((IEnumerable<Collider>) Object.FindObjectsOfType<Collider>()).Where<Collider>((Func<Collider, bool>) (x => x.get_enabled())).ToArray<Collider>());
    if (!Object.op_Implicit((Object) SingletonComponent<SpawnHandler>.Instance))
      return;
    ((SpawnHandler) SingletonComponent<SpawnHandler>.Instance).DumpReport(str);
  }

  public static string WorkoutPrefabName(GameObject obj)
  {
    if (Object.op_Equality((Object) obj, (Object) null))
      return "null";
    string str = obj.get_activeSelf() ? "" : " (inactive)";
    BaseEntity baseEntity = obj.ToBaseEntity();
    if (Object.op_Implicit((Object) baseEntity))
      return baseEntity.PrefabName + str;
    return ((Object) obj).get_name() + str;
  }

  public static void ComponentReport(string filename, string Title, Object[] objects)
  {
    File.AppendAllText(filename, "\r\n\r\n" + Title + ":\r\n\r\n");
    using (IEnumerator<IGrouping<string, Object>> enumerator = ((IEnumerable<IGrouping<string, Object>>) ((IEnumerable<Object>) objects).GroupBy<Object, string>((Func<Object, string>) (x => ServerPerformance.WorkoutPrefabName((x as Component).get_gameObject()))).OrderByDescending<IGrouping<string, Object>, int>((Func<IGrouping<string, Object>, int>) (x => ((IEnumerable<Object>) x).Count<Object>()))).GetEnumerator())
    {
      while (((IEnumerator) enumerator).MoveNext())
      {
        IGrouping<string, Object> current = enumerator.Current;
        File.AppendAllText(filename, "\t" + ServerPerformance.WorkoutPrefabName((((IEnumerable<Object>) current).ElementAt<Object>(0) as Component).get_gameObject()) + " - " + (object) ((IEnumerable<Object>) current).Count<Object>() + "\r\n");
      }
    }
    File.AppendAllText(filename, "\r\nTotal: " + (object) ((IEnumerable<Object>) objects).Count<Object>() + "\r\n\r\n\r\n");
  }
}
