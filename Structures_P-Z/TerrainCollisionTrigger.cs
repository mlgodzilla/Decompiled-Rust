// Decompiled with JetBrains decompiler
// Type: TerrainCollisionTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class TerrainCollisionTrigger : EnvironmentVolumeTrigger
{
  protected void OnTriggerEnter(Collider other)
  {
    if (!Object.op_Implicit((Object) TerrainMeta.Collision) || other.get_isTrigger())
      return;
    this.UpdateCollider(other, true);
  }

  protected void OnTriggerExit(Collider other)
  {
    if (!Object.op_Implicit((Object) TerrainMeta.Collision) || other.get_isTrigger())
      return;
    this.UpdateCollider(other, false);
  }

  private void UpdateCollider(Collider other, bool state)
  {
    TerrainMeta.Collision.SetIgnore(other, (Collider) this.volume.trigger, state);
    TerrainCollisionProxy component = (TerrainCollisionProxy) ((Component) other).GetComponent<TerrainCollisionProxy>();
    if (!Object.op_Implicit((Object) component))
      return;
    for (int index = 0; index < component.colliders.Length; ++index)
      TerrainMeta.Collision.SetIgnore((Collider) component.colliders[index], (Collider) this.volume.trigger, state);
  }
}
