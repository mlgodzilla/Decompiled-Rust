// Decompiled with JetBrains decompiler
// Type: NPCDoorTriggerBox
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using UnityEngine;

public class NPCDoorTriggerBox : MonoBehaviour
{
  private static int playerServerLayer = -1;
  private Door door;

  public void Setup(Door d)
  {
    this.door = d;
    ((Component) this).get_transform().SetParent(((Component) this.door).get_transform(), false);
    ((Component) this).get_gameObject().set_layer(18);
    M0 m0 = ((Component) this).get_gameObject().AddComponent<BoxCollider>();
    ((Collider) m0).set_isTrigger(true);
    ((BoxCollider) m0).set_center(Vector3.get_zero());
    ((BoxCollider) m0).set_size(Vector3.op_Multiply(Vector3.get_one(), AI.npc_door_trigger_size));
  }

  private void OnTriggerEnter(Collider other)
  {
    if (Object.op_Equality((Object) this.door, (Object) null) || this.door.isClient || this.door.IsLocked() || (!this.door.isSecurityDoor && this.door.IsOpen() || this.door.isSecurityDoor && !this.door.IsOpen()))
      return;
    if (NPCDoorTriggerBox.playerServerLayer < 0)
      NPCDoorTriggerBox.playerServerLayer = LayerMask.NameToLayer("Player (Server)");
    if ((((Component) other).get_gameObject().get_layer() & NPCDoorTriggerBox.playerServerLayer) <= 0)
      return;
    BasePlayer component = (BasePlayer) ((Component) other).get_gameObject().GetComponent<BasePlayer>();
    if (!Object.op_Inequality((Object) component, (Object) null) || !component.IsNpc || this.door.isSecurityDoor)
      return;
    this.door.SetOpen(true);
  }

  public NPCDoorTriggerBox()
  {
    base.\u002Ector();
  }
}
