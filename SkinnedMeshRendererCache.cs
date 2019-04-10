// Decompiled with JetBrains decompiler
// Type: SkinnedMeshRendererCache
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SkinnedMeshRendererCache
{
  public static Dictionary<Mesh, SkinnedMeshRendererCache.RigInfo> dictionary = new Dictionary<Mesh, SkinnedMeshRendererCache.RigInfo>();

  public static void Add(Mesh mesh, SkinnedMeshRendererCache.RigInfo info)
  {
    if (SkinnedMeshRendererCache.dictionary.ContainsKey(mesh))
      return;
    SkinnedMeshRendererCache.dictionary.Add(mesh, info);
  }

  public static SkinnedMeshRendererCache.RigInfo Get(
    SkinnedMeshRenderer renderer)
  {
    SkinnedMeshRendererCache.RigInfo rigInfo;
    if (!SkinnedMeshRendererCache.dictionary.TryGetValue(renderer.get_sharedMesh(), out rigInfo))
    {
      rigInfo = new SkinnedMeshRendererCache.RigInfo();
      Transform rootBone = renderer.get_rootBone();
      Transform[] bones = renderer.get_bones();
      if (Object.op_Equality((Object) rootBone, (Object) null))
      {
        Debug.LogWarning((object) ("Renderer without a valid root bone: " + ((Object) renderer).get_name() + " " + ((Object) renderer.get_sharedMesh()).get_name()), (Object) ((Component) renderer).get_gameObject());
        return rigInfo;
      }
      ((Component) renderer).get_transform().set_position(Vector3.get_zero());
      ((Component) renderer).get_transform().set_rotation(Quaternion.get_identity());
      ((Component) renderer).get_transform().set_localScale(Vector3.get_one());
      rigInfo.root = ((Object) rootBone).get_name();
      rigInfo.rootHash = rigInfo.root.GetHashCode();
      rigInfo.bones = ((IEnumerable<Transform>) bones).Select<Transform, string>((Func<Transform, string>) (x => ((Object) x).get_name())).ToArray<string>();
      rigInfo.boneHashes = ((IEnumerable<string>) rigInfo.bones).Select<string, int>((Func<string, int>) (x => x.GetHashCode())).ToArray<int>();
      rigInfo.transforms = ((IEnumerable<Transform>) bones).Select<Transform, Matrix4x4>((Func<Transform, Matrix4x4>) (x => ((Component) x).get_transform().get_localToWorldMatrix())).ToArray<Matrix4x4>();
      rigInfo.rootTransform = ((Component) renderer.get_rootBone()).get_transform().get_localToWorldMatrix();
      SkinnedMeshRendererCache.dictionary.Add(renderer.get_sharedMesh(), rigInfo);
    }
    return rigInfo;
  }

  [Serializable]
  public class RigInfo
  {
    public string root;
    public int rootHash;
    public string[] bones;
    public int[] boneHashes;
    public Matrix4x4[] transforms;
    public Matrix4x4 rootTransform;
  }
}
