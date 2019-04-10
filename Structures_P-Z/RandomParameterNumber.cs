// Decompiled with JetBrains decompiler
// Type: RandomParameterNumber
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class RandomParameterNumber : StateMachineBehaviour
{
  public string parameterName;
  public int min;
  public int max;

  public virtual void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    animator.SetInteger(this.parameterName, Random.Range(this.min, this.max));
  }

  public RandomParameterNumber()
  {
    base.\u002Ector();
  }
}
