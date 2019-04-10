// Decompiled with JetBrains decompiler
// Type: EnvironmentManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : SingletonComponent<EnvironmentManager>
{
  public static EnvironmentType Get(OBB obb)
  {
    EnvironmentType environmentType = (EnvironmentType) 0;
    List<EnvironmentVolume> list = (List<EnvironmentVolume>) Pool.GetList<EnvironmentVolume>();
    GamePhysics.OverlapOBB<EnvironmentVolume>(obb, list, 262144, (QueryTriggerInteraction) 2);
    for (int index = 0; index < list.Count; ++index)
      environmentType |= list[index].Type;
    // ISSUE: cast to a reference type
    Pool.FreeList<EnvironmentVolume>((List<M0>&) ref list);
    return environmentType;
  }

  public static EnvironmentType Get(Vector3 pos, ref List<EnvironmentVolume> list)
  {
    EnvironmentType environmentType = (EnvironmentType) 0;
    GamePhysics.OverlapSphere<EnvironmentVolume>(pos, 0.01f, list, 262144, (QueryTriggerInteraction) 2);
    for (int index = 0; index < list.Count; ++index)
      environmentType |= list[index].Type;
    return environmentType;
  }

  public static EnvironmentType Get(Vector3 pos)
  {
    List<EnvironmentVolume> list = (List<EnvironmentVolume>) Pool.GetList<EnvironmentVolume>();
    int num = (int) EnvironmentManager.Get(pos, ref list);
    // ISSUE: cast to a reference type
    Pool.FreeList<EnvironmentVolume>((List<M0>&) ref list);
    return (EnvironmentType) num;
  }

  public static bool Check(OBB obb, EnvironmentType type)
  {
    return (uint) (EnvironmentManager.Get(obb) & type) > 0U;
  }

  public static bool Check(Vector3 pos, EnvironmentType type)
  {
    return (uint) (EnvironmentManager.Get(pos) & type) > 0U;
  }

  public EnvironmentManager()
  {
    base.\u002Ector();
  }
}
