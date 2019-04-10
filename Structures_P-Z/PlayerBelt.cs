// Decompiled with JetBrains decompiler
// Type: PlayerBelt
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Oxide.Core;
using UnityEngine;

public class PlayerBelt
{
  public static int SelectedSlot = -1;
  protected BasePlayer player;

  public static int MaxBeltSlots
  {
    get
    {
      return 6;
    }
  }

  public PlayerBelt(BasePlayer player)
  {
    this.player = player;
  }

  public void DropActive(Vector3 position, Vector3 velocity)
  {
    Item activeItem = this.player.GetActiveItem();
    if (activeItem == null || Interface.CallHook("OnPlayerDropActiveItem", (object) this.player, (object) activeItem) != null)
      return;
    using (TimeWarning.New("PlayerBelt.DropActive", 0.1f))
    {
      activeItem.Drop(position, velocity, (Quaternion) null);
      this.player.svActiveItemID = 0U;
      this.player.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    }
  }
}
