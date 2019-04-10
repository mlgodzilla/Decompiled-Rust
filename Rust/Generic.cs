// Decompiled with JetBrains decompiler
// Type: Rust.Generic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine.SceneManagement;

namespace Rust
{
  public static class Generic
  {
    private static Scene _batchingScene;

    public static Scene BatchingScene
    {
      get
      {
        if (!((Scene) ref Generic._batchingScene).IsValid())
          Generic._batchingScene = SceneManager.CreateScene("Batching");
        return Generic._batchingScene;
      }
    }
  }
}
