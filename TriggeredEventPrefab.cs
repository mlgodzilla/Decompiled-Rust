// Decompiled with JetBrains decompiler
// Type: TriggeredEventPrefab
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class TriggeredEventPrefab : TriggeredEvent
{
  public GameObjectRef targetPrefab;

  private void RunEvent()
  {
    Debug.Log((object) ("[event] " + this.targetPrefab.resourcePath));
    BaseEntity entity = GameManager.server.CreateEntity(this.targetPrefab.resourcePath, (Vector3) null, (Quaternion) null, true);
    if (!Object.op_Implicit((Object) entity))
      return;
    ((Component) entity).SendMessage("TriggeredEventSpawn", (SendMessageOptions) 1);
    entity.Spawn();
  }
}
