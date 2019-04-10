// Decompiled with JetBrains decompiler
// Type: ExplosionsBillboard
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ExplosionsBillboard : MonoBehaviour
{
  public Camera Camera;
  public bool Active;
  public bool AutoInitCamera;
  private GameObject myContainer;
  private Transform t;
  private Transform camT;
  private Transform contT;

  private void Awake()
  {
    if (this.AutoInitCamera)
    {
      this.Camera = Camera.get_main();
      this.Active = true;
    }
    this.t = ((Component) this).get_transform();
    Vector3 localScale = ((Component) this.t.get_parent()).get_transform().get_localScale();
    localScale.z = localScale.x;
    ((Component) this.t.get_parent()).get_transform().set_localScale(localScale);
    this.camT = ((Component) this.Camera).get_transform();
    Transform parent = this.t.get_parent();
    GameObject gameObject = new GameObject();
    ((Object) gameObject).set_name("Billboard_" + ((Object) ((Component) this.t).get_gameObject()).get_name());
    this.myContainer = gameObject;
    this.contT = this.myContainer.get_transform();
    this.contT.set_position(this.t.get_position());
    this.t.set_parent(this.myContainer.get_transform());
    this.contT.set_parent(parent);
  }

  private void Update()
  {
    if (!this.Active)
      return;
    this.contT.LookAt(Vector3.op_Addition(this.contT.get_position(), Quaternion.op_Multiply(this.camT.get_rotation(), Vector3.get_back())), Quaternion.op_Multiply(this.camT.get_rotation(), Vector3.get_up()));
  }

  public ExplosionsBillboard()
  {
    base.\u002Ector();
  }
}
