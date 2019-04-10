// Decompiled with JetBrains decompiler
// Type: Rust.Ai.AiManagedAgent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Rust.Ai
{
  [DefaultExecutionOrder(-102)]
  public class AiManagedAgent : FacepunchBehaviour, IServerComponent
  {
    [Tooltip("TODO: Replace with actual agent type id on the NavMeshAgent when we upgrade to 5.6.1 or above.")]
    public int AgentTypeIndex;
    [ReadOnly]
    public Vector2i NavmeshGridCoord;
    private IAIAgent agent;
    private bool isRegistered;

    private void OnEnable()
    {
      this.isRegistered = false;
      if (Object.op_Equality((Object) SingletonComponent<AiManager>.Instance, (Object) null) || !((Behaviour) SingletonComponent<AiManager>.Instance).get_enabled() || AiManager.nav_disable)
      {
        ((Behaviour) this).set_enabled(false);
      }
      else
      {
        this.agent = (IAIAgent) ((Component) this).GetComponent<IAIAgent>();
        if (this.agent == null)
          return;
        if (this.agent.Entity.isClient)
        {
          ((Behaviour) this).set_enabled(false);
        }
        else
        {
          this.agent.AgentTypeIndex = this.AgentTypeIndex;
          this.Invoke(new Action(this.DelayedRegistration), SeedRandom.Value((uint) Mathf.Abs(((Object) this).GetInstanceID())) * 3f);
        }
      }
    }

    private void DelayedRegistration()
    {
      if (this.isRegistered)
        return;
      ((AiManager) SingletonComponent<AiManager>.Instance).Add(this.agent);
      this.isRegistered = true;
    }

    private void OnDisable()
    {
      if (Application.isQuitting != null || Object.op_Equality((Object) SingletonComponent<AiManager>.Instance, (Object) null) || (!((Behaviour) SingletonComponent<AiManager>.Instance).get_enabled() || this.agent == null) || (Object.op_Equality((Object) this.agent.Entity, (Object) null) || this.agent.Entity.isClient || !this.isRegistered))
        return;
      ((AiManager) SingletonComponent<AiManager>.Instance).Remove(this.agent);
    }

    public AiManagedAgent()
    {
      base.\u002Ector();
    }
  }
}
