// Decompiled with JetBrains decompiler
// Type: BasePath
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class BasePath : MonoBehaviour
{
  public List<BasePathNode> nodes;
  public List<PathInterestNode> interestZones;
  public List<PathSpeedZone> speedZones;

  public void Start()
  {
  }

  public void GetNodesNear(Vector3 point, ref List<BasePathNode> nearNodes, float dist = 10f)
  {
    foreach (BasePathNode node in this.nodes)
    {
      Vector3 vector3 = Vector3.op_Subtraction(Vector3Ex.XZ(point), Vector3Ex.XZ(((Component) node).get_transform().get_position()));
      if ((double) ((Vector3) ref vector3).get_sqrMagnitude() <= (double) dist * (double) dist)
        nearNodes.Add(node);
    }
  }

  public BasePathNode GetClosestToPoint(Vector3 point)
  {
    BasePathNode basePathNode = this.nodes[0];
    float num = float.PositiveInfinity;
    foreach (BasePathNode node in this.nodes)
    {
      if (!Object.op_Equality((Object) node, (Object) null) && !Object.op_Equality((Object) ((Component) node).get_transform(), (Object) null))
      {
        Vector3 vector3 = Vector3.op_Subtraction(point, ((Component) node).get_transform().get_position());
        float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
        if ((double) sqrMagnitude < (double) num)
        {
          num = sqrMagnitude;
          basePathNode = node;
        }
      }
    }
    return basePathNode;
  }

  public PathInterestNode GetRandomInterestNodeAwayFrom(Vector3 from, float dist = 10f)
  {
    PathInterestNode pathInterestNode = (PathInterestNode) null;
    for (int index = 0; Object.op_Equality((Object) pathInterestNode, (Object) null) && index < 20; ++index)
    {
      pathInterestNode = this.interestZones[Random.Range(0, this.interestZones.Count)];
      Vector3 vector3 = Vector3.op_Subtraction(((Component) pathInterestNode).get_transform().get_position(), from);
      if ((double) ((Vector3) ref vector3).get_sqrMagnitude() < 100.0)
        pathInterestNode = (PathInterestNode) null;
      else
        break;
    }
    if (Object.op_Equality((Object) pathInterestNode, (Object) null))
      pathInterestNode = this.interestZones[0];
    return pathInterestNode;
  }

  public BasePath()
  {
    base.\u002Ector();
  }
}
