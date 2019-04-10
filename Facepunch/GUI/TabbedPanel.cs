// Decompiled with JetBrains decompiler
// Type: Facepunch.GUI.TabbedPanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Facepunch.GUI
{
  internal class TabbedPanel
  {
    private List<TabbedPanel.Tab> tabs = new List<TabbedPanel.Tab>();
    private int selectedTabID;

    public TabbedPanel.Tab selectedTab
    {
      get
      {
        return this.tabs[this.selectedTabID];
      }
    }

    public void Add(TabbedPanel.Tab tab)
    {
      this.tabs.Add(tab);
    }

    internal void DrawVertical(float width)
    {
      GUILayout.BeginVertical(new GUILayoutOption[2]
      {
        GUILayout.Width(width),
        GUILayout.ExpandHeight(true)
      });
      for (int index = 0; index < this.tabs.Count; ++index)
      {
        if (GUILayout.Toggle(this.selectedTabID == index, this.tabs[index].name, new GUIStyle(GUIStyle.op_Implicit("devtab")), (GUILayoutOption[]) Array.Empty<GUILayoutOption>()))
          this.selectedTabID = index;
      }
      if (GUILayout.Toggle(false, "", new GUIStyle(GUIStyle.op_Implicit("devtab")), new GUILayoutOption[1]
      {
        GUILayout.ExpandHeight(true)
      }))
        this.selectedTabID = -1;
      GUILayout.EndVertical();
    }

    internal void DrawContents()
    {
      if (this.selectedTabID < 0)
        return;
      TabbedPanel.Tab selectedTab = this.selectedTab;
      GUILayout.BeginVertical(new GUIStyle(GUIStyle.op_Implicit("devtabcontents")), new GUILayoutOption[2]
      {
        GUILayout.ExpandHeight(true),
        GUILayout.ExpandWidth(true)
      });
      if (selectedTab.drawFunc != null)
        selectedTab.drawFunc();
      GUILayout.EndVertical();
    }

    public struct Tab
    {
      public string name;
      public Action drawFunc;
    }
  }
}
