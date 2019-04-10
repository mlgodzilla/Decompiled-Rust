// Decompiled with JetBrains decompiler
// Type: RotateCameraAroundObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class RotateCameraAroundObject : MonoBehaviour
{
  public GameObject m_goObjectToRotateAround;
  public float m_flRotateSpeed;

  private void FixedUpdate()
  {
    if (!Object.op_Inequality((Object) this.m_goObjectToRotateAround, (Object) null))
      return;
    ((Component) this).get_transform().LookAt(Vector3.op_Addition(this.m_goObjectToRotateAround.get_transform().get_position(), Vector3.op_Multiply(Vector3.get_up(), 0.75f)));
    ((Component) this).get_transform().Translate(Vector3.op_Multiply(Vector3.op_Multiply(Vector3.get_right(), this.m_flRotateSpeed), Time.get_deltaTime()));
  }

  public RotateCameraAroundObject()
  {
    base.\u002Ector();
  }
}
