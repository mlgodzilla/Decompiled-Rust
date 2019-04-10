// Decompiled with JetBrains decompiler
// Type: BigWheelBettingTerminal
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using UnityEngine;

public class BigWheelBettingTerminal : StorageContainer
{
  public Vector3 seatedPlayerOffset = Vector3.get_forward();
  public float offsetCheckRadius = 0.4f;
  public BigWheelGame bigWheel;
  public SoundDefinition winSound;
  public SoundDefinition loseSound;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("BigWheelBettingTerminal.OnRpcMessage", 0.1f))
      ;
    return base.OnRpcMessage(player, rpc, msg);
  }

  public new void OnDrawGizmos()
  {
    Gizmos.set_color(Color.get_yellow());
    Gizmos.DrawSphere(((Component) this).get_transform().TransformPoint(this.seatedPlayerOffset), this.offsetCheckRadius);
    base.OnDrawGizmos();
  }

  public bool IsPlayerValid(BasePlayer player)
  {
    if (!player.isMounted)
      return false;
    Vector3 vector3 = ((Component) this).get_transform().TransformPoint(this.seatedPlayerOffset);
    return (double) Vector3Ex.Distance2D(((Component) player).get_transform().get_position(), vector3) <= (double) this.offsetCheckRadius;
  }

  public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen)
  {
    if (!this.IsPlayerValid(player))
      return false;
    return base.PlayerOpenLoot(player, panelToOpen);
  }
}
