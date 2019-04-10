// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.NPCTurret.NPCTurretMemory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;

namespace Rust.Ai.HTN.NPCTurret
{
  [Serializable]
  public class NPCTurretMemory : BaseNpcMemory
  {
    [NonSerialized]
    public NPCTurretContext NPCTurretContext;

    public override BaseNpcDefinition Definition
    {
      get
      {
        return this.NPCTurretContext.Body.AiDefinition;
      }
    }

    public NPCTurretMemory(NPCTurretContext context)
      : base((BaseNpcContext) context)
    {
      this.NPCTurretContext = context;
    }

    public override void ResetState()
    {
      base.ResetState();
    }
  }
}
