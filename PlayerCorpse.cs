// Decompiled with JetBrains decompiler
// Type: PlayerCorpse
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using ProtoBuf;
using System;
using UnityEngine;

public class PlayerCorpse : LootableCorpse
{
  public Buoyancy buoyancy;
  public const BaseEntity.Flags Flag_Buoyant = BaseEntity.Flags.Reserved6;

  public bool IsBuoyant()
  {
    return this.HasFlag(BaseEntity.Flags.Reserved6);
  }

  public override bool OnStartBeingLooted(BasePlayer baseEntity)
  {
    if (baseEntity.InSafeZone() && (long) baseEntity.userID != (long) this.playerSteamID)
      return false;
    return base.OnStartBeingLooted(baseEntity);
  }

  public override void ServerInit()
  {
    base.ServerInit();
    if (Object.op_Equality((Object) this.buoyancy, (Object) null))
    {
      Debug.LogWarning((object) ("Player corpse has no buoyancy assigned, searching at runtime :" + ((Object) this).get_name()));
      this.buoyancy = (Buoyancy) ((Component) this).GetComponent<Buoyancy>();
    }
    if (!Object.op_Inequality((Object) this.buoyancy, (Object) null))
      return;
    this.buoyancy.SubmergedChanged = new Action<bool>(this.BuoyancyChanged);
  }

  public void BuoyancyChanged(bool isSubmerged)
  {
    if (this.IsBuoyant())
      return;
    this.SetFlag(BaseEntity.Flags.Reserved6, isSubmerged, false, false);
    this.SendNetworkUpdate_Flags();
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    if (!this.isServer || this.containers == null || (this.containers.Length <= 1 || info.forDisk))
      return;
    info.msg.storageBox = (__Null) Pool.Get<StorageBox>();
    ((StorageBox) info.msg.storageBox).contents = (__Null) this.containers[1].Save();
  }

  public override string Categorize()
  {
    return "playercorpse";
  }
}
