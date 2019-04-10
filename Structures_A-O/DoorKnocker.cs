// Decompiled with JetBrains decompiler
// Type: DoorKnocker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using UnityEngine;

public class DoorKnocker : BaseCombatEntity
{
  public Animator knocker1;
  public Animator knocker2;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("DoorKnocker.OnRpcMessage", 0.1f))
      ;
    return base.OnRpcMessage(player, rpc, msg);
  }

  public void Knock(BasePlayer player)
  {
    this.ClientRPC<Vector3>((Connection) null, "ClientKnock", ((Component) player).get_transform().get_position());
  }
}
