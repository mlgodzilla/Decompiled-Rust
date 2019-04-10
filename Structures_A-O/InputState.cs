// Decompiled with JetBrains decompiler
// Type: InputState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class InputState
{
  public InputMessage current = new InputMessage()
  {
    ShouldPool = (__Null) 0
  };
  public InputMessage previous = new InputMessage()
  {
    ShouldPool = (__Null) 0
  };
  private int SwallowedButtons;

  public bool IsDown(BUTTON btn)
  {
    if (this.current == null || ((BUTTON) this.SwallowedButtons & btn) == btn)
      return false;
    return ((BUTTON) this.current.buttons & btn) == btn;
  }

  public bool WasDown(BUTTON btn)
  {
    if (this.previous == null)
      return false;
    return ((BUTTON) this.previous.buttons & btn) == btn;
  }

  public bool WasJustPressed(BUTTON btn)
  {
    if (this.IsDown(btn))
      return !this.WasDown(btn);
    return false;
  }

  public bool WasJustReleased(BUTTON btn)
  {
    if (!this.IsDown(btn))
      return this.WasDown(btn);
    return false;
  }

  public void SwallowButton(BUTTON btn)
  {
    if (this.current == null)
      return;
    this.SwallowedButtons = (int) ((BUTTON) this.SwallowedButtons | btn);
  }

  private Quaternion AimAngle()
  {
    if (this.current == null)
      return Quaternion.get_identity();
    return Quaternion.Euler((Vector3) this.current.aimAngles);
  }

  public void Flip(InputMessage newcurrent)
  {
    this.SwallowedButtons = 0;
    this.previous.aimAngles = this.current.aimAngles;
    this.previous.buttons = this.current.buttons;
    this.previous.mouseDelta = this.current.mouseDelta;
    this.current.aimAngles = newcurrent.aimAngles;
    this.current.buttons = newcurrent.buttons;
    this.current.mouseDelta = newcurrent.mouseDelta;
  }

  public void Clear()
  {
    this.current.buttons = (__Null) 0;
    this.previous.buttons = (__Null) 0;
    this.SwallowedButtons = 0;
  }
}
