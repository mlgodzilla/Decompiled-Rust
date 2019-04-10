// Decompiled with JetBrains decompiler
// Type: XMasRefill
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using System;
using System.Collections.Generic;
using UnityEngine;

public class XMasRefill : BaseEntity
{
  public GameObjectRef[] giftPrefabs;
  public List<BasePlayer> goodKids;
  public List<Stocking> stockings;
  public AudioSource bells;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("XMasRefill.OnRpcMessage", 0.1f))
      ;
    return base.OnRpcMessage(player, rpc, msg);
  }

  public float GiftRadius()
  {
    return XMas.spawnRange;
  }

  public int GiftsPerPlayer()
  {
    return XMas.giftsPerPlayer;
  }

  public int GiftSpawnAttempts()
  {
    return XMas.giftsPerPlayer * XMas.spawnAttempts;
  }

  public override void ServerInit()
  {
    base.ServerInit();
    if (!XMas.enabled)
    {
      this.Invoke(new Action(this.RemoveMe), 0.1f);
    }
    else
    {
      this.goodKids = BasePlayer.activePlayerList != null ? new List<BasePlayer>((IEnumerable<BasePlayer>) BasePlayer.activePlayerList) : new List<BasePlayer>();
      this.stockings = Stocking.stockings != null ? new List<Stocking>((IEnumerable<Stocking>) Stocking.stockings.get_Values()) : new List<Stocking>();
      this.Invoke(new Action(this.RemoveMe), 60f);
      this.InvokeRepeating(new Action(this.DistributeLoot), 3f, 0.02f);
      this.Invoke(new Action(this.SendBells), 0.5f);
    }
  }

  public void SendBells()
  {
    this.ClientRPC((Connection) null, "PlayBells");
  }

  public void RemoveMe()
  {
    if (this.goodKids.Count == 0 && this.stockings.Count == 0)
      this.Kill(BaseNetworkable.DestroyMode.None);
    else
      this.Invoke(new Action(this.RemoveMe), 60f);
  }

  public void DistributeLoot()
  {
    if (this.goodKids.Count > 0)
    {
      BasePlayer player = (BasePlayer) null;
      foreach (BasePlayer goodKid in this.goodKids)
      {
        if (!goodKid.IsSleeping() && !goodKid.IsWounded() && goodKid.IsAlive())
        {
          player = goodKid;
          break;
        }
      }
      if (Object.op_Implicit((Object) player))
      {
        this.DistributeGiftsForPlayer(player);
        this.goodKids.Remove(player);
      }
    }
    if (this.stockings.Count <= 0)
      return;
    Stocking stocking = this.stockings[0];
    if (Object.op_Inequality((Object) stocking, (Object) null))
      stocking.SpawnLoot();
    this.stockings.RemoveAt(0);
  }

  protected bool DropToGround(ref Vector3 pos)
  {
    int num1 = 1235288065;
    int num2 = 8454144;
    if (Object.op_Implicit((Object) TerrainMeta.TopologyMap) && (TerrainMeta.TopologyMap.GetTopology(pos) & 82048) != 0)
      return false;
    if (Object.op_Implicit((Object) TerrainMeta.HeightMap) && Object.op_Implicit((Object) TerrainMeta.Collision) && !TerrainMeta.Collision.GetIgnore(pos, 0.01f))
    {
      float height = TerrainMeta.HeightMap.GetHeight(pos);
      pos.y = (__Null) (double) Mathf.Max((float) pos.y, height);
    }
    RaycastHit hitOut;
    if (!TransformUtil.GetGroundInfo(pos, out hitOut, 80f, LayerMask.op_Implicit(num1), (Transform) null) || (1 << ((Component) ((RaycastHit) ref hitOut).get_transform()).get_gameObject().get_layer() & num2) == 0)
      return false;
    pos = ((RaycastHit) ref hitOut).get_point();
    return true;
  }

  public bool DistributeGiftsForPlayer(BasePlayer player)
  {
    int num1 = this.GiftsPerPlayer();
    int num2 = this.GiftSpawnAttempts();
    for (int index = 0; index < num2 && num1 > 0; ++index)
    {
      Vector2 vector2 = Vector2.op_Multiply(Random.get_insideUnitCircle(), this.GiftRadius());
      Vector3 pos = Vector3.op_Addition(((Component) player).get_transform().get_position(), new Vector3((float) vector2.x, 10f, (float) vector2.y));
      Quaternion rot = Quaternion.Euler(0.0f, Random.Range(0.0f, 360f), 0.0f);
      if (this.DropToGround(ref pos))
      {
        string resourcePath = this.giftPrefabs[Random.Range(0, this.giftPrefabs.Length)].resourcePath;
        BaseEntity entity = GameManager.server.CreateEntity(resourcePath, pos, rot, true);
        if (Object.op_Implicit((Object) entity))
        {
          entity.Spawn();
          --num1;
        }
      }
    }
    return true;
  }
}
