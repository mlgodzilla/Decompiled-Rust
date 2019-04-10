// Decompiled with JetBrains decompiler
// Type: SkinSet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Skin Set")]
public class SkinSet : ScriptableObject
{
  public string Label;
  public SkinSet.MeshReplace[] MeshReplacements;
  public SkinSet.MaterialReplace[] MaterialReplacements;
  public Gradient SkinColour;
  public HairSetCollection HairCollection;

  public void Process(GameObject obj, float Seed)
  {
    List<SkinnedMeshRenderer> list = (List<SkinnedMeshRenderer>) Pool.GetList<SkinnedMeshRenderer>();
    obj.GetComponentsInChildren<SkinnedMeshRenderer>(true, (List<M0>) list);
    using (List<SkinnedMeshRenderer>.Enumerator enumerator = list.GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        SkinnedMeshRenderer current = enumerator.Current;
        if (!Object.op_Equality((Object) current.get_sharedMesh(), (Object) null) && !Object.op_Equality((Object) ((Renderer) current).get_sharedMaterial(), (Object) null))
        {
          string name1 = ((Object) current.get_sharedMesh()).get_name();
          string name2 = ((Object) ((Renderer) current).get_sharedMaterial()).get_name();
          for (int index = 0; index < this.MeshReplacements.Length; ++index)
          {
            if (this.MeshReplacements[index].Test(name1))
            {
              SkinnedMeshRenderer skinnedMeshRenderer = this.MeshReplacements[index].Get(Seed);
              current.set_sharedMesh(skinnedMeshRenderer.get_sharedMesh());
              current.set_rootBone(skinnedMeshRenderer.get_rootBone());
              current.set_bones(skinnedMeshRenderer.get_bones());
            }
          }
          for (int index = 0; index < this.MaterialReplacements.Length; ++index)
          {
            if (this.MaterialReplacements[index].Test(name2))
              ((Renderer) current).set_sharedMaterial(this.MaterialReplacements[index].Get(Seed));
          }
        }
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<SkinnedMeshRenderer>((List<M0>&) ref list);
  }

  internal Color GetSkinColor(float skinNumber)
  {
    return this.SkinColour.Evaluate(skinNumber);
  }

  public SkinSet()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class MeshReplace
  {
    [HideInInspector]
    public string FindName;
    public Mesh Find;
    public SkinnedMeshRenderer[] Replace;

    public SkinnedMeshRenderer Get(float MeshNumber)
    {
      return this.Replace[Mathf.Clamp(Mathf.FloorToInt(MeshNumber * (float) this.Replace.Length), 0, this.Replace.Length - 1)];
    }

    public bool Test(string materialName)
    {
      return this.FindName == materialName;
    }
  }

  [Serializable]
  public class MaterialReplace
  {
    [HideInInspector]
    public string FindName;
    public Material Find;
    public Material[] Replace;

    public Material Get(float MeshNumber)
    {
      return this.Replace[Mathf.Clamp(Mathf.FloorToInt(MeshNumber * (float) this.Replace.Length), 0, this.Replace.Length - 1)];
    }

    public bool Test(string materialName)
    {
      return this.FindName == materialName;
    }
  }
}
