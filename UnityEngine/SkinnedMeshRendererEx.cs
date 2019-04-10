// Decompiled with JetBrains decompiler
// Type: UnityEngine.SkinnedMeshRendererEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace UnityEngine
{
  public static class SkinnedMeshRendererEx
  {
    public static Transform FindRig(this SkinnedMeshRenderer renderer)
    {
      Transform parent = ((Component) renderer).get_transform().get_parent();
      Transform transform = renderer.get_rootBone();
      while (Object.op_Inequality((Object) transform, (Object) null) && Object.op_Inequality((Object) transform.get_parent(), (Object) null) && Object.op_Inequality((Object) transform.get_parent(), (Object) parent))
        transform = transform.get_parent();
      return transform;
    }
  }
}
