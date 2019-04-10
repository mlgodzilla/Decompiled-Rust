// Decompiled with JetBrains decompiler
// Type: ConstructionPlaceholder
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class ConstructionPlaceholder : PrefabAttribute, IPrefabPreProcess
{
  public Mesh mesh;
  public Material material;
  public bool renderer;
  public bool collider;

  protected override void AttributeSetup(
    GameObject rootObj,
    string name,
    bool serverside,
    bool clientside,
    bool bundling)
  {
    base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
    if (!clientside)
      return;
    if (this.renderer)
    {
      M0 component1 = rootObj.GetComponent<MeshFilter>();
      MeshRenderer component2 = (MeshRenderer) rootObj.GetComponent<MeshRenderer>();
      if (!Object.op_Implicit((Object) component1))
        ((MeshFilter) rootObj.AddComponent<MeshFilter>()).set_sharedMesh(this.mesh);
      if (!Object.op_Implicit((Object) component2))
      {
        MeshRenderer meshRenderer = (MeshRenderer) rootObj.AddComponent<MeshRenderer>();
        ((Renderer) meshRenderer).set_sharedMaterial(this.material);
        ((Renderer) meshRenderer).set_shadowCastingMode((ShadowCastingMode) 0);
      }
    }
    if (!this.collider || Object.op_Implicit((Object) rootObj.GetComponent<MeshCollider>()))
      return;
    ((MeshCollider) rootObj.AddComponent<MeshCollider>()).set_sharedMesh(this.mesh);
  }

  protected override System.Type GetIndexedType()
  {
    return typeof (ConstructionPlaceholder);
  }
}
