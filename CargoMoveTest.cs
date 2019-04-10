// Decompiled with JetBrains decompiler
// Type: CargoMoveTest
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class CargoMoveTest : FacepunchBehaviour
{
  public int targetNodeIndex;
  private float currentThrottle;
  private float turnScale;

  private void Awake()
  {
    this.Invoke(new Action(this.FindInitialNode), 2f);
  }

  public void FindInitialNode()
  {
    this.targetNodeIndex = this.GetClosestNodeToUs();
  }

  private void Update()
  {
    this.UpdateMovement();
  }

  public void UpdateMovement()
  {
    if (TerrainMeta.Path.OceanPatrolFar == null || TerrainMeta.Path.OceanPatrolFar.Count == 0 || this.targetNodeIndex == -1)
      return;
    Vector3 vector3_1 = TerrainMeta.Path.OceanPatrolFar[this.targetNodeIndex];
    Vector3 vector3_2 = Vector3.op_Subtraction(vector3_1, ((Component) this).get_transform().get_position());
    Vector3 normalized = ((Vector3) ref vector3_2).get_normalized();
    float num1 = Mathf.InverseLerp(0.5f, 1f, Vector3.Dot(((Component) this).get_transform().get_forward(), normalized));
    float num2 = Vector3.Dot(((Component) this).get_transform().get_right(), normalized);
    float num3 = 5f;
    this.turnScale = Mathf.Lerp(this.turnScale, Mathf.InverseLerp(0.05f, 0.5f, Mathf.Abs(num2)), Time.get_deltaTime() * 0.2f);
    float num4 = (double) num2 < 0.0 ? -1f : 1f;
    ((Component) this).get_transform().Rotate(Vector3.get_up(), num3 * Time.get_deltaTime() * this.turnScale * num4, (Space) 0);
    this.currentThrottle = Mathf.Lerp(this.currentThrottle, num1, Time.get_deltaTime() * 0.2f);
    Transform transform = ((Component) this).get_transform();
    transform.set_position(Vector3.op_Addition(transform.get_position(), Vector3.op_Multiply(Vector3.op_Multiply(Vector3.op_Multiply(((Component) this).get_transform().get_forward(), 5f), Time.get_deltaTime()), this.currentThrottle)));
    if ((double) Vector3.Distance(((Component) this).get_transform().get_position(), vector3_1) >= 60.0)
      return;
    ++this.targetNodeIndex;
    if (this.targetNodeIndex < TerrainMeta.Path.OceanPatrolFar.Count)
      return;
    this.targetNodeIndex = 0;
  }

  public int GetClosestNodeToUs()
  {
    int num1 = 0;
    float num2 = float.PositiveInfinity;
    for (int index = 0; index < TerrainMeta.Path.OceanPatrolFar.Count; ++index)
    {
      float num3 = Vector3.Distance(((Component) this).get_transform().get_position(), TerrainMeta.Path.OceanPatrolFar[index]);
      if ((double) num3 < (double) num2)
      {
        num1 = index;
        num2 = num3;
      }
    }
    return num1;
  }

  public void OnDrawGizmosSelected()
  {
    if (TerrainMeta.Path.OceanPatrolFar == null)
      return;
    Gizmos.set_color(Color.get_red());
    Gizmos.DrawSphere(TerrainMeta.Path.OceanPatrolFar[this.targetNodeIndex], 10f);
    for (int index = 0; index < TerrainMeta.Path.OceanPatrolFar.Count; ++index)
    {
      Vector3 vector3 = TerrainMeta.Path.OceanPatrolFar[index];
      Gizmos.set_color(Color.get_green());
      Gizmos.DrawSphere(vector3, 3f);
      Gizmos.DrawLine(vector3, index + 1 == TerrainMeta.Path.OceanPatrolFar.Count ? TerrainMeta.Path.OceanPatrolFar[0] : TerrainMeta.Path.OceanPatrolFar[index + 1]);
    }
  }

  public CargoMoveTest()
  {
    base.\u002Ector();
  }
}
