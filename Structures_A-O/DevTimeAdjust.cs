// Decompiled with JetBrains decompiler
// Type: DevTimeAdjust
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class DevTimeAdjust : MonoBehaviour
{
  private void Start()
  {
    if (!Object.op_Implicit((Object) TOD_Sky.get_Instance()))
      return;
    ((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).Hour = (__Null) (double) PlayerPrefs.GetFloat("DevTime");
  }

  private void OnGUI()
  {
    if (!Object.op_Implicit((Object) TOD_Sky.get_Instance()))
      return;
    float num1 = (float) Screen.get_width() * 0.2f;
    Rect rect;
    ((Rect) ref rect).\u002Ector((float) Screen.get_width() - (num1 + 20f), (float) Screen.get_height() - 30f, num1, 20f);
    float hour = (float) ((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).Hour;
    float num2 = GUI.HorizontalSlider(rect, hour, 0.0f, 24f);
    ref Rect local = ref rect;
    ((Rect) ref local).set_y(((Rect) ref local).get_y() - 20f);
    GUI.Label(rect, "Time Of Day");
    if ((double) num2 == ((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).Hour)
      return;
    ((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).Hour = (__Null) (double) num2;
    PlayerPrefs.SetFloat("DevTime", num2);
  }

  public DevTimeAdjust()
  {
    base.\u002Ector();
  }
}
