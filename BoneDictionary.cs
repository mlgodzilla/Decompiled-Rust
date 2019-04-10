// Decompiled with JetBrains decompiler
// Type: BoneDictionary
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class BoneDictionary
{
  private Dictionary<string, Transform> nameDict = new Dictionary<string, Transform>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  private Dictionary<int, Transform> hashDict = new Dictionary<int, Transform>();
  public Transform transform;
  public Transform[] transforms;
  public string[] names;

  public int Count
  {
    get
    {
      return this.transforms.Length;
    }
  }

  public BoneDictionary(Transform rootBone)
  {
    this.transform = rootBone;
    this.transforms = (Transform[]) ((Component) rootBone).GetComponentsInChildren<Transform>(true);
    this.names = new string[this.transforms.Length];
    for (int index = 0; index < this.transforms.Length; ++index)
    {
      Transform transform = this.transforms[index];
      if (Object.op_Inequality((Object) transform, (Object) null))
        this.names[index] = ((Object) transform).get_name();
    }
    this.BuildBoneDictionary();
  }

  public BoneDictionary(Transform rootBone, Transform[] boneTransforms, string[] boneNames)
  {
    this.transform = rootBone;
    this.transforms = boneTransforms;
    this.names = boneNames;
    this.BuildBoneDictionary();
  }

  private void BuildBoneDictionary()
  {
    for (int index = 0; index < this.transforms.Length; ++index)
    {
      Transform transform = this.transforms[index];
      string name = this.names[index];
      int hashCode = name.GetHashCode();
      if (!this.nameDict.ContainsKey(name))
        this.nameDict.Add(name, transform);
      if (!this.hashDict.ContainsKey(hashCode))
        this.hashDict.Add(hashCode, transform);
    }
  }

  public Transform FindBone(string name, bool defaultToRoot = true)
  {
    Transform transform = (Transform) null;
    if (this.nameDict.TryGetValue(name, out transform))
      return transform;
    if (!defaultToRoot)
      return (Transform) null;
    return this.transform;
  }

  public Transform FindBone(int hash, bool defaultToRoot = true)
  {
    Transform transform = (Transform) null;
    if (this.hashDict.TryGetValue(hash, out transform))
      return transform;
    if (!defaultToRoot)
      return (Transform) null;
    return this.transform;
  }
}
