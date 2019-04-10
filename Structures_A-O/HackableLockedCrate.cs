// Decompiled with JetBrains decompiler
// Type: HackableLockedCrate
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using Oxide.Core;
using Rust;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class HackableLockedCrate : LootContainer
{
  [ServerVar(Help = "How many seconds for the crate to unlock")]
  public static float requiredHackSeconds = 900f;
  [ServerVar(Help = "How many seconds until the crate is destroyed without any hack attempts")]
  public static float decaySeconds = 7200f;
  public const BaseEntity.Flags Flag_Hacking = BaseEntity.Flags.Reserved1;
  public const BaseEntity.Flags Flag_FullyHacked = BaseEntity.Flags.Reserved2;
  public Text timerText;
  public SoundPlayer hackProgressBeep;
  public float hackSeconds;
  public GameObjectRef shockEffect;
  public GameObjectRef mapMarkerEntityPrefab;
  public GameObjectRef landEffect;
  private BaseEntity mapMarkerInstance;
  public bool hasLanded;
  public bool wasDropped;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("HackableLockedCrate.OnRpcMessage", 0.1f))
    {
      if (rpc == 888500940U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_Hack "));
          using (TimeWarning.New("RPC_Hack", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_Hack", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_Hack(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_Hack");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public bool IsBeingHacked()
  {
    return this.HasFlag(BaseEntity.Flags.Reserved1);
  }

  public bool IsFullyHacked()
  {
    return this.HasFlag(BaseEntity.Flags.Reserved2);
  }

  public override void DestroyShared()
  {
    if (this.isServer && Object.op_Implicit((Object) this.mapMarkerInstance))
      this.mapMarkerInstance.Kill(BaseNetworkable.DestroyMode.None);
    base.DestroyShared();
  }

  public void CreateMapMarker(float durationMinutes)
  {
    if (Object.op_Implicit((Object) this.mapMarkerInstance))
      this.mapMarkerInstance.Kill(BaseNetworkable.DestroyMode.None);
    BaseEntity entity = GameManager.server.CreateEntity(this.mapMarkerEntityPrefab.resourcePath, ((Component) this).get_transform().get_position(), Quaternion.get_identity(), true);
    entity.Spawn();
    entity.SetParent((BaseEntity) this, false, false);
    ((Component) entity).get_transform().set_localPosition(Vector3.get_zero());
    entity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    this.mapMarkerInstance = entity;
  }

  public void RefreshDecay()
  {
    this.CancelInvoke(new Action(this.DelayedDestroy));
    this.Invoke(new Action(this.DelayedDestroy), HackableLockedCrate.decaySeconds);
  }

  public void DelayedDestroy()
  {
    this.Kill(BaseNetworkable.DestroyMode.None);
  }

  public override void OnAttacked(HitInfo info)
  {
    if (this.isServer)
    {
      if (StringPool.Get(info.HitBone) == "laptopcollision")
      {
        Effect.server.Run(this.shockEffect.resourcePath, info.HitPositionWorld, Vector3.get_up(), (Connection) null, false);
        this.hackSeconds -= (float) (8.0 * ((double) info.damageTypes.Total() / 50.0));
        if ((double) this.hackSeconds < 0.0)
          this.hackSeconds = 0.0f;
      }
      this.RefreshDecay();
    }
    base.OnAttacked(info);
  }

  public void SetWasDropped()
  {
    this.wasDropped = true;
    Interface.CallHook("OnCrateDropped", (object) this);
  }

  public override void ServerInit()
  {
    base.ServerInit();
    if (this.isClient)
      return;
    if (Application.isLoadingSave == null)
    {
      this.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
      this.SetFlag(BaseEntity.Flags.Reserved2, false, false, true);
      if (this.wasDropped)
        this.InvokeRepeating(new Action(this.LandCheck), 0.0f, 0.015f);
    }
    this.RefreshDecay();
    this.isLootable = this.IsFullyHacked();
    this.CreateMapMarker(120f);
  }

  public void LandCheck()
  {
    if (this.hasLanded)
    {
      Interface.CallHook("OnCrateLanded", (object) this);
    }
    else
    {
      RaycastHit raycastHit;
      if (!Physics.Raycast(new Ray(Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_up(), 0.5f)), Vector3.get_down()), ref raycastHit, 1f, 1218511105))
        return;
      Effect.server.Run(this.landEffect.resourcePath, ((RaycastHit) ref raycastHit).get_point(), Vector3.get_up(), (Connection) null, false);
      this.hasLanded = true;
      this.CancelInvoke(new Action(this.LandCheck));
    }
  }

  public override void PostServerLoad()
  {
    base.PostServerLoad();
    this.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
  }

  [BaseEntity.RPC_Server.IsVisible(3f)]
  [BaseEntity.RPC_Server]
  public void RPC_Hack(BaseEntity.RPCMessage msg)
  {
    if (this.IsBeingHacked() || Interface.CallHook("CanHackCrate", (object) msg.player, (object) this) != null)
      return;
    this.StartHacking();
  }

  public void StartHacking()
  {
    Interface.CallHook("OnCrateHack", (object) this);
    this.BroadcastEntityMessage("HackingStarted", 20f, 256);
    this.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
    this.InvokeRepeating(new Action(this.HackProgress), 1f, 1f);
    this.ClientRPC<int, int>((Connection) null, "UpdateHackProgress", 0, (int) HackableLockedCrate.requiredHackSeconds);
    this.RefreshDecay();
  }

  public void HackProgress()
  {
    ++this.hackSeconds;
    if ((double) this.hackSeconds > (double) HackableLockedCrate.requiredHackSeconds)
    {
      Interface.CallHook("OnCrateHackEnd", (object) this);
      this.RefreshDecay();
      this.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
      this.isLootable = true;
      this.CancelInvoke(new Action(this.HackProgress));
    }
    this.ClientRPC<int, int>((Connection) null, "UpdateHackProgress", (int) this.hackSeconds, (int) HackableLockedCrate.requiredHackSeconds);
  }
}
