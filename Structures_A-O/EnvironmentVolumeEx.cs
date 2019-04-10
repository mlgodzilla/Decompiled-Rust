// Decompiled with JetBrains decompiler
// Type: EnvironmentVolumeEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System.Collections.Generic;
using UnityEngine;

public static class EnvironmentVolumeEx
{
  public static bool CheckEnvironmentVolumes(
    this Transform transform,
    Vector3 pos,
    Quaternion rot,
    Vector3 scale,
    EnvironmentType type)
  {
    List<EnvironmentVolume> list = (List<EnvironmentVolume>) Pool.GetList<EnvironmentVolume>();
    ((Component) transform).GetComponentsInChildren<EnvironmentVolume>(true, (List<M0>) list);
    for (int index = 0; index < list.Count; ++index)
    {
      EnvironmentVolume environmentVolume = list[index];
      OBB obb;
      ((OBB) ref obb).\u002Ector(((Component) environmentVolume).get_transform(), new Bounds(environmentVolume.Center, environmentVolume.Size));
      ((OBB) ref obb).Transform(pos, scale, rot);
      if (EnvironmentManager.Check(obb, type))
      {
        // ISSUE: cast to a reference type
        Pool.FreeList<EnvironmentVolume>((List<M0>&) ref list);
        return true;
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<EnvironmentVolume>((List<M0>&) ref list);
    return false;
  }

  public static bool CheckEnvironmentVolumes(this Transform transform, EnvironmentType type)
  {
    return transform.CheckEnvironmentVolumes(transform.get_position(), transform.get_rotation(), transform.get_lossyScale(), type);
  }
}
