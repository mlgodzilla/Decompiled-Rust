// Decompiled with JetBrains decompiler
// Type: ServerProjectile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ServerProjectile : EntityComponent<BaseEntity>, IServerComponent
{
  public float gravityModifier = 1f;
  public float speed = 15f;
  public Vector3 _currentVelocity = Vector3.get_zero();
  public Vector3 initialVelocity;
  public float drag;
  public float scanRange;
  public Vector3 swimScale;
  public Vector3 swimSpeed;
  public float radius;
  private bool impacted;
  private float swimRandom;

  private void FixedUpdate()
  {
    if (!this.baseEntity.isServer)
      return;
    this.DoMovement();
  }

  public void DoMovement()
  {
    if (this.impacted)
      return;
    this._currentVelocity = Vector3.op_Addition(this._currentVelocity, Vector3.op_Multiply(Vector3.op_Multiply(Vector3.op_Multiply(Physics.get_gravity(), this.gravityModifier), Time.get_fixedDeltaTime()), Time.get_timeScale()));
    Vector3 vector3_1 = this._currentVelocity;
    if (Vector3.op_Inequality(this.swimScale, Vector3.get_zero()))
    {
      if ((double) this.swimRandom == 0.0)
        this.swimRandom = Random.Range(0.0f, 20f);
      float num = Time.get_time() + this.swimRandom;
      Vector3 vector3_2;
      ((Vector3) ref vector3_2).\u002Ector(Mathf.Sin(num * (float) this.swimSpeed.x) * (float) this.swimScale.x, Mathf.Cos(num * (float) this.swimSpeed.y) * (float) this.swimScale.y, Mathf.Sin(num * (float) this.swimSpeed.z) * (float) this.swimScale.z);
      Vector3 vector3_3 = ((Component) this).get_transform().InverseTransformDirection(vector3_2);
      vector3_1 = Vector3.op_Addition(vector3_1, vector3_3);
    }
    float num1 = ((Vector3) ref vector3_1).get_magnitude() * Time.get_fixedDeltaTime();
    RaycastHit hitInfo;
    if (GamePhysics.Trace(new Ray(((Component) this).get_transform().get_position(), ((Vector3) ref vector3_1).get_normalized()), this.radius, out hitInfo, num1 + this.scanRange, 1236478737, (QueryTriggerInteraction) 0))
    {
      BaseEntity entity = hitInfo.GetEntity();
      if (!entity.IsValid() || !this.baseEntity.creatorEntity.IsValid() || entity.net.ID != this.baseEntity.creatorEntity.net.ID)
      {
        Transform transform = ((Component) this).get_transform();
        transform.set_position(Vector3.op_Addition(transform.get_position(), Vector3.op_Multiply(((Component) this).get_transform().get_forward(), ((RaycastHit) ref hitInfo).get_distance())));
        ((Component) this).SendMessage("ProjectileImpact", (object) hitInfo, (SendMessageOptions) 1);
        this.impacted = true;
        return;
      }
    }
    Transform transform1 = ((Component) this).get_transform();
    transform1.set_position(Vector3.op_Addition(transform1.get_position(), Vector3.op_Multiply(((Component) this).get_transform().get_forward(), num1)));
    ((Component) this).get_transform().set_rotation(Quaternion.LookRotation(((Vector3) ref vector3_1).get_normalized()));
  }

  public void InitializeVelocity(Vector3 overrideVel)
  {
    ((Component) this).get_transform().set_rotation(Quaternion.LookRotation(((Vector3) ref overrideVel).get_normalized()));
    this.initialVelocity = overrideVel;
    this._currentVelocity = overrideVel;
  }
}
