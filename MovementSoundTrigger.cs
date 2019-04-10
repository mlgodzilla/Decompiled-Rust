// Decompiled with JetBrains decompiler
// Type: MovementSoundTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class MovementSoundTrigger : TriggerBase, IClientComponentEx, ILOD
{
  public SoundDefinition softSound;
  public SoundDefinition medSound;
  public SoundDefinition hardSound;
  public Collider collider;

  public virtual void PreClientComponentCull(IPrefabProcessor p)
  {
    p.RemoveComponent((Component) this.collider);
    p.RemoveComponent((Component) this);
    p.NominateForDeletion(((Component) this).get_gameObject());
  }
}
