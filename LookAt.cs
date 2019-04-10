// Decompiled with JetBrains decompiler
// Type: LookAt
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
public class LookAt : MonoBehaviour, IClientComponent
{
  public Transform target;

  private void Update()
  {
    if (Object.op_Equality((Object) this.target, (Object) null))
      return;
    ((Component) this).get_transform().LookAt(this.target);
  }

  public LookAt()
  {
    base.\u002Ector();
  }
}
