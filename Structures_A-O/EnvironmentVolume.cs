// Decompiled with JetBrains decompiler
// Type: EnvironmentVolume
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class EnvironmentVolume : MonoBehaviour
{
  public bool StickyGizmos;
  public EnvironmentType Type;
  public Vector3 Center;
  public Vector3 Size;

  public BoxCollider trigger { get; private set; }

  protected void OnDrawGizmos()
  {
    if (!this.StickyGizmos)
      return;
    this.DrawGizmos();
  }

  protected void OnDrawGizmosSelected()
  {
    if (this.StickyGizmos)
      return;
    this.DrawGizmos();
  }

  private void DrawGizmos()
  {
    Vector3 lossyScale = ((Component) this).get_transform().get_lossyScale();
    Quaternion rotation = ((Component) this).get_transform().get_rotation();
    Vector3 pos = Vector3.op_Addition(((Component) this).get_transform().get_position(), Quaternion.op_Multiply(rotation, Vector3.Scale(lossyScale, this.Center)));
    Vector3 size = Vector3.Scale(lossyScale, this.Size);
    Gizmos.set_color(new Color(0.5f, 0.5f, 0.5f, 0.5f));
    GizmosUtil.DrawCube(pos, size, rotation);
    GizmosUtil.DrawWireCube(pos, size, rotation);
  }

  protected virtual void Awake()
  {
    this.UpdateTrigger();
  }

  public void UpdateTrigger()
  {
    if (!Object.op_Implicit((Object) this.trigger))
      this.trigger = (BoxCollider) ((Component) this).get_gameObject().GetComponent<BoxCollider>();
    if (!Object.op_Implicit((Object) this.trigger))
      this.trigger = (BoxCollider) ((Component) this).get_gameObject().AddComponent<BoxCollider>();
    ((Collider) this.trigger).set_isTrigger(true);
    this.trigger.set_center(this.Center);
    this.trigger.set_size(this.Size);
  }

  public EnvironmentVolume()
  {
    base.\u002Ector();
  }
}
