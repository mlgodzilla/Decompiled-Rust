// Decompiled with JetBrains decompiler
// Type: Model
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class Model : MonoBehaviour, IPrefabPreProcess
{
  public SphereCollider collision;
  public Transform rootBone;
  public Transform headBone;
  public Transform eyeBone;
  public Animator animator;
  [HideInInspector]
  public Transform[] boneTransforms;
  [HideInInspector]
  public string[] boneNames;
  internal BoneDictionary boneDict;
  internal int skin;

  protected void OnEnable()
  {
    this.skin = -1;
  }

  public void BuildBoneDictionary()
  {
    if (this.boneDict != null)
      return;
    this.boneDict = new BoneDictionary(((Component) this).get_transform(), this.boneTransforms, this.boneNames);
  }

  public int GetSkin()
  {
    return this.skin;
  }

  private Transform FindBoneInternal(string name)
  {
    this.BuildBoneDictionary();
    return this.boneDict.FindBone(name, false);
  }

  public Transform FindBone(string name)
  {
    this.BuildBoneDictionary();
    Transform rootBone = this.rootBone;
    if (string.IsNullOrEmpty(name))
      return rootBone;
    return this.boneDict.FindBone(name, true);
  }

  public Transform FindBone(int hash)
  {
    this.BuildBoneDictionary();
    Transform rootBone = this.rootBone;
    if (hash == 0)
      return rootBone;
    return this.boneDict.FindBone(hash, true);
  }

  public Transform FindClosestBone(Vector3 worldPos)
  {
    Transform transform = this.rootBone;
    float num1 = float.MaxValue;
    for (int index = 0; index < this.boneTransforms.Length; ++index)
    {
      Transform boneTransform = this.boneTransforms[index];
      if (!Object.op_Equality((Object) boneTransform, (Object) null))
      {
        float num2 = Vector3.Distance(boneTransform.get_position(), worldPos);
        if ((double) num2 < (double) num1)
        {
          transform = boneTransform;
          num1 = num2;
        }
      }
    }
    return transform;
  }

  public void PreProcess(
    IPrefabProcessor process,
    GameObject rootObj,
    string name,
    bool serverside,
    bool clientside,
    bool bundling)
  {
    if (Object.op_Equality((Object) this, (Object) null))
      return;
    if (Object.op_Equality((Object) this.animator, (Object) null))
      this.animator = (Animator) ((Component) this).GetComponent<Animator>();
    if (Object.op_Equality((Object) this.rootBone, (Object) null))
      this.rootBone = ((Component) this).get_transform();
    this.boneTransforms = (Transform[]) ((Component) this.rootBone).GetComponentsInChildren<Transform>(true);
    this.boneNames = new string[this.boneTransforms.Length];
    for (int index = 0; index < this.boneTransforms.Length; ++index)
      this.boneNames[index] = ((Object) this.boneTransforms[index]).get_name();
  }

  public Model()
  {
    base.\u002Ector();
  }
}
