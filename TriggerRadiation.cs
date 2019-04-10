// Decompiled with JetBrains decompiler
// Type: TriggerRadiation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class TriggerRadiation : TriggerBase
{
  public TriggerRadiation.RadiationTier radiationTier = TriggerRadiation.RadiationTier.LOW;
  public float falloff = 0.1f;
  public float RadiationAmountOverride;
  public float radiationSize;

  public float GetRadiationAmount()
  {
    if ((double) this.RadiationAmountOverride > 0.0)
      return this.RadiationAmountOverride;
    if (this.radiationTier == TriggerRadiation.RadiationTier.MINIMAL)
      return 2f;
    if (this.radiationTier == TriggerRadiation.RadiationTier.LOW)
      return 10f;
    if (this.radiationTier == TriggerRadiation.RadiationTier.MEDIUM)
      return 25f;
    return this.radiationTier == TriggerRadiation.RadiationTier.HIGH ? 51f : 1f;
  }

  private void OnValidate()
  {
    this.radiationSize = ((SphereCollider) ((Component) this).GetComponent<SphereCollider>()).get_radius() * (float) ((Component) this).get_transform().get_localScale().y;
  }

  public float GetRadiation(Vector3 position, float radProtection)
  {
    float radiationAmount = this.GetRadiationAmount();
    float num = Mathf.InverseLerp(this.radiationSize, this.radiationSize * (1f - this.falloff), Vector3.Distance(((Component) this).get_gameObject().get_transform().get_position(), position));
    return Mathf.Clamp(radiationAmount - radProtection, 0.0f, radiationAmount) * num;
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
    if (!(baseEntity is BaseCombatEntity))
      return (GameObject) null;
    return ((Component) baseEntity).get_gameObject();
  }

  public void OnDrawGizmosSelected()
  {
    Gizmos.set_color(Color.get_green());
    Gizmos.DrawWireSphere(((Component) this).get_transform().get_position(), this.radiationSize);
    Gizmos.set_color(Color.get_red());
    Gizmos.DrawWireSphere(((Component) this).get_transform().get_position(), this.radiationSize * (1f - this.falloff));
  }

  public enum RadiationTier
  {
    MINIMAL,
    LOW,
    MEDIUM,
    HIGH,
  }
}
