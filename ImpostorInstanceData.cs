// Decompiled with JetBrains decompiler
// Type: ImpostorInstanceData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ImpostorInstanceData
{
  public ImpostorBatch Batch;
  public int BatchIndex;
  private int hash;

  public Renderer Renderer { private set; get; }

  public Mesh Mesh { private set; get; }

  public Material Material { private set; get; }

  public ImpostorInstanceData(Renderer renderer, Mesh mesh, Material material)
  {
    this.Renderer = renderer;
    this.Mesh = mesh;
    this.Material = material;
    this.hash = this.GenerateHashCode();
    this.Update();
  }

  private int GenerateHashCode()
  {
    return (17 * 31 + ((object) this.Material).GetHashCode()) * 31 + ((object) this.Mesh).GetHashCode();
  }

  public override bool Equals(object obj)
  {
    ImpostorInstanceData impostorInstanceData = obj as ImpostorInstanceData;
    if (Object.op_Equality((Object) impostorInstanceData.Material, (Object) this.Material))
      return Object.op_Equality((Object) impostorInstanceData.Mesh, (Object) this.Mesh);
    return false;
  }

  public override int GetHashCode()
  {
    return this.hash;
  }

  public Vector4 PositionAndScale()
  {
    Transform transform = ((Component) this.Renderer).get_transform();
    Vector3 position = transform.get_position();
    Vector3 lossyScale = transform.get_lossyScale();
    float num = this.Renderer.get_enabled() ? (float) lossyScale.x : (float) -lossyScale.x;
    return new Vector4((float) position.x, (float) position.y, (float) position.z, num);
  }

  public void Update()
  {
    if (this.Batch == null)
      return;
    this.Batch.Positions[this.BatchIndex] = this.PositionAndScale();
  }
}
