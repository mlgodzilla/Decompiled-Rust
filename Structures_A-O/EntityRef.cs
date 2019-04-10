// Decompiled with JetBrains decompiler
// Type: EntityRef
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public struct EntityRef
{
  internal BaseEntity ent_cached;
  internal uint id_cached;

  public bool IsSet()
  {
    return this.id_cached > 0U;
  }

  public bool IsValid(bool serverside)
  {
    return this.Get(serverside).IsValid();
  }

  public void Set(BaseEntity ent)
  {
    this.ent_cached = ent;
    this.id_cached = 0U;
    if (!this.ent_cached.IsValid())
      return;
    this.id_cached = (uint) this.ent_cached.net.ID;
  }

  public BaseEntity Get(bool serverside)
  {
    if (Object.op_Equality((Object) this.ent_cached, (Object) null) && this.id_cached > 0U)
    {
      if (serverside)
        this.ent_cached = BaseNetworkable.serverEntities.Find(this.id_cached) as BaseEntity;
      else
        Debug.LogWarning((object) "EntityRef: Looking for clientside entities on pure server!");
    }
    if (!this.ent_cached.IsValid())
      this.ent_cached = (BaseEntity) null;
    return this.ent_cached;
  }

  public uint uid
  {
    get
    {
      if (this.ent_cached.IsValid())
        this.id_cached = (uint) this.ent_cached.net.ID;
      return this.id_cached;
    }
    set
    {
      this.id_cached = value;
      if (this.id_cached == 0U)
      {
        this.ent_cached = (BaseEntity) null;
      }
      else
      {
        if (this.ent_cached.IsValid() && this.ent_cached.net.ID == (int) this.id_cached)
          return;
        this.ent_cached = (BaseEntity) null;
      }
    }
  }
}
