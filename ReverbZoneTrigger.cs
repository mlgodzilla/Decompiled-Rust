// Decompiled with JetBrains decompiler
// Type: ReverbZoneTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ReverbZoneTrigger : TriggerBase, IClientComponentEx, ILOD
{
  public float lodDistance = 100f;
  public Collider trigger;
  public AudioReverbZone reverbZone;
  public bool inRange;
  public ReverbSettings reverbSettings;

  public virtual void PreClientComponentCull(IPrefabProcessor p)
  {
    p.RemoveComponent((Component) this.trigger);
    p.RemoveComponent((Component) this.reverbZone);
    p.RemoveComponent((Component) this);
    p.NominateForDeletion(((Component) this).get_gameObject());
  }
}
