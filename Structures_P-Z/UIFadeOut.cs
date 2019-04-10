// Decompiled with JetBrains decompiler
// Type: UIFadeOut
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class UIFadeOut : MonoBehaviour
{
  public float secondsToFadeOut;
  public bool destroyOnFaded;
  public CanvasGroup targetGroup;
  private float timeStarted;

  private void Start()
  {
    this.timeStarted = Time.get_realtimeSinceStartup();
  }

  private void Update()
  {
    this.targetGroup.set_alpha(Mathf.InverseLerp(this.timeStarted + this.secondsToFadeOut, this.timeStarted, Time.get_realtimeSinceStartup()));
    if (!this.destroyOnFaded || (double) Time.get_realtimeSinceStartup() <= (double) this.timeStarted + (double) this.secondsToFadeOut)
      return;
    GameManager.Destroy(((Component) this).get_gameObject(), 0.0f);
  }

  public UIFadeOut()
  {
    base.\u002Ector();
  }
}
