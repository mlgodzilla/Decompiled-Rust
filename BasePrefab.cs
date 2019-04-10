// Decompiled with JetBrains decompiler
// Type: BasePrefab
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class BasePrefab : BaseMonoBehaviour, IPrefabPreProcess
{
  [HideInInspector]
  public uint prefabID;
  [HideInInspector]
  public bool isClient;

  public bool isServer
  {
    get
    {
      return !this.isClient;
    }
  }

  public virtual void PreProcess(
    IPrefabProcessor preProcess,
    GameObject rootObj,
    string name,
    bool serverside,
    bool clientside,
    bool bundling)
  {
    this.prefabID = StringPool.Get(name);
    this.isClient = clientside;
  }
}
