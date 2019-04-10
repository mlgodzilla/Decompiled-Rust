// Decompiled with JetBrains decompiler
// Type: DeployVolume
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System.Collections.Generic;
using UnityEngine;

public abstract class DeployVolume : PrefabAttribute
{
  public LayerMask layers = LayerMask.op_Implicit(537001984);
  [InspectorFlags]
  public ColliderInfo.Flags ignore;

  protected override System.Type GetIndexedType()
  {
    return typeof (DeployVolume);
  }

  protected abstract bool Check(Vector3 position, Quaternion rotation, int mask = -1);

  protected abstract bool Check(Vector3 position, Quaternion rotation, OBB test, int mask = -1);

  public static bool Check(
    Vector3 position,
    Quaternion rotation,
    DeployVolume[] volumes,
    int mask = -1)
  {
    for (int index = 0; index < volumes.Length; ++index)
    {
      if (volumes[index].Check(position, rotation, mask))
        return true;
    }
    return false;
  }

  public static bool Check(
    Vector3 position,
    Quaternion rotation,
    DeployVolume[] volumes,
    OBB test,
    int mask = -1)
  {
    for (int index = 0; index < volumes.Length; ++index)
    {
      if (volumes[index].Check(position, rotation, test, mask))
        return true;
    }
    return false;
  }

  public static bool CheckSphere(
    Vector3 pos,
    float radius,
    int layerMask,
    ColliderInfo.Flags ignore)
  {
    if (ignore == (ColliderInfo.Flags) 0)
      return GamePhysics.CheckSphere(pos, radius, layerMask, (QueryTriggerInteraction) 0);
    List<Collider> list = (List<Collider>) Pool.GetList<Collider>();
    GamePhysics.OverlapSphere(pos, radius, list, layerMask, (QueryTriggerInteraction) 1);
    int num = DeployVolume.CheckFlags(list, ignore) ? 1 : 0;
    // ISSUE: cast to a reference type
    Pool.FreeList<Collider>((List<M0>&) ref list);
    return num != 0;
  }

  public static bool CheckCapsule(
    Vector3 start,
    Vector3 end,
    float radius,
    int layerMask,
    ColliderInfo.Flags ignore)
  {
    if (ignore == (ColliderInfo.Flags) 0)
      return GamePhysics.CheckCapsule(start, end, radius, layerMask, (QueryTriggerInteraction) 0);
    List<Collider> list = (List<Collider>) Pool.GetList<Collider>();
    GamePhysics.OverlapCapsule(start, end, radius, list, layerMask, (QueryTriggerInteraction) 1);
    int num = DeployVolume.CheckFlags(list, ignore) ? 1 : 0;
    // ISSUE: cast to a reference type
    Pool.FreeList<Collider>((List<M0>&) ref list);
    return num != 0;
  }

  public static bool CheckOBB(OBB obb, int layerMask, ColliderInfo.Flags ignore)
  {
    if (ignore == (ColliderInfo.Flags) 0)
      return GamePhysics.CheckOBB(obb, layerMask, (QueryTriggerInteraction) 0);
    List<Collider> list = (List<Collider>) Pool.GetList<Collider>();
    GamePhysics.OverlapOBB(obb, list, layerMask, (QueryTriggerInteraction) 1);
    int num = DeployVolume.CheckFlags(list, ignore) ? 1 : 0;
    // ISSUE: cast to a reference type
    Pool.FreeList<Collider>((List<M0>&) ref list);
    return num != 0;
  }

  public static bool CheckBounds(Bounds bounds, int layerMask, ColliderInfo.Flags ignore)
  {
    if (ignore == (ColliderInfo.Flags) 0)
      return GamePhysics.CheckBounds(bounds, layerMask, (QueryTriggerInteraction) 0);
    List<Collider> list = (List<Collider>) Pool.GetList<Collider>();
    GamePhysics.OverlapBounds(bounds, list, layerMask, (QueryTriggerInteraction) 1);
    int num = DeployVolume.CheckFlags(list, ignore) ? 1 : 0;
    // ISSUE: cast to a reference type
    Pool.FreeList<Collider>((List<M0>&) ref list);
    return num != 0;
  }

  private static bool CheckFlags(List<Collider> list, ColliderInfo.Flags ignore)
  {
    for (int index = 0; index < list.Count; ++index)
    {
      ColliderInfo component = (ColliderInfo) ((Component) list[index]).get_gameObject().GetComponent<ColliderInfo>();
      if (Object.op_Equality((Object) component, (Object) null) || !component.HasFlag(ignore))
        return true;
    }
    return false;
  }
}
