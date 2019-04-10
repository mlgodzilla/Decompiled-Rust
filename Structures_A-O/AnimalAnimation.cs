// Decompiled with JetBrains decompiler
// Type: AnimalAnimation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class AnimalAnimation : MonoBehaviour, IClientComponent
{
  public BaseNpc Target;
  public Animator Animator;
  public MaterialEffect FootstepEffects;
  public Transform[] Feet;
  [ReadOnly]
  public string BaseFolder;

  public AnimalAnimation()
  {
    base.\u002Ector();
  }
}
