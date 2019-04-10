// Decompiled with JetBrains decompiler
// Type: FPSText
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class FPSText : MonoBehaviour
{
  public Text text;
  private Stopwatch fpsTimer;

  protected void Update()
  {
    if (this.fpsTimer.Elapsed.TotalSeconds < 0.5)
      return;
    ((Behaviour) this.text).set_enabled(true);
    this.fpsTimer.Reset();
    this.fpsTimer.Start();
    this.text.set_text(Performance.current.frameRate.ToString() + " FPS");
  }

  public FPSText()
  {
    base.\u002Ector();
  }
}
