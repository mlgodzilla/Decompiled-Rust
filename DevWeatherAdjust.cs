// Decompiled with JetBrains decompiler
// Type: DevWeatherAdjust
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class DevWeatherAdjust : MonoBehaviour
{
  protected void Awake()
  {
    ((Climate) SingletonComponent<Climate>.Instance).Overrides.Clouds = 0.0f;
    ((Climate) SingletonComponent<Climate>.Instance).Overrides.Fog = 0.0f;
    ((Climate) SingletonComponent<Climate>.Instance).Overrides.Wind = 0.0f;
    ((Climate) SingletonComponent<Climate>.Instance).Overrides.Rain = 0.0f;
  }

  protected void OnGUI()
  {
    float num = (float) Screen.get_width() * 0.2f;
    GUILayout.BeginArea(new Rect((float) ((double) Screen.get_width() - (double) num - 20.0), 20f, num, 400f), "", GUIStyle.op_Implicit("box"));
    GUILayout.Box("Weather", (GUILayoutOption[]) Array.Empty<GUILayoutOption>());
    GUILayout.FlexibleSpace();
    GUILayout.Label("Clouds", (GUILayoutOption[]) Array.Empty<GUILayoutOption>());
    ((Climate) SingletonComponent<Climate>.Instance).Overrides.Clouds = GUILayout.HorizontalSlider(((Climate) SingletonComponent<Climate>.Instance).Overrides.Clouds, 0.0f, 1f, (GUILayoutOption[]) Array.Empty<GUILayoutOption>());
    GUILayout.Label("Fog", (GUILayoutOption[]) Array.Empty<GUILayoutOption>());
    ((Climate) SingletonComponent<Climate>.Instance).Overrides.Fog = GUILayout.HorizontalSlider(((Climate) SingletonComponent<Climate>.Instance).Overrides.Fog, 0.0f, 1f, (GUILayoutOption[]) Array.Empty<GUILayoutOption>());
    GUILayout.Label("Wind", (GUILayoutOption[]) Array.Empty<GUILayoutOption>());
    ((Climate) SingletonComponent<Climate>.Instance).Overrides.Wind = GUILayout.HorizontalSlider(((Climate) SingletonComponent<Climate>.Instance).Overrides.Wind, 0.0f, 1f, (GUILayoutOption[]) Array.Empty<GUILayoutOption>());
    GUILayout.Label("Rain", (GUILayoutOption[]) Array.Empty<GUILayoutOption>());
    ((Climate) SingletonComponent<Climate>.Instance).Overrides.Rain = GUILayout.HorizontalSlider(((Climate) SingletonComponent<Climate>.Instance).Overrides.Rain, 0.0f, 1f, (GUILayoutOption[]) Array.Empty<GUILayoutOption>());
    GUILayout.FlexibleSpace();
    GUILayout.EndArea();
  }

  public DevWeatherAdjust()
  {
    base.\u002Ector();
  }
}
