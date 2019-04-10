// Decompiled with JetBrains decompiler
// Type: TimeSlider
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class TimeSlider : MonoBehaviour
{
  private Slider slider;

  private void Start()
  {
    this.slider = (Slider) ((Component) this).GetComponent<Slider>();
  }

  private void Update()
  {
    if (Object.op_Equality((Object) TOD_Sky.get_Instance(), (Object) null))
      return;
    this.slider.set_value((float) ((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).Hour);
  }

  public void OnValue(float f)
  {
    if (Object.op_Equality((Object) TOD_Sky.get_Instance(), (Object) null))
      return;
    ((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).Hour = (__Null) (double) f;
    TOD_Sky.get_Instance().UpdateAmbient();
    TOD_Sky.get_Instance().UpdateReflection();
    TOD_Sky.get_Instance().UpdateFog();
  }

  public TimeSlider()
  {
    base.\u002Ector();
  }
}
