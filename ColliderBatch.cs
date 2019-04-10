// Decompiled with JetBrains decompiler
// Type: ColliderBatch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Rust;
using UnityEngine;

public class ColliderBatch : MonoBehaviour, IServerComponent
{
  private ColliderGroup batchGroup;
  private MeshColliderInstance batchInstance;

  public Transform BatchTransform { get; set; }

  public MeshCollider BatchCollider { get; set; }

  public Rigidbody BatchRigidbody { get; set; }

  protected void OnEnable()
  {
    this.BatchTransform = ((Component) this).get_transform();
    this.BatchCollider = (MeshCollider) ((Component) this).GetComponent<MeshCollider>();
    this.BatchRigidbody = (Rigidbody) ((Component) this).GetComponent<Rigidbody>();
    this.Add();
  }

  protected void OnDisable()
  {
    if (Application.isQuitting != null)
      return;
    this.Remove();
  }

  public void Add()
  {
    if (this.batchGroup != null)
      this.Remove();
    if (!Batching.colliders || Object.op_Equality((Object) this.BatchTransform, (Object) null) || (Object.op_Equality((Object) this.BatchCollider, (Object) null) || this.BatchCollider.get_sharedMesh().get_subMeshCount() > Batching.collider_submeshes) || this.BatchCollider.get_sharedMesh().get_vertexCount() > Batching.collider_vertices)
      return;
    this.batchGroup = ((ColliderGrid) SingletonComponent<ColliderGrid>.Instance)[this.BatchTransform.get_position()].FindBatchGroup(this);
    this.batchGroup.Add(this);
    this.batchInstance.mesh = this.BatchCollider.get_sharedMesh();
    this.batchInstance.position = this.BatchTransform.get_position();
    this.batchInstance.rotation = this.BatchTransform.get_rotation();
    this.batchInstance.scale = this.BatchTransform.get_lossyScale();
    this.batchInstance.transform = this.BatchTransform;
    this.batchInstance.rigidbody = this.BatchRigidbody;
    this.batchInstance.collider = (Collider) this.BatchCollider;
    this.batchInstance.bounds = new OBB(this.BatchTransform, this.BatchCollider.get_sharedMesh().get_bounds());
    if (Application.isLoading == null)
      return;
    ((Collider) this.BatchCollider).set_enabled(false);
  }

  public void Remove()
  {
    if (this.batchGroup == null)
      return;
    this.batchGroup.Invalidate();
    this.batchGroup.Remove(this);
    this.batchGroup = (ColliderGroup) null;
  }

  public void Refresh()
  {
    this.Remove();
    this.Add();
  }

  public void AddBatch(ColliderGroup batchGroup)
  {
    batchGroup.Add(this.batchInstance);
  }

  public ColliderBatch()
  {
    base.\u002Ector();
  }
}
