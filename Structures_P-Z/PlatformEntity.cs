// Decompiled with JetBrains decompiler
// Type: PlatformEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class PlatformEntity : BaseEntity
{
  private Vector3 targetPosition = Vector3.get_zero();
  private Quaternion targetRotation = Quaternion.get_identity();
  private const float movementSpeed = 1f;
  private const float rotationSpeed = 10f;
  private const float radius = 10f;

  protected void FixedUpdate()
  {
    if (this.isClient)
      return;
    if (Vector3.op_Equality(this.targetPosition, Vector3.get_zero()) || (double) Vector3.Distance(((Component) this).get_transform().get_position(), this.targetPosition) < 0.00999999977648258)
    {
      Vector2 vector2 = Vector2.op_Multiply(Random.get_insideUnitCircle(), 10f);
      this.targetPosition = Vector3.op_Addition(((Component) this).get_transform().get_position(), new Vector3((float) vector2.x, 0.0f, (float) vector2.y));
      if (Object.op_Inequality((Object) TerrainMeta.HeightMap, (Object) null) && Object.op_Inequality((Object) TerrainMeta.WaterMap, (Object) null))
        this.targetPosition.y = (__Null) ((double) Mathf.Max(TerrainMeta.HeightMap.GetHeight(this.targetPosition), TerrainMeta.WaterMap.GetHeight(this.targetPosition)) + 1.0);
      this.targetRotation = Quaternion.LookRotation(Vector3.op_Subtraction(this.targetPosition, ((Component) this).get_transform().get_position()));
    }
    ((Component) this).get_transform().set_position(Vector3.MoveTowards(((Component) this).get_transform().get_position(), this.targetPosition, Time.get_fixedDeltaTime() * 1f));
    ((Component) this).get_transform().set_rotation(Quaternion.RotateTowards(((Component) this).get_transform().get_rotation(), this.targetRotation, Time.get_fixedDeltaTime() * 10f));
  }

  public override bool PhysicsDriven()
  {
    return true;
  }
}
