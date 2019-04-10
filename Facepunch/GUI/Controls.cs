// Decompiled with JetBrains decompiler
// Type: Facepunch.GUI.Controls
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Facepunch.GUI
{
  public static class Controls
  {
    public static float labelWidth = 100f;

    public static float FloatSlider(
      string strLabel,
      float value,
      float low,
      float high,
      string format = "0.00")
    {
      GUILayout.BeginHorizontal((GUILayoutOption[]) Array.Empty<GUILayoutOption>());
      GUILayout.Label(strLabel, new GUILayoutOption[1]
      {
        GUILayout.Width(Controls.labelWidth)
      });
      double num1 = (double) float.Parse(GUILayout.TextField(value.ToString(format), new GUILayoutOption[1]
      {
        GUILayout.ExpandWidth(true)
      }));
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal((GUILayoutOption[]) Array.Empty<GUILayoutOption>());
      double num2 = (double) low;
      double num3 = (double) high;
      M0[] m0Array = Array.Empty<GUILayoutOption>();
      double num4 = (double) GUILayout.HorizontalSlider((float) num1, (float) num2, (float) num3, (GUILayoutOption[]) m0Array);
      GUILayout.EndHorizontal();
      return (float) num4;
    }

    public static int IntSlider(string strLabel, int value, int low, int high, string format = "0")
    {
      GUILayout.BeginHorizontal((GUILayoutOption[]) Array.Empty<GUILayoutOption>());
      GUILayout.Label(strLabel, new GUILayoutOption[1]
      {
        GUILayout.Width(Controls.labelWidth)
      });
      int num1 = int.Parse(GUILayout.TextField(value.ToString(format), new GUILayoutOption[1]
      {
        GUILayout.ExpandWidth(true)
      }));
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal((GUILayoutOption[]) Array.Empty<GUILayoutOption>());
      int num2 = (int) GUILayout.HorizontalSlider((float) num1, (float) low, (float) high, (GUILayoutOption[]) Array.Empty<GUILayoutOption>());
      GUILayout.EndHorizontal();
      return num2;
    }

    public static string TextArea(string strName, string value)
    {
      GUILayout.BeginHorizontal((GUILayoutOption[]) Array.Empty<GUILayoutOption>());
      GUILayout.Label(strName, new GUILayoutOption[1]
      {
        GUILayout.Width(Controls.labelWidth)
      });
      string str = GUILayout.TextArea(value, (GUILayoutOption[]) Array.Empty<GUILayoutOption>());
      GUILayout.EndHorizontal();
      return str;
    }

    public static bool Checkbox(string strName, bool value)
    {
      GUILayout.BeginHorizontal((GUILayoutOption[]) Array.Empty<GUILayoutOption>());
      GUILayout.Label(strName, new GUILayoutOption[1]
      {
        GUILayout.Width(Controls.labelWidth)
      });
      int num = GUILayout.Toggle(value, "", (GUILayoutOption[]) Array.Empty<GUILayoutOption>()) ? 1 : 0;
      GUILayout.EndHorizontal();
      return num != 0;
    }

    public static bool Button(string strName)
    {
      GUILayout.BeginHorizontal((GUILayoutOption[]) Array.Empty<GUILayoutOption>());
      int num = GUILayout.Button(strName, (GUILayoutOption[]) Array.Empty<GUILayoutOption>()) ? 1 : 0;
      GUILayout.EndHorizontal();
      return num != 0;
    }
  }
}
