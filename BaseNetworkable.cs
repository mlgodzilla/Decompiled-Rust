// Decompiled with JetBrains decompiler
// Type: BaseNetworkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Network;
using Network.Visibility;
using Oxide.Core;
using ProtoBuf;
using Rust;
using Rust.Registry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class BaseNetworkable : BaseMonoBehaviour, IEntity, NetworkHandler
{
  public static BaseNetworkable.EntityRealm serverEntities = (BaseNetworkable.EntityRealm) new BaseNetworkable.EntityRealmServer();
  public static Queue<MemoryStream> EntityMemoryStreamPool = new Queue<MemoryStream>();
  [NonSerialized]
  public List<BaseEntity> children = new List<BaseEntity>();
  [ReadOnly]
  [Header("BaseNetworkable")]
  public uint prefabID;
  [Tooltip("If enabled the entity will send to everyone on the server - regardless of position")]
  public bool globalBroadcast;
  [NonSerialized]
  public Networkable net;
  private string _prefabName;
  private string _prefabNameWithoutExtension;
  private const bool isServersideEntity = true;
  public bool _limitedNetworking;
  [NonSerialized]
  public EntityRef parentEntity;
  public int creationFrame;
  public bool isSpawned;
  private MemoryStream _NetworkCache;
  private MemoryStream _SaveCache;

  public bool IsDestroyed { get; private set; }

  public string PrefabName
  {
    get
    {
      if (this._prefabName == null)
        this._prefabName = StringPool.Get(this.prefabID);
      return this._prefabName;
    }
  }

  public string ShortPrefabName
  {
    get
    {
      if (this._prefabNameWithoutExtension == null)
        this._prefabNameWithoutExtension = Path.GetFileNameWithoutExtension(this.PrefabName);
      return this._prefabNameWithoutExtension;
    }
  }

  public virtual Vector3 GetNetworkPosition()
  {
    return ((Component) this).get_transform().get_localPosition();
  }

  public virtual Quaternion GetNetworkRotation()
  {
    return ((Component) this).get_transform().get_localRotation();
  }

  public virtual bool PhysicsDriven()
  {
    return false;
  }

  public float GetNetworkTime()
  {
    if (this.PhysicsDriven())
      return Time.get_fixedTime();
    return Time.get_time();
  }

  public string InvokeString()
  {
    StringBuilder stringBuilder = new StringBuilder();
    List<InvokeAction> list = (List<InvokeAction>) Pool.GetList<InvokeAction>();
    InvokeHandler.FindInvokes((Behaviour) this, list);
    using (List<InvokeAction>.Enumerator enumerator = list.GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        InvokeAction current = enumerator.Current;
        if (stringBuilder.Length > 0)
          stringBuilder.Append(", ");
        stringBuilder.Append(((Delegate) current.action).Method.Name);
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<InvokeAction>((List<M0>&) ref list);
    return stringBuilder.ToString();
  }

  public BaseEntity LookupPrefab()
  {
    return this.gameManager.FindPrefab(this.PrefabName).ToBaseEntity();
  }

  public bool EqualNetID(BaseNetworkable other)
  {
    if (Object.op_Inequality((Object) other, (Object) null) && other.net != null && this.net != null)
      return other.net.ID == this.net.ID;
    return false;
  }

  public virtual void ResetState()
  {
    if (this.children.Count <= 0)
      return;
    this.children.Clear();
  }

  public virtual void InitShared()
  {
  }

  public virtual void PreInitShared()
  {
  }

  public virtual void PostInitShared()
  {
  }

  public virtual void DestroyShared()
  {
  }

  public virtual void OnNetworkGroupEnter(Group group)
  {
  }

  public virtual void OnNetworkGroupLeave(Group group)
  {
  }

  public void OnNetworkGroupChange()
  {
    if (this.children == null)
      return;
    foreach (BaseEntity child in this.children)
    {
      if (child.ShouldInheritNetworkGroup())
        child.net.SwitchGroup((Group) this.net.group);
    }
  }

  public void OnNetworkSubscribersEnter(List<Connection> connections)
  {
    if (!((Network.Server) Net.sv).IsConnected())
      return;
    using (List<Connection>.Enumerator enumerator = connections.GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        BasePlayer player = enumerator.Current.player as BasePlayer;
        if (!Object.op_Equality((Object) player, (Object) null))
          player.QueueUpdate(BasePlayer.NetworkQueue.Update, (BaseNetworkable) (this as BaseEntity));
      }
    }
  }

  public void OnNetworkSubscribersLeave(List<Connection> connections)
  {
    if (!((Network.Server) Net.sv).IsConnected())
      return;
    this.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "LeaveVisibility");
    if (!((Write) ((NetworkPeer) Net.sv).write).Start())
      return;
    ((Write) ((NetworkPeer) Net.sv).write).PacketID((Message.Type) 6);
    ((Write) ((NetworkPeer) Net.sv).write).EntityID((uint) this.net.ID);
    ((Write) ((NetworkPeer) Net.sv).write).UInt8((byte) 0);
    ((Write) ((NetworkPeer) Net.sv).write).Send(new SendInfo((IEnumerable<Connection>) connections));
  }

  private void EntityDestroy()
  {
    if (!Object.op_Implicit((Object) ((Component) this).get_gameObject()))
      return;
    this.ResetState();
    this.gameManager.Retire(((Component) this).get_gameObject());
  }

  private void DoEntityDestroy()
  {
    if (this.IsDestroyed)
      return;
    this.IsDestroyed = true;
    if (Application.isQuitting != null)
      return;
    this.DestroyShared();
    if (this.isServer)
      this.DoServerDestroy();
    using (TimeWarning.New("Registry.Entity.Unregister", 0.1f))
      Entity.Unregister(((Component) this).get_gameObject());
  }

  private void SpawnShared()
  {
    this.IsDestroyed = false;
    using (TimeWarning.New("Registry.Entity.Register", 0.1f))
      Entity.Register(((Component) this).get_gameObject(), (IEntity) this);
  }

  public virtual void Save(BaseNetworkable.SaveInfo info)
  {
    if (this.prefabID == 0U)
      Debug.LogError((object) ("PrefabID is 0! " + ((Component) this).get_transform().GetRecursiveName("")), (Object) ((Component) this).get_gameObject());
    info.msg.baseNetworkable = (__Null) Pool.Get<BaseNetworkable>();
    ((BaseNetworkable) info.msg.baseNetworkable).uid = this.net.ID;
    ((BaseNetworkable) info.msg.baseNetworkable).prefabID = (__Null) (int) this.prefabID;
    if (this.net.group != null)
      ((BaseNetworkable) info.msg.baseNetworkable).group = ((Group) this.net.group).ID;
    if (info.forDisk)
      return;
    info.msg.createdThisFrame = (__Null) (this.creationFrame == Time.get_frameCount() ? 1 : 0);
  }

  public virtual void PostSave(BaseNetworkable.SaveInfo info)
  {
  }

  public void InitLoad(uint entityID)
  {
    this.net = ((Network.Server) Net.sv).CreateNetworkable(entityID);
    BaseNetworkable.serverEntities.RegisterID(this);
    this.PreServerLoad();
  }

  public virtual void PreServerLoad()
  {
  }

  public virtual void Load(BaseNetworkable.LoadInfo info)
  {
    if (info.msg.baseNetworkable == null)
      return;
    BaseNetworkable baseNetworkable = (BaseNetworkable) info.msg.baseNetworkable;
    if ((int) this.prefabID == baseNetworkable.prefabID)
      return;
    Debug.LogError((object) ("Prefab IDs don't match! " + (object) this.prefabID + "/" + (object) (uint) baseNetworkable.prefabID + " -> " + (object) ((Component) this).get_gameObject()), (Object) ((Component) this).get_gameObject());
  }

  public virtual void PostServerLoad()
  {
    ((Component) this).get_gameObject().SendOnSendNetworkUpdate(this as BaseEntity);
  }

  public bool isServer
  {
    get
    {
      return true;
    }
  }

  public bool isClient
  {
    get
    {
      return false;
    }
  }

  public T ToServer<T>() where T : BaseNetworkable
  {
    if (this.isServer)
      return this as T;
    return default (T);
  }

  public virtual bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    return false;
  }

  public static IEnumerable<Connection> GetConnectionsWithin(
    Vector3 position,
    float distance)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerable<Connection>) new BaseNetworkable.\u003CGetConnectionsWithin\u003Ed__52(-2)
    {
      \u003C\u003E3__position = position,
      \u003C\u003E3__distance = distance
    };
  }

  public bool limitNetworking
  {
    get
    {
      return this._limitedNetworking;
    }
    set
    {
      if (value == this._limitedNetworking)
        return;
      this._limitedNetworking = value;
      if (this._limitedNetworking)
        this.OnNetworkLimitStart();
      else
        this.OnNetworkLimitEnd();
    }
  }

  private void OnNetworkLimitStart()
  {
    this.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, nameof (OnNetworkLimitStart));
    List<Connection> subscribers = this.GetSubscribers();
    if (subscribers == null)
      return;
    List<Connection> list = ((IEnumerable<Connection>) subscribers).ToList<Connection>();
    list.RemoveAll((Predicate<Connection>) (x => this.ShouldNetworkTo(x.player as BasePlayer)));
    this.OnNetworkSubscribersLeave(list);
    if (this.children == null)
      return;
    foreach (BaseNetworkable child in this.children)
      child.OnNetworkLimitStart();
  }

  private void OnNetworkLimitEnd()
  {
    this.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, nameof (OnNetworkLimitEnd));
    List<Connection> subscribers = this.GetSubscribers();
    if (subscribers == null)
      return;
    this.OnNetworkSubscribersEnter(subscribers);
    if (this.children == null)
      return;
    foreach (BaseNetworkable child in this.children)
      child.OnNetworkLimitEnd();
  }

  public BaseEntity GetParentEntity()
  {
    return this.parentEntity.Get(this.isServer);
  }

  public bool HasParent()
  {
    return this.parentEntity.IsValid(this.isServer);
  }

  public void AddChild(BaseEntity child)
  {
    if (this.children.Contains(child))
      return;
    this.children.Add(child);
  }

  public void RemoveChild(BaseEntity child)
  {
    this.children.Remove(child);
  }

  public GameManager gameManager
  {
    get
    {
      if (this.isServer)
        return GameManager.server;
      throw new NotImplementedException("Missing gameManager path");
    }
  }

  public PrefabAttribute.Library prefabAttribute
  {
    get
    {
      if (this.isServer)
        return PrefabAttribute.server;
      throw new NotImplementedException("Missing prefabAttribute path");
    }
  }

  public static Group GlobalNetworkGroup
  {
    get
    {
      return ((Manager) ((Network.Server) Net.sv).visibility).Get(0U);
    }
  }

  public static Group LimboNetworkGroup
  {
    get
    {
      return ((Manager) ((Network.Server) Net.sv).visibility).Get(1U);
    }
  }

  public virtual void Spawn()
  {
    this.SpawnShared();
    if (this.net == null)
      this.net = ((Network.Server) Net.sv).CreateNetworkable();
    this.creationFrame = Time.get_frameCount();
    this.PreInitShared();
    this.InitShared();
    this.ServerInit();
    this.PostInitShared();
    this.UpdateNetworkGroup();
    this.isSpawned = true;
    Interface.CallHook("OnEntitySpawned", (object) this);
    this.SendNetworkUpdateImmediate(true);
    if (Application.isLoading == null || Application.isLoadingSave != null)
      return;
    ((Component) this).get_gameObject().SendOnSendNetworkUpdate(this as BaseEntity);
  }

  public bool IsFullySpawned()
  {
    return this.isSpawned;
  }

  public virtual void ServerInit()
  {
    BaseNetworkable.serverEntities.RegisterID(this);
    if (this.net == null)
      return;
    this.net.handler = (__Null) this;
  }

  protected List<Connection> GetSubscribers()
  {
    if (this.net == null)
      return (List<Connection>) null;
    if (this.net.group == null)
      return (List<Connection>) null;
    return (List<Connection>) ((Group) this.net.group).subscribers;
  }

  public void KillMessage()
  {
    this.Kill(BaseNetworkable.DestroyMode.None);
  }

  public void Kill(BaseNetworkable.DestroyMode mode = BaseNetworkable.DestroyMode.None)
  {
    if (this.IsDestroyed)
    {
      Debug.LogWarning((object) ("Calling kill - but already IsDestroyed!? " + (object) this));
    }
    else
    {
      Interface.CallHook("OnEntityKill", (object) this);
      ((Component) this).get_gameObject().BroadcastOnParentDestroying();
      this.DoEntityDestroy();
      this.TerminateOnClient(mode);
      this.TerminateOnServer();
      this.EntityDestroy();
    }
  }

  private void TerminateOnClient(BaseNetworkable.DestroyMode mode)
  {
    if (this.net == null || this.net.group == null || !((Network.Server) Net.sv).IsConnected())
      return;
    this.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "Term {0}", (object) mode);
    if (!((Write) ((NetworkPeer) Net.sv).write).Start())
      return;
    ((Write) ((NetworkPeer) Net.sv).write).PacketID((Message.Type) 6);
    ((Write) ((NetworkPeer) Net.sv).write).EntityID((uint) this.net.ID);
    ((Write) ((NetworkPeer) Net.sv).write).UInt8((byte) mode);
    ((Write) ((NetworkPeer) Net.sv).write).Send(new SendInfo((IEnumerable<Connection>) ((Group) this.net.group).subscribers));
  }

  private void TerminateOnServer()
  {
    if (this.net == null)
      return;
    this.InvalidateNetworkCache();
    BaseNetworkable.serverEntities.UnregisterID(this);
    ((Network.Server) Net.sv).DestroyNetworkable(ref this.net);
    ((MonoBehaviour) this).StopAllCoroutines();
    ((Component) this).get_gameObject().SetActive(false);
  }

  internal virtual void DoServerDestroy()
  {
    this.isSpawned = false;
  }

  public virtual bool ShouldNetworkTo(BasePlayer player)
  {
    object obj = Interface.CallHook("CanNetworkTo", (object) this, (object) player);
    if (obj is bool)
      return (bool) obj;
    return true;
  }

  protected void SendNetworkGroupChange()
  {
    if (!this.isSpawned || !((Network.Server) Net.sv).IsConnected())
      return;
    if (this.net.group == null)
    {
      Debug.LogWarning((object) (((object) this).ToString() + " changed its network group to null"));
    }
    else
    {
      if (!((Write) ((NetworkPeer) Net.sv).write).Start())
        return;
      ((Write) ((NetworkPeer) Net.sv).write).PacketID((Message.Type) 7);
      ((Write) ((NetworkPeer) Net.sv).write).EntityID((uint) this.net.ID);
      ((Write) ((NetworkPeer) Net.sv).write).GroupID((uint) ((Group) this.net.group).ID);
      ((Write) ((NetworkPeer) Net.sv).write).Send(new SendInfo((IEnumerable<Connection>) ((Group) this.net.group).subscribers));
    }
  }

  protected void SendAsSnapshot(Connection connection, bool justCreated = false)
  {
    if (!((Write) ((NetworkPeer) Net.sv).write).Start())
      return;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ref __Null local = ref (^(Connection.Validation&) ref connection.validate).entityUpdates;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ref local = (int) ^(uint&) ref local + 1;
    BaseNetworkable.SaveInfo saveInfo = new BaseNetworkable.SaveInfo()
    {
      forConnection = connection,
      forDisk = false
    };
    ((Write) ((NetworkPeer) Net.sv).write).PacketID((Message.Type) 5);
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ((Write) ((NetworkPeer) Net.sv).write).UInt32((uint) (^(Connection.Validation&) ref connection.validate).entityUpdates);
    this.ToStreamForNetwork((Stream) ((NetworkPeer) Net.sv).write, saveInfo);
    ((Write) ((NetworkPeer) Net.sv).write).Send(new SendInfo(connection));
  }

  public void SendNetworkUpdate(BasePlayer.NetworkQueue queue = BasePlayer.NetworkQueue.Update)
  {
    if (Application.isLoading != null || Application.isLoadingSave != null || (this.IsDestroyed || this.net == null) || !this.isSpawned)
      return;
    using (TimeWarning.New(nameof (SendNetworkUpdate), 0.1f))
    {
      this.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, nameof (SendNetworkUpdate));
      this.InvalidateNetworkCache();
      List<Connection> subscribers = this.GetSubscribers();
      if (subscribers != null)
      {
        if (subscribers.Count > 0)
        {
          for (int index = 0; index < subscribers.Count; ++index)
          {
            BasePlayer player = subscribers[index].player as BasePlayer;
            if (!Object.op_Equality((Object) player, (Object) null) && this.ShouldNetworkTo(player))
              player.QueueUpdate(queue, this);
          }
        }
      }
    }
    ((Component) this).get_gameObject().SendOnSendNetworkUpdate(this as BaseEntity);
  }

  public void SendNetworkUpdateImmediate(bool justCreated = false)
  {
    if (Application.isLoading != null || Application.isLoadingSave != null || (this.IsDestroyed || this.net == null) || !this.isSpawned)
      return;
    using (TimeWarning.New(nameof (SendNetworkUpdateImmediate), 0.1f))
    {
      this.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, nameof (SendNetworkUpdateImmediate));
      this.InvalidateNetworkCache();
      List<Connection> subscribers = this.GetSubscribers();
      if (subscribers != null)
      {
        if (subscribers.Count > 0)
        {
          for (int index = 0; index < subscribers.Count; ++index)
          {
            Connection connection = subscribers[index];
            BasePlayer player = connection.player as BasePlayer;
            if (!Object.op_Equality((Object) player, (Object) null) && this.ShouldNetworkTo(player))
              this.SendAsSnapshot(connection, justCreated);
          }
        }
      }
    }
    ((Component) this).get_gameObject().SendOnSendNetworkUpdate(this as BaseEntity);
  }

  protected void SendNetworkUpdate_Position()
  {
    if (Application.isLoading != null || Application.isLoadingSave != null || (this.IsDestroyed || this.net == null) || !this.isSpawned)
      return;
    List<Connection> subscribers = this.GetSubscribers();
    if (subscribers == null || subscribers.Count == 0)
      return;
    this.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, nameof (SendNetworkUpdate_Position));
    if (!((Write) ((NetworkPeer) Net.sv).write).Start())
      return;
    ((Write) ((NetworkPeer) Net.sv).write).PacketID((Message.Type) 10);
    ((Write) ((NetworkPeer) Net.sv).write).EntityID((uint) this.net.ID);
    ((Write) ((NetworkPeer) Net.sv).write).Vector3(this.GetNetworkPosition());
    // ISSUE: variable of the null type
    __Null write1 = ((NetworkPeer) Net.sv).write;
    Quaternion networkRotation = this.GetNetworkRotation();
    Vector3 eulerAngles = ((Quaternion) ref networkRotation).get_eulerAngles();
    ((Write) write1).Vector3(eulerAngles);
    ((Write) ((NetworkPeer) Net.sv).write).Float(this.GetNetworkTime());
    uint uid = this.parentEntity.uid;
    if (uid > 0U)
      ((Write) ((NetworkPeer) Net.sv).write).EntityID(uid);
    // ISSUE: variable of the null type
    __Null write2 = ((NetworkPeer) Net.sv).write;
    SendInfo sendInfo1;
    ((SendInfo) ref sendInfo1).\u002Ector((IEnumerable<Connection>) subscribers);
    sendInfo1.method = (__Null) 1;
    sendInfo1.priority = (__Null) 0;
    SendInfo sendInfo2 = sendInfo1;
    ((Write) write2).Send(sendInfo2);
  }

  private void ToStream(Stream stream, BaseNetworkable.SaveInfo saveInfo)
  {
    using (saveInfo.msg = (Entity) Pool.Get<Entity>())
    {
      this.Save(saveInfo);
      if (saveInfo.msg.baseEntity == null)
        Debug.LogError((object) (this.ToString() + ": ToStream - no BaseEntity!?"));
      if (saveInfo.msg.baseNetworkable == null)
        Debug.LogError((object) (this.ToString() + ": ToStream - no baseNetworkable!?"));
      saveInfo.msg.ToProto(stream);
      this.PostSave(saveInfo);
    }
  }

  public virtual bool CanUseNetworkCache(Connection connection)
  {
    return ConVar.Server.netcache;
  }

  public void ToStreamForNetwork(Stream stream, BaseNetworkable.SaveInfo saveInfo)
  {
    if (!this.CanUseNetworkCache(saveInfo.forConnection))
    {
      this.ToStream(stream, saveInfo);
    }
    else
    {
      if (this._NetworkCache == null)
      {
        this._NetworkCache = BaseNetworkable.EntityMemoryStreamPool.Count > 0 ? (this._NetworkCache = BaseNetworkable.EntityMemoryStreamPool.Dequeue()) : new MemoryStream(8);
        this.ToStream((Stream) this._NetworkCache, saveInfo);
        ConVar.Server.netcachesize += (int) this._NetworkCache.Length;
      }
      this._NetworkCache.WriteTo(stream);
    }
  }

  public void InvalidateNetworkCache()
  {
    using (TimeWarning.New(nameof (InvalidateNetworkCache), 0.1f))
    {
      if (this._SaveCache != null)
      {
        ConVar.Server.savecachesize -= (int) this._SaveCache.Length;
        this._SaveCache.SetLength(0L);
        this._SaveCache.Position = 0L;
        BaseNetworkable.EntityMemoryStreamPool.Enqueue(this._SaveCache);
        this._SaveCache = (MemoryStream) null;
      }
      if (this._NetworkCache != null)
      {
        ConVar.Server.netcachesize -= (int) this._NetworkCache.Length;
        this._NetworkCache.SetLength(0L);
        this._NetworkCache.Position = 0L;
        BaseNetworkable.EntityMemoryStreamPool.Enqueue(this._NetworkCache);
        this._NetworkCache = (MemoryStream) null;
      }
      this.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 3, nameof (InvalidateNetworkCache));
    }
  }

  public MemoryStream GetSaveCache()
  {
    if (this._SaveCache == null)
    {
      this._SaveCache = BaseNetworkable.EntityMemoryStreamPool.Count <= 0 ? new MemoryStream(8) : BaseNetworkable.EntityMemoryStreamPool.Dequeue();
      this.ToStream((Stream) this._SaveCache, new BaseNetworkable.SaveInfo()
      {
        forDisk = true
      });
      ConVar.Server.savecachesize += (int) this._SaveCache.Length;
    }
    return this._SaveCache;
  }

  public virtual void UpdateNetworkGroup()
  {
    Assert.IsTrue(this.isServer, "UpdateNetworkGroup called on clientside entity!");
    if (this.net == null)
      return;
    using (TimeWarning.New("UpdateGroups", 0.1f))
    {
      if (!this.net.UpdateGroups(((Component) this).get_transform().get_position()))
        return;
      this.SendNetworkGroupChange();
    }
  }

  public struct SaveInfo
  {
    public Entity msg;
    public bool forDisk;
    public Connection forConnection;

    internal bool SendingTo(Connection ownerConnection)
    {
      if (ownerConnection == null || this.forConnection == null)
        return false;
      return this.forConnection == ownerConnection;
    }
  }

  public struct LoadInfo
  {
    public Entity msg;
    public bool fromDisk;
  }

  public class EntityRealmServer : BaseNetworkable.EntityRealm
  {
    protected override Manager visibilityManager
    {
      get
      {
        if (Net.sv == null)
          return (Manager) null;
        return (Manager) ((Network.Server) Net.sv).visibility;
      }
    }
  }

  public abstract class EntityRealm : IEnumerable<BaseNetworkable>, IEnumerable
  {
    private ListDictionary<uint, BaseNetworkable> entityList = new ListDictionary<uint, BaseNetworkable>(8);

    public int Count
    {
      get
      {
        return this.entityList.get_Count();
      }
    }

    protected abstract Manager visibilityManager { get; }

    public BaseNetworkable Find(uint uid)
    {
      BaseNetworkable baseNetworkable = (BaseNetworkable) null;
      if (!this.entityList.TryGetValue(uid, ref baseNetworkable))
        return (BaseNetworkable) null;
      return baseNetworkable;
    }

    public void RegisterID(BaseNetworkable ent)
    {
      if (ent.net == null)
        return;
      if (this.entityList.Contains((uint) ent.net.ID))
        this.entityList.set_Item((uint) ent.net.ID, ent);
      else
        this.entityList.Add((uint) ent.net.ID, ent);
    }

    public void UnregisterID(BaseNetworkable ent)
    {
      if (ent.net == null)
        return;
      this.entityList.Remove((uint) ent.net.ID);
    }

    public Group FindGroup(uint uid)
    {
      return this.visibilityManager?.Get(uid);
    }

    public Group TryFindGroup(uint uid)
    {
      return this.visibilityManager?.TryGet(uid);
    }

    public void FindInGroup(uint uid, List<BaseNetworkable> list)
    {
      Group group = this.TryFindGroup(uid);
      if (group == null)
        return;
      int count = ((ListHashSet<Networkable>) group.networkables).get_Values().get_Count();
      Networkable[] buffer = ((ListHashSet<Networkable>) group.networkables).get_Values().get_Buffer();
      for (int index = 0; index < count; ++index)
      {
        BaseNetworkable baseNetworkable = this.Find((uint) buffer[index].ID);
        if (!Object.op_Equality((Object) baseNetworkable, (Object) null) && baseNetworkable.net != null && baseNetworkable.net.group != null)
        {
          if (((Group) baseNetworkable.net.group).ID != (int) uid)
            Debug.LogWarning((object) ("Group ID mismatch: " + ((object) baseNetworkable).ToString()));
          else
            list.Add(baseNetworkable);
        }
      }
    }

    public IEnumerator<BaseNetworkable> GetEnumerator()
    {
      return this.entityList.get_Values().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }
  }

  public enum DestroyMode : byte
  {
    None,
    Gib,
  }
}
