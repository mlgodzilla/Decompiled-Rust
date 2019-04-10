// Decompiled with JetBrains decompiler
// Type: WaterCollision
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCollision : MonoBehaviour
{
  private ListDictionary<Collider, List<Collider>> ignoredColliders;
  private HashSet<Collider> waterColliders;

  private void Awake()
  {
    this.ignoredColliders = new ListDictionary<Collider, List<Collider>>(8);
    this.waterColliders = new HashSet<Collider>();
  }

  public void Clear()
  {
    if (this.waterColliders.Count == 0)
      return;
    foreach (Collider waterCollider in this.waterColliders)
    {
      using (IEnumerator<Collider> enumerator = this.ignoredColliders.get_Keys().GetEnumerator())
      {
        while (((IEnumerator) enumerator).MoveNext())
          Physics.IgnoreCollision(enumerator.Current, waterCollider, false);
      }
    }
    this.ignoredColliders.Clear();
  }

  public void Reset(Collider collider)
  {
    if (this.waterColliders.Count == 0 || !Object.op_Implicit((Object) collider))
      return;
    foreach (Collider waterCollider in this.waterColliders)
      Physics.IgnoreCollision(collider, waterCollider, false);
    this.ignoredColliders.Remove(collider);
  }

  public bool GetIgnore(Vector3 pos, float radius = 0.01f)
  {
    return GamePhysics.CheckSphere<WaterVisibilityTrigger>(pos, radius, 262144, (QueryTriggerInteraction) 2);
  }

  public bool GetIgnore(Bounds bounds)
  {
    return GamePhysics.CheckBounds<WaterVisibilityTrigger>(bounds, 262144, (QueryTriggerInteraction) 2);
  }

  public bool GetIgnore(RaycastHit hit)
  {
    if (this.waterColliders.Contains(((RaycastHit) ref hit).get_collider()))
      return this.GetIgnore(((RaycastHit) ref hit).get_point(), 0.01f);
    return false;
  }

  public bool GetIgnore(Collider collider)
  {
    if (this.waterColliders.Count == 0 || !Object.op_Implicit((Object) collider))
      return false;
    return this.ignoredColliders.Contains(collider);
  }

  public void SetIgnore(Collider collider, Collider trigger, bool ignore = true)
  {
    if (this.waterColliders.Count == 0 || !Object.op_Implicit((Object) collider))
      return;
    if (!this.GetIgnore(collider))
    {
      if (!ignore)
        return;
      List<Collider> colliderList1 = new List<Collider>();
      colliderList1.Add(trigger);
      List<Collider> colliderList2 = colliderList1;
      foreach (Collider waterCollider in this.waterColliders)
        Physics.IgnoreCollision(collider, waterCollider, true);
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
    for (int index = 0; index < this.ignoredColliders.get_Count(); ++index)
    {
      KeyValuePair<Collider, List<Collider>> byIndex = this.ignoredColliders.GetByIndex(index);
      Collider key = byIndex.Key;
      List<Collider> colliderList = byIndex.Value;
      if (Object.op_Equality((Object) key, (Object) null))
        this.ignoredColliders.RemoveAt(index--);
      else if (colliderList.Count == 0)
      {
        foreach (Collider waterCollider in this.waterColliders)
          Physics.IgnoreCollision(key, waterCollider, false);
        this.ignoredColliders.RemoveAt(index--);
      }
    }
  }

  public WaterCollision()
  {
    base.\u002Ector();
  }
}
