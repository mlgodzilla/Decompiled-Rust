// Decompiled with JetBrains decompiler
// Type: AITraversalArea
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class AITraversalArea : TriggerBase
{
  public Transform entryPoint1;
  public Transform entryPoint2;
  public AITraversalWaitPoint[] waitPoints;
  public Bounds movementArea;
  public Transform activeEntryPoint;
  public float nextFreeTime;

  public void OnValidate()
  {
    ((Bounds) ref this.movementArea).set_center(((Component) this).get_transform().get_position());
  }

  internal override GameObject InterestedInObject(GameObject obj)
  {
    obj = base.InterestedInObject(obj);
    if (Object.op_Equality((Object) obj, (Object) null))
      return (GameObject) null;
    BaseEntity baseEntity = obj.ToBaseEntity();
    if (Object.op_Equality((Object) baseEntity, (Object) null))
      return (GameObject) null;
    if (baseEntity.isClient)
      return (GameObject) null;
    if (!baseEntity.IsNpc)
      return (GameObject) null;
    return ((Component) baseEntity).get_gameObject();
  }

  public bool CanTraverse(BaseEntity ent)
  {
    return (double) Time.get_time() > (double) this.nextFreeTime;
  }

  public Transform GetClosestEntry(Vector3 position)
  {
    if ((double) Vector3.Distance(position, this.entryPoint1.get_position()) < (double) Vector3.Distance(position, this.entryPoint2.get_position()))
      return this.entryPoint1;
    return this.entryPoint2;
  }

  public Transform GetFarthestEntry(Vector3 position)
  {
    if ((double) Vector3.Distance(position, this.entryPoint1.get_position()) > (double) Vector3.Distance(position, this.entryPoint2.get_position()))
      return this.entryPoint1;
    return this.entryPoint2;
  }

  public void SetBusyFor(float dur = 1f)
  {
    this.nextFreeTime = Time.get_time() + dur;
  }

  public bool CanUse(Vector3 dirFrom)
  {
    return (double) Time.get_time() > (double) this.nextFreeTime;
  }

  internal override void OnEntityEnter(BaseEntity ent)
  {
    base.OnEntityEnter(ent);
    ((Component) ent).GetComponent<HumanNPC>();
    Debug.Log((object) "Enter Traversal Area");
  }

  public AITraversalWaitPoint GetEntryPointNear(Vector3 pos)
  {
    Vector3 position1 = this.GetClosestEntry(pos).get_position();
    Vector3 position2 = this.GetFarthestEntry(pos).get_position();
    BaseEntity[] baseEntityArray = new BaseEntity[1];
    AITraversalWaitPoint traversalWaitPoint = (AITraversalWaitPoint) null;
    float num1 = 0.0f;
    foreach (AITraversalWaitPoint waitPoint in this.waitPoints)
    {
      if (!waitPoint.Occupied())
      {
        Vector3 position3 = ((Component) waitPoint).get_transform().get_position();
        float num2 = Vector3.Distance(position1, position3);
        if ((double) Vector3.Distance(position2, position3) >= (double) num2)
        {
          float num3 = (float) ((1.0 - (double) Mathf.InverseLerp(0.0f, 20f, Vector3.Distance(position3, pos))) * 100.0);
          if ((double) num3 > (double) num1)
          {
            num1 = num3;
            traversalWaitPoint = waitPoint;
          }
        }
      }
    }
    return traversalWaitPoint;
  }

  public bool EntityFilter(BaseEntity ent)
  {
    if (ent.IsNpc)
      return ent.isServer;
    return false;
  }

  internal override void OnEntityLeave(BaseEntity ent)
  {
    base.OnEntityLeave(ent);
    ((Component) ent).GetComponent<HumanNPC>();
    Debug.Log((object) "Leave Traversal Area");
  }

  public void OnDrawGizmos()
  {
    Gizmos.set_color(Color.get_magenta());
    Gizmos.DrawCube(Vector3.op_Addition(this.entryPoint1.get_position(), Vector3.op_Multiply(Vector3.get_up(), 0.125f)), new Vector3(0.5f, 0.25f, 0.5f));
    Gizmos.DrawCube(Vector3.op_Addition(this.entryPoint2.get_position(), Vector3.op_Multiply(Vector3.get_up(), 0.125f)), new Vector3(0.5f, 0.25f, 0.5f));
    Gizmos.set_color(new Color(0.2f, 1f, 0.2f, 0.5f));
    Gizmos.DrawCube(((Bounds) ref this.movementArea).get_center(), ((Bounds) ref this.movementArea).get_size());
    Gizmos.set_color(Color.get_magenta());
    foreach (Component waitPoint in this.waitPoints)
      GizmosUtil.DrawCircleY(waitPoint.get_transform().get_position(), 0.5f);
  }
}
