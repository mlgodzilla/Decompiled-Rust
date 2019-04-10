// Decompiled with JetBrains decompiler
// Type: EnvironmentVolumeTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class EnvironmentVolumeTrigger : MonoBehaviour
{
  [HideInInspector]
  public Vector3 Center;
  [HideInInspector]
  public Vector3 Size;

  public EnvironmentVolume volume { get; private set; }

  protected void Awake()
  {
    this.volume = (EnvironmentVolume) ((Component) this).get_gameObject().GetComponent<EnvironmentVolume>();
    if (!Object.op_Equality((Object) this.volume, (Object) null))
      return;
    this.volume = (EnvironmentVolume) ((Component) this).get_gameObject().AddComponent<EnvironmentVolume>();
    this.volume.Center = this.Center;
    this.volume.Size = this.Size;
    this.volume.UpdateTrigger();
  }

  public EnvironmentVolumeTrigger()
  {
    base.\u002Ector();
  }
}
