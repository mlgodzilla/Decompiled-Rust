// Decompiled with JetBrains decompiler
// Type: GameObjectUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public static class GameObjectUtil
{
  public static void GlobalBroadcast(string messageName, object param = null)
  {
    foreach (Component rootObject in TransformUtil.GetRootObjects())
      rootObject.BroadcastMessage(messageName, param, (SendMessageOptions) 1);
  }
}
