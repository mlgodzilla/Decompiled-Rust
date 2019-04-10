// Decompiled with JetBrains decompiler
// Type: ColliderCell
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using System.Collections;
using UnityEngine;

public class ColliderCell
{
  private ListDictionary<ColliderKey, ColliderGroup> batches = new ListDictionary<ColliderKey, ColliderGroup>(8);
  public Vector3 position;
  public ColliderGrid grid;
  public bool interrupt;

  public ColliderCell(ColliderGrid grid, Vector3 position)
  {
    this.grid = grid;
    this.position = position;
  }

  public bool NeedsRefresh()
  {
    BufferList<ColliderGroup> values = this.batches.get_Values();
    for (int index = 0; index < values.get_Count(); ++index)
    {
      if (values.get_Item(index).NeedsRefresh)
        return true;
    }
    return false;
  }

  public int MeshCount()
  {
    int num = 0;
    BufferList<ColliderGroup> values = this.batches.get_Values();
    for (int index = 0; index < values.get_Count(); ++index)
      num += values.get_Item(index).MeshCount();
    return num;
  }

  public int BatchedMeshCount()
  {
    int num = 0;
    BufferList<ColliderGroup> values = this.batches.get_Values();
    for (int index = 0; index < values.get_Count(); ++index)
      num += values.get_Item(index).BatchedMeshCount();
    return num;
  }

  public void Refresh()
  {
    this.interrupt = false;
    BufferList<ColliderGroup> values = this.batches.get_Values();
    for (int index = 0; index < values.get_Count(); ++index)
    {
      ColliderGroup grp = values.get_Item(index);
      if (!grp.Processing)
      {
        if (grp.Count > 0)
        {
          grp.Start();
          if (grp.Processing)
          {
            grp.UpdateData();
            grp.CreateBatches();
            grp.RefreshBatches();
            grp.ApplyBatches();
            grp.DisplayBatches();
          }
          grp.End();
        }
        else
        {
          grp.Clear();
          this.DestroyColliderGroup(ref grp);
          this.batches.RemoveAt(index--);
        }
      }
    }
  }

  public IEnumerator RefreshAsync()
  {
    this.interrupt = false;
    BufferList<ColliderGroup> batchGroups = this.batches.get_Values();
    for (int index = 0; index < batchGroups.get_Count(); ++index)
    {
      ColliderGroup colliderGroup = batchGroups.get_Item(index);
      if (colliderGroup.Count > 0)
        colliderGroup.Start();
    }
    int i;
    IEnumerator enumerator;
    for (i = 0; i < batchGroups.get_Count() && !this.interrupt; ++i)
    {
      ColliderGroup colliderGroup = batchGroups.get_Item(i);
      if (colliderGroup.Processing)
      {
        if (Batching.collider_threading)
        {
          enumerator = colliderGroup.UpdateDataAsync();
          while (enumerator.MoveNext())
            yield return enumerator.Current;
          this.grid.ResetTimeout();
          enumerator = (IEnumerator) null;
        }
        else
        {
          colliderGroup.UpdateData();
          if (this.grid.NeedsTimeout)
          {
            yield return (object) CoroutineEx.waitForEndOfFrame;
            this.grid.ResetTimeout();
          }
        }
      }
    }
    for (i = 0; i < batchGroups.get_Count() && !this.interrupt; ++i)
    {
      ColliderGroup colliderGroup = batchGroups.get_Item(i);
      if (colliderGroup.Processing)
      {
        colliderGroup.CreateBatches();
        if (this.grid.NeedsTimeout)
        {
          yield return (object) CoroutineEx.waitForEndOfFrame;
          this.grid.ResetTimeout();
        }
      }
    }
    for (i = 0; i < batchGroups.get_Count() && !this.interrupt; ++i)
    {
      ColliderGroup colliderGroup = batchGroups.get_Item(i);
      if (colliderGroup.Processing)
      {
        if (Batching.collider_threading)
        {
          enumerator = colliderGroup.RefreshBatchesAsync();
          while (enumerator.MoveNext())
            yield return enumerator.Current;
          this.grid.ResetTimeout();
          enumerator = (IEnumerator) null;
        }
        else
        {
          colliderGroup.RefreshBatches();
          if (this.grid.NeedsTimeout)
          {
            yield return (object) CoroutineEx.waitForEndOfFrame;
            this.grid.ResetTimeout();
          }
        }
      }
    }
    for (i = 0; i < batchGroups.get_Count() && !this.interrupt; ++i)
    {
      ColliderGroup batchGroup = batchGroups.get_Item(i);
      if (batchGroup.Processing)
      {
        for (int j = 0; j < batchGroup.TempBatches.Count && !this.interrupt; ++j)
        {
          batchGroup.TempBatches[j].Apply();
          if (this.grid.NeedsTimeout)
          {
            yield return (object) CoroutineEx.waitForEndOfFrame;
            this.grid.ResetTimeout();
          }
        }
      }
      batchGroup = (ColliderGroup) null;
    }
    for (int index = 0; index < batchGroups.get_Count() && !this.interrupt; ++index)
    {
      ColliderGroup colliderGroup = batchGroups.get_Item(index);
      if (colliderGroup.Processing)
        colliderGroup.DisplayBatches();
    }
    for (int index = 0; index < batchGroups.get_Count(); ++index)
    {
      ColliderGroup grp = batchGroups.get_Item(index);
      if (grp.Processing || grp.Preserving)
        grp.End();
      else if (grp.Count == 0 && !this.interrupt)
      {
        grp.Clear();
        this.DestroyColliderGroup(ref grp);
        this.batches.RemoveAt(index--);
      }
    }
  }

  public ColliderGroup FindBatchGroup(ColliderBatch collider)
  {
    ColliderKey key = new ColliderKey(collider);
    ColliderGroup colliderGroup;
    if (!this.batches.TryGetValue(key, ref colliderGroup))
    {
      colliderGroup = this.CreateColliderGroup(this.grid, this, key);
      this.batches.Add(key, colliderGroup);
    }
    return colliderGroup;
  }

  private ColliderGroup CreateColliderGroup(
    ColliderGrid grid,
    ColliderCell cell,
    ColliderKey key)
  {
    M0 m0 = Pool.Get<ColliderGroup>();
    ((ColliderGroup) m0).Initialize(grid, cell, key);
    return (ColliderGroup) m0;
  }

  private void DestroyColliderGroup(ref ColliderGroup grp)
  {
    // ISSUE: cast to a reference type
    Pool.Free<ColliderGroup>((M0&) ref grp);
  }
}
