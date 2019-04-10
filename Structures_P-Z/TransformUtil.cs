// Decompiled with JetBrains decompiler
// Type: TransformUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TransformUtil
{
  public static bool GetGroundInfo(Vector3 startPos, out RaycastHit hit, Transform ignoreTransform = null)
  {
    return TransformUtil.GetGroundInfo(startPos, out hit, 100f, LayerMask.op_Implicit(-1), ignoreTransform);
  }

  public static bool GetGroundInfo(
    Vector3 startPos,
    out RaycastHit hit,
    float range,
    Transform ignoreTransform = null)
  {
    return TransformUtil.GetGroundInfo(startPos, out hit, range, LayerMask.op_Implicit(-1), ignoreTransform);
  }

  public static bool GetGroundInfo(
    Vector3 startPos,
    out RaycastHit hitOut,
    float range,
    LayerMask mask,
    Transform ignoreTransform = null)
  {
    ref __Null local = ref startPos.y;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local = ^(float&) ref local + 0.25f;
    range += 0.25f;
    hitOut = (RaycastHit) null;
    RaycastHit raycastHit;
    if (!Physics.Raycast(new Ray(startPos, Vector3.get_down()), ref raycastHit, range, LayerMask.op_Implicit(mask)))
      return false;
    if (Object.op_Inequality((Object) ignoreTransform, (Object) null) && (Object.op_Equality((Object) ((Component) ((RaycastHit) ref raycastHit).get_collider()).get_transform(), (Object) ignoreTransform) || ((Component) ((RaycastHit) ref raycastHit).get_collider()).get_transform().IsChildOf(ignoreTransform)))
      return TransformUtil.GetGroundInfo(Vector3.op_Subtraction(startPos, new Vector3(0.0f, 0.01f, 0.0f)), out hitOut, range, mask, ignoreTransform);
    hitOut = raycastHit;
    return true;
  }

  public static bool GetGroundInfo(
    Vector3 startPos,
    out Vector3 pos,
    out Vector3 normal,
    Transform ignoreTransform = null)
  {
    return TransformUtil.GetGroundInfo(startPos, out pos, out normal, 100f, LayerMask.op_Implicit(-1), ignoreTransform);
  }

  public static bool GetGroundInfo(
    Vector3 startPos,
    out Vector3 pos,
    out Vector3 normal,
    float range,
    Transform ignoreTransform = null)
  {
    return TransformUtil.GetGroundInfo(startPos, out pos, out normal, range, LayerMask.op_Implicit(-1), ignoreTransform);
  }

  public static bool GetGroundInfo(
    Vector3 startPos,
    out Vector3 pos,
    out Vector3 normal,
    float range,
    LayerMask mask,
    Transform ignoreTransform = null)
  {
    ref __Null local = ref startPos.y;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local = ^(float&) ref local + 0.25f;
    range += 0.25f;
    using (IEnumerator<RaycastHit> enumerator = ((IEnumerable<RaycastHit>) ((IEnumerable<RaycastHit>) Physics.RaycastAll(new Ray(startPos, Vector3.get_down()), range, LayerMask.op_Implicit(mask))).OrderBy<RaycastHit, float>((Func<RaycastHit, float>) (h => ((RaycastHit) ref h).get_distance()))).GetEnumerator())
    {
      while (((IEnumerator) enumerator).MoveNext())
      {
        RaycastHit current = enumerator.Current;
        if (!Object.op_Inequality((Object) ignoreTransform, (Object) null) || !Object.op_Equality((Object) ((Component) ((RaycastHit) ref current).get_collider()).get_transform(), (Object) ignoreTransform) && !((Component) ((RaycastHit) ref current).get_collider()).get_transform().IsChildOf(ignoreTransform))
        {
          pos = ((RaycastHit) ref current).get_point();
          normal = ((RaycastHit) ref current).get_normal();
          return true;
        }
      }
    }
    pos = startPos;
    normal = Vector3.get_up();
    return false;
  }

  public static bool GetGroundInfoTerrainOnly(
    Vector3 startPos,
    out Vector3 pos,
    out Vector3 normal)
  {
    return TransformUtil.GetGroundInfoTerrainOnly(startPos, out pos, out normal, 100f, LayerMask.op_Implicit(-1));
  }

  public static bool GetGroundInfoTerrainOnly(
    Vector3 startPos,
    out Vector3 pos,
    out Vector3 normal,
    float range)
  {
    return TransformUtil.GetGroundInfoTerrainOnly(startPos, out pos, out normal, range, LayerMask.op_Implicit(-1));
  }

  public static bool GetGroundInfoTerrainOnly(
    Vector3 startPos,
    out Vector3 pos,
    out Vector3 normal,
    float range,
    LayerMask mask)
  {
    ref __Null local = ref startPos.y;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local = ^(float&) ref local + 0.25f;
    range += 0.25f;
    RaycastHit raycastHit;
    if (Physics.Raycast(new Ray(startPos, Vector3.get_down()), ref raycastHit, range, LayerMask.op_Implicit(mask)) && ((RaycastHit) ref raycastHit).get_collider() is TerrainCollider)
    {
      pos = ((RaycastHit) ref raycastHit).get_point();
      normal = ((RaycastHit) ref raycastHit).get_normal();
      return true;
    }
    pos = startPos;
    normal = Vector3.get_up();
    return false;
  }

  public static Transform[] GetRootObjects()
  {
    return ((IEnumerable<Transform>) Object.FindObjectsOfType<Transform>()).Where<Transform>((Func<Transform, bool>) (x => Object.op_Equality((Object) ((Component) x).get_transform(), (Object) ((Component) x).get_transform().get_root()))).ToArray<Transform>();
  }
}
