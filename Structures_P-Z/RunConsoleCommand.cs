// Decompiled with JetBrains decompiler
// Type: RunConsoleCommand
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class RunConsoleCommand : MonoBehaviour
{
  public void ClientRun(string command)
  {
    ConsoleSystem.Run(ConsoleSystem.Option.get_Client(), command, (object[]) Array.Empty<object>());
  }

  public RunConsoleCommand()
  {
    base.\u002Ector();
  }
}
