﻿// Decompiled with JetBrains decompiler
// Type: EntityComponentBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;

public class EntityComponentBase : BaseMonoBehaviour
{
  protected virtual BaseEntity GetBaseEntity()
  {
    return (BaseEntity) null;
  }

  public virtual bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    return false;
  }
}
