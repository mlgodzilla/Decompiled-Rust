// Decompiled with JetBrains decompiler
// Type: ImpostorBatch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ImpostorBatch
{
  private uint[] args = new uint[5];
  private Queue<int> recycle = new Queue<int>(32);
  public SimpleList<Vector4> Positions;

  public Mesh Mesh { private set; get; }

  public Material Material { private set; get; }

  public int DeferredPass { private set; get; } = -1;

  public int ShadowPass { private set; get; } = -1;

  public ComputeBuffer PositionBuffer { private set; get; }

  public ComputeBuffer ArgsBuffer { private set; get; }

  public int Count
  {
    get
    {
      return this.Positions.Count;
    }
  }

  public bool Visible
  {
    get
    {
      return this.Positions.Count - this.recycle.Count > 0;
    }
  }

  private ComputeBuffer SafeRelease(ComputeBuffer buffer)
  {
    buffer?.Release();
    return (ComputeBuffer) null;
  }

  public void Initialize(Mesh mesh, Material material)
  {
    this.Mesh = mesh;
    this.Material = material;
    this.DeferredPass = material.FindPass("DEFERRED");
    this.ShadowPass = material.FindPass("SHADOWCASTER");
    this.Positions = (SimpleList<Vector4>) Pool.Get<SimpleList<Vector4>>();
    this.Positions.Clear();
    this.ArgsBuffer = this.SafeRelease(this.ArgsBuffer);
    this.ArgsBuffer = new ComputeBuffer(1, this.args.Length * 4, (ComputeBufferType) 256);
    this.args[0] = this.Mesh.GetIndexCount(0);
    this.args[2] = this.Mesh.GetIndexStart(0);
    this.args[3] = this.Mesh.GetBaseVertex(0);
  }

  public void Release()
  {
    this.recycle.Clear();
    // ISSUE: cast to a reference type
    Pool.Free<SimpleList<Vector4>>((M0&) ref this.Positions);
    this.PositionBuffer = this.SafeRelease(this.PositionBuffer);
    this.ArgsBuffer = this.SafeRelease(this.ArgsBuffer);
  }

  public void AddInstance(ImpostorInstanceData data)
  {
    data.Batch = this;
    if (this.recycle.Count > 0)
    {
      data.BatchIndex = this.recycle.Dequeue();
      this.Positions[data.BatchIndex] = data.PositionAndScale();
    }
    else
    {
      data.BatchIndex = this.Positions.Count;
      this.Positions.Add(data.PositionAndScale());
    }
  }

  public void RemoveInstance(ImpostorInstanceData data)
  {
    this.Positions[data.BatchIndex] = new Vector4(0.0f, 0.0f, 0.0f, -1f);
    this.recycle.Enqueue(data.BatchIndex);
    data.BatchIndex = 0;
    data.Batch = (ImpostorBatch) null;
  }

  public void UpdateBuffers()
  {
    bool flag = false;
    if (this.PositionBuffer == null || this.PositionBuffer.get_count() != this.Positions.Count)
    {
      this.PositionBuffer = this.SafeRelease(this.PositionBuffer);
      this.PositionBuffer = new ComputeBuffer(this.Positions.Count, 16);
      flag = true;
    }
    if (this.PositionBuffer != null)
      this.PositionBuffer.SetData((Array) this.Positions.Array, 0, 0, this.Positions.Count);
    if (!(this.ArgsBuffer != null & flag))
      return;
    this.args[1] = (uint) this.Positions.Count;
    this.ArgsBuffer.SetData((Array) this.args);
  }
}
