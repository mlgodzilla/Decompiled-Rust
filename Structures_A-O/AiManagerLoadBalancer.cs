// Decompiled with JetBrains decompiler
// Type: AiManagerLoadBalancer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.LoadBalancing;

public sealed class AiManagerLoadBalancer : LoadBalancer
{
  public static readonly ILoadBalancer aiManagerLoadBalancer = (ILoadBalancer) new LoadBalancedQueue(1, 2.5f, 1, 4);

  private AiManagerLoadBalancer()
  {
    base.\u002Ector();
  }
}
