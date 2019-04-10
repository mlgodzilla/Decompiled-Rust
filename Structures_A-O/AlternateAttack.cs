// Decompiled with JetBrains decompiler
// Type: AlternateAttack
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class AlternateAttack : StateMachineBehaviour
{
  public bool random;
  public bool dontIncrement;
  public string[] targetTransitions;

  public virtual void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    if (this.random)
    {
      string targetTransition = this.targetTransitions[Random.Range(0, this.targetTransitions.Length)];
      animator.Play(targetTransition, layerIndex, 0.0f);
    }
    else
    {
      int integer = animator.GetInteger("lastAttack");
      string targetTransition = this.targetTransitions[integer % this.targetTransitions.Length];
      animator.Play(targetTransition, layerIndex, 0.0f);
      if (this.dontIncrement)
        return;
      animator.SetInteger("lastAttack", integer + 1);
    }
  }

  public AlternateAttack()
  {
    base.\u002Ector();
  }
}
