// Decompiled with JetBrains decompiler
// Type: AnimationEvents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class AnimationEvents : BaseMonoBehaviour
{
  public Transform rootObject;
  public HeldEntity targetEntity;
  [Tooltip("Path to the effect folder for these animations. Relative to this object.")]
  public string effectFolder;
  public string localFolder;
  public bool IsBusy;

  protected void OnEnable()
  {
    if (!Object.op_Equality((Object) this.rootObject, (Object) null))
      return;
    this.rootObject = ((Component) this).get_transform();
  }
}
