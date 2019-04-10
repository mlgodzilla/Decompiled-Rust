// Decompiled with JetBrains decompiler
// Type: DebrisEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using System;

public class DebrisEntity : BaseCombatEntity
{
  public override void ServerInit()
  {
    this.ResetRemovalTime();
    base.ServerInit();
  }

  public void RemoveCorpse()
  {
    this.Kill(BaseNetworkable.DestroyMode.None);
  }

  public void ResetRemovalTime(float dur)
  {
    using (TimeWarning.New(nameof (ResetRemovalTime), 0.1f))
    {
      if (this.IsInvoking(new Action(this.RemoveCorpse)))
        this.CancelInvoke(new Action(this.RemoveCorpse));
      this.Invoke(new Action(this.RemoveCorpse), dur);
    }
  }

  public float GetRemovalTime()
  {
    return Server.debrisdespawn;
  }

  public void ResetRemovalTime()
  {
    this.ResetRemovalTime(this.GetRemovalTime());
  }

  public override string Categorize()
  {
    return "debris";
  }
}
