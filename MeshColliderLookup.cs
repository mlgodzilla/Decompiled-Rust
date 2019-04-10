// Decompiled with JetBrains decompiler
// Type: MeshColliderLookup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class MeshColliderLookup
{
  public MeshColliderLookup.LookupGroup src = new MeshColliderLookup.LookupGroup();
  public MeshColliderLookup.LookupGroup dst = new MeshColliderLookup.LookupGroup();

  public void Apply()
  {
    MeshColliderLookup.LookupGroup src = this.src;
    this.src = this.dst;
    this.dst = src;
    this.dst.Clear();
  }

  public void Add(MeshColliderInstance instance)
  {
    this.dst.Add(instance);
  }

  public MeshColliderLookup.LookupEntry Get(int index)
  {
    return this.src.Get(index);
  }

  public class LookupGroup
  {
    public List<MeshColliderLookup.LookupEntry> data = new List<MeshColliderLookup.LookupEntry>();
    public List<int> indices = new List<int>();

    public void Clear()
    {
      this.data.Clear();
      this.indices.Clear();
    }

    public void Add(MeshColliderInstance instance)
    {
      this.data.Add(new MeshColliderLookup.LookupEntry(instance));
      int num1 = this.data.Count - 1;
      int num2 = instance.data.triangles.Length / 3;
      for (int index = 0; index < num2; ++index)
        this.indices.Add(num1);
    }

    public MeshColliderLookup.LookupEntry Get(int index)
    {
      return this.data[this.indices[index]];
    }
  }

  public struct LookupEntry
  {
    public Transform transform;
    public Rigidbody rigidbody;
    public Collider collider;
    public OBB bounds;

    public LookupEntry(MeshColliderInstance instance)
    {
      this.transform = instance.transform;
      this.rigidbody = instance.rigidbody;
      this.collider = instance.collider;
      this.bounds = instance.bounds;
    }
  }
}
