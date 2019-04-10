// Decompiled with JetBrains decompiler
// Type: Keycard
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class Keycard : AttackEntity
{
  public int accessLevel
  {
    get
    {
      Item obj = this.GetItem();
      if (obj == null)
        return 0;
      ItemModKeycard component = (ItemModKeycard) ((Component) obj.info).GetComponent<ItemModKeycard>();
      if (Object.op_Equality((Object) component, (Object) null))
        return 0;
      return component.accessLevel;
    }
  }
}
