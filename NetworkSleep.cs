// Decompiled with JetBrains decompiler
// Type: NetworkSleep
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class NetworkSleep : MonoBehaviour
{
  public static int totalBehavioursDisabled;
  public static int totalCollidersDisabled;
  public Behaviour[] behaviours;
  public Collider[] colliders;
  internal int BehavioursDisabled;
  internal int CollidersDisabled;

  public NetworkSleep()
  {
    base.\u002Ector();
  }
}
