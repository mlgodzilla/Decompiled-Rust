// Decompiled with JetBrains decompiler
// Type: AIInformationZone
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIInformationZone : MonoBehaviour
{
  public static List<AIInformationZone> zones = new List<AIInformationZone>();
  public List<AICoverPoint> coverPoints;
  public List<AIMovePoint> movePoints;
  public List<NavMeshLink> navMeshLinks;
  public Bounds bounds;
  private OBB areaBox;

  public void OnValidate()
  {
  }

  public void Start()
  {
    this.Process();
    this.areaBox = new OBB(((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_lossyScale(), ((Component) this).get_transform().get_rotation(), this.bounds);
    AIInformationZone.zones.Add(this);
  }

  public void OnDrawGizmos()
  {
    Gizmos.set_color(new Color(1f, 0.0f, 0.0f, 0.5f));
    Gizmos.DrawCube(Vector3.op_Addition(((Component) this).get_transform().get_position(), ((Bounds) ref this.bounds).get_center()), ((Bounds) ref this.bounds).get_size());
  }

  public void Process()
  {
    this.coverPoints.AddRange((IEnumerable<AICoverPoint>) ((Component) ((Component) this).get_transform()).GetComponentsInChildren<AICoverPoint>());
    this.movePoints.AddRange((IEnumerable<AIMovePoint>) ((Component) ((Component) this).get_transform()).GetComponentsInChildren<AIMovePoint>(true));
    this.navMeshLinks.AddRange((IEnumerable<NavMeshLink>) ((Component) ((Component) this).get_transform()).GetComponentsInChildren<NavMeshLink>(true));
  }

  public static AIInformationZone GetForPoint(Vector3 point, BaseEntity from = null)
  {
    if (AIInformationZone.zones == null || AIInformationZone.zones.Count == 0)
      return (AIInformationZone) null;
    foreach (AIInformationZone zone in AIInformationZone.zones)
    {
      if (((OBB) ref zone.areaBox).Contains(point))
        return zone;
    }
    return AIInformationZone.zones[0];
  }

  public AIMovePoint GetBestMovePointNear(
    Vector3 targetPosition,
    Vector3 fromPosition,
    float minRange,
    float maxRange,
    bool checkLOS = false,
    BaseEntity forObject = null)
  {
    AIMovePoint aiMovePoint = (AIMovePoint) null;
    float num1 = -1f;
    foreach (AIMovePoint movePoint in this.movePoints)
    {
      float num2 = 0.0f;
      Vector3 position = ((Component) movePoint).get_transform().get_position();
      float num3 = Vector3.Distance(targetPosition, position);
      if ((double) num3 <= (double) maxRange && ((Component) ((Component) movePoint).get_transform().get_parent()).get_gameObject().get_activeSelf())
      {
        float num4 = num2 + (movePoint.CanBeUsedBy(forObject) ? 100f : 0.0f) + (float) ((1.0 - (double) Mathf.InverseLerp(minRange, maxRange, num3)) * 100.0);
        if ((double) num4 >= (double) num1 && (!checkLOS || !Physics.Linecast(Vector3.op_Addition(targetPosition, Vector3.op_Multiply(Vector3.get_up(), 1f)), Vector3.op_Addition(position, Vector3.op_Multiply(Vector3.get_up(), 1f)), 1218519297, (QueryTriggerInteraction) 1)) && (double) num4 > (double) num1)
        {
          aiMovePoint = movePoint;
          num1 = num4;
        }
      }
    }
    return aiMovePoint;
  }

  public Vector3 GetBestPositionNear(
    Vector3 targetPosition,
    Vector3 fromPosition,
    float minRange,
    float maxRange,
    bool checkLOS = false)
  {
    AIMovePoint aiMovePoint = (AIMovePoint) null;
    float num1 = -1f;
    foreach (AIMovePoint movePoint in this.movePoints)
    {
      float num2 = 0.0f;
      Vector3 position = ((Component) movePoint).get_transform().get_position();
      float num3 = Vector3.Distance(targetPosition, position);
      if ((double) num3 <= (double) maxRange && ((Component) ((Component) movePoint).get_transform().get_parent()).get_gameObject().get_activeSelf())
      {
        float num4 = num2 + (float) ((1.0 - (double) Mathf.InverseLerp(minRange, maxRange, num3)) * 100.0);
        if ((!checkLOS || !Physics.Linecast(Vector3.op_Addition(targetPosition, Vector3.op_Multiply(Vector3.get_up(), 1f)), Vector3.op_Addition(position, Vector3.op_Multiply(Vector3.get_up(), 1f)), 1218650369, (QueryTriggerInteraction) 1)) && (double) num4 > (double) num1)
        {
          aiMovePoint = movePoint;
          num1 = num4;
        }
      }
    }
    if (Object.op_Inequality((Object) aiMovePoint, (Object) null))
      return ((Component) aiMovePoint).get_transform().get_position();
    return targetPosition;
  }

  public AICoverPoint GetBestCoverPoint(
    Vector3 currentPosition,
    Vector3 hideFromPosition,
    float minRange = 0.0f,
    float maxRange = 20f,
    BaseEntity forObject = null)
  {
    AICoverPoint aiCoverPoint = (AICoverPoint) null;
    float num1 = 0.0f;
    foreach (AICoverPoint coverPoint in this.coverPoints)
    {
      if (!coverPoint.InUse() || coverPoint.IsUsedBy(forObject))
      {
        Vector3 position = ((Component) coverPoint).get_transform().get_position();
        Vector3 vector3 = Vector3.op_Subtraction(hideFromPosition, position);
        Vector3 normalized = ((Vector3) ref vector3).get_normalized();
        float num2 = Vector3.Dot(((Component) coverPoint).get_transform().get_forward(), normalized);
        if ((double) num2 >= 1.0 - (double) coverPoint.coverDot)
        {
          float num3 = Vector3.Distance(currentPosition, position);
          float num4 = 0.0f;
          if ((double) minRange > 0.0)
            num4 -= (float) ((1.0 - (double) Mathf.InverseLerp(0.0f, minRange, num3)) * 100.0);
          float num5 = Mathf.Abs((float) (position.y - currentPosition.y));
          float num6 = num4 + (float) ((1.0 - (double) Mathf.InverseLerp(1f, 5f, num5)) * 500.0) + Mathf.InverseLerp(1f - coverPoint.coverDot, 1f, num2) * 50f + (float) ((1.0 - (double) Mathf.InverseLerp(2f, maxRange, num3)) * 100.0);
          float num7 = 1f - Mathf.InverseLerp(4f, 10f, Vector3.Distance(currentPosition, hideFromPosition));
          vector3 = Vector3.op_Subtraction(((Component) coverPoint).get_transform().get_position(), currentPosition);
          float num8 = Vector3.Dot(((Vector3) ref vector3).get_normalized(), normalized);
          float num9 = num6 - Mathf.InverseLerp(-1f, 0.25f, num8) * 50f * num7;
          if ((double) num9 > (double) num1)
          {
            aiCoverPoint = coverPoint;
            num1 = num9;
          }
        }
      }
    }
    if (Object.op_Implicit((Object) aiCoverPoint))
      return aiCoverPoint;
    return (AICoverPoint) null;
  }

  public NavMeshLink GetClosestNavMeshLink(Vector3 pos)
  {
    NavMeshLink navMeshLink = (NavMeshLink) null;
    float num1 = float.PositiveInfinity;
    using (List<NavMeshLink>.Enumerator enumerator = this.navMeshLinks.GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        NavMeshLink current = enumerator.Current;
        float num2 = Vector3.Distance(((Component) current).get_gameObject().get_transform().get_position(), pos);
        if ((double) num2 < (double) num1)
        {
          navMeshLink = current;
          num1 = num2;
          if ((double) num2 < 0.25)
            break;
        }
      }
    }
    return navMeshLink;
  }

  public AIInformationZone()
  {
    base.\u002Ector();
  }
}
