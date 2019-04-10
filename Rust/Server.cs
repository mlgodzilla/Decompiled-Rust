// Decompiled with JetBrains decompiler
// Type: Rust.Server
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine.SceneManagement;

namespace Rust
{
  public static class Server
  {
    public const float UseDistance = 3f;
    private static Scene _entityScene;

    public static Scene EntityScene
    {
      get
      {
        if (!((Scene) ref Server._entityScene).IsValid())
          Server._entityScene = SceneManager.CreateScene("Server Entities");
        return Server._entityScene;
      }
    }
  }
}
