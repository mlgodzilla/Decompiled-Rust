// Decompiled with JetBrains decompiler
// Type: CachedTransform`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public struct CachedTransform<T> where T : Component
{
  public T component;
  public Vector3 position;
  public Quaternion rotation;
  public Vector3 localScale;

  public CachedTransform(T instance)
  {
    this.component = instance;
    if (Object.op_Implicit((Object) (object) this.component))
    {
      this.position = ((Component) (object) this.component).get_transform().get_position();
      this.rotation = ((Component) (object) this.component).get_transform().get_rotation();
      this.localScale = ((Component) (object) this.component).get_transform().get_localScale();
    }
    else
    {
      this.position = Vector3.get_zero();
      this.rotation = Quaternion.get_identity();
      this.localScale = Vector3.get_one();
    }
  }

  public void Apply()
  {
    if (!Object.op_Implicit((Object) (object) this.component))
      return;
    ((Component) (object) this.component).get_transform().SetPositionAndRotation(this.position, this.rotation);
    ((Component) (object) this.component).get_transform().set_localScale(this.localScale);
  }

  public void RotateAround(Vector3 center, Vector3 axis, float angle)
  {
    Quaternion quaternion = Quaternion.AngleAxis(angle, axis);
    Vector3 vector3 = Quaternion.op_Multiply(quaternion, Vector3.op_Subtraction(this.position, center));
    this.position = Vector3.op_Addition(center, vector3);
    this.rotation = Quaternion.op_Multiply(this.rotation, Quaternion.op_Multiply(Quaternion.op_Multiply(Quaternion.Inverse(this.rotation), quaternion), this.rotation));
  }

  public Matrix4x4 localToWorldMatrix
  {
    get
    {
      return Matrix4x4.TRS(this.position, this.rotation, this.localScale);
    }
  }

  public Matrix4x4 worldToLocalMatrix
  {
    get
    {
      Matrix4x4 localToWorldMatrix = this.localToWorldMatrix;
      return ((Matrix4x4) ref localToWorldMatrix).get_inverse();
    }
  }

  public Vector3 forward
  {
    get
    {
      return Quaternion.op_Multiply(this.rotation, Vector3.get_forward());
    }
  }

  public Vector3 up
  {
    get
    {
      return Quaternion.op_Multiply(this.rotation, Vector3.get_up());
    }
  }

  public Vector3 right
  {
    get
    {
      return Quaternion.op_Multiply(this.rotation, Vector3.get_right());
    }
  }

  public static implicit operator bool(CachedTransform<T> instance)
  {
    return Object.op_Inequality((Object) (object) instance.component, (Object) null);
  }
}
