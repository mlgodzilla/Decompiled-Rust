// Decompiled with JetBrains decompiler
// Type: ConVar.FPS
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("fps")]
  public class FPS : ConsoleSystem
  {
    private static int m_graph;

    [ServerVar(Saved = true)]
    [ClientVar(Saved = true)]
    public static int limit
    {
      get
      {
        return Application.get_targetFrameRate();
      }
      set
      {
        Application.set_targetFrameRate(value);
      }
    }

    [ClientVar]
    public static int graph
    {
      get
      {
        return FPS.m_graph;
      }
      set
      {
        FPS.m_graph = value;
        if (!Object.op_Implicit((Object) MainCamera.mainCamera))
          return;
        FPSGraph component = (FPSGraph) ((Component) MainCamera.mainCamera).GetComponent<FPSGraph>();
        if (!Object.op_Implicit((Object) component))
          return;
        component.Refresh();
      }
    }

    public FPS()
    {
      base.\u002Ector();
    }
  }
}
