// Decompiled with JetBrains decompiler
// Type: TreeEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using System;
using UnityEngine;

public class TreeEntity : ResourceEntity
{
  public bool hasBonusGame = true;
  private float lastDirection = -1f;
  [Header("Falling")]
  public bool fallOnKilled = true;
  public float fallDuration = 1.5f;
  [NonSerialized]
  public bool[] usedHeights = new bool[20];
  public GameObjectRef prefab;
  public GameObjectRef bonusHitEffect;
  public GameObjectRef bonusHitSound;
  public Collider serverCollider;
  public Collider clientCollider;
  public SoundDefinition smallCrackSoundDef;
  public SoundDefinition medCrackSoundDef;
  private float lastAttackDamage;
  [NonSerialized]
  protected BaseEntity xMarker;
  private int currentBonusLevel;
  private float lastHitTime;
  public GameObjectRef fallStartSound;
  public GameObjectRef fallImpactSound;
  public GameObjectRef fallImpactParticles;
  public SoundDefinition fallLeavesLoopDef;
  public bool impactSoundPlayed;
  [NonSerialized]
  public float treeDistanceUponFalling;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("TreeEntity.OnRpcMessage", 0.1f))
      ;
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override void DestroyShared()
  {
    base.DestroyShared();
    if (!this.isServer)
      return;
    this.CleanupMarker();
  }

  public override float BoundsPadding()
  {
    return 1f;
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.lastDirection = Random.Range(0, 2) == 0 ? -1f : 1f;
  }

  public bool DidHitMarker(HitInfo info)
  {
    return !Object.op_Equality((Object) this.xMarker, (Object) null) && (double) Vector3.Dot(Vector3Ex.Direction2D(((Component) this).get_transform().get_position(), ((Component) this.xMarker).get_transform().get_position()), info.attackNormal) >= 0.3 && (double) Vector3.Distance(((Component) this.xMarker).get_transform().get_position(), info.HitPositionWorld) <= 0.200000002980232;
  }

  public void StartBonusGame()
  {
    if (this.IsInvoking(new Action(this.StopBonusGame)))
      this.CancelInvoke(new Action(this.StopBonusGame));
    this.Invoke(new Action(this.StopBonusGame), 60f);
  }

  public void StopBonusGame()
  {
    this.CleanupMarker();
    this.lastHitTime = 0.0f;
    this.currentBonusLevel = 0;
  }

  public bool BonusActive()
  {
    return Object.op_Inequality((Object) this.xMarker, (Object) null);
  }

  public override void OnAttacked(HitInfo info)
  {
    bool canGather = info.CanGather;
    float num1 = Time.get_time() - this.lastHitTime;
    this.lastHitTime = Time.get_time();
    if (!this.hasBonusGame || !canGather || Object.op_Equality((Object) info.Initiator, (Object) null) || this.BonusActive() && !this.DidHitMarker(info))
    {
      base.OnAttacked(info);
    }
    else
    {
      if (Object.op_Inequality((Object) this.xMarker, (Object) null) && !info.DidGather && (double) info.gatherScale > 0.0)
      {
        this.xMarker.ClientRPC<int>((Connection) null, "MarkerHit", this.currentBonusLevel);
        ++this.currentBonusLevel;
        info.gatherScale = 1f + Mathf.Clamp((float) this.currentBonusLevel * 0.125f, 0.0f, 1f);
      }
      Vector3 vector3_1 = Object.op_Inequality((Object) this.xMarker, (Object) null) ? ((Component) this.xMarker).get_transform().get_position() : info.HitPositionWorld;
      this.CleanupMarker();
      Vector3 vector3_2 = Vector3Ex.Direction2D(((Component) this).get_transform().get_position(), vector3_1);
      Vector3 vector3_3 = Vector3.Lerp(Vector3.op_UnaryNegation(vector3_2), Vector3.op_Multiply(Vector3.Cross(vector3_2, Vector3.get_up()), this.lastDirection), Random.Range(0.5f, 0.5f));
      Vector3 pos = ((Component) this).get_transform().InverseTransformPoint(this.GetCollider().ClosestPoint(((Component) this).get_transform().TransformPoint(Vector3.op_Multiply(((Component) this).get_transform().InverseTransformDirection(((Vector3) ref vector3_3).get_normalized()), 2.5f))));
      Vector3 vector3_4 = ((Component) this).get_transform().TransformPoint(pos);
      Vector3 vector3_5 = ((Component) this).get_transform().InverseTransformPoint(info.HitPositionWorld);
      pos.y = vector3_5.y;
      Vector3 vector3_6 = ((Component) this).get_transform().InverseTransformPoint(info.Initiator.CenterPoint());
      float num2 = Mathf.Max(0.75f, (float) vector3_6.y);
      float num3 = (float) (vector3_6.y + 0.5);
      pos.y = (__Null) (double) Mathf.Clamp((float) (pos.y + (double) Random.Range(0.1f, 0.2f) * (Random.Range(0, 2) == 0 ? -1.0 : 1.0)), num2, num3);
      Vector3 vector3_7 = Vector3Ex.Direction2D(((Component) this).get_transform().get_position(), vector3_4);
      Vector3 vector3_8 = vector3_7;
      QuaternionEx.LookRotationNormal(Vector3.op_UnaryNegation(((Component) this).get_transform().InverseTransformDirection(vector3_7)), Vector3.get_zero());
      pos = ((Component) this).get_transform().TransformPoint(pos);
      Quaternion rot = QuaternionEx.LookRotationNormal(Vector3.op_UnaryNegation(vector3_8), Vector3.get_zero());
      this.xMarker = GameManager.server.CreateEntity("assets/content/nature/treesprefabs/trees/effects/tree_marking.prefab", pos, rot, true);
      this.xMarker.Spawn();
      if ((double) num1 > 5.0)
        this.StartBonusGame();
      base.OnAttacked(info);
      if ((double) this.health <= 0.0)
        return;
      this.lastAttackDamage = info.damageTypes.Total();
      int num4 = Mathf.CeilToInt(this.health / this.lastAttackDamage);
      if (num4 < 2)
      {
        this.ClientRPC<int>((Connection) null, "CrackSound", 1);
      }
      else
      {
        if (num4 >= 5)
          return;
        this.ClientRPC<int>((Connection) null, "CrackSound", 0);
      }
    }
  }

  public void CleanupMarker()
  {
    if (Object.op_Implicit((Object) this.xMarker))
      this.xMarker.Kill(BaseNetworkable.DestroyMode.None);
    this.xMarker = (BaseEntity) null;
  }

  public Collider GetCollider()
  {
    if (this.isServer)
    {
      if (!Object.op_Equality((Object) this.serverCollider, (Object) null))
        return this.serverCollider;
      return (Collider) ((Component) this).GetComponentInChildren<CapsuleCollider>();
    }
    if (!Object.op_Equality((Object) this.clientCollider, (Object) null))
      return this.clientCollider;
    return (Collider) ((Component) this).GetComponent<Collider>();
  }

  public override void OnKilled(HitInfo info)
  {
    if (this.isKilled)
      return;
    this.isKilled = true;
    this.CleanupMarker();
    if (this.fallOnKilled)
    {
      Collider collider = this.GetCollider();
      if (Object.op_Implicit((Object) collider))
        collider.set_enabled(false);
      this.ClientRPC<Vector3>((Connection) null, "TreeFall", info.attackNormal);
      this.Invoke(new Action(this.DelayedKill), this.fallDuration + 1f);
    }
    else
      this.DelayedKill();
  }

  public void DelayedKill()
  {
    this.Kill(BaseNetworkable.DestroyMode.None);
  }
}
