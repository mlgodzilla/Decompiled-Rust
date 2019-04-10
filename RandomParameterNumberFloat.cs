// Decompiled with JetBrains decompiler
// Type: RandomParameterNumberFloat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class RandomParameterNumberFloat : StateMachineBehaviour
{
  public string parameterName;
  public int min;
  public int max;

  public virtual void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    if (string.IsNullOrEmpty(this.parameterName))
      return;
    animator.SetFloat(this.parameterName, Mathf.Floor(Random.Range((float) this.min, (float) this.max + 0.5f)));
  }

  public RandomParameterNumberFloat()
  {
    base.\u002Ector();
  }
}
