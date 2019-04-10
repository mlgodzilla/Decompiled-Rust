// Decompiled with JetBrains decompiler
// Type: MeshRendererInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Rendering;

public class MeshRendererInfo : ComponentInfo<MeshRenderer>
{
  public ShadowCastingMode shadows;
  public Material material;
  public Mesh mesh;

  public override void Reset()
  {
    ((Renderer) this.component).set_shadowCastingMode(this.shadows);
    if (Object.op_Implicit((Object) this.material))
      ((Renderer) this.component).set_sharedMaterial(this.material);
    ((MeshFilter) ((Component) this.component).GetComponent<MeshFilter>()).set_sharedMesh(this.mesh);
  }

  public override void Setup()
  {
    this.shadows = ((Renderer) this.component).get_shadowCastingMode();
    this.material = ((Renderer) this.component).get_sharedMaterial();
    this.mesh = ((MeshFilter) ((Component) this.component).GetComponent<MeshFilter>()).get_sharedMesh();
  }
}
