// Decompiled with JetBrains decompiler
// Type: Rust.UI.Utility.ForceWeather
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace Rust.UI.Utility
{
  [RequireComponent(typeof (Toggle))]
  internal class ForceWeather : MonoBehaviour
  {
    private Toggle component;
    public bool Rain;
    public bool Fog;
    public bool Wind;
    public bool Clouds;

    public void OnEnable()
    {
      this.component = (Toggle) ((Component) this).GetComponent<Toggle>();
    }

    public void Update()
    {
      if (Object.op_Equality((Object) SingletonComponent<Climate>.Instance, (Object) null))
        return;
      if (this.Rain)
        ((Climate) SingletonComponent<Climate>.Instance).Overrides.Rain = Mathf.MoveTowards(((Climate) SingletonComponent<Climate>.Instance).Overrides.Rain, this.component.get_isOn() ? 1f : 0.0f, Time.get_deltaTime() / 2f);
      if (this.Fog)
        ((Climate) SingletonComponent<Climate>.Instance).Overrides.Fog = Mathf.MoveTowards(((Climate) SingletonComponent<Climate>.Instance).Overrides.Fog, this.component.get_isOn() ? 1f : 0.0f, Time.get_deltaTime() / 2f);
      if (this.Wind)
        ((Climate) SingletonComponent<Climate>.Instance).Overrides.Wind = Mathf.MoveTowards(((Climate) SingletonComponent<Climate>.Instance).Overrides.Wind, this.component.get_isOn() ? 1f : 0.0f, Time.get_deltaTime() / 2f);
      if (!this.Clouds)
        return;
      ((Climate) SingletonComponent<Climate>.Instance).Overrides.Clouds = Mathf.MoveTowards(((Climate) SingletonComponent<Climate>.Instance).Overrides.Clouds, this.component.get_isOn() ? 1f : 0.0f, Time.get_deltaTime() / 2f);
    }

    public ForceWeather()
    {
      base.\u002Ector();
    }
  }
}
