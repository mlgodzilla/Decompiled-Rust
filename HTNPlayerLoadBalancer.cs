// Decompiled with JetBrains decompiler
// Type: HTNPlayerLoadBalancer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.LoadBalancing;

public sealed class HTNPlayerLoadBalancer : LoadBalancer
{
  public static readonly ILoadBalancer HTNPlayerBalancer = (ILoadBalancer) new LoadBalancedQueue(50, 0.1f, 50, 4);

  private HTNPlayerLoadBalancer()
  {
    base.\u002Ector();
  }
}
