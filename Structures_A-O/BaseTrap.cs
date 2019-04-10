// Decompiled with JetBrains decompiler
// Type: BaseTrap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class BaseTrap : DecayEntity
{
  public virtual void ObjectEntered(GameObject obj)
  {
  }

  public virtual void Arm()
  {
    this.SetFlag(BaseEntity.Flags.On, true, false, true);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public virtual void OnEmpty()
  {
  }
}
