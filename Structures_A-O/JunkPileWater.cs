// Decompiled with JetBrains decompiler
// Type: JunkPileWater
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class JunkPileWater : JunkPile
{
  private Quaternion baseRotation = Quaternion.get_identity();
  private bool first = true;
  public Transform[] buoyancyPoints;
  public bool debugDraw;

  public override void Spawn()
  {
    Vector3 position = ((Component) this).get_transform().get_position();
    position.y = (__Null) (double) TerrainMeta.WaterMap.GetHeight(((Component) this).get_transform().get_position());
    ((Component) this).get_transform().set_position(position);
    base.Spawn();
    Quaternion rotation = ((Component) this).get_transform().get_rotation();
    this.baseRotation = Quaternion.Euler(0.0f, (float) ((Quaternion) ref rotation).get_eulerAngles().y, 0.0f);
  }

  public void FixedUpdate()
  {
    if (this.isClient)
      return;
    this.UpdateMovement();
  }

  public void UpdateMovement()
  {
    if (this.isSinking)
      return;
    ((Component) this).get_transform().set_position(new Vector3((float) ((Component) this).get_transform().get_position().x, WaterSystem.GetHeight(((Component) this).get_transform().get_position()), (float) ((Component) this).get_transform().get_position().z));
    if (this.buoyancyPoints == null || this.buoyancyPoints.Length < 3)
      return;
    Vector3 position = ((Component) this).get_transform().get_position();
    Vector3 localPosition1 = this.buoyancyPoints[0].get_localPosition();
    Vector3 localPosition2 = this.buoyancyPoints[1].get_localPosition();
    Vector3 localPosition3 = this.buoyancyPoints[2].get_localPosition();
    Vector3 pos1 = Vector3.op_Addition(localPosition1, position);
    Vector3 pos2 = Vector3.op_Addition(localPosition2, position);
    Vector3 vector3_1 = position;
    Vector3 pos3 = Vector3.op_Addition(localPosition3, vector3_1);
    pos1.y = (__Null) (double) WaterSystem.GetHeight(pos1);
    pos2.y = (__Null) (double) WaterSystem.GetHeight(pos2);
    pos3.y = (__Null) (double) WaterSystem.GetHeight(pos3);
    ((Component) this).get_transform().set_position(new Vector3((float) position.x, (float) (pos1.y - localPosition1.y), (float) position.z));
    Vector3 vector3_2 = Vector3.op_Subtraction(pos2, pos1);
    Vector3 vector3_3 = Vector3.Cross(Vector3.op_Subtraction(pos3, pos1), vector3_2);
    Quaternion quaternion1 = Quaternion.LookRotation(new Vector3((float) vector3_3.x, (float) vector3_3.z, (float) vector3_3.y));
    Vector3 eulerAngles = ((Quaternion) ref quaternion1).get_eulerAngles();
    Quaternion quaternion2 = Quaternion.Euler((float) -eulerAngles.x, 0.0f, (float) -eulerAngles.y);
    if (this.first)
    {
      Quaternion rotation = ((Component) this).get_transform().get_rotation();
      this.baseRotation = Quaternion.Euler(0.0f, (float) ((Quaternion) ref rotation).get_eulerAngles().y, 0.0f);
      this.first = false;
    }
    ((Component) this).get_transform().set_rotation(Quaternion.op_Multiply(quaternion2, this.baseRotation));
  }
}
