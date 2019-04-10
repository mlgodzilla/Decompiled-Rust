// Decompiled with JetBrains decompiler
// Type: ObjectSpam
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ObjectSpam : MonoBehaviour
{
  public GameObject source;
  public int amount;
  public float radius;

  private void Start()
  {
    for (int index = 0; index < this.amount; ++index)
    {
      M0 m0 = Object.Instantiate<GameObject>((M0) this.source);
      ((GameObject) m0).get_transform().set_position(Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3Ex.Range(-this.radius, this.radius)));
      ((Object) m0).set_hideFlags((HideFlags) 3);
    }
  }

  public ObjectSpam()
  {
    base.\u002Ector();
  }
}
