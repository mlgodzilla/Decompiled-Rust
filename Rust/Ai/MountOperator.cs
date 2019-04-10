// Decompiled with JetBrains decompiler
// Type: Rust.Ai.MountOperator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Serialization;
using ConVar;
using UnityEngine;

namespace Rust.Ai
{
  public class MountOperator : BaseAction
  {
    [ApexSerialization]
    public MountOperator.MountOperationType Type;

    public override void DoExecute(BaseContext c)
    {
      MountOperator.MountOperation(c as NPCHumanContext, this.Type);
    }

    public static void MountOperation(NPCHumanContext c, MountOperator.MountOperationType type)
    {
      switch (type)
      {
        case MountOperator.MountOperationType.Mount:
          if (c.GetFact(NPCPlayerApex.Facts.IsMounted) != (byte) 0 || AI.npc_ignore_chairs)
            break;
          BaseChair chairTarget = c.ChairTarget;
          if (!Object.op_Inequality((Object) chairTarget, (Object) null))
            break;
          c.Human.Mount((BaseMountable) chairTarget);
          break;
        case MountOperator.MountOperationType.Dismount:
          if (c.GetFact(NPCPlayerApex.Facts.IsMounted) != (byte) 1)
            break;
          c.Human.Dismount();
          break;
      }
    }

    public enum MountOperationType
    {
      Mount,
      Dismount,
    }
  }
}
