// Decompiled with JetBrains decompiler
// Type: NPCSensesLoadBalancer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.LoadBalancing;

public sealed class NPCSensesLoadBalancer : LoadBalancer
{
  public static readonly ILoadBalancer NpcSensesLoadBalancer = (ILoadBalancer) new LoadBalancedQueue(50, 0.1f, 50, 4);

  private NPCSensesLoadBalancer()
  {
    base.\u002Ector();
  }
}
