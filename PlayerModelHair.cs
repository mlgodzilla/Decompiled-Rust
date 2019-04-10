// Decompiled with JetBrains decompiler
// Type: PlayerModelHair
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModelHair : MonoBehaviour
{
  public HairType type;
  private Dictionary<Renderer, PlayerModelHair.RendererMaterials> materials;

  public Dictionary<Renderer, PlayerModelHair.RendererMaterials> Materials
  {
    get
    {
      return this.materials;
    }
  }

  private void CacheOriginalMaterials()
  {
    if (this.materials != null)
      return;
    this.materials = new Dictionary<Renderer, PlayerModelHair.RendererMaterials>();
    List<SkinnedMeshRenderer> list = (List<SkinnedMeshRenderer>) Pool.GetList<SkinnedMeshRenderer>();
    ((Component) this).get_gameObject().GetComponentsInChildren<SkinnedMeshRenderer>(true, (List<M0>) list);
    this.materials.Clear();
    using (List<SkinnedMeshRenderer>.Enumerator enumerator = list.GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        SkinnedMeshRenderer current = enumerator.Current;
        this.materials.Add((Renderer) current, new PlayerModelHair.RendererMaterials((Renderer) current));
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<SkinnedMeshRenderer>((List<M0>&) ref list);
  }

  private void Setup(
    HairType type,
    HairSetCollection hair,
    int meshIndex,
    float typeNum,
    float dyeNum,
    MaterialPropertyBlock block)
  {
    this.CacheOriginalMaterials();
    HairSetCollection.HairSetEntry hairSetEntry = hair.Get(type, typeNum);
    if (Object.op_Equality((Object) hairSetEntry.HairSet, (Object) null))
    {
      Debug.LogWarning((object) "Hair.Get returned a NULL hair");
    }
    else
    {
      int blendShapeIndex = -1;
      if (type == HairType.Facial || type == HairType.Eyebrow)
        blendShapeIndex = meshIndex;
      HairDye dye = (HairDye) null;
      HairDyeCollection hairDyeCollection = hairSetEntry.HairDyeCollection;
      if (Object.op_Inequality((Object) hairDyeCollection, (Object) null))
        dye = hairDyeCollection.Get(dyeNum);
      hairSetEntry.HairSet.Process(this, hairDyeCollection, dye, block);
      hairSetEntry.HairSet.ProcessMorphs(((Component) this).get_gameObject(), blendShapeIndex);
    }
  }

  public void Setup(
    SkinSetCollection skin,
    float hairNum,
    float meshNum,
    MaterialPropertyBlock block)
  {
    int index = skin.GetIndex(meshNum);
    SkinSet skin1 = skin.Skins[index];
    if (Object.op_Equality((Object) skin1, (Object) null))
    {
      Debug.LogError((object) "Skin.Get returned a NULL skin");
    }
    else
    {
      int type = (int) this.type;
      float typeNum;
      float dyeNum;
      PlayerModelHair.GetRandomVariation(hairNum, type, index, out typeNum, out dyeNum);
      this.Setup(this.type, skin1.HairCollection, index, typeNum, dyeNum, block);
    }
  }

  public static void GetRandomVariation(
    float hairNum,
    int typeIndex,
    int meshIndex,
    out float typeNum,
    out float dyeNum)
  {
    int num = Mathf.FloorToInt(hairNum * 100000f);
    Random.InitState(num + typeIndex);
    typeNum = Random.Range(0.0f, 1f);
    Random.InitState(num + meshIndex);
    dyeNum = Random.Range(0.0f, 1f);
  }

  public PlayerModelHair()
  {
    base.\u002Ector();
  }

  public struct RendererMaterials
  {
    public string[] names;
    public Material[] original;
    public Material[] replacement;

    public RendererMaterials(Renderer r)
    {
      this.original = r.get_sharedMaterials();
      this.replacement = this.original.Clone() as Material[];
      this.names = new string[this.original.Length];
      for (int index = 0; index < this.original.Length; ++index)
        this.names[index] = ((Object) this.original[index]).get_name();
    }
  }
}
