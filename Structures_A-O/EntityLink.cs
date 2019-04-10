// Decompiled with JetBrains decompiler
// Type: EntityLink
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System.Collections.Generic;
using UnityEngine;

public class EntityLink : Pool.IPooled
{
  public List<EntityLink> connections = new List<EntityLink>(8);
  public int capacity = int.MaxValue;
  public BaseEntity owner;
  public Socket_Base socket;

  public string name
  {
    get
    {
      return this.socket.socketName;
    }
  }

  public void Setup(BaseEntity owner, Socket_Base socket)
  {
    this.owner = owner;
    this.socket = socket;
    if (!socket.monogamous)
      return;
    this.capacity = 1;
  }

  public void EnterPool()
  {
    this.owner = (BaseEntity) null;
    this.socket = (Socket_Base) null;
    this.capacity = int.MaxValue;
  }

  public void LeavePool()
  {
  }

  public bool Contains(EntityLink entity)
  {
    return this.connections.Contains(entity);
  }

  public void Add(EntityLink entity)
  {
    this.connections.Add(entity);
  }

  public void Remove(EntityLink entity)
  {
    this.connections.Remove(entity);
  }

  public void Clear()
  {
    for (int index = 0; index < this.connections.Count; ++index)
      this.connections[index].Remove(this);
    this.connections.Clear();
  }

  public bool IsEmpty()
  {
    return this.connections.Count == 0;
  }

  public bool IsOccupied()
  {
    return this.connections.Count >= this.capacity;
  }

  public bool IsMale()
  {
    return this.socket.male;
  }

  public bool IsFemale()
  {
    return this.socket.female;
  }

  public bool CanConnect(EntityLink link)
  {
    if (this.IsOccupied() || link == null || link.IsOccupied())
      return false;
    return this.socket.CanConnect(((Component) this.owner).get_transform().get_position(), ((Component) this.owner).get_transform().get_rotation(), link.socket, ((Component) link.owner).get_transform().get_position(), ((Component) link.owner).get_transform().get_rotation());
  }
}
