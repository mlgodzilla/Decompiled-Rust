// Decompiled with JetBrains decompiler
// Type: CursorManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class CursorManager : SingletonComponent<CursorManager>
{
  private static int iHoldOpen;
  private static int iPreviousOpen;

  private void Update()
  {
    if (Object.op_Inequality((Object) SingletonComponent<CursorManager>.Instance, (Object) this))
      return;
    if (CursorManager.iHoldOpen == 0 && CursorManager.iPreviousOpen == 0)
      this.SwitchToGame();
    else
      this.SwitchToUI();
    CursorManager.iPreviousOpen = CursorManager.iHoldOpen;
    CursorManager.iHoldOpen = 0;
  }

  private void SwitchToGame()
  {
    if (Cursor.get_lockState() != 1)
      Cursor.set_lockState((CursorLockMode) 1);
    if (!Cursor.get_visible())
      return;
    Cursor.set_visible(false);
  }

  private void SwitchToUI()
  {
    if (Cursor.get_lockState() != null)
      Cursor.set_lockState((CursorLockMode) 0);
    if (Cursor.get_visible())
      return;
    Cursor.set_visible(true);
  }

  public static void HoldOpen(bool cursorVisible = false)
  {
    ++CursorManager.iHoldOpen;
  }

  public CursorManager()
  {
    base.\u002Ector();
  }
}
