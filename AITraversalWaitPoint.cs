// Decompiled with JetBrains decompiler
// Type: AITraversalWaitPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class AITraversalWaitPoint : MonoBehaviour
{
  public float nextFreeTime;

  public bool Occupied()
  {
    return (double) Time.get_time() > (double) this.nextFreeTime;
  }

  public void Occupy(float dur = 1f)
  {
    this.nextFreeTime = Time.get_time() + dur;
  }

  public AITraversalWaitPoint()
  {
    base.\u002Ector();
  }
}
