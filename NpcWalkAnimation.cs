// Decompiled with JetBrains decompiler
// Type: NpcWalkAnimation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class NpcWalkAnimation : MonoBehaviour, IClientComponent
{
  public Vector3 HipFudge;
  public BaseNpc Npc;
  public Animator Animator;
  public Transform HipBone;
  public Transform LookBone;
  public bool UpdateWalkSpeed;
  public bool UpdateFacingDirection;
  public bool UpdateGroundNormal;
  public Transform alignmentRoot;
  public bool LaggyAss;
  public bool LookAtTarget;
  public float MaxLaggyAssRotation;
  public float MaxWalkAnimSpeed;
  public bool UseDirectionBlending;

  public NpcWalkAnimation()
  {
    base.\u002Ector();
  }
}
