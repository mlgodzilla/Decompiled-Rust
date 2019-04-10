// Decompiled with JetBrains decompiler
// Type: IgnoreCollision
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class IgnoreCollision : MonoBehaviour
{
  public Collider collider;

  protected void OnTriggerEnter(Collider other)
  {
    Debug.Log((object) ("IgnoreCollision: " + ((Object) ((Component) this.collider).get_gameObject()).get_name() + " + " + ((Object) ((Component) other).get_gameObject()).get_name()));
    Physics.IgnoreCollision(other, this.collider, true);
  }

  public IgnoreCollision()
  {
    base.\u002Ector();
  }
}
