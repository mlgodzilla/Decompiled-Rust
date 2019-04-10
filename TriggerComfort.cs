// Decompiled with JetBrains decompiler
// Type: TriggerComfort
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class TriggerComfort : TriggerBase
{
  public float baseComfort = 0.5f;
  public float minComfortRange = 2.5f;
  private List<BasePlayer> _players = new List<BasePlayer>();
  public float triggerSize;
  private const float perPlayerComfortBonus = 0.25f;
  private const float bonusComfort = 0.0f;

  private void OnValidate()
  {
    this.triggerSize = ((SphereCollider) ((Component) this).GetComponent<SphereCollider>()).get_radius() * (float) ((Component) this).get_transform().get_localScale().y;
  }

  internal override GameObject InterestedInObject(GameObject obj)
  {
    obj = base.InterestedInObject(obj);
    if (Object.op_Equality((Object) obj, (Object) null))
      return (GameObject) null;
    BaseEntity baseEntity = obj.ToBaseEntity();
    if (Object.op_Equality((Object) baseEntity, (Object) null))
      return (GameObject) null;
    if (baseEntity.isClient)
      return (GameObject) null;
    return ((Component) baseEntity).get_gameObject();
  }

  public float CalculateComfort(Vector3 position, BasePlayer forPlayer = null)
  {
    float num1 = Vector3.Distance(((Component) this).get_gameObject().get_transform().get_position(), position);
    float num2 = 1f - Mathf.Clamp(num1 - this.minComfortRange, 0.0f, num1 / (this.triggerSize - this.minComfortRange));
    float num3 = 0.0f;
    foreach (BasePlayer player in this._players)
    {
      if (!Object.op_Equality((Object) player, (Object) forPlayer))
        num3 += (float) (0.25 * (player.IsSleeping() ? 0.5 : 1.0) * (player.IsAlive() ? 1.0 : 0.0));
    }
    return (this.baseComfort + (0.0f + num3)) * num2;
  }

  internal override void OnEntityEnter(BaseEntity ent)
  {
    BasePlayer basePlayer = ent as BasePlayer;
    if (!Object.op_Implicit((Object) basePlayer))
      return;
    this._players.Add(basePlayer);
  }

  internal override void OnEntityLeave(BaseEntity ent)
  {
    BasePlayer basePlayer = ent as BasePlayer;
    if (!Object.op_Implicit((Object) basePlayer))
      return;
    this._players.Remove(basePlayer);
  }
}
