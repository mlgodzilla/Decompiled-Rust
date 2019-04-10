// Decompiled with JetBrains decompiler
// Type: Ragdoll
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class Ragdoll : BaseMonoBehaviour
{
  public Transform eyeTransform;
  public Transform centerBone;
  public Rigidbody primaryBody;
  public PhysicMaterial physicMaterial;
  public SpringJoint corpseJoint;
  public GameObject GibEffect;
}
