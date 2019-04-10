// Decompiled with JetBrains decompiler
// Type: DoorAnimEvents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using UnityEngine;

public class DoorAnimEvents : MonoBehaviour, IClientComponent
{
  public GameObjectRef openStart;
  public GameObjectRef openEnd;
  public GameObjectRef closeStart;
  public GameObjectRef closeEnd;

  public Animator animator
  {
    get
    {
      return (Animator) ((Component) this).GetComponent<Animator>();
    }
  }

  private void DoorOpenStart()
  {
    if (Application.isLoading != null || !this.openStart.isValid || this.animator.IsInTransition(0))
      return;
    AnimatorStateInfo animatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
    if ((double) ((AnimatorStateInfo) ref animatorStateInfo).get_normalizedTime() > 0.5)
      return;
    Effect.client.Run(this.openStart.resourcePath, ((Component) this).get_gameObject());
  }

  private void DoorOpenEnd()
  {
    if (Application.isLoading != null || !this.openEnd.isValid || this.animator.IsInTransition(0))
      return;
    AnimatorStateInfo animatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
    if ((double) ((AnimatorStateInfo) ref animatorStateInfo).get_normalizedTime() < 0.5)
      return;
    Effect.client.Run(this.openEnd.resourcePath, ((Component) this).get_gameObject());
  }

  private void DoorCloseStart()
  {
    if (Application.isLoading != null || !this.closeStart.isValid || this.animator.IsInTransition(0))
      return;
    AnimatorStateInfo animatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
    if ((double) ((AnimatorStateInfo) ref animatorStateInfo).get_normalizedTime() > 0.5)
      return;
    Effect.client.Run(this.closeStart.resourcePath, ((Component) this).get_gameObject());
  }

  private void DoorCloseEnd()
  {
    if (Application.isLoading != null || !this.closeEnd.isValid || this.animator.IsInTransition(0))
      return;
    AnimatorStateInfo animatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
    if ((double) ((AnimatorStateInfo) ref animatorStateInfo).get_normalizedTime() < 0.5)
      return;
    Effect.client.Run(this.closeEnd.resourcePath, ((Component) this).get_gameObject());
  }

  public DoorAnimEvents()
  {
    base.\u002Ector();
  }
}
