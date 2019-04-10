// Decompiled with JetBrains decompiler
// Type: CH47ReinforcementListener
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class CH47ReinforcementListener : BaseEntity
{
  public float startDist = 300f;
  public string listenString;
  public GameObjectRef heliPrefab;

  public override void OnEntityMessage(BaseEntity from, string msg)
  {
    if (!(msg == this.listenString))
      return;
    this.Call();
  }

  public void Call()
  {
    CH47HelicopterAIController component = (CH47HelicopterAIController) ((Component) GameManager.server.CreateEntity(this.heliPrefab.resourcePath, (Vector3) null, (Quaternion) null, true)).GetComponent<CH47HelicopterAIController>();
    if (!Object.op_Implicit((Object) component))
      return;
    Vector3 size = TerrainMeta.Size;
    CH47LandingZone closest = CH47LandingZone.GetClosest(((Component) this).get_transform().get_position());
    Vector3 zero = Vector3.get_zero();
    zero.y = ((Component) closest).get_transform().get_position().y;
    Vector3 vector3_1 = Vector3Ex.Direction2D(((Component) closest).get_transform().get_position(), zero);
    Vector3 vector3_2 = Vector3.op_Addition(((Component) closest).get_transform().get_position(), Vector3.op_Multiply(vector3_1, this.startDist));
    vector3_2.y = ((Component) closest).get_transform().get_position().y;
    ((Component) component).get_transform().set_position(vector3_2);
    component.SetLandingTarget(((Component) closest).get_transform().get_position());
    component.Spawn();
  }
}
