// Decompiled with JetBrains decompiler
// Type: MeshPaintableSource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshPaintableSource : MonoBehaviour, IClientComponent
{
  public int texWidth;
  public int texHeight;
  public string replacementTextureName;
  public float cameraFOV;
  public float cameraDistance;
  [NonSerialized]
  public Texture2D texture;
  public GameObject sourceObject;
  public Mesh collisionMesh;
  public Vector3 localPosition;
  public Vector3 localRotation;
  private static MaterialPropertyBlock block;

  public void Init()
  {
    if (Object.op_Implicit((Object) this.texture))
      return;
    this.texture = new Texture2D(this.texWidth, this.texHeight, (TextureFormat) 5, false);
    ((Object) this.texture).set_name("MeshPaintableSource_" + ((Object) ((Component) this).get_gameObject()).get_name());
    this.texture.Clear(Color32.op_Implicit(Color.get_clear()));
    if (MeshPaintableSource.block == null)
      MeshPaintableSource.block = new MaterialPropertyBlock();
    else
      MeshPaintableSource.block.Clear();
    MeshPaintableSource.block.SetTexture(this.replacementTextureName, (Texture) this.texture);
    List<Renderer> list = (List<Renderer>) Pool.GetList<Renderer>();
    ((Component) ((Component) this).get_transform().get_root()).GetComponentsInChildren<Renderer>(true, (List<M0>) list);
    using (List<Renderer>.Enumerator enumerator = list.GetEnumerator())
    {
      while (enumerator.MoveNext())
        enumerator.Current.SetPropertyBlock(MeshPaintableSource.block);
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<Renderer>((List<M0>&) ref list);
  }

  public void Free()
  {
    if (!Object.op_Implicit((Object) this.texture))
      return;
    Object.Destroy((Object) this.texture);
    this.texture = (Texture2D) null;
  }

  public void UpdateFrom(Texture2D input)
  {
    this.Init();
    this.texture.SetPixels32(input.GetPixels32());
    this.texture.Apply(true, false);
  }

  public void Load(byte[] data)
  {
    this.Init();
    if (data == null)
      return;
    ImageConversion.LoadImage(this.texture, data);
    if (((Texture) this.texture).get_width() != this.texWidth || ((Texture) this.texture).get_height() != this.texHeight)
      this.texture.Resize(this.texWidth, this.texHeight);
    this.texture.Apply(true, false);
  }

  public void Clear()
  {
    if (Object.op_Equality((Object) this.texture, (Object) null))
      return;
    this.texture.Clear(Color32.op_Implicit(new Color(0.0f, 0.0f, 0.0f, 0.0f)));
    this.texture.Apply(true, false);
  }

  public MeshPaintableSource()
  {
    base.\u002Ector();
  }
}
