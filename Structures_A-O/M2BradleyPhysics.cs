// Decompiled with JetBrains decompiler
// Type: M2BradleyPhysics
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class M2BradleyPhysics : MonoBehaviour
{
  private m2bradleyAnimator m2Animator;
  public WheelCollider[] Wheels;
  public WheelCollider[] TurningWheels;
  public Rigidbody mainRigidbody;
  public Transform[] waypoints;
  private Vector3 currentWaypoint;
  private Vector3 nextWaypoint;

  public M2BradleyPhysics()
  {
    base.\u002Ector();
  }
}
