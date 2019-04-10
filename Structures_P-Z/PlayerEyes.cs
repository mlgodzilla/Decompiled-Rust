// Decompiled with JetBrains decompiler
// Type: PlayerEyes
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class PlayerEyes : EntityComponent<BasePlayer>
{
  public static readonly Vector3 EyeOffset = new Vector3(0.0f, 1.5f, 0.0f);
  public static readonly Vector3 DuckOffset = new Vector3(0.0f, -0.6f, 0.0f);
  public Vector3 thirdPersonSleepingOffset = new Vector3(0.43f, 1.25f, 0.7f);
  private Vector3 viewOffset = Vector3.get_zero();
  public LazyAimProperties defaultLazyAim;

  public Vector3 worldMountedPosition
  {
    get
    {
      if (Object.op_Implicit((Object) this.baseEntity) && this.baseEntity.isMounted)
      {
        Vector3 vector3 = this.baseEntity.GetMounted().EyePositionForPlayer(this.baseEntity);
        if (Vector3.op_Inequality(vector3, Vector3.get_zero()))
          return vector3;
      }
      return this.worldStandingPosition;
    }
  }

  public Vector3 worldStandingPosition
  {
    get
    {
      return Vector3.op_Addition(((Component) this).get_transform().get_position(), PlayerEyes.EyeOffset);
    }
  }

  public Vector3 worldCrouchedPosition
  {
    get
    {
      return Vector3.op_Addition(this.worldStandingPosition, PlayerEyes.DuckOffset);
    }
  }

  public Vector3 position
  {
    get
    {
      if (Object.op_Implicit((Object) this.baseEntity) && this.baseEntity.isMounted)
      {
        Vector3 vector3 = this.baseEntity.GetMounted().EyePositionForPlayer(this.baseEntity);
        if (Vector3.op_Inequality(vector3, Vector3.get_zero()))
          return vector3;
      }
      return Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(((Component) this).get_transform().get_up(), (float) (PlayerEyes.EyeOffset.y + this.viewOffset.y)));
    }
  }

  public Vector3 center
  {
    get
    {
      if (Object.op_Implicit((Object) this.baseEntity) && this.baseEntity.isMounted)
      {
        Vector3 vector3 = this.baseEntity.GetMounted().EyePositionForPlayer(this.baseEntity);
        if (Vector3.op_Inequality(vector3, Vector3.get_zero()))
          return vector3;
      }
      return Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(((Component) this).get_transform().get_up(), (float) (PlayerEyes.EyeOffset.y + PlayerEyes.DuckOffset.y)));
    }
  }

  public Vector3 offset
  {
    get
    {
      return Vector3.op_Multiply(((Component) this).get_transform().get_up(), (float) (PlayerEyes.EyeOffset.y + this.viewOffset.y));
    }
  }

  public Quaternion rotation
  {
    get
    {
      return Quaternion.op_Multiply(this.parentRotation, this.bodyRotation);
    }
    set
    {
      this.bodyRotation = Quaternion.op_Multiply(Quaternion.Inverse(this.parentRotation), value);
    }
  }

  public Quaternion bodyRotation { get; set; }

  public Quaternion headRotation { get; set; }

  public Quaternion rotationLook { get; set; }

  public Quaternion parentRotation
  {
    get
    {
      if (!Object.op_Inequality((Object) ((Component) this).get_transform().get_parent(), (Object) null))
        return Quaternion.get_identity();
      Quaternion rotation = ((Component) this).get_transform().get_parent().get_rotation();
      return Quaternion.Euler(0.0f, (float) ((Quaternion) ref rotation).get_eulerAngles().y, 0.0f);
    }
  }

  public void NetworkUpdate(Quaternion rot)
  {
    this.viewOffset = this.baseEntity.IsDucked() ? PlayerEyes.DuckOffset : Vector3.get_zero();
    this.bodyRotation = rot;
    this.headRotation = Quaternion.get_identity();
  }

  public Vector3 MovementForward()
  {
    Quaternion rotation = this.rotation;
    return Quaternion.op_Multiply(Quaternion.Euler(new Vector3(0.0f, (float) ((Quaternion) ref rotation).get_eulerAngles().y, 0.0f)), Vector3.get_forward());
  }

  public Vector3 MovementRight()
  {
    Quaternion rotation = this.rotation;
    return Quaternion.op_Multiply(Quaternion.Euler(new Vector3(0.0f, (float) ((Quaternion) ref rotation).get_eulerAngles().y, 0.0f)), Vector3.get_right());
  }

  public Ray BodyRay()
  {
    return new Ray(this.position, this.BodyForward());
  }

  public Vector3 BodyForward()
  {
    return Quaternion.op_Multiply(this.rotation, Vector3.get_forward());
  }

  public Vector3 BodyRight()
  {
    return Quaternion.op_Multiply(this.rotation, Vector3.get_right());
  }

  public Vector3 BodyUp()
  {
    return Quaternion.op_Multiply(this.rotation, Vector3.get_up());
  }

  public Ray HeadRay()
  {
    return new Ray(this.position, this.HeadForward());
  }

  public Vector3 HeadForward()
  {
    return Quaternion.op_Multiply(Quaternion.op_Multiply(this.rotation, this.headRotation), Vector3.get_forward());
  }

  public Vector3 HeadRight()
  {
    return Quaternion.op_Multiply(Quaternion.op_Multiply(this.rotation, this.headRotation), Vector3.get_right());
  }

  public Vector3 HeadUp()
  {
    return Quaternion.op_Multiply(Quaternion.op_Multiply(this.rotation, this.headRotation), Vector3.get_up());
  }
}
