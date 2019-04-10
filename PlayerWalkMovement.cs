// Decompiled with JetBrains decompiler
// Type: PlayerWalkMovement
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class PlayerWalkMovement : BaseMovement
{
  public float capsuleHeight = 1f;
  public float capsuleCenter = 1f;
  public float capsuleHeightDucked = 1f;
  public float capsuleCenterDucked = 1f;
  public float gravityTestRadius = 0.2f;
  public float gravityMultiplier = 2.5f;
  public float gravityMultiplierSwimming = 0.1f;
  public float maxAngleWalking = 50f;
  public float maxAngleClimbing = 60f;
  public float maxAngleSliding = 90f;
  public const float WaterLevelHead = 0.75f;
  public const float WaterLevelNeck = 0.65f;
  public PhysicMaterial zeroFrictionMaterial;
  public PhysicMaterial highFrictionMaterial;
}
