// Decompiled with JetBrains decompiler
// Type: AnimationEventForward
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class AnimationEventForward : MonoBehaviour
{
  public GameObject targetObject;

  public void Event(string type)
  {
    this.targetObject.SendMessage(type);
  }

  public AnimationEventForward()
  {
    base.\u002Ector();
  }
}
