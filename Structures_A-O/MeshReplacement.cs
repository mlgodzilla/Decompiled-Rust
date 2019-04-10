// Decompiled with JetBrains decompiler
// Type: MeshReplacement
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class MeshReplacement : MonoBehaviour
{
  public SkinnedMeshRenderer Female;

  internal static void Process(GameObject go, bool IsFemale)
  {
    if (!IsFemale)
      return;
    foreach (MeshReplacement componentsInChild in (MeshReplacement[]) go.GetComponentsInChildren<MeshReplacement>(true))
    {
      M0 component = ((Component) componentsInChild).GetComponent<SkinnedMeshRenderer>();
      ((SkinnedMeshRenderer) component).set_sharedMesh(componentsInChild.Female.get_sharedMesh());
      ((SkinnedMeshRenderer) component).set_rootBone(componentsInChild.Female.get_rootBone());
      ((SkinnedMeshRenderer) component).set_bones(componentsInChild.Female.get_bones());
    }
  }

  public MeshReplacement()
  {
    base.\u002Ector();
  }
}
