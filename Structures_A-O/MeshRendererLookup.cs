// Decompiled with JetBrains decompiler
// Type: MeshRendererLookup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class MeshRendererLookup
{
  public MeshRendererLookup.LookupGroup src = new MeshRendererLookup.LookupGroup();
  public MeshRendererLookup.LookupGroup dst = new MeshRendererLookup.LookupGroup();

  public void Apply()
  {
    MeshRendererLookup.LookupGroup src = this.src;
    this.src = this.dst;
    this.dst = src;
    this.dst.Clear();
  }

  public void Clear()
  {
    this.dst.Clear();
  }

  public void Add(MeshRendererInstance instance)
  {
    this.dst.Add(instance);
  }

  public MeshRendererLookup.LookupEntry Get(int index)
  {
    return this.src.Get(index);
  }

  public class LookupGroup
  {
    public List<MeshRendererLookup.LookupEntry> data = new List<MeshRendererLookup.LookupEntry>();

    public void Clear()
    {
      this.data.Clear();
    }

    public void Add(MeshRendererInstance instance)
    {
      this.data.Add(new MeshRendererLookup.LookupEntry(instance));
    }

    public MeshRendererLookup.LookupEntry Get(int index)
    {
      return this.data[index];
    }
  }

  public struct LookupEntry
  {
    public Renderer renderer;

    public LookupEntry(MeshRendererInstance instance)
    {
      this.renderer = instance.renderer;
    }
  }
}
