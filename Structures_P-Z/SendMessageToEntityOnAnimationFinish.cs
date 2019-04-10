// Decompiled with JetBrains decompiler
// Type: SendMessageToEntityOnAnimationFinish
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class SendMessageToEntityOnAnimationFinish : StateMachineBehaviour
{
  public string messageToSendToEntity;
  public float repeatRate;
  private const float lastMessageSent = 0.0f;

  public virtual void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    if (0.0 + (double) this.repeatRate > (double) Time.get_time() || animator.IsInTransition(layerIndex) || (double) ((AnimatorStateInfo) ref stateInfo).get_normalizedTime() < 1.0)
      return;
    for (int index = 0; index < animator.get_layerCount(); ++index)
    {
      if (index != layerIndex)
      {
        if (animator.IsInTransition(index))
          return;
        AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(index);
        if ((double) ((AnimatorStateInfo) ref animatorStateInfo).get_speed() > 0.0 && (double) ((AnimatorStateInfo) ref animatorStateInfo).get_normalizedTime() < 1.0)
          return;
      }
    }
    BaseEntity baseEntity = ((Component) animator).get_gameObject().ToBaseEntity();
    if (!Object.op_Implicit((Object) baseEntity))
      return;
    ((Component) baseEntity).SendMessage(this.messageToSendToEntity);
  }

  public SendMessageToEntityOnAnimationFinish()
  {
    base.\u002Ector();
  }
}
