// Decompiled with JetBrains decompiler
// Type: DropBox
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class DropBox : Mailbox
{
  public override bool PlayerIsOwner(BasePlayer player)
  {
    return this.PlayerBehind(player);
  }

  public bool PlayerBehind(BasePlayer player)
  {
    Vector3 forward = ((Component) this).get_transform().get_forward();
    Vector3 vector3 = Vector3.op_Subtraction(((Component) player).get_transform().get_position(), ((Component) this).get_transform().get_position());
    Vector3 normalized = ((Vector3) ref vector3).get_normalized();
    return (double) Vector3.Dot(forward, normalized) <= -0.300000011920929;
  }

  public bool PlayerInfront(BasePlayer player)
  {
    Vector3 forward = ((Component) this).get_transform().get_forward();
    Vector3 vector3 = Vector3.op_Subtraction(((Component) player).get_transform().get_position(), ((Component) this).get_transform().get_position());
    Vector3 normalized = ((Vector3) ref vector3).get_normalized();
    return (double) Vector3.Dot(forward, normalized) >= 0.699999988079071;
  }
}
