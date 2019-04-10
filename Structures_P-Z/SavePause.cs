// Decompiled with JetBrains decompiler
// Type: SavePause
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using UnityEngine;

public class SavePause : MonoBehaviour, IServerComponent
{
  private bool tracked;

  protected void OnEnable()
  {
    if (!Object.op_Implicit((Object) SingletonComponent<SaveRestore>.Instance) || this.tracked)
      return;
    this.tracked = true;
    // ISSUE: variable of the null type
    __Null instance = SingletonComponent<SaveRestore>.Instance;
    ((SaveRestore) instance).timedSavePause = ((SaveRestore) instance).timedSavePause + 1;
  }

  protected void OnDisable()
  {
    if (Application.isQuitting != null || !Object.op_Implicit((Object) SingletonComponent<SaveRestore>.Instance) || !this.tracked)
      return;
    this.tracked = false;
    // ISSUE: variable of the null type
    __Null instance = SingletonComponent<SaveRestore>.Instance;
    ((SaveRestore) instance).timedSavePause = ((SaveRestore) instance).timedSavePause - 1;
  }

  public SavePause()
  {
    base.\u002Ector();
  }
}
