// Decompiled with JetBrains decompiler
// Type: GamePhysics
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class GamePhysics
{
  private static RaycastHit[] hitBuffer = new RaycastHit[8192];
  private static Collider[] colBuffer = new Collider[8192];
  public const int BufferLength = 8192;

  public static bool CheckSphere(
    Vector3 position,
    float radius,
    int layerMask = -5,
    QueryTriggerInteraction triggerInteraction = 0)
  {
    layerMask = GamePhysics.HandleTerrainCollision(position, layerMask);
    return Physics.CheckSphere(position, radius, layerMask, triggerInteraction);
  }

  public static bool CheckCapsule(
    Vector3 start,
    Vector3 end,
    float radius,
    int layerMask = -5,
    QueryTriggerInteraction triggerInteraction = 0)
  {
    layerMask = GamePhysics.HandleTerrainCollision(Vector3.op_Multiply(Vector3.op_Addition(start, end), 0.5f), layerMask);
    return Physics.CheckCapsule(start, end, radius, layerMask, triggerInteraction);
  }

  public static bool CheckOBB(OBB obb, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0)
  {
    layerMask = GamePhysics.HandleTerrainCollision((Vector3) obb.position, layerMask);
    return Physics.CheckBox((Vector3) obb.position, (Vector3) obb.extents, (Quaternion) obb.rotation, layerMask, triggerInteraction);
  }

  public static bool CheckBounds(
    Bounds bounds,
    int layerMask = -5,
    QueryTriggerInteraction triggerInteraction = 0)
  {
    layerMask = GamePhysics.HandleTerrainCollision(((Bounds) ref bounds).get_center(), layerMask);
    return Physics.CheckBox(((Bounds) ref bounds).get_center(), ((Bounds) ref bounds).get_extents(), Quaternion.get_identity(), layerMask, triggerInteraction);
  }

  public static void OverlapSphere(
    Vector3 position,
    float radius,
    List<Collider> list,
    int layerMask = -5,
    QueryTriggerInteraction triggerInteraction = 1)
  {
    layerMask = GamePhysics.HandleTerrainCollision(position, layerMask);
    GamePhysics.BufferToList(Physics.OverlapSphereNonAlloc(position, radius, GamePhysics.colBuffer, layerMask, triggerInteraction), list);
  }

  public static void OverlapCapsule(
    Vector3 point0,
    Vector3 point1,
    float radius,
    List<Collider> list,
    int layerMask = -5,
    QueryTriggerInteraction triggerInteraction = 1)
  {
    layerMask = GamePhysics.HandleTerrainCollision(point0, layerMask);
    layerMask = GamePhysics.HandleTerrainCollision(point1, layerMask);
    GamePhysics.BufferToList(Physics.OverlapCapsuleNonAlloc(point0, point1, radius, GamePhysics.colBuffer, layerMask, triggerInteraction), list);
  }

  public static void OverlapOBB(
    OBB obb,
    List<Collider> list,
    int layerMask = -5,
    QueryTriggerInteraction triggerInteraction = 1)
  {
    layerMask = GamePhysics.HandleTerrainCollision((Vector3) obb.position, layerMask);
    GamePhysics.BufferToList(Physics.OverlapBoxNonAlloc((Vector3) obb.position, (Vector3) obb.extents, GamePhysics.colBuffer, (Quaternion) obb.rotation, layerMask, triggerInteraction), list);
  }

  public static void OverlapBounds(
    Bounds bounds,
    List<Collider> list,
    int layerMask = -5,
    QueryTriggerInteraction triggerInteraction = 1)
  {
    layerMask = GamePhysics.HandleTerrainCollision(((Bounds) ref bounds).get_center(), layerMask);
    GamePhysics.BufferToList(Physics.OverlapBoxNonAlloc(((Bounds) ref bounds).get_center(), ((Bounds) ref bounds).get_extents(), GamePhysics.colBuffer, Quaternion.get_identity(), layerMask, triggerInteraction), list);
  }

  private static void BufferToList(int count, List<Collider> list)
  {
    if (count >= GamePhysics.colBuffer.Length)
      Debug.LogWarning((object) "Physics query is exceeding collider buffer length.");
    for (int index = 0; index < count; ++index)
    {
      list.Add(GamePhysics.colBuffer[index]);
      GamePhysics.colBuffer[index] = (Collider) null;
    }
  }

  public static bool CheckSphere<T>(
    Vector3 pos,
    float radius,
    int layerMask = -5,
    QueryTriggerInteraction triggerInteraction = 1)
    where T : Component
  {
    List<Collider> list = (List<Collider>) Pool.GetList<Collider>();
    GamePhysics.OverlapSphere(pos, radius, list, layerMask, triggerInteraction);
    int num = GamePhysics.CheckComponent<T>(list) ? 1 : 0;
    // ISSUE: cast to a reference type
    Pool.FreeList<Collider>((List<M0>&) ref list);
    return num != 0;
  }

  public static bool CheckCapsule<T>(
    Vector3 start,
    Vector3 end,
    float radius,
    int layerMask = -5,
    QueryTriggerInteraction triggerInteraction = 1)
    where T : Component
  {
    List<Collider> list = (List<Collider>) Pool.GetList<Collider>();
    GamePhysics.OverlapCapsule(start, end, radius, list, layerMask, triggerInteraction);
    int num = GamePhysics.CheckComponent<T>(list) ? 1 : 0;
    // ISSUE: cast to a reference type
    Pool.FreeList<Collider>((List<M0>&) ref list);
    return num != 0;
  }

  public static bool CheckOBB<T>(
    OBB obb,
    int layerMask = -5,
    QueryTriggerInteraction triggerInteraction = 1)
    where T : Component
  {
    List<Collider> list = (List<Collider>) Pool.GetList<Collider>();
    GamePhysics.OverlapOBB(obb, list, layerMask, triggerInteraction);
    int num = GamePhysics.CheckComponent<T>(list) ? 1 : 0;
    // ISSUE: cast to a reference type
    Pool.FreeList<Collider>((List<M0>&) ref list);
    return num != 0;
  }

  public static bool CheckBounds<T>(
    Bounds bounds,
    int layerMask = -5,
    QueryTriggerInteraction triggerInteraction = 1)
    where T : Component
  {
    List<Collider> list = (List<Collider>) Pool.GetList<Collider>();
    GamePhysics.OverlapBounds(bounds, list, layerMask, triggerInteraction);
    int num = GamePhysics.CheckComponent<T>(list) ? 1 : 0;
    // ISSUE: cast to a reference type
    Pool.FreeList<Collider>((List<M0>&) ref list);
    return num != 0;
  }

  private static bool CheckComponent<T>(List<Collider> list)
  {
    for (int index = 0; index < list.Count; ++index)
    {
      if ((object) ((Component) list[index]).get_gameObject().GetComponent<T>() != null)
        return true;
    }
    return false;
  }

  public static void OverlapSphere<T>(
    Vector3 position,
    float radius,
    List<T> list,
    int layerMask = -5,
    QueryTriggerInteraction triggerInteraction = 1)
    where T : Component
  {
    layerMask = GamePhysics.HandleTerrainCollision(position, layerMask);
    GamePhysics.BufferToList<T>(Physics.OverlapSphereNonAlloc(position, radius, GamePhysics.colBuffer, layerMask, triggerInteraction), list);
  }

  public static void OverlapCapsule<T>(
    Vector3 point0,
    Vector3 point1,
    float radius,
    List<T> list,
    int layerMask = -5,
    QueryTriggerInteraction triggerInteraction = 1)
    where T : Component
  {
    layerMask = GamePhysics.HandleTerrainCollision(point0, layerMask);
    layerMask = GamePhysics.HandleTerrainCollision(point1, layerMask);
    GamePhysics.BufferToList<T>(Physics.OverlapCapsuleNonAlloc(point0, point1, radius, GamePhysics.colBuffer, layerMask, triggerInteraction), list);
  }

  public static void OverlapOBB<T>(
    OBB obb,
    List<T> list,
    int layerMask = -5,
    QueryTriggerInteraction triggerInteraction = 1)
    where T : Component
  {
    layerMask = GamePhysics.HandleTerrainCollision((Vector3) obb.position, layerMask);
    GamePhysics.BufferToList<T>(Physics.OverlapBoxNonAlloc((Vector3) obb.position, (Vector3) obb.extents, GamePhysics.colBuffer, (Quaternion) obb.rotation, layerMask, triggerInteraction), list);
  }

  public static void OverlapBounds<T>(
    Bounds bounds,
    List<T> list,
    int layerMask = -5,
    QueryTriggerInteraction triggerInteraction = 1)
    where T : Component
  {
    layerMask = GamePhysics.HandleTerrainCollision(((Bounds) ref bounds).get_center(), layerMask);
    GamePhysics.BufferToList<T>(Physics.OverlapBoxNonAlloc(((Bounds) ref bounds).get_center(), ((Bounds) ref bounds).get_extents(), GamePhysics.colBuffer, Quaternion.get_identity(), layerMask, triggerInteraction), list);
  }

  private static void BufferToList<T>(int count, List<T> list) where T : Component
  {
    if (count >= GamePhysics.colBuffer.Length)
      Debug.LogWarning((object) "Physics query is exceeding collider buffer length.");
    for (int index = 0; index < count; ++index)
    {
      T component = ((Component) GamePhysics.colBuffer[index]).get_gameObject().GetComponent<T>();
      if (Object.op_Implicit((Object) (object) component))
        list.Add(component);
      GamePhysics.colBuffer[index] = (Collider) null;
    }
  }

  public static bool Trace(
    Ray ray,
    float radius,
    out RaycastHit hitInfo,
    float maxDistance = float.PositiveInfinity,
    int layerMask = -5,
    QueryTriggerInteraction triggerInteraction = 0)
  {
    List<RaycastHit> list = (List<RaycastHit>) Pool.GetList<RaycastHit>();
    GamePhysics.TraceAllUnordered(ray, radius, list, maxDistance, layerMask, triggerInteraction);
    if (list.Count == 0)
    {
      hitInfo = (RaycastHit) null;
      // ISSUE: cast to a reference type
      Pool.FreeList<RaycastHit>((List<M0>&) ref list);
      return false;
    }
    GamePhysics.Sort(list);
    hitInfo = list[0];
    // ISSUE: cast to a reference type
    Pool.FreeList<RaycastHit>((List<M0>&) ref list);
    return true;
  }

  public static void TraceAll(
    Ray ray,
    float radius,
    List<RaycastHit> hits,
    float maxDistance = float.PositiveInfinity,
    int layerMask = -5,
    QueryTriggerInteraction triggerInteraction = 0)
  {
    GamePhysics.TraceAllUnordered(ray, radius, hits, maxDistance, layerMask, triggerInteraction);
    GamePhysics.Sort(hits);
  }

  public static void TraceAllUnordered(
    Ray ray,
    float radius,
    List<RaycastHit> hits,
    float maxDistance = float.PositiveInfinity,
    int layerMask = -5,
    QueryTriggerInteraction triggerInteraction = 0)
  {
    int num = (double) radius != 0.0 ? Physics.SphereCastNonAlloc(ray, radius, GamePhysics.hitBuffer, maxDistance, layerMask, triggerInteraction) : Physics.RaycastNonAlloc(ray, GamePhysics.hitBuffer, maxDistance, layerMask, triggerInteraction);
    if (num == 0)
      return;
    if (num >= GamePhysics.hitBuffer.Length)
      Debug.LogWarning((object) "Physics query is exceeding hit buffer length.");
    for (int index = 0; index < num; ++index)
    {
      RaycastHit hitInfo = GamePhysics.hitBuffer[index];
      if (GamePhysics.Verify(hitInfo))
        hits.Add(hitInfo);
    }
  }

  public static bool LineOfSight(Vector3 p0, Vector3 p1, int layerMask, float padding = 0.0f)
  {
    return GamePhysics.LineOfSightInternal(p0, p1, layerMask, padding, padding);
  }

  public static bool LineOfSight(
    Vector3 p0,
    Vector3 p1,
    Vector3 p2,
    int layerMask,
    float padding = 0.0f)
  {
    if (GamePhysics.LineOfSightInternal(p0, p1, layerMask, padding, 0.0f))
      return GamePhysics.LineOfSightInternal(p1, p2, layerMask, 0.0f, padding);
    return false;
  }

  public static bool LineOfSight(
    Vector3 p0,
    Vector3 p1,
    Vector3 p2,
    Vector3 p3,
    int layerMask,
    float padding = 0.0f)
  {
    if (GamePhysics.LineOfSightInternal(p0, p1, layerMask, padding, 0.0f) && GamePhysics.LineOfSightInternal(p1, p2, layerMask, 0.0f, 0.0f))
      return GamePhysics.LineOfSightInternal(p2, p3, layerMask, 0.0f, padding);
    return false;
  }

  public static bool LineOfSight(
    Vector3 p0,
    Vector3 p1,
    Vector3 p2,
    Vector3 p3,
    Vector3 p4,
    int layerMask,
    float padding = 0.0f)
  {
    if (GamePhysics.LineOfSightInternal(p0, p1, layerMask, padding, 0.0f) && GamePhysics.LineOfSightInternal(p1, p2, layerMask, 0.0f, 0.0f) && GamePhysics.LineOfSightInternal(p2, p3, layerMask, 0.0f, 0.0f))
      return GamePhysics.LineOfSightInternal(p3, p4, layerMask, 0.0f, padding);
    return false;
  }

  private static bool LineOfSightInternal(
    Vector3 p0,
    Vector3 p1,
    int layerMask,
    float padding0,
    float padding1)
  {
    Vector3 vector3_1 = Vector3.op_Subtraction(p1, p0);
    float magnitude = ((Vector3) ref vector3_1).get_magnitude();
    if ((double) magnitude <= (double) padding0 + (double) padding1)
      return true;
    Vector3 vector3_2 = Vector3.op_Division(vector3_1, magnitude);
    Vector3 vector3_3 = Vector3.op_Multiply(vector3_2, padding0);
    Vector3 vector3_4 = Vector3.op_Multiply(vector3_2, padding1);
    RaycastHit raycastHit;
    if (!Physics.Linecast(Vector3.op_Addition(p0, vector3_3), Vector3.op_Subtraction(p1, vector3_4), ref raycastHit, layerMask, (QueryTriggerInteraction) 1))
    {
      if (ConVar.Vis.lineofsight)
        ConsoleNetwork.BroadcastToAllClients("ddraw.line", (object) 60f, (object) Color.get_green(), (object) p0, (object) p1);
      return true;
    }
    if (ConVar.Vis.lineofsight)
    {
      ConsoleNetwork.BroadcastToAllClients("ddraw.line", (object) 60f, (object) Color.get_red(), (object) p0, (object) p1);
      ConsoleNetwork.BroadcastToAllClients("ddraw.text", (object) 60f, (object) Color.get_white(), (object) ((RaycastHit) ref raycastHit).get_point(), (object) ((Object) ((RaycastHit) ref raycastHit).get_collider()).get_name());
    }
    return false;
  }

  public static bool Verify(RaycastHit hitInfo)
  {
    return GamePhysics.Verify(((RaycastHit) ref hitInfo).get_collider(), ((RaycastHit) ref hitInfo).get_point());
  }

  public static bool Verify(Collider collider, Vector3 point)
  {
    if (collider is TerrainCollider && Object.op_Implicit((Object) TerrainMeta.Collision) && TerrainMeta.Collision.GetIgnore(point, 0.01f))
      return false;
    return collider.get_enabled();
  }

  public static int HandleTerrainCollision(Vector3 position, int layerMask)
  {
    int num = 8388608;
    if ((layerMask & num) != 0 && Object.op_Implicit((Object) TerrainMeta.Collision) && TerrainMeta.Collision.GetIgnore(position, 0.01f))
      layerMask &= ~num;
    return layerMask;
  }

  public static void Sort(List<RaycastHit> hits)
  {
    hits.Sort((Comparison<RaycastHit>) ((a, b) => ((RaycastHit) ref a).get_distance().CompareTo(((RaycastHit) ref b).get_distance())));
  }

  public static void Sort(RaycastHit[] hits)
  {
    Array.Sort<RaycastHit>(hits, (Comparison<RaycastHit>) ((a, b) => ((RaycastHit) ref a).get_distance().CompareTo(((RaycastHit) ref b).get_distance())));
  }
}
