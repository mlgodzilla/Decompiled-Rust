// Decompiled with JetBrains decompiler
// Type: DiagnosticsConSys
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

[ConsoleSystem.Factory("global")]
public class DiagnosticsConSys : ConsoleSystem
{
  private static void DumpAnimators(string targetFolder)
  {
    Animator[] objectsOfType = (Animator[]) Object.FindObjectsOfType<Animator>();
    StringBuilder stringBuilder1 = new StringBuilder();
    stringBuilder1.AppendLine("All animators");
    stringBuilder1.AppendLine();
    foreach (Animator animator in objectsOfType)
    {
      stringBuilder1.AppendFormat("{1}\t{0}", (object) ((Component) animator).get_transform().GetRecursiveName(""), (object) ((Behaviour) animator).get_enabled());
      stringBuilder1.AppendLine();
    }
    DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Animators.List.txt", stringBuilder1.ToString());
    StringBuilder stringBuilder2 = new StringBuilder();
    stringBuilder2.AppendLine("All animators - grouped by object name");
    stringBuilder2.AppendLine();
    using (IEnumerator<IGrouping<string, Animator>> enumerator = ((IEnumerable<IGrouping<string, Animator>>) ((IEnumerable<Animator>) objectsOfType).GroupBy<Animator, string>((Func<Animator, string>) (x => ((Component) x).get_transform().GetRecursiveName(""))).OrderByDescending<IGrouping<string, Animator>, int>((Func<IGrouping<string, Animator>, int>) (x => ((IEnumerable<Animator>) x).Count<Animator>()))).GetEnumerator())
    {
      while (((IEnumerator) enumerator).MoveNext())
      {
        IGrouping<string, Animator> current = enumerator.Current;
        stringBuilder2.AppendFormat("{1:N0}\t{0}", (object) ((Component) ((IEnumerable<Animator>) current).First<Animator>()).get_transform().GetRecursiveName(""), (object) ((IEnumerable<Animator>) current).Count<Animator>());
        stringBuilder2.AppendLine();
      }
    }
    DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Animators.Counts.txt", stringBuilder2.ToString());
    StringBuilder stringBuilder3 = new StringBuilder();
    stringBuilder3.AppendLine("All animators - grouped by enabled/disabled");
    stringBuilder3.AppendLine();
    using (IEnumerator<IGrouping<string, Animator>> enumerator = ((IEnumerable<IGrouping<string, Animator>>) ((IEnumerable<Animator>) objectsOfType).GroupBy<Animator, string>((Func<Animator, string>) (x => ((Component) x).get_transform().GetRecursiveName(((Behaviour) x).get_enabled() ? "" : " (DISABLED)"))).OrderByDescending<IGrouping<string, Animator>, int>((Func<IGrouping<string, Animator>, int>) (x => ((IEnumerable<Animator>) x).Count<Animator>()))).GetEnumerator())
    {
      while (((IEnumerator) enumerator).MoveNext())
      {
        IGrouping<string, Animator> current = enumerator.Current;
        stringBuilder3.AppendFormat("{1:N0}\t{0}", (object) ((Component) ((IEnumerable<Animator>) current).First<Animator>()).get_transform().GetRecursiveName(((Behaviour) ((IEnumerable<Animator>) current).First<Animator>()).get_enabled() ? "" : " (DISABLED)"), (object) ((IEnumerable<Animator>) current).Count<Animator>());
        stringBuilder3.AppendLine();
      }
    }
    DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Animators.Counts.Enabled.txt", stringBuilder3.ToString());
  }

  [ClientVar]
  [ServerVar]
  public static void dump(ConsoleSystem.Arg args)
  {
    if (Directory.Exists("diagnostics"))
      Directory.CreateDirectory("diagnostics");
    int num = 1;
    while (Directory.Exists("diagnostics/" + (object) num))
      ++num;
    Directory.CreateDirectory("diagnostics/" + (object) num);
    string targetFolder = "diagnostics/" + (object) num + "/";
    DiagnosticsConSys.DumpLODGroups(targetFolder);
    DiagnosticsConSys.DumpSystemInformation(targetFolder);
    DiagnosticsConSys.DumpGameObjects(targetFolder);
    DiagnosticsConSys.DumpObjects(targetFolder);
    DiagnosticsConSys.DumpEntities(targetFolder);
    DiagnosticsConSys.DumpNetwork(targetFolder);
    DiagnosticsConSys.DumpPhysics(targetFolder);
    DiagnosticsConSys.DumpAnimators(targetFolder);
  }

  private static void DumpSystemInformation(string targetFolder)
  {
    DiagnosticsConSys.WriteTextToFile(targetFolder + "System.Info.txt", SystemInfoGeneralText.currentInfo);
  }

  private static void WriteTextToFile(string file, string text)
  {
    File.WriteAllText(file, text);
  }

  private static void DumpEntities(string targetFolder)
  {
    StringBuilder stringBuilder1 = new StringBuilder();
    stringBuilder1.AppendLine("All entities");
    stringBuilder1.AppendLine();
    foreach (BaseNetworkable serverEntity in BaseNetworkable.serverEntities)
    {
      stringBuilder1.AppendFormat("{1}\t{0}", (object) serverEntity.PrefabName, (object) (uint) (serverEntity.net != null ? (int) serverEntity.net.ID : 0));
      stringBuilder1.AppendLine();
    }
    DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Entity.SV.List.txt", stringBuilder1.ToString());
    StringBuilder stringBuilder2 = new StringBuilder();
    stringBuilder2.AppendLine("All entities");
    stringBuilder2.AppendLine();
    foreach (IGrouping<uint, BaseNetworkable> source in (IEnumerable<IGrouping<uint, BaseNetworkable>>) BaseNetworkable.serverEntities.GroupBy<BaseNetworkable, uint>((Func<BaseNetworkable, uint>) (x => x.prefabID)).OrderByDescending<IGrouping<uint, BaseNetworkable>, int>((Func<IGrouping<uint, BaseNetworkable>, int>) (x => x.Count<BaseNetworkable>())))
    {
      stringBuilder2.AppendFormat("{1:N0}\t{0}", (object) source.First<BaseNetworkable>().PrefabName, (object) source.Count<BaseNetworkable>());
      stringBuilder2.AppendLine();
    }
    DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Entity.SV.Counts.txt", stringBuilder2.ToString());
    StringBuilder stringBuilder3 = new StringBuilder();
    stringBuilder3.AppendLine("Saved entities");
    stringBuilder3.AppendLine();
    foreach (IGrouping<uint, BaseEntity> source in (IEnumerable<IGrouping<uint, BaseEntity>>) BaseEntity.saveList.GroupBy<BaseEntity, uint>((Func<BaseEntity, uint>) (x => x.prefabID)).OrderByDescending<IGrouping<uint, BaseEntity>, int>((Func<IGrouping<uint, BaseEntity>, int>) (x => x.Count<BaseEntity>())))
    {
      stringBuilder3.AppendFormat("{1:N0}\t{0}", (object) source.First<BaseEntity>().PrefabName, (object) source.Count<BaseEntity>());
      stringBuilder3.AppendLine();
    }
    DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Entity.SV.Savelist.Counts.txt", stringBuilder3.ToString());
  }

  private static void DumpLODGroups(string targetFolder)
  {
    DiagnosticsConSys.DumpLODGroupTotals(targetFolder);
  }

  private static void DumpLODGroupTotals(string targetFolder)
  {
    M0[] objectsOfType = Object.FindObjectsOfType<LODGroup>();
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.AppendLine("LODGroups");
    stringBuilder.AppendLine();
    using (IEnumerator<IGrouping<string, LODGroup>> enumerator = ((IEnumerable<IGrouping<string, LODGroup>>) ((IEnumerable<LODGroup>) objectsOfType).GroupBy<LODGroup, string>((Func<LODGroup, string>) (x => ((Component) x).get_transform().GetRecursiveName(""))).OrderByDescending<IGrouping<string, LODGroup>, int>((Func<IGrouping<string, LODGroup>, int>) (x => ((IEnumerable<LODGroup>) x).Count<LODGroup>()))).GetEnumerator())
    {
      while (((IEnumerator) enumerator).MoveNext())
      {
        IGrouping<string, LODGroup> current = enumerator.Current;
        stringBuilder.AppendFormat("{1:N0}\t{0}", (object) current.Key, (object) ((IEnumerable<LODGroup>) current).Count<LODGroup>());
        stringBuilder.AppendLine();
      }
    }
    DiagnosticsConSys.WriteTextToFile(targetFolder + "LODGroups.Objects.txt", stringBuilder.ToString());
  }

  private static void DumpNetwork(string targetFolder)
  {
    if (!((Server) Net.sv).IsConnected())
      return;
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.AppendLine("Server Network Statistics");
    stringBuilder.AppendLine();
    stringBuilder.Append(((NetworkPeer) Net.sv).GetDebug((Connection) null).Replace("\n", "\r\n"));
    stringBuilder.AppendLine();
    foreach (BasePlayer activePlayer in BasePlayer.activePlayerList)
    {
      stringBuilder.AppendLine("Name: " + activePlayer.displayName);
      stringBuilder.AppendLine("SteamID: " + (object) activePlayer.userID);
      stringBuilder.Append(activePlayer.net == null ? "INVALID - NET IS NULL" : ((NetworkPeer) Net.sv).GetDebug(activePlayer.net.get_connection()).Replace("\n", "\r\n"));
      stringBuilder.AppendLine();
      stringBuilder.AppendLine();
      stringBuilder.AppendLine();
      stringBuilder.AppendLine();
    }
    DiagnosticsConSys.WriteTextToFile(targetFolder + "Network.Server.txt", stringBuilder.ToString());
  }

  private static void DumpObjects(string targetFolder)
  {
    Object[] objectsOfType = (Object[]) Object.FindObjectsOfType<Object>();
    StringBuilder stringBuilder1 = new StringBuilder();
    stringBuilder1.AppendLine("All active UnityEngine.Object, ordered by count");
    stringBuilder1.AppendLine();
    using (IEnumerator<IGrouping<System.Type, Object>> enumerator = ((IEnumerable<IGrouping<System.Type, Object>>) ((IEnumerable<Object>) objectsOfType).GroupBy<Object, System.Type>((Func<Object, System.Type>) (x => ((object) x).GetType())).OrderByDescending<IGrouping<System.Type, Object>, int>((Func<IGrouping<System.Type, Object>, int>) (x => ((IEnumerable<Object>) x).Count<Object>()))).GetEnumerator())
    {
      while (((IEnumerator) enumerator).MoveNext())
      {
        IGrouping<System.Type, Object> current = enumerator.Current;
        stringBuilder1.AppendFormat("{1:N0}\t{0}", (object) ((object) ((IEnumerable<Object>) current).First<Object>()).GetType().Name, (object) ((IEnumerable<Object>) current).Count<Object>());
        stringBuilder1.AppendLine();
      }
    }
    DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.Object.Count.txt", stringBuilder1.ToString());
    StringBuilder stringBuilder2 = new StringBuilder();
    stringBuilder2.AppendLine("All active UnityEngine.ScriptableObject, ordered by count");
    stringBuilder2.AppendLine();
    using (IEnumerator<IGrouping<System.Type, Object>> enumerator = ((IEnumerable<IGrouping<System.Type, Object>>) ((IEnumerable<Object>) objectsOfType).Where<Object>((Func<Object, bool>) (x => x is ScriptableObject)).GroupBy<Object, System.Type>((Func<Object, System.Type>) (x => ((object) x).GetType())).OrderByDescending<IGrouping<System.Type, Object>, int>((Func<IGrouping<System.Type, Object>, int>) (x => ((IEnumerable<Object>) x).Count<Object>()))).GetEnumerator())
    {
      while (((IEnumerator) enumerator).MoveNext())
      {
        IGrouping<System.Type, Object> current = enumerator.Current;
        stringBuilder2.AppendFormat("{1:N0}\t{0}", (object) ((object) ((IEnumerable<Object>) current).First<Object>()).GetType().Name, (object) ((IEnumerable<Object>) current).Count<Object>());
        stringBuilder2.AppendLine();
      }
    }
    DiagnosticsConSys.WriteTextToFile(targetFolder + "UnityEngine.ScriptableObject.Count.txt", stringBuilder2.ToString());
  }

  private static void DumpPhysics(string targetFolder)
  {
    DiagnosticsConSys.DumpTotals(targetFolder);
    DiagnosticsConSys.DumpColliders(targetFolder);
    DiagnosticsConSys.DumpRigidBodies(targetFolder);
  }

  private static void DumpTotals(string targetFolder)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.AppendLine("Physics Information");
    stringBuilder.AppendLine();
    stringBuilder.AppendFormat("Total Colliders:\t{0:N0}", (object) ((IEnumerable<Collider>) Object.FindObjectsOfType<Collider>()).Count<Collider>());
    stringBuilder.AppendLine();
    stringBuilder.AppendFormat("Active Colliders:\t{0:N0}", (object) ((IEnumerable<Collider>) Object.FindObjectsOfType<Collider>()).Where<Collider>((Func<Collider, bool>) (x => x.get_enabled())).Count<Collider>());
    stringBuilder.AppendLine();
    stringBuilder.AppendFormat("Batched Colliders:\t{0:N0}", (object) (Object.op_Implicit((Object) SingletonComponent<ColliderGrid>.Instance) ? ((ColliderGrid) SingletonComponent<ColliderGrid>.Instance).BatchedMeshCount() : 0));
    stringBuilder.AppendLine();
    stringBuilder.AppendFormat("Total RigidBodys:\t{0:N0}", (object) ((IEnumerable<Rigidbody>) Object.FindObjectsOfType<Rigidbody>()).Count<Rigidbody>());
    stringBuilder.AppendLine();
    stringBuilder.AppendLine();
    stringBuilder.AppendFormat("Mesh Colliders:\t{0:N0}", (object) ((IEnumerable<MeshCollider>) Object.FindObjectsOfType<MeshCollider>()).Count<MeshCollider>());
    stringBuilder.AppendLine();
    stringBuilder.AppendFormat("Box Colliders:\t{0:N0}", (object) ((IEnumerable<BoxCollider>) Object.FindObjectsOfType<BoxCollider>()).Count<BoxCollider>());
    stringBuilder.AppendLine();
    stringBuilder.AppendFormat("Sphere Colliders:\t{0:N0}", (object) ((IEnumerable<SphereCollider>) Object.FindObjectsOfType<SphereCollider>()).Count<SphereCollider>());
    stringBuilder.AppendLine();
    stringBuilder.AppendFormat("Capsule Colliders:\t{0:N0}", (object) ((IEnumerable<CapsuleCollider>) Object.FindObjectsOfType<CapsuleCollider>()).Count<CapsuleCollider>());
    stringBuilder.AppendLine();
    DiagnosticsConSys.WriteTextToFile(targetFolder + "Physics.txt", stringBuilder.ToString());
  }

  private static void DumpColliders(string targetFolder)
  {
    M0[] objectsOfType = Object.FindObjectsOfType<Collider>();
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.AppendLine("Physics Colliders");
    stringBuilder.AppendLine();
    using (IEnumerator<IGrouping<string, Collider>> enumerator = ((IEnumerable<IGrouping<string, Collider>>) ((IEnumerable<Collider>) objectsOfType).GroupBy<Collider, string>((Func<Collider, string>) (x => ((Component) x).get_transform().GetRecursiveName(""))).OrderByDescending<IGrouping<string, Collider>, int>((Func<IGrouping<string, Collider>, int>) (x => ((IEnumerable<Collider>) x).Count<Collider>()))).GetEnumerator())
    {
      while (((IEnumerator) enumerator).MoveNext())
      {
        IGrouping<string, Collider> current = enumerator.Current;
        stringBuilder.AppendFormat("{1:N0}\t{0} ({2:N0} triggers) ({3:N0} enabled)", (object) current.Key, (object) ((IEnumerable<Collider>) current).Count<Collider>(), (object) ((IEnumerable<Collider>) current).Count<Collider>((Func<Collider, bool>) (x => x.get_isTrigger())), (object) ((IEnumerable<Collider>) current).Count<Collider>((Func<Collider, bool>) (x => x.get_enabled())));
        stringBuilder.AppendLine();
      }
    }
    DiagnosticsConSys.WriteTextToFile(targetFolder + "Physics.Colliders.Objects.txt", stringBuilder.ToString());
  }

  private static void DumpRigidBodies(string targetFolder)
  {
    M0[] objectsOfType = Object.FindObjectsOfType<Rigidbody>();
    StringBuilder stringBuilder1 = new StringBuilder();
    stringBuilder1.AppendLine("RigidBody");
    stringBuilder1.AppendLine();
    StringBuilder stringBuilder2 = new StringBuilder();
    stringBuilder2.AppendLine("RigidBody");
    stringBuilder2.AppendLine();
    using (IEnumerator<IGrouping<string, Rigidbody>> enumerator1 = ((IEnumerable<IGrouping<string, Rigidbody>>) ((IEnumerable<Rigidbody>) objectsOfType).GroupBy<Rigidbody, string>((Func<Rigidbody, string>) (x => ((Component) x).get_transform().GetRecursiveName(""))).OrderByDescending<IGrouping<string, Rigidbody>, int>((Func<IGrouping<string, Rigidbody>, int>) (x => ((IEnumerable<Rigidbody>) x).Count<Rigidbody>()))).GetEnumerator())
    {
      while (((IEnumerator) enumerator1).MoveNext())
      {
        IGrouping<string, Rigidbody> current1 = enumerator1.Current;
        stringBuilder1.AppendFormat("{1:N0}\t{0} ({2:N0} awake) ({3:N0} kinematic) ({4:N0} non-discrete)", (object) current1.Key, (object) ((IEnumerable<Rigidbody>) current1).Count<Rigidbody>(), (object) ((IEnumerable<Rigidbody>) current1).Count<Rigidbody>((Func<Rigidbody, bool>) (x => !x.IsSleeping())), (object) ((IEnumerable<Rigidbody>) current1).Count<Rigidbody>((Func<Rigidbody, bool>) (x => x.get_isKinematic())), (object) ((IEnumerable<Rigidbody>) current1).Count<Rigidbody>((Func<Rigidbody, bool>) (x => x.get_collisionDetectionMode() > 0)));
        stringBuilder1.AppendLine();
        using (IEnumerator<Rigidbody> enumerator2 = ((IEnumerable<Rigidbody>) current1).GetEnumerator())
        {
          while (((IEnumerator) enumerator2).MoveNext())
          {
            Rigidbody current2 = enumerator2.Current;
            stringBuilder2.AppendFormat("{0} -{1}{2}{3}", (object) current1.Key, current2.get_isKinematic() ? (object) " KIN" : (object) "", current2.IsSleeping() ? (object) " SLEEP" : (object) "", current2.get_useGravity() ? (object) " GRAVITY" : (object) "");
            stringBuilder2.AppendLine();
            stringBuilder2.AppendFormat("Mass: {0}\tVelocity: {1}\tsleepThreshold: {2}", (object) current2.get_mass(), (object) current2.get_velocity(), (object) current2.get_sleepThreshold());
            stringBuilder2.AppendLine();
            stringBuilder2.AppendLine();
          }
        }
      }
    }
    DiagnosticsConSys.WriteTextToFile(targetFolder + "Physics.RigidBody.Objects.txt", stringBuilder1.ToString());
    DiagnosticsConSys.WriteTextToFile(targetFolder + "Physics.RigidBody.All.txt", stringBuilder2.ToString());
  }

  private static void DumpGameObjects(string targetFolder)
  {
    Transform[] rootObjects = TransformUtil.GetRootObjects();
    StringBuilder str1 = new StringBuilder();
    str1.AppendLine("All active game objects");
    str1.AppendLine();
    foreach (Transform tx in rootObjects)
    {
      DiagnosticsConSys.DumpGameObjectRecursive(str1, tx, 0, false);
      str1.AppendLine();
    }
    DiagnosticsConSys.WriteTextToFile(targetFolder + "GameObject.Hierarchy.txt", str1.ToString());
    StringBuilder str2 = new StringBuilder();
    str2.AppendLine("All active game objects including components");
    str2.AppendLine();
    foreach (Transform tx in rootObjects)
    {
      DiagnosticsConSys.DumpGameObjectRecursive(str2, tx, 0, true);
      str2.AppendLine();
    }
    DiagnosticsConSys.WriteTextToFile(targetFolder + "GameObject.Hierarchy.Components.txt", str2.ToString());
    StringBuilder stringBuilder1 = new StringBuilder();
    stringBuilder1.AppendLine("Root gameobjects, grouped by name, ordered by the total number of objects excluding children");
    stringBuilder1.AppendLine();
    using (IEnumerator<IGrouping<string, Transform>> enumerator = ((IEnumerable<IGrouping<string, Transform>>) ((IEnumerable<Transform>) rootObjects).GroupBy<Transform, string>((Func<Transform, string>) (x => ((Object) x).get_name())).OrderByDescending<IGrouping<string, Transform>, int>((Func<IGrouping<string, Transform>, int>) (x => ((IEnumerable<Transform>) x).Count<Transform>()))).GetEnumerator())
    {
      while (((IEnumerator) enumerator).MoveNext())
      {
        IGrouping<string, Transform> current = enumerator.Current;
        Transform transform = ((IEnumerable<Transform>) current).First<Transform>();
        stringBuilder1.AppendFormat("{1:N0}\t{0}", (object) ((Object) transform).get_name(), (object) ((IEnumerable<Transform>) current).Count<Transform>());
        stringBuilder1.AppendLine();
      }
    }
    DiagnosticsConSys.WriteTextToFile(targetFolder + "GameObject.Count.txt", stringBuilder1.ToString());
    StringBuilder stringBuilder2 = new StringBuilder();
    stringBuilder2.AppendLine("Root gameobjects, grouped by name, ordered by the total number of objects including children");
    stringBuilder2.AppendLine();
    using (IEnumerator<KeyValuePair<Transform, int>> enumerator = ((IEnumerable<KeyValuePair<Transform, int>>) ((IEnumerable<Transform>) rootObjects).GroupBy<Transform, string>((Func<Transform, string>) (x => ((Object) x).get_name())).Select<IGrouping<string, Transform>, KeyValuePair<Transform, int>>((Func<IGrouping<string, Transform>, KeyValuePair<Transform, int>>) (x => new KeyValuePair<Transform, int>(((IEnumerable<Transform>) x).First<Transform>(), ((IEnumerable<Transform>) x).Sum<Transform>((Func<Transform, int>) (y => y.GetAllChildren().Count))))).OrderByDescending<KeyValuePair<Transform, int>, int>((Func<KeyValuePair<Transform, int>, int>) (x => x.Value))).GetEnumerator())
    {
      while (((IEnumerator) enumerator).MoveNext())
      {
        KeyValuePair<Transform, int> current = enumerator.Current;
        stringBuilder2.AppendFormat("{1:N0}\t{0}", (object) ((Object) current.Key).get_name(), (object) current.Value);
        stringBuilder2.AppendLine();
      }
    }
    DiagnosticsConSys.WriteTextToFile(targetFolder + "GameObject.Count.Children.txt", stringBuilder2.ToString());
  }

  private static void DumpGameObjectRecursive(
    StringBuilder str,
    Transform tx,
    int indent,
    bool includeComponents = false)
  {
    if (Object.op_Equality((Object) tx, (Object) null))
      return;
    for (int index = 0; index < indent; ++index)
      str.Append(" ");
    str.AppendFormat("{0} {1:N0}", (object) ((Object) tx).get_name(), (object) (((Component) tx).GetComponents<Component>().Length - 1));
    str.AppendLine();
    if (includeComponents)
    {
      foreach (Component component in (Component[]) ((Component) tx).GetComponents<Component>())
      {
        if (!(component is Transform))
        {
          for (int index = 0; index < indent + 1; ++index)
            str.Append(" ");
          str.AppendFormat("[c] {0}", Object.op_Equality((Object) component, (Object) null) ? (object) "NULL" : (object) ((object) component).GetType().ToString());
          str.AppendLine();
        }
      }
    }
    for (int index = 0; index < tx.get_childCount(); ++index)
      DiagnosticsConSys.DumpGameObjectRecursive(str, tx.GetChild(index), indent + 2, includeComponents);
  }

  public DiagnosticsConSys()
  {
    base.\u002Ector();
  }
}
