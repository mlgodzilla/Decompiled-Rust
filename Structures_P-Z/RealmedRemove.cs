// Decompiled with JetBrains decompiler
// Type: RealmedRemove
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RealmedRemove : MonoBehaviour, IPrefabPreProcess
{
  public GameObject[] removedFromClient;
  public Component[] removedComponentFromClient;
  public GameObject[] removedFromServer;
  public Component[] removedComponentFromServer;
  public Component[] doNotRemoveFromServer;
  public Component[] doNotRemoveFromClient;

  public void PreProcess(
    IPrefabProcessor process,
    GameObject rootObj,
    string name,
    bool serverside,
    bool clientside,
    bool bundling)
  {
    if (clientside)
    {
      foreach (Object @object in this.removedFromClient)
        Object.DestroyImmediate(@object, true);
      foreach (Object @object in this.removedComponentFromClient)
        Object.DestroyImmediate(@object, true);
    }
    if (serverside)
    {
      foreach (Object @object in this.removedFromServer)
        Object.DestroyImmediate(@object, true);
      foreach (Object @object in this.removedComponentFromServer)
        Object.DestroyImmediate(@object, true);
    }
    if (bundling)
      return;
    process.RemoveComponent((Component) this);
  }

  public bool ShouldDelete(Component comp, bool client, bool server)
  {
    return (!client || this.doNotRemoveFromClient == null || !((IEnumerable<Component>) this.doNotRemoveFromClient).Contains<Component>(comp)) && (!server || this.doNotRemoveFromServer == null || !((IEnumerable<Component>) this.doNotRemoveFromServer).Contains<Component>(comp));
  }

  public RealmedRemove()
  {
    base.\u002Ector();
  }
}
