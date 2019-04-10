// Decompiled with JetBrains decompiler
// Type: TriggerTemperature
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class TriggerTemperature : TriggerBase
{
  public float Temperature = 50f;
  public float triggerSize;
  public float minSize;

  private void OnValidate()
  {
    this.triggerSize = ((SphereCollider) ((Component) this).GetComponent<SphereCollider>()).get_radius() * (float) ((Component) this).get_transform().get_localScale().y;
  }

  public float WorkoutTemperature(Vector3 position, float oldTemperature)
  {
    float num = Mathf.InverseLerp(this.triggerSize, this.minSize, Vector3.Distance(((Component) this).get_gameObject().get_transform().get_position(), position));
    return Mathf.Lerp(oldTemperature, this.Temperature, num);
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
}
