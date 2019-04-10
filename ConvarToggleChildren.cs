// Decompiled with JetBrains decompiler
// Type: ConvarToggleChildren
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections;
using UnityEngine;

public class ConvarToggleChildren : MonoBehaviour
{
  public string ConvarName;
  public string ConvarEnabled;
  private bool state;
  private ConsoleSystem.Command Command;

  protected void Awake()
  {
    this.Command = ConsoleSystem.Index.Client.Find(this.ConvarName);
    if (this.Command == null)
      this.Command = ConsoleSystem.Index.Server.Find(this.ConvarName);
    if (this.Command == null)
      return;
    this.SetState(this.Command.get_String() == this.ConvarEnabled);
  }

  protected void Update()
  {
    if (this.Command == null)
      return;
    bool newState = this.Command.get_String() == this.ConvarEnabled;
    if (this.state == newState)
      return;
    this.SetState(newState);
  }

  private void SetState(bool newState)
  {
    IEnumerator enumerator = ((Component) this).get_transform().GetEnumerator();
    try
    {
      while (enumerator.MoveNext())
        ((Component) enumerator.Current).get_gameObject().SetActive(newState);
    }
    finally
    {
      (enumerator as IDisposable)?.Dispose();
    }
    this.state = newState;
  }

  public ConvarToggleChildren()
  {
    base.\u002Ector();
  }
}
