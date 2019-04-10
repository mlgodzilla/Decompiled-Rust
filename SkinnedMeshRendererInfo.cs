// Decompiled with JetBrains decompiler
// Type: SkinnedMeshRendererInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Rendering;

public class SkinnedMeshRendererInfo : ComponentInfo<SkinnedMeshRenderer>
{
  public ShadowCastingMode shadows;
  public Material material;
  public Mesh mesh;
  public Bounds bounds;
  public Mesh cachedMesh;
  public SkinnedMeshRendererCache.RigInfo cachedRig;
  private Transform root;
  private Transform[] bones;

  public override void Reset()
  {
    if (Object.op_Equality((Object) this.component, (Object) null))
      return;
    ((Renderer) this.component).set_shadowCastingMode(this.shadows);
    if (Object.op_Implicit((Object) this.material))
      ((Renderer) this.component).set_sharedMaterial(this.material);
    this.component.set_sharedMesh(this.mesh);
    this.component.set_localBounds(this.bounds);
  }

  public override void Setup()
  {
    this.shadows = ((Renderer) this.component).get_shadowCastingMode();
    this.material = ((Renderer) this.component).get_sharedMaterial();
    this.mesh = this.component.get_sharedMesh();
    this.bounds = this.component.get_localBounds();
    this.RefreshCache();
  }

  private void RefreshCache()
  {
    if (!Object.op_Inequality((Object) this.cachedMesh, (Object) this.component.get_sharedMesh()))
      return;
    if (Object.op_Inequality((Object) this.cachedMesh, (Object) null))
      SkinnedMeshRendererCache.Add(this.cachedMesh, this.cachedRig);
    this.cachedMesh = this.component.get_sharedMesh();
    this.cachedRig = SkinnedMeshRendererCache.Get(this.component);
  }

  public void BuildRig()
  {
    this.RefreshCache();
    Vector3 position = ((Component) this).get_transform().get_position();
    Quaternion rotation = ((Component) this).get_transform().get_rotation();
    ((Component) this).get_transform().set_rotation(Quaternion.get_identity());
    ((Component) this).get_transform().set_position(Vector3.get_zero());
    Transform[] transformArray = new Transform[this.cachedRig.transforms.Length];
    for (int index = 0; index < this.cachedRig.transforms.Length; ++index)
    {
      GameObject gameObject = new GameObject(this.cachedRig.bones[index]);
      gameObject.get_transform().set_position(((Matrix4x4) ref this.cachedRig.transforms[index]).MultiplyPoint(Vector3.get_zero()));
      gameObject.get_transform().set_rotation(Quaternion.LookRotation(Vector4.op_Implicit(((Matrix4x4) ref this.cachedRig.transforms[index]).GetColumn(2)), Vector4.op_Implicit(((Matrix4x4) ref this.cachedRig.transforms[index]).GetColumn(1))));
      gameObject.get_transform().SetParent(((Component) this).get_transform(), true);
      transformArray[index] = gameObject.get_transform();
    }
    GameObject gameObject1 = new GameObject("root");
    gameObject1.get_transform().set_position(((Matrix4x4) ref this.cachedRig.rootTransform).MultiplyPoint(Vector3.get_zero()));
    gameObject1.get_transform().set_rotation(Quaternion.LookRotation(Vector4.op_Implicit(((Matrix4x4) ref this.cachedRig.rootTransform).GetColumn(2)), Vector4.op_Implicit(((Matrix4x4) ref this.cachedRig.rootTransform).GetColumn(1))));
    gameObject1.get_transform().SetParent(((Component) this).get_transform(), true);
    this.component.set_rootBone(gameObject1.get_transform());
    this.component.set_bones(transformArray);
    ((Component) this).get_transform().set_rotation(rotation);
    ((Component) this).get_transform().set_position(position);
  }
}
