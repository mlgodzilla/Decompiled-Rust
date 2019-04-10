// Decompiled with JetBrains decompiler
// Type: MeshBatch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using UnityEngine;

public abstract class MeshBatch : MonoBehaviour
{
  public bool NeedsRefresh { get; private set; }

  public int Count { get; private set; }

  public int BatchedCount { get; private set; }

  public int VertexCount { get; private set; }

  protected abstract void AllocMemory();

  protected abstract void FreeMemory();

  protected abstract void RefreshMesh();

  protected abstract void ApplyMesh();

  protected abstract void ToggleMesh(bool state);

  protected abstract void OnPooled();

  public abstract int VertexCapacity { get; }

  public abstract int VertexCutoff { get; }

  public int AvailableVertices
  {
    get
    {
      return Mathf.Clamp(this.VertexCapacity, this.VertexCutoff, 65534) - this.VertexCount;
    }
  }

  public void Alloc()
  {
    this.AllocMemory();
  }

  public void Free()
  {
    this.FreeMemory();
  }

  public void Refresh()
  {
    this.RefreshMesh();
  }

  public void Apply()
  {
    this.NeedsRefresh = false;
    this.ApplyMesh();
  }

  public void Display()
  {
    this.ToggleMesh(true);
    this.BatchedCount = this.Count;
  }

  public void Invalidate()
  {
    this.ToggleMesh(false);
    this.BatchedCount = 0;
  }

  protected void AddVertices(int vertices)
  {
    this.NeedsRefresh = true;
    ++this.Count;
    this.VertexCount += vertices;
  }

  protected void OnEnable()
  {
    this.NeedsRefresh = false;
    this.Count = 0;
    this.BatchedCount = 0;
    this.VertexCount = 0;
  }

  protected void OnDisable()
  {
    if (Application.isQuitting != null)
      return;
    this.NeedsRefresh = false;
    this.Count = 0;
    this.BatchedCount = 0;
    this.VertexCount = 0;
    this.OnPooled();
  }

  protected MeshBatch()
  {
    base.\u002Ector();
  }
}
