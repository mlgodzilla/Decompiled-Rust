// Decompiled with JetBrains decompiler
// Type: TargetTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class TargetTrigger : TriggerBase
{
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
