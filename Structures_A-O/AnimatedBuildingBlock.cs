// Decompiled with JetBrains decompiler
// Type: AnimatedBuildingBlock
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using UnityEngine;

public class AnimatedBuildingBlock : StabilityEntity
{
  private bool animatorNeedsInitializing = true;
  private bool animatorIsOpen = true;
  private bool isAnimating;

  public override void ServerInit()
  {
    base.ServerInit();
    if (Application.isLoadingSave != null)
      return;
    this.UpdateAnimationParameters(true);
  }

  public override void PostServerLoad()
  {
    base.PostServerLoad();
    this.UpdateAnimationParameters(true);
  }

  public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
  {
    base.OnFlagsChanged(old, next);
    this.UpdateAnimationParameters(false);
  }

  protected void UpdateAnimationParameters(bool init)
  {
    if (!Object.op_Implicit((Object) this.model) || !Object.op_Implicit((Object) this.model.animator) || !this.model.animator.get_isInitialized())
      return;
    int num = this.animatorNeedsInitializing || this.animatorIsOpen != this.IsOpen() ? 1 : (!init ? 0 : (this.isAnimating ? 1 : 0));
    bool flag = this.animatorNeedsInitializing | init;
    if (num != 0)
    {
      this.isAnimating = true;
      ((Behaviour) this.model.animator).set_enabled(true);
      this.model.animator.SetBool("open", this.animatorIsOpen = this.IsOpen());
      if (flag)
      {
        this.model.animator.set_fireEvents(false);
        for (int index = 0; (double) index < 20.0; ++index)
          this.model.animator.Update(1f);
        this.PutAnimatorToSleep();
      }
      else
      {
        this.model.animator.set_fireEvents(this.isClient);
        if (this.isServer)
          this.SetFlag(BaseEntity.Flags.Busy, true, false, true);
      }
    }
    else if (flag)
      this.PutAnimatorToSleep();
    this.animatorNeedsInitializing = false;
  }

  protected void OnAnimatorFinished()
  {
    if (!this.isAnimating)
      this.PutAnimatorToSleep();
    this.isAnimating = false;
  }

  private void PutAnimatorToSleep()
  {
    if (!Object.op_Implicit((Object) this.model) || !Object.op_Implicit((Object) this.model.animator))
    {
      Debug.LogWarning((object) (((Component) this).get_transform().GetRecursiveName("") + " has missing model/animator"), (Object) ((Component) this).get_gameObject());
    }
    else
    {
      ((Behaviour) this.model.animator).set_enabled(false);
      if (this.isServer)
        this.SetFlag(BaseEntity.Flags.Busy, false, false, true);
      this.OnAnimatorDisabled();
    }
  }

  protected virtual void OnAnimatorDisabled()
  {
  }

  public override bool SupportsChildDeployables()
  {
    return false;
  }
}
