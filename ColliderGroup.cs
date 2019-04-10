// Decompiled with JetBrains decompiler
// Type: ColliderGroup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderGroup : Pool.IPooled
{
  public ListHashSet<ColliderBatch> Colliders = new ListHashSet<ColliderBatch>(8);
  public List<ColliderBatch> TempColliders = new List<ColliderBatch>();
  public List<MeshColliderBatch> Batches = new List<MeshColliderBatch>();
  public List<MeshColliderBatch> TempBatches = new List<MeshColliderBatch>();
  public List<MeshColliderInstance> TempInstances = new List<MeshColliderInstance>();
  public bool Invalidated;
  public bool NeedsRefresh;
  public bool Processing;
  public bool Preserving;
  private ColliderGrid grid;
  private ColliderCell cell;
  private ColliderKey key;
  private Action updateData;
  private Action refreshBatches;

  public float Size
  {
    get
    {
      return this.grid.CellSize;
    }
  }

  public Vector3 Position
  {
    get
    {
      return this.cell.position;
    }
  }

  public int Count
  {
    get
    {
      return this.Colliders.get_Count();
    }
  }

  public void Initialize(ColliderGrid grid, ColliderCell cell, ColliderKey key)
  {
    this.grid = grid;
    this.cell = cell;
    this.key = key;
  }

  public void EnterPool()
  {
    this.Invalidated = false;
    this.NeedsRefresh = false;
    this.Processing = false;
    this.Preserving = false;
    this.Colliders.Clear();
    this.TempColliders.Clear();
    this.Batches.Clear();
    this.TempBatches.Clear();
    this.TempInstances.Clear();
    this.grid = (ColliderGrid) null;
    this.cell = (ColliderCell) null;
    this.key = new ColliderKey();
  }

  public void LeavePool()
  {
  }

  public void Add(ColliderBatch collider)
  {
    this.Colliders.Add(collider);
    this.NeedsRefresh = true;
  }

  public void Remove(ColliderBatch collider)
  {
    this.Colliders.Remove(collider);
    this.NeedsRefresh = true;
  }

  public void Invalidate()
  {
    if (!this.Invalidated)
    {
      for (int index = 0; index < this.Batches.Count; ++index)
        this.Batches[index].Invalidate();
      this.Invalidated = true;
    }
    this.cell.interrupt = true;
  }

  public void Add(MeshColliderInstance instance)
  {
    this.TempInstances.Add(instance);
  }

  public void UpdateData()
  {
    this.TempInstances.Clear();
    for (int index = 0; index < this.TempColliders.Count && !this.cell.interrupt; ++index)
      this.TempColliders[index].AddBatch(this);
  }

  public void CreateBatches()
  {
    if (this.TempInstances.Count == 0)
      return;
    MeshColliderBatch batch = this.CreateBatch();
    for (int index = 0; index < this.TempInstances.Count; ++index)
    {
      MeshColliderInstance tempInstance = this.TempInstances[index];
      if (batch.AvailableVertices < tempInstance.mesh.get_vertexCount())
        batch = this.CreateBatch();
      batch.Add(tempInstance);
    }
    this.TempInstances.Clear();
  }

  public void RefreshBatches()
  {
    for (int index = 0; index < this.TempBatches.Count && !this.cell.interrupt; ++index)
      this.TempBatches[index].Refresh();
  }

  public void ApplyBatches()
  {
    for (int index = 0; index < this.TempBatches.Count && !this.cell.interrupt; ++index)
      this.TempBatches[index].Apply();
  }

  public void DisplayBatches()
  {
    for (int index = 0; index < this.TempBatches.Count && !this.cell.interrupt; ++index)
      this.TempBatches[index].Display();
  }

  public IEnumerator UpdateDataAsync()
  {
    if (this.updateData == null)
      this.updateData = new Action(this.UpdateData);
    return Parallel.Coroutine(this.updateData);
  }

  public IEnumerator RefreshBatchesAsync()
  {
    if (this.refreshBatches == null)
      this.refreshBatches = new Action(this.RefreshBatches);
    return Parallel.Coroutine(this.refreshBatches);
  }

  public void Start()
  {
    if (this.NeedsRefresh)
    {
      this.Processing = true;
      this.TempColliders.Clear();
      this.TempColliders.AddRange((IEnumerable<ColliderBatch>) this.Colliders.get_Values());
      this.NeedsRefresh = false;
    }
    else
      this.Preserving = true;
  }

  public void End()
  {
    if (this.Processing)
    {
      if (!this.cell.interrupt)
      {
        this.Clear();
        for (int index = 0; index < this.TempBatches.Count; ++index)
          this.TempBatches[index].Free();
        List<MeshColliderBatch> batches = this.Batches;
        this.Batches = this.TempBatches;
        this.TempBatches = batches;
        this.Invalidated = false;
      }
      else
        this.Cancel();
      this.TempColliders.Clear();
      this.Processing = false;
    }
    else
      this.Preserving = false;
  }

  public void Clear()
  {
    for (int index = 0; index < this.Batches.Count; ++index)
      this.grid.RecycleInstance(this.Batches[index]);
    this.Batches.Clear();
  }

  public void Cancel()
  {
    for (int index = 0; index < this.TempBatches.Count; ++index)
      this.grid.RecycleInstance(this.TempBatches[index]);
    this.TempBatches.Clear();
  }

  public int MeshCount()
  {
    int num = 0;
    for (int index = 0; index < this.Batches.Count; ++index)
      num += this.Batches[index].Count;
    return num;
  }

  public int BatchedMeshCount()
  {
    int num = 0;
    for (int index = 0; index < this.Batches.Count; ++index)
      num += this.Batches[index].BatchedCount;
    return num;
  }

  public MeshColliderBatch CreateBatch()
  {
    MeshColliderBatch instance = this.grid.CreateInstance();
    instance.Setup(this.cell.position, LayerMask.op_Implicit(this.key.layer), this.key.material);
    instance.Alloc();
    this.TempBatches.Add(instance);
    return instance;
  }
}
