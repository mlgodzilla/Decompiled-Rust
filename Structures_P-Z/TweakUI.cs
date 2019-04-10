// Decompiled with JetBrains decompiler
// Type: TweakUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class TweakUI : SingletonComponent<TweakUI>
{
  public static bool isOpen;

  private void Update()
  {
    if (!Input.GetKeyDown((KeyCode) 283) || !this.CanToggle())
      return;
    this.SetVisible(!TweakUI.isOpen);
  }

  protected bool CanToggle()
  {
    return LevelManager.isLoaded;
  }

  public void SetVisible(bool b)
  {
    if (b)
    {
      TweakUI.isOpen = true;
    }
    else
    {
      TweakUI.isOpen = false;
      ConsoleSystem.Run(ConsoleSystem.Option.get_Client(), "writecfg", (object[]) Array.Empty<object>());
      ConsoleSystem.Run(ConsoleSystem.Option.get_Client(), "trackir.refresh", (object[]) Array.Empty<object>());
    }
  }

  public TweakUI()
  {
    base.\u002Ector();
  }
}
