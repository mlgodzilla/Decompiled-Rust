// Decompiled with JetBrains decompiler
// Type: DeployableDecay
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class DeployableDecay : Decay
{
  public float decayDelay = 8f;
  public float decayDuration = 8f;

  public override float GetDecayDelay(BaseEntity entity)
  {
    return (float) ((double) this.decayDelay * 60.0 * 60.0);
  }

  public override float GetDecayDuration(BaseEntity entity)
  {
    return (float) ((double) this.decayDuration * 60.0 * 60.0);
  }

  public override bool ShouldDecay(BaseEntity entity)
  {
    if (ConVar.Decay.upkeep)
      return true;
    return entity.IsOutside();
  }
}
