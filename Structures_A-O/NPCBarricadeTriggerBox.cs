// Decompiled with JetBrains decompiler
// Type: NPCBarricadeTriggerBox
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using UnityEngine;

public class NPCBarricadeTriggerBox : MonoBehaviour
{
  private static int playerServerLayer = -1;
  private Barricade target;

  public void Setup(Barricade t)
  {
    this.target = t;
    ((Component) this).get_transform().SetParent(((Component) this.target).get_transform(), false);
    ((Component) this).get_gameObject().set_layer(18);
    M0 m0 = ((Component) this).get_gameObject().AddComponent<BoxCollider>();
    ((Collider) m0).set_isTrigger(true);
    ((BoxCollider) m0).set_center(Vector3.get_zero());
    ((BoxCollider) m0).set_size(Vector3.op_Addition(Vector3.op_Multiply(Vector3.get_one(), AI.npc_door_trigger_size), Vector3.op_Multiply(Vector3.get_right(), (float) ((Bounds) ref this.target.bounds).get_size().x)));
  }

  private void OnTriggerEnter(Collider other)
  {
    if (Object.op_Equality((Object) this.target, (Object) null) || this.target.isClient)
      return;
    if (NPCBarricadeTriggerBox.playerServerLayer < 0)
      NPCBarricadeTriggerBox.playerServerLayer = LayerMask.NameToLayer("Player (Server)");
    if ((((Component) other).get_gameObject().get_layer() & NPCBarricadeTriggerBox.playerServerLayer) <= 0)
      return;
    BasePlayer component = (BasePlayer) ((Component) other).get_gameObject().GetComponent<BasePlayer>();
    if (!Object.op_Inequality((Object) component, (Object) null) || !component.IsNpc)
      return;
    this.target.Kill(BaseNetworkable.DestroyMode.Gib);
  }

  public NPCBarricadeTriggerBox()
  {
    base.\u002Ector();
  }
}
