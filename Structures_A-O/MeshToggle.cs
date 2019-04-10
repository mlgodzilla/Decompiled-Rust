// Decompiled with JetBrains decompiler
// Type: MeshToggle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class MeshToggle : MonoBehaviour
{
  public Mesh[] RendererMeshes;
  public Mesh[] ColliderMeshes;

  public void SwitchRenderer(int index)
  {
    if (this.RendererMeshes.Length == 0)
      return;
    MeshFilter component = (MeshFilter) ((Component) this).GetComponent<MeshFilter>();
    if (!Object.op_Implicit((Object) component))
      return;
    component.set_sharedMesh(this.RendererMeshes[Mathf.Clamp(index, 0, this.RendererMeshes.Length - 1)]);
  }

  public void SwitchRenderer(float factor)
  {
    this.SwitchRenderer(Mathf.RoundToInt(factor * (float) this.RendererMeshes.Length));
  }

  public void SwitchCollider(int index)
  {
    if (this.ColliderMeshes.Length == 0)
      return;
    MeshCollider component = (MeshCollider) ((Component) this).GetComponent<MeshCollider>();
    if (!Object.op_Implicit((Object) component))
      return;
    component.set_sharedMesh(this.ColliderMeshes[Mathf.Clamp(index, 0, this.ColliderMeshes.Length - 1)]);
  }

  public void SwitchCollider(float factor)
  {
    this.SwitchCollider(Mathf.RoundToInt(factor * (float) this.ColliderMeshes.Length));
  }

  public void SwitchAll(int index)
  {
    this.SwitchRenderer(index);
    this.SwitchCollider(index);
  }

  public void SwitchAll(float factor)
  {
    this.SwitchRenderer(factor);
    this.SwitchCollider(factor);
  }

  public MeshToggle()
  {
    base.\u002Ector();
  }
}
