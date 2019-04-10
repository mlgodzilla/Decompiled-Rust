// Decompiled with JetBrains decompiler
// Type: HumanBodyResourceDispenser
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class HumanBodyResourceDispenser : ResourceDispenser
{
  public override bool OverrideOwnership(Item item, AttackEntity weapon)
  {
    if (item.info.shortname == "skull.human")
    {
      PlayerCorpse component = (PlayerCorpse) ((Component) this).GetComponent<PlayerCorpse>();
      if (Object.op_Implicit((Object) component))
      {
        item.name = "Skull of \"" + component.playerName + "\"";
        return true;
      }
    }
    return false;
  }
}
