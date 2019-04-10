// Decompiled with JetBrains decompiler
// Type: ItemEventFlag
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;

public class ItemEventFlag : MonoBehaviour, IItemUpdate
{
  public Item.Flag flag;
  public UnityEvent onEnabled;
  public UnityEvent onDisable;
  internal bool firstRun;
  internal bool lastState;

  public void OnItemUpdate(Item item)
  {
    bool flag = item.HasFlag(this.flag);
    if (!this.firstRun && flag == this.lastState)
      return;
    if (flag)
      this.onEnabled.Invoke();
    else
      this.onDisable.Invoke();
    this.lastState = flag;
    this.firstRun = false;
  }

  public ItemEventFlag()
  {
    base.\u002Ector();
  }
}
