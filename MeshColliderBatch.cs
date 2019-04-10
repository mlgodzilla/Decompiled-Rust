// Decompiled with JetBrains decompiler
// Type: MeshColliderBatch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Rust;
using System.Collections.Generic;
using UnityEngine;

public class MeshColliderBatch : MeshBatch
{
  private Vector3 position;
  private Mesh meshBatch;
  private MeshCollider meshCollider;
  private MeshColliderData meshData;
  private MeshColliderGroup meshGroup;
  public MeshColliderLookup meshLookup;

  public override int VertexCapacity
  {
    get
    {
      return Batching.collider_capacity;
    }
  }

  public override int VertexCutoff
  {
    get
    {
      return Batching.collider_vertices;
    }
  }

  public Transform LookupTransform(int triangleIndex)
  {
    return this.meshLookup.Get(triangleIndex).transform;
  }

  public Rigidbody LookupRigidbody(int triangleIndex)
  {
    return this.meshLookup.Get(triangleIndex).rigidbody;
  }

  public Collider LookupCollider(int triangleIndex)
  {
    return this.meshLookup.Get(triangleIndex).collider;
  }

  public void LookupColliders<T>(Vector3 position, float distance, List<T> list) where T : Collider
  {
    List<MeshColliderLookup.LookupEntry> data = this.meshLookup.src.data;
    float num = distance * distance;
    for (int index = 0; index < data.Count; ++index)
    {
      MeshColliderLookup.LookupEntry lookupEntry = data[index];
      if (Object.op_Implicit((Object) lookupEntry.collider))
      {
        Vector3 vector3 = Vector3.op_Subtraction(((OBB) ref lookupEntry.bounds).ClosestPoint(position), position);
        if ((double) ((Vector3) ref vector3).get_sqrMagnitude() <= (double) num)
          list.Add(lookupEntry.collider as T);
      }
    }
  }

  public void LookupColliders<T>(OBB bounds, List<T> list) where T : Collider
  {
    List<MeshColliderLookup.LookupEntry> data = this.meshLookup.src.data;
    for (int index = 0; index < data.Count; ++index)
    {
      MeshColliderLookup.LookupEntry lookupEntry = data[index];
      if (Object.op_Implicit((Object) lookupEntry.collider) && ((OBB) ref lookupEntry.bounds).Intersects(bounds))
        list.Add(lookupEntry.collider as T);
    }
  }

  protected void Awake()
  {
    this.meshCollider = (MeshCollider) ((Component) this).GetComponent<MeshCollider>();
    this.meshData = new MeshColliderData();
    this.meshGroup = new MeshColliderGroup();
    this.meshLookup = new MeshColliderLookup();
  }

  public void Setup(Vector3 position, LayerMask layer, PhysicMaterial material)
  {
    Vector3 vector3;
    ((Component) this).get_transform().set_position(vector3 = position);
    this.position = vector3;
    ((Component) this).get_gameObject().set_layer(LayerMask.op_Implicit(layer));
    ((Collider) this.meshCollider).set_sharedMaterial(material);
  }

  public void Add(MeshColliderInstance instance)
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
  }

  protected override void ToggleMesh(bool state)
  {
    if (Application.isLoading == null)
    {
      List<MeshColliderLookup.LookupEntry> data = this.meshLookup.src.data;
      for (int index = 0; index < data.Count; ++index)
      {
        Collider collider = data[index].collider;
        if (Object.op_Implicit((Object) collider))
          collider.set_enabled(!state);
      }
    }
    if (state)
    {
      if (!Object.op_Implicit((Object) this.meshCollider))
        return;
      this.meshCollider.set_sharedMesh(this.meshBatch);
      ((Collider) this.meshCollider).set_enabled(false);
      ((Collider) this.meshCollider).set_enabled(true);
    }
    else
    {
      if (!Object.op_Implicit((Object) this.meshCollider))
        return;
      this.meshCollider.set_sharedMesh((Mesh) null);
      ((Collider) this.meshCollider).set_enabled(false);
    }
  }

  protected override void OnPooled()
  {
    if (Object.op_Implicit((Object) this.meshCollider))
      this.meshCollider.set_sharedMesh((Mesh) null);
    if (Object.op_Implicit((Object) this.meshBatch))
      AssetPool.Free(ref this.meshBatch);
    this.meshData.Free();
    this.meshGroup.Free();
    this.meshLookup.src.Clear();
    this.meshLookup.dst.Clear();
  }
}
