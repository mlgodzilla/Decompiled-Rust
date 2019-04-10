// Decompiled with JetBrains decompiler
// Type: SpiderWeb
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class SpiderWeb : BaseCombatEntity
{
  public bool Fresh()
  {
    if (!this.HasFlag(BaseEntity.Flags.Reserved1) && !this.HasFlag(BaseEntity.Flags.Reserved2) && !this.HasFlag(BaseEntity.Flags.Reserved3))
      return !this.HasFlag(BaseEntity.Flags.Reserved4);
    return false;
  }

  public override void ServerInit()
  {
    base.ServerInit();
    if (!this.Fresh())
      return;
    int num = Random.Range(0, 4);
    BaseEntity.Flags f = BaseEntity.Flags.Reserved1;
    switch (num)
    {
      case 0:
        f = BaseEntity.Flags.Reserved1;
        break;
      case 1:
        f = BaseEntity.Flags.Reserved2;
        break;
      case 2:
        f = BaseEntity.Flags.Reserved3;
        break;
      case 3:
        f = BaseEntity.Flags.Reserved4;
        break;
    }
    this.SetFlag(f, true, false, true);
  }
}
