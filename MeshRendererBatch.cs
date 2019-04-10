// Decompiled with JetBrains decompiler
// Type: MeshRendererBatch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MeshRendererBatch : MeshBatch
{
  private Vector3 position;
  private Mesh meshBatch;
  private MeshFilter meshFilter;
  private MeshRenderer meshRenderer;
  private MeshRendererData meshData;
  private MeshRendererGroup meshGroup;
  private MeshRendererLookup meshLookup;

  public override int VertexCapacity
  {
    get
    {
      return Batching.renderer_capacity;
    }
  }

  public override int VertexCutoff
  {
    get
    {
      return Batching.renderer_vertices;
    }
  }

  protected void Awake()
  {
    this.meshFilter = (MeshFilter) ((Component) this).GetComponent<MeshFilter>();
    this.meshRenderer = (MeshRenderer) ((Component) this).GetComponent<MeshRenderer>();
    this.meshData = new MeshRendererData();
    this.meshGroup = new MeshRendererGroup();
    this.meshLookup = new MeshRendererLookup();
  }

  public void Setup(Vector3 position, Material material, ShadowCastingMode shadows, int layer)
  {
    Vector3 vector3;
    ((Component) this).get_transform().set_position(vector3 = position);
    this.position = vector3;
    ((Component) this).get_gameObject().set_layer(layer);
    ((Renderer) this.meshRenderer).set_sharedMaterial(material);
    ((Renderer) this.meshRenderer).set_shadowCastingMode(shadows);
    if (shadows == 3)
    {
      ((Renderer) this.meshRenderer).set_receiveShadows(false);
      ((Renderer) this.meshRenderer).set_motionVectors(false);
      ((Renderer) this.meshRenderer).set_lightProbeUsage((LightProbeUsage) 0);
      ((Renderer) this.meshRenderer).set_reflectionProbeUsage((ReflectionProbeUsage) 0);
    }
    else
    {
      ((Renderer) this.meshRenderer).set_receiveShadows(true);
      ((Renderer) this.meshRenderer).set_motionVectors(true);
      ((Renderer) this.meshRenderer).set_lightProbeUsage((LightProbeUsage) 1);
      ((Renderer) this.meshRenderer).set_reflectionProbeUsage((ReflectionProbeUsage) 1);
    }
  }

  public void Add(MeshRendererInstance instance)
  {
    ref Vector3 local = ref instance.position;
    local = Vector3.op_Subtraction(local, this.position);
    this.meshGroup.data.Add(instance);
    this.AddVertices(instance.mesh.get_vertexCount());
  }

  protected override void AllocMemory()
  {
    this.meshGroup.Alloc();
    this.meshData.Alloc();
  }

  protected override void FreeMemory()
  {
    this.meshGroup.Free();
    this.meshData.Free();
  }

  protected override void RefreshMesh()
  {
    this.meshLookup.dst.Clear();
    this.meshData.Clear();
    this.meshData.Combine(this.meshGroup, this.meshLookup);
  }

  protected override void ApplyMesh()
  {
    if (!Object.op_Implicit((Object) this.meshBatch))
      this.meshBatch = (Mesh) AssetPool.Get<Mesh>();
    this.meshLookup.Apply();
    this.meshData.Apply(this.meshBatch);
    this.meshBatch.UploadMeshData(false);
  }

  protected override void ToggleMesh(bool state)
  {
    List<MeshRendererLookup.LookupEntry> data = this.meshLookup.src.data;
    for (int index = 0; index < data.Count; ++index)
    {
      Renderer renderer = data[index].renderer;
      if (Object.op_Implicit((Object) renderer))
        renderer.set_enabled(!state);
    }
    if (state)
    {
      if (Object.op_Implicit((Object) this.meshFilter))
        this.meshFilter.set_sharedMesh(this.meshBatch);
      if (!Object.op_Implicit((Object) this.meshRenderer))
        return;
      ((Renderer) this.meshRenderer).set_enabled(true);
    }
    else
    {
      if (Object.op_Implicit((Object) this.meshFilter))
        this.meshFilter.set_sharedMesh((Mesh) null);
      if (!Object.op_Implicit((Object) this.meshRenderer))
        return;
      ((Renderer) this.meshRenderer).set_enabled(false);
    }
  }

  protected override void OnPooled()
  {
    if (Object.op_Implicit((Object) this.meshFilter))
      this.meshFilter.set_sharedMesh((Mesh) null);
    if (Object.op_Implicit((Object) this.meshBatch))
      AssetPool.Free(ref this.meshBatch);
    this.meshData.Free();
    this.meshGroup.Free();
    this.meshLookup.src.Clear();
    this.meshLookup.dst.Clear();
  }
}
