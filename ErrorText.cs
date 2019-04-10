// Decompiled with JetBrains decompiler
// Type: ErrorText
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Rust;
using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class ErrorText : MonoBehaviour
{
  public Text text;
  public int maxLength;
  private Stopwatch stopwatch;

  public void OnEnable()
  {
    Output.OnMessage += new Action<string, string, LogType>(this.CaptureLog);
  }

  public void OnDisable()
  {
    if (Application.isQuitting != null)
      return;
    Output.OnMessage -= new Action<string, string, LogType>(this.CaptureLog);
  }

  internal void CaptureLog(string error, string stacktrace, LogType type)
  {
    if (type != null && type != 4 && type != 1)
      return;
    Text text = this.text;
    text.set_text(text.get_text() + error + "\n" + stacktrace + "\n\n");
    if (this.text.get_text().Length > this.maxLength)
      this.text.set_text(this.text.get_text().Substring(this.text.get_text().Length - this.maxLength, this.maxLength));
    this.stopwatch = Stopwatch.StartNew();
  }

  protected void Update()
  {
    if (this.stopwatch == null || this.stopwatch.Elapsed.TotalSeconds <= 30.0)
      return;
    this.text.set_text(string.Empty);
    this.stopwatch = (Stopwatch) null;
  }

  public ErrorText()
  {
    base.\u002Ector();
  }
}
