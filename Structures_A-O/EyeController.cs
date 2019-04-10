// Decompiled with JetBrains decompiler
// Type: EyeController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class EyeController : MonoBehaviour
{
  public const float MaxLookDot = 0.8f;
  public bool debug;
  public Transform LeftEye;
  public Transform RightEye;
  public Transform EyeTransform;
  public Vector3 Fudge;
  public Vector3 FlickerRange;
  private Transform Focus;
  private float FocusUpdateTime;
  private Vector3 Flicker;
  private Vector3 FlickerTarget;
  private float TimeToUpdateFlicker;
  private float FlickerSpeed;

  public void UpdateEyes()
  {
    Vector3 defaultLookAtPos = Vector3.op_Addition(this.EyeTransform.get_position(), Vector3.op_Multiply(this.EyeTransform.get_forward(), 100f));
    Vector3 LookAt = defaultLookAtPos;
    this.UpdateFocus(defaultLookAtPos);
    this.UpdateFlicker();
    if (Object.op_Inequality((Object) this.Focus, (Object) null))
    {
      LookAt = this.Focus.get_position();
      Vector3 vector3_1 = Vector3.op_Subtraction(this.EyeTransform.get_position(), defaultLookAtPos);
      Vector3 vector3_2 = Vector3.op_Subtraction(this.EyeTransform.get_position(), LookAt);
      if ((double) Vector3.Dot(((Vector3) ref vector3_1).get_normalized(), ((Vector3) ref vector3_2).get_normalized()) < 0.800000011920929)
        this.Focus = (Transform) null;
    }
    this.UpdateEye(this.LeftEye, LookAt);
    this.UpdateEye(this.RightEye, LookAt);
  }

  private void UpdateEye(Transform eye, Vector3 LookAt)
  {
    Vector3 vector3 = Vector3.op_Subtraction(LookAt, eye.get_position());
    eye.set_rotation(Quaternion.op_Multiply(Quaternion.op_Multiply(Quaternion.LookRotation(((Vector3) ref vector3).get_normalized(), this.EyeTransform.get_up()), Quaternion.Euler(this.Fudge)), Quaternion.Euler(this.Flicker)));
  }

  private void UpdateFlicker()
  {
    this.TimeToUpdateFlicker -= Time.get_deltaTime();
    this.Flicker = Vector3.Lerp(this.Flicker, this.FlickerTarget, Time.get_deltaTime() * this.FlickerSpeed);
    if ((double) this.TimeToUpdateFlicker >= 0.0)
      return;
    this.TimeToUpdateFlicker = Random.Range(0.2f, 2f);
    this.FlickerTarget = Vector3.op_Multiply(new Vector3(Random.Range((float) -this.FlickerRange.x, (float) this.FlickerRange.x), Random.Range((float) -this.FlickerRange.y, (float) this.FlickerRange.y), Random.Range((float) -this.FlickerRange.z, (float) this.FlickerRange.z)), Object.op_Implicit((Object) this.Focus) ? 0.01f : 1f);
    this.FlickerSpeed = Random.Range(10f, 30f);
  }

  private void UpdateFocus(Vector3 defaultLookAtPos)
  {
  }

  public EyeController()
  {
    base.\u002Ector();
  }
}
