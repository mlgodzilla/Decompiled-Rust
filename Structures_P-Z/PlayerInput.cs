// Decompiled with JetBrains decompiler
// Type: PlayerInput
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using System;

public class PlayerInput : EntityComponent<BasePlayer>
{
  public InputState state = new InputState();
  [NonSerialized]
  public bool hadInputBuffer = true;

  protected void OnDisable()
  {
    if (Application.isQuitting != null)
      return;
    this.state.Clear();
  }
}
