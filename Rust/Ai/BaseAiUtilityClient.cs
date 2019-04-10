// Decompiled with JetBrains decompiler
// Type: Rust.Ai.BaseAiUtilityClient
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.AI.Components;
using System;

namespace Rust.Ai
{
  public class BaseAiUtilityClient : UtilityAIClient
  {
    public BaseAiUtilityClient(Guid aiId, IContextProvider contextProvider)
    {
      base.\u002Ector(aiId, contextProvider);
    }

    public BaseAiUtilityClient(IUtilityAI ai, IContextProvider contextProvider)
    {
      base.\u002Ector(ai, contextProvider);
    }

    protected virtual void OnPause()
    {
    }

    protected virtual void OnResume()
    {
    }

    protected virtual void OnStart()
    {
    }

    protected virtual void OnStop()
    {
    }

    public void Initialize()
    {
      this.Start();
    }

    public void Kill()
    {
      this.Stop();
    }
  }
}
