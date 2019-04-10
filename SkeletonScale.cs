// Decompiled with JetBrains decompiler
// Type: SkeletonScale
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class SkeletonScale : MonoBehaviour
{
  protected BoneInfoComponent[] bones;
  public int seed;
  public GameObject leftShoulder;
  public GameObject rightShoulder;
  public GameObject spine;

  protected void Awake()
  {
    this.bones = (BoneInfoComponent[]) ((Component) this).GetComponentsInChildren<BoneInfoComponent>(true);
  }

  public void UpdateBones(int seedNumber)
  {
    this.seed = seedNumber;
    foreach (BoneInfoComponent bone in this.bones)
    {
      if (!Vector3.op_Equality(bone.sizeVariation, Vector3.get_zero()))
      {
        Random.State state = Random.get_state();
        Random.InitState(bone.sizeVariationSeed + this.seed);
        ((Component) bone).get_transform().set_localScale(Vector3.op_Addition(Vector3.get_one(), Vector3.op_Multiply(bone.sizeVariation, Random.Range(-1f, 1f))));
        Random.set_state(state);
      }
    }
    if (!Object.op_Inequality((Object) this.spine, (Object) null))
      return;
    Transform transform1 = this.rightShoulder.get_transform();
    Transform transform2 = this.leftShoulder.get_transform();
    Vector3 vector3_1;
    ((Vector3) ref vector3_1).\u002Ector((float) (1.0 / this.spine.get_transform().get_localScale().x), (float) (1.0 / this.spine.get_transform().get_localScale().y), (float) (1.0 / this.spine.get_transform().get_localScale().z));
    Vector3 vector3_2 = vector3_1;
    transform2.set_localScale(vector3_2);
    Vector3 vector3_3 = vector3_1;
    transform1.set_localScale(vector3_3);
  }

  public void Reset()
  {
    foreach (BoneInfoComponent bone in this.bones)
    {
      if (!Vector3.op_Equality(bone.sizeVariation, Vector3.get_zero()))
        ((Component) bone).get_transform().set_localScale(Vector3.get_one());
    }
  }

  public SkeletonScale()
  {
    base.\u002Ector();
  }
}
