// Decompiled with JetBrains decompiler
// Type: BaseAnimalNPC
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class BaseAnimalNPC : BaseNpc
{
  public override void OnKilled(HitInfo hitInfo = null)
  {
    if (hitInfo != null)
    {
      BasePlayer initiatorPlayer = hitInfo.InitiatorPlayer;
      if (Object.op_Inequality((Object) initiatorPlayer, (Object) null))
        initiatorPlayer.GiveAchievement("KILL_ANIMAL");
    }
    base.OnKilled((HitInfo) null);
  }
}
