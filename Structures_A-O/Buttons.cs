// Decompiled with JetBrains decompiler
// Type: Buttons
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class Buttons
{
  public class ConButton : ConsoleSystem.IConsoleCommand
  {
    private int frame;

    public bool IsDown { get; set; }

    public bool JustPressed
    {
      get
      {
        if (this.IsDown)
          return this.frame == Time.get_frameCount();
        return false;
      }
    }

    public bool JustReleased
    {
      get
      {
        if (!this.IsDown)
          return this.frame == Time.get_frameCount();
        return false;
      }
    }

    public void Call(ConsoleSystem.Arg arg)
    {
      this.IsDown = arg.GetBool(0, false);
      this.frame = Time.get_frameCount();
    }
  }
}
