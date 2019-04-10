// Decompiled with JetBrains decompiler
// Type: HairSet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Hair Set")]
public class HairSet : ScriptableObject
{
  public HairSet.MeshReplace[] MeshReplacements;
  public HairSet.MaterialReplace[] MaterialReplacements;

  public void Process(
    PlayerModelHair playerModelHair,
    HairDyeCollection dyeCollection,
    HairDye dye,
    MaterialPropertyBlock block)
  {
    List<SkinnedMeshRenderer> list = (List<SkinnedMeshRenderer>) Pool.GetList<SkinnedMeshRenderer>();
    ((Component) playerModelHair).get_gameObject().GetComponentsInChildren<SkinnedMeshRenderer>(true, (List<M0>) list);
    using (List<SkinnedMeshRenderer>.Enumerator enumerator = list.GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        SkinnedMeshRenderer current = enumerator.Current;
        if (!Object.op_Equality((Object) current.get_sharedMesh(), (Object) null) && !Object.op_Equality((Object) ((Renderer) current).get_sharedMaterial(), (Object) null))
        {
          string name = ((Object) current.get_sharedMesh()).get_name();
          ((Object) ((Renderer) current).get_sharedMaterial()).get_name();
          if (!((Component) current).get_gameObject().get_activeSelf())
            ((Component) current).get_gameObject().SetActive(true);
          for (int index = 0; index < this.MeshReplacements.Length; ++index)
          {
            if (this.MeshReplacements[index].Test(name))
            {
              SkinnedMeshRenderer replace = this.MeshReplacements[index].Replace;
              if (Object.op_Equality((Object) replace, (Object) null))
              {
                ((Component) current).get_gameObject().SetActive(false);
              }
              else
              {
                current.set_sharedMesh(replace.get_sharedMesh());
                current.set_rootBone(replace.get_rootBone());
                current.set_bones(this.MeshReplacements[index].GetBones());
              }
            }
          }
          PlayerModelHair.RendererMaterials rendererMaterials;
          if (playerModelHair.Materials.TryGetValue((Renderer) current, out rendererMaterials))
          {
            Array.Copy((Array) rendererMaterials.original, (Array) rendererMaterials.replacement, rendererMaterials.original.Length);
            for (int index1 = 0; index1 < rendererMaterials.original.Length; ++index1)
            {
              for (int index2 = 0; index2 < this.MaterialReplacements.Length; ++index2)
              {
                if (this.MaterialReplacements[index2].Test(rendererMaterials.names[index1]))
                  rendererMaterials.replacement[index1] = this.MaterialReplacements[index2].Replace;
              }
            }
            ((Renderer) current).set_sharedMaterials(rendererMaterials.replacement);
          }
          else
            Debug.LogWarning((object) ("[HairSet.Process] Missing cached renderer materials in " + ((Object) playerModelHair).get_name()));
          if (dye != null && ((Component) current).get_gameObject().get_activeSelf())
            dye.Apply(dyeCollection, block);
        }
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<SkinnedMeshRenderer>((List<M0>&) ref list);
  }

  public void ProcessMorphs(GameObject obj, int blendShapeIndex = -1)
  {
  }

  public HairSet()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class MeshReplace
  {
    [HideInInspector]
    public string FindName;
    public Mesh Find;
    public SkinnedMeshRenderer Replace;
    private Transform[] bones;

    public bool Test(string materialName)
    {
      return this.FindName == materialName;
    }

    public Transform[] GetBones()
    {
      if (this.bones == null)
        this.bones = this.Replace.get_bones();
      return this.bones;
    }
  }

  [Serializable]
  public class MaterialReplace
  {
    [HideInInspector]
    public string FindName;
    public Material Find;
    public Material Replace;

    public bool Test(string materialName)
    {
      return this.FindName == materialName;
    }
  }
}
