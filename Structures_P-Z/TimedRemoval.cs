// Decompiled with JetBrains decompiler
// Type: TimedRemoval
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class TimedRemoval : MonoBehaviour
{
  public Object objectToDestroy;
  public float removeDelay;

  private void OnEnable()
  {
    Object.Destroy(this.objectToDestroy, this.removeDelay);
  }

  public TimedRemoval()
  {
    base.\u002Ector();
  }
}
