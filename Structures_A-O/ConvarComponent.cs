// Decompiled with JetBrains decompiler
// Type: ConvarComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using System;
using UnityEngine;

public class ConvarComponent : MonoBehaviour
{
  public bool runOnServer;
  public bool runOnClient;
  public System.Collections.Generic.List<ConvarComponent.ConvarEvent> List;

  protected void OnEnable()
  {
    if (!this.ShouldRun())
      return;
    foreach (ConvarComponent.ConvarEvent convarEvent in this.List)
      convarEvent.OnEnable();
  }

  protected void OnDisable()
  {
    if (Application.isQuitting != null || !this.ShouldRun())
      return;
    foreach (ConvarComponent.ConvarEvent convarEvent in this.List)
      convarEvent.OnDisable();
  }

  private bool ShouldRun()
  {
    return this.runOnServer;
  }

  public ConvarComponent()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class ConvarEvent
  {
    public string convar;
    public string on;
    public MonoBehaviour component;
    internal ConsoleSystem.Command cmd;

    public void OnEnable()
    {
      this.cmd = ConsoleSystem.Index.Client.Find(this.convar);
      if (this.cmd == null)
        this.cmd = ConsoleSystem.Index.Server.Find(this.convar);
      if (this.cmd == null)
        return;
      this.cmd.add_OnValueChanged(new Action<ConsoleSystem.Command>(this.cmd_OnValueChanged));
      this.cmd_OnValueChanged(this.cmd);
    }

    private void cmd_OnValueChanged(ConsoleSystem.Command obj)
    {
      if (Object.op_Equality((Object) this.component, (Object) null))
        return;
      bool flag = obj.get_String() == this.on;
      if (((Behaviour) this.component).get_enabled() == flag)
        return;
      ((Behaviour) this.component).set_enabled(flag);
    }

    public void OnDisable()
    {
      if (Application.isQuitting != null || this.cmd == null)
        return;
      this.cmd.remove_OnValueChanged(new Action<ConsoleSystem.Command>(this.cmd_OnValueChanged));
    }
  }
}
