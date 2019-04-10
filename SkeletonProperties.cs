// Decompiled with JetBrains decompiler
// Type: SkeletonProperties
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Skeleton Properties")]
public class SkeletonProperties : ScriptableObject
{
  public GameObject boneReference;
  [global::BoneProperty]
  public SkeletonProperties.BoneProperty[] bones;
  [NonSerialized]
  private Dictionary<uint, SkeletonProperties.BoneProperty> quickLookup;

  public void OnValidate()
  {
    if (Object.op_Equality((Object) this.boneReference, (Object) null))
    {
      Debug.LogWarning((object) "boneReference is null", (Object) this);
    }
    else
    {
      List<SkeletonProperties.BoneProperty> list = ((IEnumerable<SkeletonProperties.BoneProperty>) this.bones).ToList<SkeletonProperties.BoneProperty>();
      using (List<Transform>.Enumerator enumerator = this.boneReference.get_transform().GetAllChildren().GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Transform child = enumerator.Current;
          if (list.All<SkeletonProperties.BoneProperty>((Func<SkeletonProperties.BoneProperty, bool>) (x => Object.op_Inequality((Object) x.bone, (Object) ((Component) child).get_gameObject()))))
            list.Add(new SkeletonProperties.BoneProperty()
            {
              bone = ((Component) child).get_gameObject(),
              name = new Translate.Phrase("", "")
              {
                token = ((Object) child).get_name().ToLower(),
                english = ((Object) child).get_name().ToLower()
              }
            });
        }
      }
      this.bones = list.ToArray();
    }
  }

  private void BuildDictionary()
  {
    this.quickLookup = new Dictionary<uint, SkeletonProperties.BoneProperty>();
    foreach (SkeletonProperties.BoneProperty bone in this.bones)
    {
      uint key = StringPool.Get(((Object) bone.bone).get_name());
      if (!this.quickLookup.ContainsKey(key))
      {
        this.quickLookup.Add(key, bone);
      }
      else
      {
        string name1 = ((Object) bone.bone).get_name();
        string name2 = ((Object) this.quickLookup[key].bone).get_name();
        Debug.LogWarning((object) ("Duplicate bone id " + (object) key + " for " + name1 + " and " + name2));
      }
    }
  }

  public SkeletonProperties.BoneProperty FindBone(uint id)
  {
    if (this.quickLookup == null)
      this.BuildDictionary();
    SkeletonProperties.BoneProperty boneProperty = (SkeletonProperties.BoneProperty) null;
    if (!this.quickLookup.TryGetValue(id, out boneProperty))
      return (SkeletonProperties.BoneProperty) null;
    return boneProperty;
  }

  public SkeletonProperties()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class BoneProperty
  {
    public GameObject bone;
    public Translate.Phrase name;
    public HitArea area;
  }
}
