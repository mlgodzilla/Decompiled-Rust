// Decompiled with JetBrains decompiler
// Type: BaseNPCContext
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Ai;

public class BaseNPCContext : BaseContext
{
  public NPCPlayerApex Human;
  public AiLocationManager AiLocationManager;

  public BaseNPCContext(IAIAgent agent)
    : base(agent)
  {
    this.Human = agent as NPCPlayerApex;
  }
}
