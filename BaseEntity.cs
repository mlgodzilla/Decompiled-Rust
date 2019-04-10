// Decompiled with JetBrains decompiler
// Type: BaseEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Facepunch.Extend;
using Network;
using Network.Visibility;
using ProtoBuf;
using Rust;
using Rust.Ai;
using Rust.Workshop;
using Spatial;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class BaseEntity : BaseNetworkable, IOnParentSpawning, IPrefabPreProcess
{
  private static Queue<BaseEntity> globalBroadcastQueue = new Queue<BaseEntity>();
  private static uint globalBroadcastProtocol = 0;
  public static HashSet<BaseEntity> saveList = new HashSet<BaseEntity>();
  public bool enableSaving = true;
  private List<EntityLink> links = new List<EntityLink>();
  private int doneMovingWithoutARigidBodyCheck = 1;
  private EntityRef[] entitySlots = new EntityRef[7];
  protected bool isVisible = true;
  protected bool isAnimatorVisible = true;
  protected bool isShadowVisible = true;
  protected OccludeeSphere localOccludee = new OccludeeSphere(-1);
  [Header("BaseEntity")]
  public Bounds bounds;
  public GameObjectRef impactEffect;
  public bool syncPosition;
  public Model model;
  [InspectorFlags]
  public BaseEntity.Flags flags;
  [NonSerialized]
  public uint parentBone;
  [NonSerialized]
  public ulong skinID;
  private EntityComponentBase[] _components;
  [NonSerialized]
  public string _name;
  private uint broadcastProtocol;
  private bool linkedToNeighbours;
  private Spawnable _spawnable;
  [NonSerialized]
  public BaseEntity creatorEntity;
  private bool isCallingUpdateNetworkGroup;
  protected List<TriggerBase> triggers;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("BaseEntity.OnRpcMessage", 0.1f))
    {
      if (rpc == 1552640099U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - BroadcastSignalFromClient "));
        using (TimeWarning.New("BroadcastSignalFromClient", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.FromOwner.Test("BroadcastSignalFromClient", this, player))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.BroadcastSignalFromClient(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in BroadcastSignalFromClient");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 3645147041U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SV_RequestFile "));
          using (TimeWarning.New("SV_RequestFile", 0.1f))
          {
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.SV_RequestFile(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in SV_RequestFile");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public virtual void OnCollision(Collision collision, BaseEntity hitEntity)
  {
    throw new NotImplementedException();
  }

  protected void ReceiveCollisionMessages(bool b)
  {
    if (b)
      TransformEx.GetOrAddComponent<EntityCollisionMessage>(((Component) this).get_gameObject().get_transform());
    else
      ((Component) this).get_gameObject().get_transform().RemoveComponent<EntityCollisionMessage>();
  }

  public EntityComponentBase[] Components
  {
    get
    {
      return this._components ?? (this._components = (EntityComponentBase[]) ((Component) this).GetComponentsInChildren<EntityComponentBase>(true));
    }
  }

  public virtual BasePlayer ToPlayer()
  {
    return (BasePlayer) null;
  }

  public virtual bool IsNpc
  {
    get
    {
      return false;
    }
  }

  public override void InitShared()
  {
    base.InitShared();
    this.InitEntityLinks();
  }

  public override void DestroyShared()
  {
    base.DestroyShared();
    this.FreeEntityLinks();
  }

  public override void ResetState()
  {
    base.ResetState();
    this.parentBone = 0U;
    this.OwnerID = 0UL;
    this.flags = (BaseEntity.Flags) 0;
    this.parentEntity = new EntityRef();
    this._spawnable = (Spawnable) null;
  }

  public virtual float InheritedVelocityScale()
  {
    return 0.0f;
  }

  public Vector3 GetInheritedProjectileVelocity()
  {
    BaseEntity baseEntity = this.parentEntity.Get(this.isServer);
    if (Object.op_Equality((Object) baseEntity, (Object) null))
      return Vector3.get_zero();
    return Vector3.op_Multiply(this.GetParentVelocity(), baseEntity.InheritedVelocityScale());
  }

  public Vector3 GetInheritedThrowVelocity()
  {
    return this.GetParentVelocity();
  }

  public Vector3 GetInheritedDropVelocity()
  {
    BaseEntity baseEntity = this.parentEntity.Get(this.isServer);
    if (!Object.op_Inequality((Object) baseEntity, (Object) null))
      return Vector3.get_zero();
    return baseEntity.GetWorldVelocity();
  }

  public Vector3 GetParentVelocity()
  {
    BaseEntity baseEntity = this.parentEntity.Get(this.isServer);
    if (!Object.op_Inequality((Object) baseEntity, (Object) null))
      return Vector3.get_zero();
    return Vector3.op_Addition(baseEntity.GetWorldVelocity(), Vector3.op_Subtraction(Quaternion.op_Multiply(baseEntity.GetAngularVelocity(), ((Component) this).get_transform().get_localPosition()), ((Component) this).get_transform().get_localPosition()));
  }

  public Vector3 GetWorldVelocity()
  {
    BaseEntity baseEntity = this.parentEntity.Get(this.isServer);
    if (!Object.op_Inequality((Object) baseEntity, (Object) null))
      return this.GetLocalVelocity();
    return Vector3.op_Addition(Vector3.op_Addition(baseEntity.GetWorldVelocity(), Vector3.op_Subtraction(Quaternion.op_Multiply(baseEntity.GetAngularVelocity(), ((Component) this).get_transform().get_localPosition()), ((Component) this).get_transform().get_localPosition())), ((Component) baseEntity).get_transform().TransformDirection(this.GetLocalVelocity()));
  }

  public Vector3 GetLocalVelocity()
  {
    if (this.isServer)
      return this.GetLocalVelocityServer();
    return Vector3.get_zero();
  }

  public Quaternion GetAngularVelocity()
  {
    if (this.isServer)
      return this.GetAngularVelocityServer();
    return Quaternion.get_identity();
  }

  public OBB WorldSpaceBounds()
  {
    return new OBB(((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_lossyScale(), ((Component) this).get_transform().get_rotation(), this.bounds);
  }

  public Vector3 PivotPoint()
  {
    return ((Component) this).get_transform().get_position();
  }

  public Vector3 CenterPoint()
  {
    return (Vector3) this.WorldSpaceBounds().position;
  }

  public Vector3 ClosestPoint(Vector3 position)
  {
    OBB obb = this.WorldSpaceBounds();
    return ((OBB) ref obb).ClosestPoint(position);
  }

  public float Distance(Vector3 position)
  {
    Vector3 vector3 = Vector3.op_Subtraction(this.ClosestPoint(position), position);
    return ((Vector3) ref vector3).get_magnitude();
  }

  public float SqrDistance(Vector3 position)
  {
    Vector3 vector3 = Vector3.op_Subtraction(this.ClosestPoint(position), position);
    return ((Vector3) ref vector3).get_sqrMagnitude();
  }

  public float Distance(BaseEntity other)
  {
    return this.Distance(((Component) other).get_transform().get_position());
  }

  public float SqrDistance(BaseEntity other)
  {
    return this.SqrDistance(((Component) other).get_transform().get_position());
  }

  public float Distance2D(Vector3 position)
  {
    return Vector3Ex.Magnitude2D(Vector3.op_Subtraction(this.ClosestPoint(position), position));
  }

  public float SqrDistance2D(Vector3 position)
  {
    return Vector3Ex.SqrMagnitude2D(Vector3.op_Subtraction(this.ClosestPoint(position), position));
  }

  public float Distance2D(BaseEntity other)
  {
    return this.Distance(((Component) other).get_transform().get_position());
  }

  public float SqrDistance2D(BaseEntity other)
  {
    return this.SqrDistance(((Component) other).get_transform().get_position());
  }

  public bool IsVisible(Ray ray, float maxDistance = float.PositiveInfinity)
  {
    if (Vector3Ex.IsNaNOrInfinity(((Ray) ref ray).get_origin()) || Vector3Ex.IsNaNOrInfinity(((Ray) ref ray).get_direction()) || Vector3.op_Equality(((Ray) ref ray).get_direction(), Vector3.get_zero()))
      return false;
    OBB obb = this.WorldSpaceBounds();
    RaycastHit raycastHit;
    if (!((OBB) ref obb).Trace(ray, ref raycastHit, maxDistance))
      return false;
    RaycastHit hitInfo;
    if (!GamePhysics.Trace(ray, 0.0f, out hitInfo, maxDistance, 1218519041, (QueryTriggerInteraction) 0))
      return true;
    BaseEntity entity = hitInfo.GetEntity();
    return Object.op_Equality((Object) entity, (Object) this) || Object.op_Inequality((Object) entity, (Object) null) && Object.op_Implicit((Object) this.GetParentEntity()) && (this.GetParentEntity().EqualNetID((BaseNetworkable) entity) && Object.op_Inequality((Object) ((RaycastHit) ref hitInfo).get_collider(), (Object) null)) && ((Component) ((RaycastHit) ref hitInfo).get_collider()).get_gameObject().get_layer() == 13 || (double) ((RaycastHit) ref hitInfo).get_distance() > (double) ((RaycastHit) ref raycastHit).get_distance();
  }

  public bool IsVisible(Vector3 position, Vector3 target, float maxDistance = float.PositiveInfinity)
  {
    Vector3 vector3_1 = Vector3.op_Subtraction(target, position);
    float magnitude = ((Vector3) ref vector3_1).get_magnitude();
    if ((double) magnitude < Mathf.Epsilon)
      return true;
    Vector3 vector3_2 = Vector3.op_Division(vector3_1, magnitude);
    Vector3 vector3_3 = Vector3.op_Multiply(vector3_2, Mathf.Min(magnitude, 0.01f));
    return this.IsVisible(new Ray(Vector3.op_Addition(position, vector3_3), vector3_2), maxDistance);
  }

  public bool IsVisible(Vector3 position, float maxDistance = float.PositiveInfinity)
  {
    if (!this.IsVisible(position, this.CenterPoint(), maxDistance))
      return this.IsVisible(position, this.ClosestPoint(position), maxDistance);
    return true;
  }

  public bool IsOlderThan(BaseEntity other)
  {
    if (Object.op_Equality((Object) other, (Object) null))
      return true;
    return (this.net == null ? 0U : (uint) (int) this.net.ID) < (other.net == null ? 0U : (uint) (int) other.net.ID);
  }

  public virtual bool IsOutside()
  {
    OBB obb = this.WorldSpaceBounds();
    return this.IsOutside(Vector3.op_Addition((Vector3) obb.position, Vector3.op_Multiply((Vector3) obb.up, (float) ((Vector3) obb.extents).y)));
  }

  public bool IsOutside(Vector3 position)
  {
    return !Physics.Raycast(position, Vector3.get_up(), 100f, 1101070337);
  }

  public virtual float WaterFactor()
  {
    OBB obb = this.WorldSpaceBounds();
    return WaterLevel.Factor(((OBB) ref obb).ToBounds());
  }

  public virtual float Health()
  {
    return 0.0f;
  }

  public virtual float MaxHealth()
  {
    return 0.0f;
  }

  public virtual float MaxVelocity()
  {
    return 0.0f;
  }

  public virtual float BoundsPadding()
  {
    return 0.1f;
  }

  public virtual float PenetrationResistance(HitInfo info)
  {
    return 1f;
  }

  public virtual GameObjectRef GetImpactEffect(HitInfo info)
  {
    return this.impactEffect;
  }

  public virtual void OnAttacked(HitInfo info)
  {
  }

  public virtual Item GetItem()
  {
    return (Item) null;
  }

  public virtual Item GetItem(uint itemId)
  {
    return (Item) null;
  }

  public virtual void GiveItem(Item item, BaseEntity.GiveItemReason reason = BaseEntity.GiveItemReason.Generic)
  {
    item.Remove(0.0f);
  }

  public virtual bool CanBeLooted(BasePlayer player)
  {
    return true;
  }

  public virtual BaseEntity GetEntity()
  {
    return this;
  }

  public virtual string ToString()
  {
    if (this._name == null)
      this._name = !this.isServer ? this.ShortPrefabName : string.Format("{1}[{0}]", (object) (uint) (this.net != null ? (int) this.net.ID : 0), (object) this.ShortPrefabName);
    return this._name;
  }

  public virtual string Categorize()
  {
    return "entity";
  }

  public void Log(string str)
  {
    if (this.isClient)
      Debug.Log((object) ("<color=#ffa>[" + ((object) this).ToString() + "] " + str + "</color>"), (Object) ((Component) this).get_gameObject());
    else
      Debug.Log((object) ("<color=#aff>[" + ((object) this).ToString() + "] " + str + "</color>"), (Object) ((Component) this).get_gameObject());
  }

  public void SetModel(Model mdl)
  {
    if (Object.op_Equality((Object) this.model, (Object) mdl))
      return;
    this.model = mdl;
  }

  public Model GetModel()
  {
    return this.model;
  }

  public virtual Transform[] GetBones()
  {
    return (Transform[]) null;
  }

  public virtual Transform FindBone(string strName)
  {
    if (Object.op_Implicit((Object) this.model))
      return this.model.FindBone(strName);
    return ((Component) this).get_transform();
  }

  public virtual Transform FindClosestBone(Vector3 worldPos)
  {
    if (Object.op_Implicit((Object) this.model))
      return this.model.FindClosestBone(worldPos);
    return ((Component) this).get_transform();
  }

  public ulong OwnerID { get; set; }

  public virtual bool ShouldBlockProjectiles()
  {
    return true;
  }

  public virtual bool ShouldInheritNetworkGroup()
  {
    return true;
  }

  public virtual bool SupportsChildDeployables()
  {
    return true;
  }

  public void BroadcastEntityMessage(string msg, float radius = 20f, int layerMask = 1218652417)
  {
    if (this.isClient)
      return;
    List<BaseEntity> list = (List<BaseEntity>) Pool.GetList<BaseEntity>();
    Vis.Entities<BaseEntity>(((Component) this).get_transform().get_position(), radius, list, layerMask, (QueryTriggerInteraction) 2);
    foreach (BaseEntity baseEntity in list)
    {
      if (baseEntity.isServer)
        baseEntity.OnEntityMessage(this, msg);
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<BaseEntity>((List<M0>&) ref list);
  }

  public virtual void OnEntityMessage(BaseEntity from, string msg)
  {
  }

  public virtual void DebugServer(int rep, float time)
  {
    this.DebugText(Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_up(), 1f)), string.Format("{0}: {1}\n{2}", (object) (uint) (this.net != null ? (int) this.net.ID : 0), (object) ((Object) this).get_name(), (object) this.DebugText()), Color.get_white(), time);
  }

  public virtual string DebugText()
  {
    return "";
  }

  public void OnDebugStart()
  {
    EntityDebug entityDebug = (EntityDebug) ((Component) this).get_gameObject().GetComponent<EntityDebug>();
    if (Object.op_Equality((Object) entityDebug, (Object) null))
      entityDebug = (EntityDebug) ((Component) this).get_gameObject().AddComponent<EntityDebug>();
    ((Behaviour) entityDebug).set_enabled(true);
  }

  protected void DebugText(Vector3 pos, string str, Color color, float time)
  {
    if (!this.isServer)
      return;
    ConsoleNetwork.BroadcastToAllClients("ddraw.text", (object) time, (object) color, (object) pos, (object) str);
  }

  public bool HasFlag(BaseEntity.Flags f)
  {
    return (this.flags & f) == f;
  }

  public bool ParentHasFlag(BaseEntity.Flags f)
  {
    BaseEntity parentEntity = this.GetParentEntity();
    if (Object.op_Equality((Object) parentEntity, (Object) null))
      return false;
    return parentEntity.HasFlag(f);
  }

  public void SetFlag(BaseEntity.Flags f, bool b, bool recursive = false, bool networkupdate = true)
  {
    BaseEntity.Flags flags = this.flags;
    if (b)
    {
      if (this.HasFlag(f))
        return;
      this.flags |= f;
    }
    else
    {
      if (!this.HasFlag(f))
        return;
      this.flags &= ~f;
    }
    this.OnFlagsChanged(flags, this.flags);
    if (networkupdate)
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    if (!recursive || this.children == null)
      return;
    for (int index = 0; index < this.children.Count; ++index)
      this.children[index].SetFlag(f, b, true, true);
  }

  public bool IsOn()
  {
    return this.HasFlag(BaseEntity.Flags.On);
  }

  public bool IsOpen()
  {
    return this.HasFlag(BaseEntity.Flags.Open);
  }

  public bool IsOnFire()
  {
    return this.HasFlag(BaseEntity.Flags.OnFire);
  }

  public bool IsLocked()
  {
    return this.HasFlag(BaseEntity.Flags.Locked);
  }

  public override bool IsDebugging()
  {
    return this.HasFlag(BaseEntity.Flags.Debugging);
  }

  public bool IsDisabled()
  {
    if (!this.HasFlag(BaseEntity.Flags.Disabled))
      return this.ParentHasFlag(BaseEntity.Flags.Disabled);
    return true;
  }

  public bool IsBroken()
  {
    return this.HasFlag(BaseEntity.Flags.Broken);
  }

  public bool IsBusy()
  {
    return this.HasFlag(BaseEntity.Flags.Busy);
  }

  public override string GetLogColor()
  {
    return this.isServer ? "cyan" : "yellow";
  }

  public virtual void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
  {
    if (!this.IsDebugging() || (old & BaseEntity.Flags.Debugging) == (next & BaseEntity.Flags.Debugging))
      return;
    this.OnDebugStart();
  }

  protected void SendNetworkUpdate_Flags()
  {
    if (Application.isLoading != null || Application.isLoadingSave != null || (this.IsDestroyed || this.net == null) || !this.isSpawned)
      return;
    List<Connection> subscribers = this.GetSubscribers();
    if (subscribers == null || subscribers.Count == 0)
      return;
    this.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, nameof (SendNetworkUpdate_Flags));
    if (!((Write) ((NetworkPeer) Net.sv).write).Start())
      return;
    ((Write) ((NetworkPeer) Net.sv).write).PacketID((Message.Type) 23);
    ((Write) ((NetworkPeer) Net.sv).write).EntityID((uint) this.net.ID);
    ((Write) ((NetworkPeer) Net.sv).write).Int32((int) this.flags);
    ((Write) ((NetworkPeer) Net.sv).write).Send(new SendInfo((IEnumerable<Connection>) subscribers));
  }

  public bool IsOccupied(Socket_Base socket)
  {
    EntityLink link = this.FindLink(socket);
    if (link != null)
      return link.IsOccupied();
    return false;
  }

  public bool IsOccupied(string socketName)
  {
    EntityLink link = this.FindLink(socketName);
    if (link != null)
      return link.IsOccupied();
    return false;
  }

  public EntityLink FindLink(Socket_Base socket)
  {
    List<EntityLink> entityLinks = this.GetEntityLinks(true);
    for (int index = 0; index < entityLinks.Count; ++index)
    {
      if ((PrefabAttribute) entityLinks[index].socket == (PrefabAttribute) socket)
        return entityLinks[index];
    }
    return (EntityLink) null;
  }

  public EntityLink FindLink(string socketName)
  {
    List<EntityLink> entityLinks = this.GetEntityLinks(true);
    for (int index = 0; index < entityLinks.Count; ++index)
    {
      if (entityLinks[index].socket.socketName == socketName)
        return entityLinks[index];
    }
    return (EntityLink) null;
  }

  public T FindLinkedEntity<T>() where T : BaseEntity
  {
    List<EntityLink> entityLinks = this.GetEntityLinks(true);
    for (int index1 = 0; index1 < entityLinks.Count; ++index1)
    {
      EntityLink entityLink = entityLinks[index1];
      for (int index2 = 0; index2 < entityLink.connections.Count; ++index2)
      {
        EntityLink connection = entityLink.connections[index2];
        if (connection.owner is T)
          return connection.owner as T;
      }
    }
    return default (T);
  }

  public void EntityLinkMessage<T>(Action<T> action) where T : BaseEntity
  {
    List<EntityLink> entityLinks = this.GetEntityLinks(true);
    for (int index1 = 0; index1 < entityLinks.Count; ++index1)
    {
      EntityLink entityLink = entityLinks[index1];
      for (int index2 = 0; index2 < entityLink.connections.Count; ++index2)
      {
        EntityLink connection = entityLink.connections[index2];
        if (connection.owner is T)
          action(connection.owner as T);
      }
    }
  }

  public void EntityLinkBroadcast<T>(Action<T> action) where T : BaseEntity
  {
    ++BaseEntity.globalBroadcastProtocol;
    BaseEntity.globalBroadcastQueue.Clear();
    this.broadcastProtocol = BaseEntity.globalBroadcastProtocol;
    BaseEntity.globalBroadcastQueue.Enqueue(this);
    if (this is T)
      action(this as T);
    while (BaseEntity.globalBroadcastQueue.Count > 0)
    {
      List<EntityLink> entityLinks = BaseEntity.globalBroadcastQueue.Dequeue().GetEntityLinks(true);
      for (int index1 = 0; index1 < entityLinks.Count; ++index1)
      {
        EntityLink entityLink = entityLinks[index1];
        for (int index2 = 0; index2 < entityLink.connections.Count; ++index2)
        {
          BaseEntity owner = entityLink.connections[index2].owner;
          if ((int) owner.broadcastProtocol != (int) BaseEntity.globalBroadcastProtocol)
          {
            owner.broadcastProtocol = BaseEntity.globalBroadcastProtocol;
            BaseEntity.globalBroadcastQueue.Enqueue(owner);
            if (owner is T)
              action(owner as T);
          }
        }
      }
    }
  }

  public void EntityLinkBroadcast()
  {
    ++BaseEntity.globalBroadcastProtocol;
    BaseEntity.globalBroadcastQueue.Clear();
    this.broadcastProtocol = BaseEntity.globalBroadcastProtocol;
    BaseEntity.globalBroadcastQueue.Enqueue(this);
    while (BaseEntity.globalBroadcastQueue.Count > 0)
    {
      List<EntityLink> entityLinks = BaseEntity.globalBroadcastQueue.Dequeue().GetEntityLinks(true);
      for (int index1 = 0; index1 < entityLinks.Count; ++index1)
      {
        EntityLink entityLink = entityLinks[index1];
        for (int index2 = 0; index2 < entityLink.connections.Count; ++index2)
        {
          BaseEntity owner = entityLink.connections[index2].owner;
          if ((int) owner.broadcastProtocol != (int) BaseEntity.globalBroadcastProtocol)
          {
            owner.broadcastProtocol = BaseEntity.globalBroadcastProtocol;
            BaseEntity.globalBroadcastQueue.Enqueue(owner);
          }
        }
      }
    }
  }

  public bool ReceivedEntityLinkBroadcast()
  {
    return (int) this.broadcastProtocol == (int) BaseEntity.globalBroadcastProtocol;
  }

  public List<EntityLink> GetEntityLinks(bool linkToNeighbours = true)
  {
    if (Application.isLoadingSave != null || !(!this.linkedToNeighbours & linkToNeighbours))
      return this.links;
    this.LinkToNeighbours();
    return this.links;
  }

  private void LinkToEntity(BaseEntity other)
  {
    if (Object.op_Equality((Object) this, (Object) other) || this.links.Count == 0 || other.links.Count == 0)
      return;
    using (TimeWarning.New(nameof (LinkToEntity), 0.1f))
    {
      for (int index1 = 0; index1 < this.links.Count; ++index1)
      {
        EntityLink link1 = this.links[index1];
        for (int index2 = 0; index2 < other.links.Count; ++index2)
        {
          EntityLink link2 = other.links[index2];
          if (link1.CanConnect(link2))
          {
            if (!link1.Contains(link2))
              link1.Add(link2);
            if (!link2.Contains(link1))
              link2.Add(link1);
          }
        }
      }
    }
  }

  private void LinkToNeighbours()
  {
    if (this.links.Count == 0)
      return;
    this.linkedToNeighbours = true;
    using (TimeWarning.New(nameof (LinkToNeighbours), 0.1f))
    {
      List<BaseEntity> list = (List<BaseEntity>) Pool.GetList<BaseEntity>();
      OBB obb = this.WorldSpaceBounds();
      Vis.Entities<BaseEntity>((Vector3) obb.position, ((Vector3) ref obb.extents).get_magnitude() + 1f, list, -1, (QueryTriggerInteraction) 2);
      for (int index = 0; index < list.Count; ++index)
      {
        BaseEntity other = list[index];
        if (other.isServer == this.isServer)
          this.LinkToEntity(other);
      }
      // ISSUE: cast to a reference type
      Pool.FreeList<BaseEntity>((List<M0>&) ref list);
    }
  }

  private void InitEntityLinks()
  {
    using (TimeWarning.New(nameof (InitEntityLinks), 0.1f))
    {
      if (!this.isServer)
        return;
      this.links.AddLinks(this, PrefabAttribute.server.FindAll<Socket_Base>(this.prefabID));
    }
  }

  private void FreeEntityLinks()
  {
    using (TimeWarning.New(nameof (FreeEntityLinks), 0.1f))
    {
      this.links.FreeLinks();
      this.linkedToNeighbours = false;
    }
  }

  public void RefreshEntityLinks()
  {
    using (TimeWarning.New(nameof (RefreshEntityLinks), 0.1f))
    {
      this.links.ClearLinks();
      this.LinkToNeighbours();
    }
  }

  [BaseEntity.RPC_Server]
  public void SV_RequestFile(BaseEntity.RPCMessage msg)
  {
    uint crc = msg.read.UInt32();
    FileStorage.Type type = (FileStorage.Type) msg.read.UInt8();
    string funcName = StringPool.Get(msg.read.UInt32());
    byte[] numArray = FileStorage.server.Get(crc, type, (uint) this.net.ID);
    if (numArray == null)
      return;
    SendInfo sendInfo;
    ((SendInfo) ref sendInfo).\u002Ector(msg.connection);
    sendInfo.channel = (__Null) 2;
    sendInfo.method = (__Null) 0;
    this.ClientRPCEx<uint, uint, byte[]>(sendInfo, (Connection) null, funcName, crc, (uint) numArray.Length, numArray);
  }

  public void SetParent(BaseEntity entity, bool worldPositionStays = false, bool sendImmediate = false)
  {
    this.SetParent(entity, 0U, worldPositionStays, sendImmediate);
  }

  public void SetParent(
    BaseEntity entity,
    string strBone,
    bool worldPositionStays = false,
    bool sendImmediate = false)
  {
    this.SetParent(entity, string.IsNullOrEmpty(strBone) ? 0U : StringPool.Get(strBone), worldPositionStays, sendImmediate);
  }

  public bool HasChild(BaseEntity c)
  {
    if (Object.op_Equality((Object) c, (Object) this))
      return true;
    BaseEntity parentEntity = c.GetParentEntity();
    if (Object.op_Inequality((Object) parentEntity, (Object) null))
      return this.HasChild(parentEntity);
    return false;
  }

  public void SetParent(
    BaseEntity entity,
    uint boneID,
    bool worldPositionStays = false,
    bool sendImmediate = false)
  {
    if (Object.op_Inequality((Object) entity, (Object) null))
    {
      if (Object.op_Equality((Object) entity, (Object) this))
      {
        Debug.LogError((object) ("Trying to parent to self " + (object) this), (Object) ((Component) this).get_gameObject());
        return;
      }
      if (this.HasChild(entity))
      {
        Debug.LogError((object) ("Trying to parent to child " + (object) this), (Object) ((Component) this).get_gameObject());
        return;
      }
    }
    this.LogEntry(BaseMonoBehaviour.LogEntryType.Hierarchy, 2, "SetParent {0} {1}", (object) entity, (object) boneID);
    BaseEntity parentEntity = this.GetParentEntity();
    if (Object.op_Implicit((Object) parentEntity))
      parentEntity.RemoveChild(this);
    if (this.limitNetworking && Object.op_Inequality((Object) parentEntity, (Object) null) && Object.op_Inequality((Object) parentEntity, (Object) entity))
    {
      BasePlayer ent = parentEntity as BasePlayer;
      if (ent.IsValid())
        this.DestroyOnClient(ent.net.get_connection());
    }
    if (Object.op_Equality((Object) entity, (Object) null))
    {
      if (!this.parentEntity.IsValid(this.isServer))
        return;
      this.OnParentChanging(parentEntity, (BaseEntity) null);
      this.parentEntity.Set((BaseEntity) null);
      ((Component) this).get_transform().SetParent((Transform) null, worldPositionStays);
      this.parentBone = 0U;
      this.UpdateNetworkGroup();
      if (sendImmediate)
      {
        this.SendNetworkUpdateImmediate(false);
        this.SendChildrenNetworkUpdateImmediate();
      }
      else
      {
        this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
        this.SendChildrenNetworkUpdate();
      }
    }
    else
    {
      Debug.Assert(entity.isServer, "SetParent - child should be a SERVER entity");
      Debug.Assert(entity.net != null, "Setting parent to entity that hasn't spawned yet! (net is null)");
      Debug.Assert(entity.net.ID > 0, "Setting parent to entity that hasn't spawned yet! (id = 0)");
      entity.AddChild(this);
      this.OnParentChanging(parentEntity, entity);
      this.parentEntity.Set(entity);
      if (boneID != 0U && (int) boneID != (int) StringPool.closest)
        ((Component) this).get_transform().SetParent(entity.FindBone(StringPool.Get(boneID)), worldPositionStays);
      else
        ((Component) this).get_transform().SetParent(((Component) entity).get_transform(), worldPositionStays);
      this.parentBone = boneID;
      this.UpdateNetworkGroup();
      if (sendImmediate)
      {
        this.SendNetworkUpdateImmediate(false);
        this.SendChildrenNetworkUpdateImmediate();
      }
      else
      {
        this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
        this.SendChildrenNetworkUpdate();
      }
    }
  }

  public void DestroyOnClient(Connection connection)
  {
    if (this.children != null)
    {
      foreach (BaseEntity child in this.children)
        child.DestroyOnClient(connection);
    }
    if (!((Network.Server) Net.sv).IsConnected() || !((Write) ((NetworkPeer) Net.sv).write).Start())
      return;
    ((Write) ((NetworkPeer) Net.sv).write).PacketID((Message.Type) 6);
    ((Write) ((NetworkPeer) Net.sv).write).EntityID((uint) this.net.ID);
    ((Write) ((NetworkPeer) Net.sv).write).UInt8((byte) 0);
    ((Write) ((NetworkPeer) Net.sv).write).Send(new SendInfo(connection));
    this.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "EntityDestroy");
  }

  private void SendChildrenNetworkUpdate()
  {
    if (this.children == null)
      return;
    foreach (BaseEntity child in this.children)
    {
      child.UpdateNetworkGroup();
      child.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    }
  }

  private void SendChildrenNetworkUpdateImmediate()
  {
    if (this.children == null)
      return;
    foreach (BaseEntity child in this.children)
    {
      child.UpdateNetworkGroup();
      child.SendNetworkUpdateImmediate(false);
    }
  }

  public virtual void SwitchParent(BaseEntity ent)
  {
    this.Log("SwitchParent Missed " + (object) ent);
  }

  public virtual void OnParentChanging(BaseEntity oldParent, BaseEntity newParent)
  {
    Rigidbody component = (Rigidbody) ((Component) this).GetComponent<Rigidbody>();
    if (!Object.op_Implicit((Object) component))
      return;
    if (Object.op_Inequality((Object) oldParent, (Object) null))
    {
      Rigidbody rigidbody = component;
      rigidbody.set_velocity(Vector3.op_Addition(rigidbody.get_velocity(), oldParent.GetWorldVelocity()));
    }
    if (!Object.op_Inequality((Object) newParent, (Object) null))
      return;
    Rigidbody rigidbody1 = component;
    rigidbody1.set_velocity(Vector3.op_Subtraction(rigidbody1.get_velocity(), newParent.GetWorldVelocity()));
  }

  public virtual BuildingPrivlidge GetBuildingPrivilege()
  {
    return this.GetBuildingPrivilege(this.WorldSpaceBounds());
  }

  public BuildingPrivlidge GetBuildingPrivilege(OBB obb)
  {
    BuildingBlock buildingBlock1 = (BuildingBlock) null;
    BuildingPrivlidge buildingPrivlidge = (BuildingPrivlidge) null;
    List<BuildingBlock> list = (List<BuildingBlock>) Pool.GetList<BuildingBlock>();
    Vis.Entities<BuildingBlock>((Vector3) obb.position, 16f + ((Vector3) ref obb.extents).get_magnitude(), list, 2097152, (QueryTriggerInteraction) 2);
    for (int index = 0; index < list.Count; ++index)
    {
      BuildingBlock buildingBlock2 = list[index];
      if (buildingBlock2.isServer == this.isServer && buildingBlock2.IsOlderThan((BaseEntity) buildingBlock1) && (double) ((OBB) ref obb).Distance(buildingBlock2.WorldSpaceBounds()) <= 16.0)
      {
        BuildingManager.Building building = buildingBlock2.GetBuilding();
        if (building != null)
        {
          BuildingPrivlidge buildingPrivilege = building.GetDominatingBuildingPrivilege();
          if (!Object.op_Equality((Object) buildingPrivilege, (Object) null))
          {
            buildingBlock1 = buildingBlock2;
            buildingPrivlidge = buildingPrivilege;
          }
        }
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<BuildingBlock>((List<M0>&) ref list);
    return buildingPrivlidge;
  }

  public void SV_RPCMessage(uint nameID, Message message)
  {
    Assert.IsTrue(this.isServer, "Should be server!");
    BasePlayer basePlayer = message.Player();
    if (!basePlayer.IsValid())
    {
      if (Global.developer <= 0)
        return;
      Debug.Log((object) ("SV_RPCMessage: From invalid player " + (object) basePlayer));
    }
    else if (basePlayer.isStalled)
    {
      if (Global.developer <= 0)
        return;
      Debug.Log((object) ("SV_RPCMessage: player is stalled " + (object) basePlayer));
    }
    else
    {
      if (this.OnRpcMessage(basePlayer, nameID, message))
        return;
      int index = 0;
      while (index < this.Components.Length && !this.Components[index].OnRpcMessage(basePlayer, nameID, message))
        ++index;
    }
  }

  public void ClientRPCPlayer<T1, T2, T3, T4, T5>(
    Connection sourceConnection,
    BasePlayer player,
    string funcName,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5)
  {
    if (!((Network.Server) Net.sv).IsConnected() || this.net == null || player.net.get_connection() == null)
      return;
    this.ClientRPCEx<T1, T2, T3, T4, T5>(new SendInfo(player.net.get_connection()), sourceConnection, funcName, arg1, arg2, arg3, arg4, arg5);
  }

  public void ClientRPCPlayer<T1, T2, T3, T4>(
    Connection sourceConnection,
    BasePlayer player,
    string funcName,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4)
  {
    if (!((Network.Server) Net.sv).IsConnected() || this.net == null || player.net.get_connection() == null)
      return;
    this.ClientRPCEx<T1, T2, T3, T4>(new SendInfo(player.net.get_connection()), sourceConnection, funcName, arg1, arg2, arg3, arg4);
  }

  public void ClientRPCPlayer<T1, T2, T3>(
    Connection sourceConnection,
    BasePlayer player,
    string funcName,
    T1 arg1,
    T2 arg2,
    T3 arg3)
  {
    if (!((Network.Server) Net.sv).IsConnected() || this.net == null || player.net.get_connection() == null)
      return;
    this.ClientRPCEx<T1, T2, T3>(new SendInfo(player.net.get_connection()), sourceConnection, funcName, arg1, arg2, arg3);
  }

  public void ClientRPCPlayer<T1, T2>(
    Connection sourceConnection,
    BasePlayer player,
    string funcName,
    T1 arg1,
    T2 arg2)
  {
    if (!((Network.Server) Net.sv).IsConnected() || this.net == null || player.net.get_connection() == null)
      return;
    this.ClientRPCEx<T1, T2>(new SendInfo(player.net.get_connection()), sourceConnection, funcName, arg1, arg2);
  }

  public void ClientRPCPlayer<T1>(
    Connection sourceConnection,
    BasePlayer player,
    string funcName,
    T1 arg1)
  {
    if (!((Network.Server) Net.sv).IsConnected() || this.net == null || player.net.get_connection() == null)
      return;
    this.ClientRPCEx<T1>(new SendInfo(player.net.get_connection()), sourceConnection, funcName, arg1);
  }

  public void ClientRPCPlayer(Connection sourceConnection, BasePlayer player, string funcName)
  {
    if (!((Network.Server) Net.sv).IsConnected() || this.net == null || player.net.get_connection() == null)
      return;
    this.ClientRPCEx(new SendInfo(player.net.get_connection()), sourceConnection, funcName);
  }

  public void ClientRPC<T1, T2, T3, T4, T5>(
    Connection sourceConnection,
    string funcName,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5)
  {
    if (!((Network.Server) Net.sv).IsConnected() || this.net == null || this.net.group == null)
      return;
    this.ClientRPCEx<T1, T2, T3, T4, T5>(new SendInfo((IEnumerable<Connection>) ((Group) this.net.group).subscribers), sourceConnection, funcName, arg1, arg2, arg3, arg4, arg5);
  }

  public void ClientRPC<T1, T2, T3, T4>(
    Connection sourceConnection,
    string funcName,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4)
  {
    if (!((Network.Server) Net.sv).IsConnected() || this.net == null || this.net.group == null)
      return;
    this.ClientRPCEx<T1, T2, T3, T4>(new SendInfo((IEnumerable<Connection>) ((Group) this.net.group).subscribers), sourceConnection, funcName, arg1, arg2, arg3, arg4);
  }

  public void ClientRPC<T1, T2, T3>(
    Connection sourceConnection,
    string funcName,
    T1 arg1,
    T2 arg2,
    T3 arg3)
  {
    if (!((Network.Server) Net.sv).IsConnected() || this.net == null || this.net.group == null)
      return;
    this.ClientRPCEx<T1, T2, T3>(new SendInfo((IEnumerable<Connection>) ((Group) this.net.group).subscribers), sourceConnection, funcName, arg1, arg2, arg3);
  }

  public void ClientRPC<T1, T2>(Connection sourceConnection, string funcName, T1 arg1, T2 arg2)
  {
    if (!((Network.Server) Net.sv).IsConnected() || this.net == null || this.net.group == null)
      return;
    this.ClientRPCEx<T1, T2>(new SendInfo((IEnumerable<Connection>) ((Group) this.net.group).subscribers), sourceConnection, funcName, arg1, arg2);
  }

  public void ClientRPC<T1>(Connection sourceConnection, string funcName, T1 arg1)
  {
    if (!((Network.Server) Net.sv).IsConnected() || this.net == null || this.net.group == null)
      return;
    this.ClientRPCEx<T1>(new SendInfo((IEnumerable<Connection>) ((Group) this.net.group).subscribers), sourceConnection, funcName, arg1);
  }

  public void ClientRPC(Connection sourceConnection, string funcName)
  {
    if (!((Network.Server) Net.sv).IsConnected() || this.net == null || this.net.group == null)
      return;
    this.ClientRPCEx(new SendInfo((IEnumerable<Connection>) ((Group) this.net.group).subscribers), sourceConnection, funcName);
  }

  public void ClientRPCEx<T1, T2, T3, T4, T5>(
    SendInfo sendInfo,
    Connection sourceConnection,
    string funcName,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5)
  {
    if (!((Network.Server) Net.sv).IsConnected() || this.net == null || !this.ClientRPCStart(sourceConnection, funcName))
      return;
    this.ClientRPCWrite<T1>(arg1);
    this.ClientRPCWrite<T2>(arg2);
    this.ClientRPCWrite<T3>(arg3);
    this.ClientRPCWrite<T4>(arg4);
    this.ClientRPCWrite<T5>(arg5);
    this.ClientRPCSend(sendInfo);
  }

  public void ClientRPCEx<T1, T2, T3, T4>(
    SendInfo sendInfo,
    Connection sourceConnection,
    string funcName,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4)
  {
    if (!((Network.Server) Net.sv).IsConnected() || this.net == null || !this.ClientRPCStart(sourceConnection, funcName))
      return;
    this.ClientRPCWrite<T1>(arg1);
    this.ClientRPCWrite<T2>(arg2);
    this.ClientRPCWrite<T3>(arg3);
    this.ClientRPCWrite<T4>(arg4);
    this.ClientRPCSend(sendInfo);
  }

  public void ClientRPCEx<T1, T2, T3>(
    SendInfo sendInfo,
    Connection sourceConnection,
    string funcName,
    T1 arg1,
    T2 arg2,
    T3 arg3)
  {
    if (!((Network.Server) Net.sv).IsConnected() || this.net == null || !this.ClientRPCStart(sourceConnection, funcName))
      return;
    this.ClientRPCWrite<T1>(arg1);
    this.ClientRPCWrite<T2>(arg2);
    this.ClientRPCWrite<T3>(arg3);
    this.ClientRPCSend(sendInfo);
  }

  public void ClientRPCEx<T1, T2>(
    SendInfo sendInfo,
    Connection sourceConnection,
    string funcName,
    T1 arg1,
    T2 arg2)
  {
    if (!((Network.Server) Net.sv).IsConnected() || this.net == null || !this.ClientRPCStart(sourceConnection, funcName))
      return;
    this.ClientRPCWrite<T1>(arg1);
    this.ClientRPCWrite<T2>(arg2);
    this.ClientRPCSend(sendInfo);
  }

  public void ClientRPCEx<T1>(
    SendInfo sendInfo,
    Connection sourceConnection,
    string funcName,
    T1 arg1)
  {
    if (!((Network.Server) Net.sv).IsConnected() || this.net == null || !this.ClientRPCStart(sourceConnection, funcName))
      return;
    this.ClientRPCWrite<T1>(arg1);
    this.ClientRPCSend(sendInfo);
  }

  public void ClientRPCEx(SendInfo sendInfo, Connection sourceConnection, string funcName)
  {
    if (!((Network.Server) Net.sv).IsConnected() || this.net == null || !this.ClientRPCStart(sourceConnection, funcName))
      return;
    this.ClientRPCSend(sendInfo);
  }

  private bool ClientRPCStart(Connection sourceConnection, string funcName)
  {
    if (!((Write) ((NetworkPeer) Net.sv).write).Start())
      return false;
    ((Write) ((NetworkPeer) Net.sv).write).PacketID((Message.Type) 9);
    ((Write) ((NetworkPeer) Net.sv).write).UInt32((uint) this.net.ID);
    ((Write) ((NetworkPeer) Net.sv).write).UInt32(StringPool.Get(funcName));
    ((Write) ((NetworkPeer) Net.sv).write).UInt64(sourceConnection == null ? 0UL : (ulong) (long) sourceConnection.userid);
    return true;
  }

  private void ClientRPCWrite<T>(T arg)
  {
    ((Write) ((NetworkPeer) Net.sv).write).WriteObject<T>(arg);
  }

  private void ClientRPCSend(SendInfo sendInfo)
  {
    ((Write) ((NetworkPeer) Net.sv).write).Send(sendInfo);
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.baseEntity = (__Null) Pool.Get<BaseEntity>();
    if (info.forDisk)
    {
      ((BaseEntity) info.msg.baseEntity).pos = (__Null) ((Component) this).get_transform().get_localPosition();
      // ISSUE: variable of the null type
      __Null baseEntity = info.msg.baseEntity;
      Quaternion localRotation = ((Component) this).get_transform().get_localRotation();
      Vector3 eulerAngles = ((Quaternion) ref localRotation).get_eulerAngles();
      ((BaseEntity) baseEntity).rot = (__Null) eulerAngles;
    }
    else
    {
      ((BaseEntity) info.msg.baseEntity).pos = (__Null) this.GetNetworkPosition();
      // ISSUE: variable of the null type
      __Null baseEntity = info.msg.baseEntity;
      Quaternion networkRotation = this.GetNetworkRotation();
      Vector3 eulerAngles = ((Quaternion) ref networkRotation).get_eulerAngles();
      ((BaseEntity) baseEntity).rot = (__Null) eulerAngles;
      ((BaseEntity) info.msg.baseEntity).time = (__Null) (double) this.GetNetworkTime();
    }
    ((BaseEntity) info.msg.baseEntity).flags = (__Null) this.flags;
    ((BaseEntity) info.msg.baseEntity).skinid = (__Null) (long) this.skinID;
    if (this.parentEntity.IsValid(this.isServer))
    {
      info.msg.parent = (__Null) Pool.Get<ParentInfo>();
      ((ParentInfo) info.msg.parent).uid = (__Null) (int) this.parentEntity.uid;
      ((ParentInfo) info.msg.parent).bone = (__Null) (int) this.parentBone;
    }
    if (this.HasAnySlot())
    {
      info.msg.entitySlots = (__Null) Pool.Get<EntitySlots>();
      ((EntitySlots) info.msg.entitySlots).slotLock = (__Null) (int) this.entitySlots[0].uid;
      ((EntitySlots) info.msg.entitySlots).slotFireMod = (__Null) (int) this.entitySlots[1].uid;
      ((EntitySlots) info.msg.entitySlots).slotUpperModification = (__Null) (int) this.entitySlots[2].uid;
      ((EntitySlots) info.msg.entitySlots).centerDecoration = (__Null) (int) this.entitySlots[5].uid;
      ((EntitySlots) info.msg.entitySlots).lowerCenterDecoration = (__Null) (int) this.entitySlots[6].uid;
    }
    if (info.forDisk && Object.op_Implicit((Object) this._spawnable))
      this._spawnable.Save(info);
    if (this.OwnerID == 0UL || !info.forDisk && !this.ShouldNetworkOwnerInfo())
      return;
    info.msg.ownerInfo = (__Null) Pool.Get<OwnerInfo>();
    ((OwnerInfo) info.msg.ownerInfo).steamid = (__Null) (long) this.OwnerID;
  }

  public virtual bool ShouldNetworkOwnerInfo()
  {
    return false;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.baseEntity != null)
    {
      BaseEntity baseEntity = (BaseEntity) info.msg.baseEntity;
      BaseEntity.Flags flags = this.flags;
      this.flags = (BaseEntity.Flags) baseEntity.flags;
      this.OnFlagsChanged(flags, this.flags);
      this.OnSkinChanged(this.skinID, (ulong) ((BaseEntity) info.msg.baseEntity).skinid);
      if (info.fromDisk)
      {
        if (Vector3Ex.IsNaNOrInfinity((Vector3) baseEntity.pos))
        {
          Debug.LogWarning((object) (((object) this).ToString() + " has broken position - " + (object) (Vector3) baseEntity.pos));
          baseEntity.pos = (__Null) Vector3.get_zero();
        }
        ((Component) this).get_transform().set_localPosition((Vector3) baseEntity.pos);
        ((Component) this).get_transform().set_localRotation(Quaternion.Euler((Vector3) baseEntity.rot));
      }
    }
    if (info.msg.entitySlots != null)
    {
      this.entitySlots[0].uid = (uint) ((EntitySlots) info.msg.entitySlots).slotLock;
      this.entitySlots[1].uid = (uint) ((EntitySlots) info.msg.entitySlots).slotFireMod;
      this.entitySlots[2].uid = (uint) ((EntitySlots) info.msg.entitySlots).slotUpperModification;
      this.entitySlots[5].uid = (uint) ((EntitySlots) info.msg.entitySlots).centerDecoration;
      this.entitySlots[6].uid = (uint) ((EntitySlots) info.msg.entitySlots).lowerCenterDecoration;
    }
    if (info.msg.parent != null)
    {
      if (this.isServer)
        this.SetParent(BaseNetworkable.serverEntities.Find((uint) ((ParentInfo) info.msg.parent).uid) as BaseEntity, (uint) ((ParentInfo) info.msg.parent).bone, false, false);
      this.parentEntity.uid = (uint) ((ParentInfo) info.msg.parent).uid;
      this.parentBone = (uint) ((ParentInfo) info.msg.parent).bone;
    }
    else
    {
      this.parentEntity.uid = 0U;
      this.parentBone = 0U;
    }
    if (info.msg.ownerInfo != null)
      this.OwnerID = (ulong) ((OwnerInfo) info.msg.ownerInfo).steamid;
    if (!Object.op_Implicit((Object) this._spawnable))
      return;
    this._spawnable.Load(info);
  }

  public virtual Vector3 GetLocalVelocityServer()
  {
    return Vector3.get_zero();
  }

  public virtual Quaternion GetAngularVelocityServer()
  {
    return Quaternion.get_identity();
  }

  public override void ServerInit()
  {
    this._spawnable = (Spawnable) ((Component) this).GetComponent<Spawnable>();
    base.ServerInit();
    if (this.enableSaving)
    {
      Assert.IsTrue(!BaseEntity.saveList.Contains(this), "Already in save list - server Init being called twice?");
      BaseEntity.saveList.Add(this);
    }
    if (this.flags != (BaseEntity.Flags) 0)
      this.OnFlagsChanged((BaseEntity.Flags) 0, this.flags);
    if (this.syncPosition && (double) this.PositionTickRate >= 0.0)
      this.InvokeRandomized(new Action(this.NetworkPositionTick), this.PositionTickRate, this.PositionTickRate - this.PositionTickRate * 0.05f, this.PositionTickRate * 0.05f);
    BaseEntity.Query.Server.Add(this);
  }

  public virtual void OnSensation(Sensation sensation)
  {
  }

  protected virtual float PositionTickRate
  {
    get
    {
      return 0.1f;
    }
  }

  protected void NetworkPositionTick()
  {
    if (!((Component) this).get_transform().get_hasChanged() && (this.ShouldInheritNetworkGroup() || !this.HasParent()))
      return;
    this.TransformChanged();
    ((Component) this).get_transform().set_hasChanged(false);
  }

  public void TransformChanged()
  {
    if (BaseEntity.Query.Server != null)
      BaseEntity.Query.Server.Move(this);
    if (this.net == null)
      return;
    this.InvalidateNetworkCache();
    if (!this.globalBroadcast && !ValidBounds.Test(((Component) this).get_transform().get_position()))
    {
      this.OnInvalidPosition();
    }
    else
    {
      if (!this.syncPosition)
        return;
      if (!this.isCallingUpdateNetworkGroup)
      {
        this.Invoke(new Action(((BaseNetworkable) this).UpdateNetworkGroup), 5f);
        this.isCallingUpdateNetworkGroup = true;
      }
      this.SendNetworkUpdate_Position();
      this.OnPositionalNetworkUpdate();
    }
  }

  public virtual void OnPositionalNetworkUpdate()
  {
  }

  public void DoMovingWithoutARigidBodyCheck()
  {
    if (this.doneMovingWithoutARigidBodyCheck > 10)
      return;
    ++this.doneMovingWithoutARigidBodyCheck;
    if (this.doneMovingWithoutARigidBodyCheck < 10 || Object.op_Equality((Object) ((Component) this).GetComponent<Collider>(), (Object) null) || !Object.op_Equality((Object) ((Component) this).GetComponent<Rigidbody>(), (Object) null))
      return;
    Debug.LogWarning((object) ("Entity moving without a rigid body! (" + (object) ((Component) this).get_gameObject() + ")"), (Object) this);
  }

  public override void Spawn()
  {
    base.Spawn();
    ((Component) this).get_gameObject().BroadcastOnParentSpawning();
  }

  public void OnParentSpawning()
  {
    if (this.net != null || this.IsDestroyed)
      return;
    if (Application.isLoadingSave != null)
    {
      Object.Destroy((Object) ((Component) this).get_gameObject());
    }
    else
    {
      BaseEntity entity = Object.op_Inequality((Object) ((Component) this).get_transform().get_parent(), (Object) null) ? (BaseEntity) ((Component) ((Component) this).get_transform().get_parent()).GetComponentInParent<BaseEntity>() : (BaseEntity) null;
      this.Spawn();
      if (!Object.op_Inequality((Object) entity, (Object) null))
        return;
      this.SetParent(entity, true, false);
    }
  }

  public void SpawnAsMapEntity()
  {
    if (this.net != null || this.IsDestroyed || !Object.op_Equality(Object.op_Inequality((Object) ((Component) this).get_transform().get_parent(), (Object) null) ? (Object) ((Component) ((Component) this).get_transform().get_parent()).GetComponentInParent<BaseEntity>() : (Object) null, (Object) null))
      return;
    ((Component) this).get_transform().set_parent((Transform) null);
    SceneManager.MoveGameObjectToScene(((Component) this).get_gameObject(), Rust.Server.EntityScene);
    ((Component) this).get_gameObject().SetActive(true);
    this.Spawn();
  }

  public virtual void PostMapEntitySpawn()
  {
  }

  internal override void DoServerDestroy()
  {
    this.CancelInvoke(new Action(this.NetworkPositionTick));
    BaseEntity.saveList.Remove(this);
    this.RemoveFromTriggers();
    if (this.children != null)
    {
      foreach (BaseEntity baseEntity in this.children.ToArray())
        baseEntity.OnParentRemoved();
    }
    this.SetParent((BaseEntity) null, false, false);
    BaseEntity.Query.Server.Remove(this, false);
    base.DoServerDestroy();
  }

  internal virtual void OnParentRemoved()
  {
    this.Kill(BaseNetworkable.DestroyMode.None);
  }

  public virtual void OnInvalidPosition()
  {
    Debug.Log((object) ("Invalid Position: " + (object) this + " " + (object) ((Component) this).get_transform().get_position() + " (destroying)"));
    this.Kill(BaseNetworkable.DestroyMode.None);
  }

  public BaseCorpse DropCorpse(string strCorpsePrefab)
  {
    Assert.IsTrue(this.isServer, "DropCorpse called on client!");
    if (!ConVar.Server.corpses)
      return (BaseCorpse) null;
    if (string.IsNullOrEmpty(strCorpsePrefab))
      return (BaseCorpse) null;
    BaseCorpse entity = GameManager.server.CreateEntity(strCorpsePrefab, (Vector3) null, (Quaternion) null, true) as BaseCorpse;
    if (Object.op_Equality((Object) entity, (Object) null))
    {
      Debug.LogWarning((object) ("Error creating corpse: " + (object) ((Component) this).get_gameObject() + " - " + strCorpsePrefab));
      return (BaseCorpse) null;
    }
    entity.InitCorpse(this);
    return entity;
  }

  public override void UpdateNetworkGroup()
  {
    this.isCallingUpdateNetworkGroup = false;
    if (this.net == null || Net.sv == null || ((Network.Server) Net.sv).visibility == null)
      return;
    using (TimeWarning.New(nameof (UpdateNetworkGroup), 0.1f))
    {
      if (this.globalBroadcast)
      {
        if (!this.net.SwitchGroup(BaseNetworkable.GlobalNetworkGroup))
          return;
        this.SendNetworkGroupChange();
      }
      else if (this.ShouldInheritNetworkGroup() && this.parentEntity.IsSet())
      {
        BaseEntity parentEntity = this.GetParentEntity();
        if (!parentEntity.IsValid())
        {
          Debug.LogWarning((object) ("UpdateNetworkGroup: Missing parent entity " + (object) this.parentEntity.uid));
          this.Invoke(new Action(((BaseNetworkable) this).UpdateNetworkGroup), 2f);
          this.isCallingUpdateNetworkGroup = true;
        }
        else if (Object.op_Inequality((Object) parentEntity, (Object) null))
        {
          if (!this.net.SwitchGroup((Group) parentEntity.net.group))
            return;
          this.SendNetworkGroupChange();
        }
        else
          Debug.LogWarning((object) (((Component) this).get_gameObject().ToString() + ": has parent id - but couldn't find parent! " + (object) this.parentEntity));
      }
      else
        base.UpdateNetworkGroup();
    }
  }

  public virtual void Eat(BaseNpc baseNpc, float timeSpent)
  {
    baseNpc.AddCalories(100f);
  }

  public virtual void OnDeployed(BaseEntity parent)
  {
  }

  public override bool ShouldNetworkTo(BasePlayer player)
  {
    if (Object.op_Equality((Object) player, (Object) this))
      return true;
    BaseEntity parentEntity = this.GetParentEntity();
    if (this.limitNetworking && (Object.op_Equality((Object) parentEntity, (Object) null) || Object.op_Inequality((Object) parentEntity, (Object) player)))
      return false;
    if (Object.op_Inequality((Object) parentEntity, (Object) null))
      return parentEntity.ShouldNetworkTo(player);
    return base.ShouldNetworkTo(player);
  }

  public virtual void AttackerInfo(PlayerLifeStory.DeathInfo info)
  {
    info.attackerName = (__Null) this.ShortPrefabName;
    info.attackerSteamID = (__Null) 0L;
    info.inflictorName = (__Null) "";
  }

  public virtual void Push(Vector3 velocity)
  {
    this.SetVelocity(velocity);
  }

  public virtual void SetVelocity(Vector3 velocity)
  {
    Rigidbody component = (Rigidbody) ((Component) this).GetComponent<Rigidbody>();
    if (!Object.op_Implicit((Object) component))
      return;
    component.set_velocity(velocity);
  }

  public virtual void SetAngularVelocity(Vector3 velocity)
  {
    Rigidbody component = (Rigidbody) ((Component) this).GetComponent<Rigidbody>();
    if (!Object.op_Implicit((Object) component))
      return;
    component.set_angularVelocity(velocity);
  }

  public virtual Vector3 GetDropPosition()
  {
    return ((Component) this).get_transform().get_position();
  }

  public virtual Vector3 GetDropVelocity()
  {
    return Vector3.op_Addition(this.GetInheritedDropVelocity(), Vector3.get_up());
  }

  public virtual bool OnStartBeingLooted(BasePlayer baseEntity)
  {
    return true;
  }

  public virtual Vector3 ServerPosition
  {
    get
    {
      return ((Component) this).get_transform().get_localPosition();
    }
    set
    {
      if (Vector3.op_Equality(((Component) this).get_transform().get_localPosition(), value))
        return;
      ((Component) this).get_transform().set_localPosition(value);
      ((Component) this).get_transform().set_hasChanged(true);
    }
  }

  public virtual Quaternion ServerRotation
  {
    get
    {
      return ((Component) this).get_transform().get_localRotation();
    }
    set
    {
      if (Quaternion.op_Equality(((Component) this).get_transform().get_localRotation(), value))
        return;
      ((Component) this).get_transform().set_localRotation(value);
      ((Component) this).get_transform().set_hasChanged(true);
    }
  }

  public float radiationLevel
  {
    get
    {
      if (this.triggers == null)
        return 0.0f;
      float num = 0.0f;
      for (int index = 0; index < this.triggers.Count; ++index)
      {
        TriggerRadiation trigger = this.triggers[index] as TriggerRadiation;
        if (!Object.op_Equality((Object) trigger, (Object) null))
        {
          Vector3 position = this.GetNetworkPosition();
          BaseEntity parentEntity = this.GetParentEntity();
          if (Object.op_Inequality((Object) parentEntity, (Object) null))
            position = ((Component) parentEntity).get_transform().TransformPoint(position);
          num = Mathf.Max(num, trigger.GetRadiation(position, this.RadiationProtection()));
        }
      }
      return num;
    }
  }

  public virtual float RadiationProtection()
  {
    return 0.0f;
  }

  public virtual float RadiationExposureFraction()
  {
    return 1f;
  }

  public float currentTemperature
  {
    get
    {
      float oldTemperature = Climate.GetTemperature(((Component) this).get_transform().get_position());
      if (this.triggers == null)
        return oldTemperature;
      for (int index = 0; index < this.triggers.Count; ++index)
      {
        TriggerTemperature trigger = this.triggers[index] as TriggerTemperature;
        if (!Object.op_Equality((Object) trigger, (Object) null))
          oldTemperature = trigger.WorkoutTemperature(this.GetNetworkPosition(), oldTemperature);
      }
      return oldTemperature;
    }
  }

  [BaseEntity.RPC_Server.FromOwner]
  [BaseEntity.RPC_Server]
  private void BroadcastSignalFromClient(BaseEntity.RPCMessage msg)
  {
    this.SignalBroadcast((BaseEntity.Signal) msg.read.Int32(), msg.read.String(), msg.connection);
  }

  public void SignalBroadcast(BaseEntity.Signal signal, string arg, Connection sourceConnection = null)
  {
    if (this.net == null || this.net.group == null)
      return;
    SendInfo sendInfo;
    ((SendInfo) ref sendInfo).\u002Ector((IEnumerable<Connection>) ((Group) this.net.group).subscribers);
    sendInfo.method = (__Null) 3;
    sendInfo.priority = (__Null) 0;
    this.ClientRPCEx<int, string>(sendInfo, sourceConnection, "SignalFromServerEx", (int) signal, arg);
  }

  public void SignalBroadcast(BaseEntity.Signal signal, Connection sourceConnection = null)
  {
    if (this.net == null || this.net.group == null)
      return;
    SendInfo sendInfo;
    ((SendInfo) ref sendInfo).\u002Ector((IEnumerable<Connection>) ((Group) this.net.group).subscribers);
    sendInfo.method = (__Null) 3;
    sendInfo.priority = (__Null) 0;
    this.ClientRPCEx<int>(sendInfo, sourceConnection, "SignalFromServer", (int) signal);
  }

  private void OnSkinChanged(ulong oldSkinID, ulong newSkinID)
  {
    if ((long) oldSkinID == (long) newSkinID)
      return;
    this.skinID = newSkinID;
  }

  public virtual void PreProcess(
    IPrefabProcessor preProcess,
    GameObject rootObj,
    string name,
    bool serverside,
    bool clientside,
    bool bundling)
  {
    if (!clientside || Skinnable.All == null || !Object.op_Inequality((Object) Skinnable.FindForEntity(name), (Object) null))
      return;
    WorkshopSkin.Prepare(rootObj);
    MaterialReplacement.Prepare(rootObj);
  }

  public bool HasAnySlot()
  {
    for (int index = 0; index < this.entitySlots.Length; ++index)
    {
      if (this.entitySlots[index].IsValid(this.isServer))
        return true;
    }
    return false;
  }

  public BaseEntity GetSlot(BaseEntity.Slot slot)
  {
    return this.entitySlots[(int) slot].Get(this.isServer);
  }

  public string GetSlotAnchorName(BaseEntity.Slot slot)
  {
    return slot.ToString().ToLower();
  }

  public void SetSlot(BaseEntity.Slot slot, BaseEntity ent)
  {
    this.entitySlots[(int) slot].Set(ent);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public virtual bool HasSlot(BaseEntity.Slot slot)
  {
    return false;
  }

  public virtual BaseEntity.TraitFlag Traits
  {
    get
    {
      return BaseEntity.TraitFlag.None;
    }
  }

  public bool HasTrait(BaseEntity.TraitFlag f)
  {
    return (this.Traits & f) == f;
  }

  public bool HasAnyTrait(BaseEntity.TraitFlag f)
  {
    return (uint) (this.Traits & f) > 0U;
  }

  public virtual bool EnterTrigger(TriggerBase trigger)
  {
    if (this.triggers == null)
      this.triggers = (List<TriggerBase>) Pool.Get<List<TriggerBase>>();
    this.triggers.Add(trigger);
    return true;
  }

  public virtual void LeaveTrigger(TriggerBase trigger)
  {
    if (this.triggers == null)
      return;
    this.triggers.Remove(trigger);
    if (this.triggers.Count != 0)
      return;
    // ISSUE: cast to a reference type
    Pool.FreeList<TriggerBase>((List<M0>&) ref this.triggers);
  }

  public void RemoveFromTriggers()
  {
    if (this.triggers == null)
      return;
    using (TimeWarning.New(nameof (RemoveFromTriggers), 0.1f))
    {
      foreach (TriggerBase triggerBase in this.triggers.ToArray())
      {
        if (Object.op_Implicit((Object) triggerBase))
          triggerBase.RemoveEntity(this);
      }
      if (this.triggers == null || this.triggers.Count != 0)
        return;
      // ISSUE: cast to a reference type
      Pool.FreeList<TriggerBase>((List<M0>&) ref this.triggers);
    }
  }

  public T FindTrigger<T>() where T : TriggerBase
  {
    if (this.triggers == null)
      return default (T);
    foreach (TriggerBase trigger in this.triggers)
    {
      if (!Object.op_Equality((Object) (object) (trigger as T), (Object) null))
        return trigger as T;
    }
    return default (T);
  }

  public class Menu : Attribute
  {
    public string TitleToken;
    public string TitleEnglish;
    public string UseVariable;
    public int Order;
    public string ProxyFunction;
    public float Time;
    public string OnStart;
    public string OnProgress;
    public bool LongUseOnly;

    public Menu()
    {
    }

    public Menu(string menuTitleToken, string menuTitleEnglish)
    {
      this.TitleToken = menuTitleToken;
      this.TitleEnglish = menuTitleEnglish;
    }

    [Serializable]
    public struct Option
    {
      public Translate.Phrase name;
      public Translate.Phrase description;
      public Sprite icon;
      public int order;
    }

    public class Description : Attribute
    {
      public string token;
      public string english;

      public Description(string t, string e)
      {
        this.token = t;
        this.english = e;
      }
    }

    public class Icon : Attribute
    {
      public string icon;

      public Icon(string i)
      {
        this.icon = i;
      }
    }

    public class ShowIf : Attribute
    {
      public string functionName;

      public ShowIf(string testFunc)
      {
        this.functionName = testFunc;
      }
    }
  }

  [Serializable]
  public struct MovementModify
  {
    public float drag;
  }

  public enum GiveItemReason
  {
    Generic,
    ResourceHarvested,
    PickedUp,
    Crafted,
  }

  [System.Flags]
  public enum Flags
  {
    Placeholder = 1,
    On = 2,
    OnFire = 4,
    Open = 8,
    Locked = 16, // 0x00000010
    Debugging = 32, // 0x00000020
    Disabled = 64, // 0x00000040
    Reserved1 = 128, // 0x00000080
    Reserved2 = 256, // 0x00000100
    Reserved3 = 512, // 0x00000200
    Reserved4 = 1024, // 0x00000400
    Reserved5 = 2048, // 0x00000800
    Broken = 4096, // 0x00001000
    Busy = 8192, // 0x00002000
    Reserved6 = 16384, // 0x00004000
    Reserved7 = 32768, // 0x00008000
    Reserved8 = 65536, // 0x00010000
  }

  public static class Query
  {
    public static BaseEntity.Query.EntityTree Server;

    public class EntityTree
    {
      private Grid<BaseEntity> Grid;
      private Grid<BasePlayer> PlayerGrid;

      public EntityTree(float worldSize)
      {
        this.Grid = new Grid<BaseEntity>(32, worldSize);
        this.PlayerGrid = new Grid<BasePlayer>(32, worldSize);
      }

      public void Add(BaseEntity ent)
      {
        Vector3 position = ((Component) ent).get_transform().get_position();
        this.Grid.Add(ent, (float) position.x, (float) position.z);
      }

      public void AddPlayer(BasePlayer player)
      {
        Vector3 position = ((Component) player).get_transform().get_position();
        this.PlayerGrid.Add(player, (float) position.x, (float) position.z);
      }

      public void Remove(BaseEntity ent, bool isPlayer = false)
      {
        this.Grid.Remove(ent);
        if (!isPlayer)
          return;
        BasePlayer basePlayer = ent as BasePlayer;
        if (!Object.op_Inequality((Object) basePlayer, (Object) null))
          return;
        this.PlayerGrid.Remove(basePlayer);
      }

      public void RemovePlayer(BasePlayer player)
      {
        this.PlayerGrid.Remove(player);
      }

      public void Move(BaseEntity ent)
      {
        Vector3 position = ((Component) ent).get_transform().get_position();
        this.Grid.Move(ent, (float) position.x, (float) position.z);
        BasePlayer player = ent as BasePlayer;
        if (!Object.op_Inequality((Object) player, (Object) null))
          return;
        this.MovePlayer(player);
      }

      public void MovePlayer(BasePlayer player)
      {
        Vector3 position = ((Component) player).get_transform().get_position();
        this.PlayerGrid.Move(player, (float) position.x, (float) position.z);
      }

      public int GetInSphere(
        Vector3 position,
        float distance,
        BaseEntity[] results,
        Func<BaseEntity, bool> filter = null)
      {
        return this.Grid.Query((float) position.x, (float) position.z, distance, results, filter);
      }

      public int GetPlayersInSphere(
        Vector3 position,
        float distance,
        BasePlayer[] results,
        Func<BasePlayer, bool> filter = null)
      {
        return this.PlayerGrid.Query((float) position.x, (float) position.z, distance, results, filter);
      }
    }
  }

  public class RPC_Shared : Attribute
  {
  }

  public struct RPCMessage
  {
    public Connection connection;
    public BasePlayer player;
    public Read read;
  }

  public class RPC_Server : BaseEntity.RPC_Shared
  {
    public abstract class Conditional : Attribute
    {
      public virtual string GetArgs()
      {
        return (string) null;
      }
    }

    public class MaxDistance : BaseEntity.RPC_Server.Conditional
    {
      private float maximumDistance;

      public MaxDistance(float maxDist)
      {
        this.maximumDistance = maxDist;
      }

      public override string GetArgs()
      {
        return this.maximumDistance.ToString("0.00f");
      }

      public static bool Test(
        string debugName,
        BaseEntity ent,
        BasePlayer player,
        float maximumDistance)
      {
        if (Object.op_Equality((Object) ent, (Object) null) || Object.op_Equality((Object) player, (Object) null))
          return false;
        return (double) ent.Distance(player.eyes.position) <= (double) maximumDistance;
      }
    }

    public class IsVisible : BaseEntity.RPC_Server.Conditional
    {
      private float maximumDistance;

      public IsVisible(float maxDist)
      {
        this.maximumDistance = maxDist;
      }

      public override string GetArgs()
      {
        return this.maximumDistance.ToString("0.00f");
      }

      public static bool Test(
        string debugName,
        BaseEntity ent,
        BasePlayer player,
        float maximumDistance)
      {
        if (Object.op_Equality((Object) ent, (Object) null) || Object.op_Equality((Object) player, (Object) null) || !GamePhysics.LineOfSight(player.eyes.center, player.eyes.position, 2162688, 0.0f))
          return false;
        if (!ent.IsVisible(player.eyes.HeadRay(), maximumDistance))
          return ent.IsVisible(player.eyes.position, maximumDistance);
        return true;
      }
    }

    public class FromOwner : BaseEntity.RPC_Server.Conditional
    {
      public static bool Test(string debugName, BaseEntity ent, BasePlayer player)
      {
        return !Object.op_Equality((Object) ent, (Object) null) && !Object.op_Equality((Object) player, (Object) null) && (ent.net != null && player.net != null) && (ent.net.ID == player.net.ID || (int) ent.parentEntity.uid == player.net.ID);
      }
    }

    public class IsActiveItem : BaseEntity.RPC_Server.Conditional
    {
      public static bool Test(string debugName, BaseEntity ent, BasePlayer player)
      {
        if (Object.op_Equality((Object) ent, (Object) null) || Object.op_Equality((Object) player, (Object) null) || (ent.net == null || player.net == null))
          return false;
        if (ent.net.ID == player.net.ID)
          return true;
        if ((int) ent.parentEntity.uid != player.net.ID)
          return false;
        Item activeItem = player.GetActiveItem();
        return activeItem != null && !Object.op_Inequality((Object) activeItem.GetHeldEntity(), (Object) ent);
      }
    }
  }

  public enum Signal
  {
    Attack,
    Alt_Attack,
    DryFire,
    Reload,
    Deploy,
    Flinch_Head,
    Flinch_Chest,
    Flinch_Stomach,
    Flinch_RearHead,
    Flinch_RearTorso,
    Throw,
    Relax,
    Gesture,
    PhysImpact,
    Eat,
    Startled,
  }

  public enum Slot
  {
    Lock,
    FireMod,
    UpperModifier,
    MiddleModifier,
    LowerModifier,
    CenterDecoration,
    LowerCenterDecoration,
    Count,
  }

  [System.Flags]
  public enum TraitFlag
  {
    None = 0,
    Alive = 1,
    Animal = 2,
    Human = 4,
    Interesting = 8,
    Food = 16, // 0x00000010
    Meat = 32, // 0x00000020
    Water = Meat, // 0x00000020
  }

  public static class Util
  {
    public static BaseEntity[] FindTargets(string strFilter, bool onlyPlayers)
    {
      return BaseNetworkable.serverEntities.Where<BaseNetworkable>((Func<BaseNetworkable, bool>) (x =>
      {
        if (x is BasePlayer)
        {
          BasePlayer basePlayer = x as BasePlayer;
          return string.IsNullOrEmpty(strFilter) || strFilter == "!alive" && basePlayer.IsAlive() || (strFilter == "!sleeping" && basePlayer.IsSleeping() || (strFilter[0] == '!' || StringEx.Contains(basePlayer.displayName, strFilter, CompareOptions.IgnoreCase))) || basePlayer.UserIDString.Contains(strFilter);
        }
        return !onlyPlayers && !string.IsNullOrEmpty(strFilter) && x.ShortPrefabName.Contains(strFilter);
      })).Select<BaseNetworkable, BaseEntity>((Func<BaseNetworkable, BaseEntity>) (x => x as BaseEntity)).ToArray<BaseEntity>();
    }
  }
}
