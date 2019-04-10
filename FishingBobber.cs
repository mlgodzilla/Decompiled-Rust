// Decompiled with JetBrains decompiler
// Type: FishingBobber
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class FishingBobber : BaseCombatEntity
{
  public Transform centerOfMass;
  public Rigidbody myRigidBody;

  public override void ServerInit()
  {
    this.myRigidBody.set_centerOfMass(this.centerOfMass.get_localPosition());
    base.ServerInit();
  }
}
