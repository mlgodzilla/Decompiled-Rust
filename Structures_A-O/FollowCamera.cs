// Decompiled with JetBrains decompiler
// Type: FollowCamera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class FollowCamera : MonoBehaviour, IClientComponent
{
  private void LateUpdate()
  {
    if (Object.op_Equality((Object) MainCamera.mainCamera, (Object) null))
      return;
    ((Component) this).get_transform().set_position(MainCamera.position);
  }

  public FollowCamera()
  {
    base.\u002Ector();
  }
}
