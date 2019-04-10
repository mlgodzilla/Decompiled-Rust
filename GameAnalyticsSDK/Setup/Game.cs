// Decompiled with JetBrains decompiler
// Type: GameAnalyticsSDK.Setup.Game
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace GameAnalyticsSDK.Setup
{
  public class Game
  {
    public string Name { get; private set; }

    public int ID { get; private set; }

    public string GameKey { get; private set; }

    public string SecretKey { get; private set; }

    public Game(string name, int id, string gameKey, string secretKey)
    {
      this.Name = name;
      this.ID = id;
      this.GameKey = gameKey;
      this.SecretKey = secretKey;
    }
  }
}
