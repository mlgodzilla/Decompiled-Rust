// Decompiled with JetBrains decompiler
// Type: TriggerBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Oxide.Core;
using Rust;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TriggerBase : BaseMonoBehaviour
{
  public LayerMask interestLayers;
  [NonSerialized]
  public HashSet<GameObject> contents;
  [NonSerialized]
  public HashSet<BaseEntity> entityContents;

  internal virtual GameObject InterestedInObject(GameObject obj)
  {
    int num = 1 << obj.get_layer();
    if ((((LayerMask) ref this.interestLayers).get_value() & num) != num)
      return (GameObject) null;
    return obj;
  }

  protected virtual void OnDisable()
  {
    if (Application.isQuitting != null || this.contents == null)
      return;
    foreach (GameObject targetObj in ((IEnumerable<GameObject>) this.contents).ToArray<GameObject>())
      this.OnTriggerExit(targetObj);
    this.contents = (HashSet<GameObject>) null;
  }

  internal virtual void OnEntityEnter(BaseEntity ent)
  {
    if (Object.op_Equality((Object) ent, (Object) null))
      return;
    if (this.entityContents == null)
      this.entityContents = new HashSet<BaseEntity>();
    Interface.CallHook(nameof (OnEntityEnter), (object) this, (object) ent);
    this.entityContents.Add(ent);
  }

  internal virtual void OnEntityLeave(BaseEntity ent)
  {
    if (this.entityContents == null)
      return;
    Interface.CallHook(nameof (OnEntityLeave), (object) this, (object) ent);
    this.entityContents.Remove(ent);
  }

  internal virtual void OnObjectAdded(GameObject obj)
  {
    if (Object.op_Equality((Object) obj, (Object) null))
      return;
    BaseEntity baseEntity = obj.ToBaseEntity();
    if (!Object.op_Implicit((Object) baseEntity))
      return;
    this.OnEntityEnter(baseEntity);
    baseEntity.EnterTrigger(this);
  }

  internal virtual void OnObjectRemoved(GameObject obj)
  {
    if (Object.op_Equality((Object) obj, (Object) null))
      return;
    BaseEntity baseEntity = obj.ToBaseEntity();
    if (!Object.op_Implicit((Object) baseEntity))
      return;
    this.OnEntityLeave(baseEntity);
    baseEntity.LeaveTrigger(this);
  }

  internal void RemoveInvalidEntities()
  {
    if (this.entityContents == null)
      return;
    Collider component = (Collider) ((Component) this).GetComponent<Collider>();
    if (Object.op_Equality((Object) component, (Object) null))
      return;
    Bounds bounds = component.get_bounds();
    ((Bounds) ref bounds).Expand(1f);
    foreach (BaseEntity baseEntity in this.entityContents.ToArray<BaseEntity>())
    {
      if (Object.op_Equality((Object) baseEntity, (Object) null))
        Debug.LogWarning((object) ("Trigger " + ((object) this).ToString() + " contains destroyed entity."));
      else if (!((Bounds) ref bounds).Contains(baseEntity.ClosestPoint(((Component) this).get_transform().get_position())))
      {
        Debug.LogWarning((object) ("Trigger " + ((object) this).ToString() + " contains entity that is too far away: " + ((object) baseEntity).ToString()));
        this.RemoveEntity(baseEntity);
      }
    }
  }

  internal bool CheckEntity(BaseEntity ent)
  {
    if (Object.op_Equality((Object) ent, (Object) null))
      return true;
    Collider component = (Collider) ((Component) this).GetComponent<Collider>();
    if (Object.op_Equality((Object) component, (Object) null))
      return true;
    Bounds bounds = component.get_bounds();
    ((Bounds) ref bounds).Expand(1f);
    return ((Bounds) ref bounds).Contains(ent.ClosestPoint(((Component) this).get_transform().get_position()));
  }

  internal virtual void OnObjects()
  {
  }

  internal virtual void OnEmpty()
  {
    this.contents = (HashSet<GameObject>) null;
    this.entityContents = (HashSet<BaseEntity>) null;
  }

  public void RemoveObject(GameObject obj)
  {
    if (Object.op_Equality((Object) obj, (Object) null))
      return;
    Collider component = (Collider) obj.GetComponent<Collider>();
    if (Object.op_Equality((Object) component, (Object) null))
      return;
    this.OnTriggerExit(component);
  }

  public void RemoveEntity(BaseEntity obj)
  {
    this.OnTriggerExit(((Component) obj).get_gameObject());
  }

  public void OnTriggerEnter(Collider collider)
  {
    if (Object.op_Equality((Object) this, (Object) null))
      return;
    using (TimeWarning.New("TriggerBase.OnTriggerEnter", 0.1f))
    {
      GameObject gameObject = this.InterestedInObject(((Component) collider).get_gameObject());
      if (Object.op_Equality((Object) gameObject, (Object) null))
        return;
      if (this.contents == null)
        this.contents = new HashSet<GameObject>();
      if (this.contents.Contains(gameObject))
        return;
      int count = this.contents.Count;
      this.contents.Add(gameObject);
      this.OnObjectAdded(gameObject);
      if (count == 0)
      {
        if (this.contents.Count == 1)
          this.OnObjects();
      }
    }
    if (!Debugging.checktriggers)
      return;
    this.RemoveInvalidEntities();
  }

  public void OnTriggerExit(Collider collider)
  {
    if (Object.op_Equality((Object) this, (Object) null) || Object.op_Equality((Object) collider, (Object) null))
      return;
    GameObject targetObj = this.InterestedInObject(((Component) collider).get_gameObject());
    if (Object.op_Equality((Object) targetObj, (Object) null))
      return;
    this.OnTriggerExit(targetObj);
    if (!Debugging.checktriggers)
      return;
    this.RemoveInvalidEntities();
  }

  private void OnTriggerExit(GameObject targetObj)
  {
    if (this.contents == null || !this.contents.Contains(targetObj))
      return;
    this.contents.Remove(targetObj);
    this.OnObjectRemoved(targetObj);
    if (this.contents != null && this.contents.Count != 0)
      return;
    this.OnEmpty();
  }
}
