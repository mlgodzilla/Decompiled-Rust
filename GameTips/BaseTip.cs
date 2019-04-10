// Decompiled with JetBrains decompiler
// Type: GameTips.BaseTip
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace GameTips
{
  public abstract class BaseTip
  {
    public abstract Translate.Phrase GetPhrase();

    public abstract bool ShouldShow { get; }

    public string Type
    {
      get
      {
        return this.GetType().Name;
      }
    }
  }
}
