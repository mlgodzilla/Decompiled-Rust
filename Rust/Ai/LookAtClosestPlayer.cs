// Decompiled with JetBrains decompiler
// Type: Rust.Ai.LookAtClosestPlayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace Rust.Ai
{
  public class LookAtClosestPlayer : BaseAction
  {
    public override void DoExecute(BaseContext context)
    {
      NPCHumanContext c = context as NPCHumanContext;
      if (c == null)
        return;
      LookAtClosestPlayer.Do(c);
    }

    public static void Do(NPCHumanContext c)
    {
      c.Human.LookAtEyes = c.ClosestPlayer.eyes;
    }
  }
}
