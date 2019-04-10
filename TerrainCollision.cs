// Decompiled with JetBrains decompiler
// Type: TerrainCollision
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCollision : TerrainExtension
{
  private ListDictionary<Collider, List<Collider>> ignoredColliders;
  private TerrainCollider terrainCollider;

  public override void Setup()
  {
    this.ignoredColliders = new ListDictionary<Collider, List<Collider>>(8);
    this.terrainCollider = (TerrainCollider) ((Component) this.terrain).GetComponent<TerrainCollider>();
  }

  public void Clear()
  {
    if (!Object.op_Implicit((Object) this.terrainCollider))
      return;
    using (IEnumerator<Collider> enumerator = this.ignoredColliders.get_Keys().GetEnumerator())
    {
      while (((IEnumerator) enumerator).MoveNext())
        Physics.IgnoreCollision(enumerator.Current, (Collider) this.terrainCollider, false);
    }
    this.ignoredColliders.Clear();
  }

  public void Reset(Collider collider)
  {
    if (!Object.op_Implicit((Object) this.terrainCollider) || !Object.op_Implicit((Object) collider))
      return;
    Physics.IgnoreCollision(collider, (Collider) this.terrainCollider, false);
    this.ignoredColliders.Remove(collider);
  }

  public bool GetIgnore(Vector3 pos, float radius = 0.01f)
  {
    return GamePhysics.CheckSphere<TerrainCollisionTrigger>(pos, radius, 262144, (QueryTriggerInteraction) 2);
  }

  public bool GetIgnore(RaycastHit hit)
  {
    if (((RaycastHit) ref hit).get_collider() is TerrainCollider)
      return this.GetIgnore(((RaycastHit) ref hit).get_point(), 0.01f);
    return false;
  }

  public bool GetIgnore(Collider collider)
  {
    if (!Object.op_Implicit((Object) this.terrainCollider) || !Object.op_Implicit((Object) collider))
      return false;
    return this.ignoredColliders.Contains(collider);
  }

  public void SetIgnore(Collider collider, Collider trigger, bool ignore = true)
  {
    if (!Object.op_Implicit((Object) this.terrainCollider) || !Object.op_Implicit((Object) collider))
      return;
    if (!this.GetIgnore(collider))
    {
      if (!ignore)
        return;
      List<Collider> colliderList1 = new List<Collider>();
      colliderList1.Add(trigger);
      List<Collider> colliderList2 = colliderList1;
      Physics.IgnoreCollision(collider, (Collider) this.terrainCollider, true);
      this.ignoredColliders.Add(collider, colliderList2);
    }
    else
    {
      List<Collider> colliderList = this.ignoredColliders.get_Item(collider);
      if (ignore)
      {
        if (colliderList.Contains(trigger))
          return;
        colliderList.Add(trigger);
      }
      else
      {
        if (!colliderList.Contains(trigger))
          return;
        colliderList.Remove(trigger);
      }
    }
  }

  protected void LateUpdate()
  {
    if (this.ignoredColliders == null)
      return;
    for (int index = 0; index < this.ignoredColliders.get_Count(); ++index)
    {
      KeyValuePair<Collider, List<Collider>> byIndex = this.ignoredColliders.GetByIndex(index);
      Collider key = byIndex.Key;
      List<Collider> colliderList = byIndex.Value;
      if (Object.op_Equality((Object) key, (Object) null))
        this.ignoredColliders.RemoveAt(index--);
      else if (colliderList.Count == 0)
      {
        Physics.IgnoreCollision(key, (Collider) this.terrainCollider, false);
        this.ignoredColliders.RemoveAt(index--);
      }
    }
  }
}
