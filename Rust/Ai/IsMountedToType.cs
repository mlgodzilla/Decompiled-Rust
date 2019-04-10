// Decompiled with JetBrains decompiler
// Type: Rust.Ai.IsMountedToType
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using UnityEngine;

namespace Rust.Ai
{
  public class IsMountedToType : BaseScorer
  {
    [ApexSerialization]
    public PlayerModel.MountPoses MountableType;

    public override float GetScore(BaseContext context)
    {
      return IsMountedToType.Test(context as NPCHumanContext, this.MountableType);
    }

    public static float Test(NPCHumanContext c, PlayerModel.MountPoses mountableType)
    {
      BaseMountable mounted = c.Human.GetMounted();
      return Object.op_Equality((Object) mounted, (Object) null) || mounted.mountPose != mountableType ? 0.0f : 1f;
    }
  }
}
