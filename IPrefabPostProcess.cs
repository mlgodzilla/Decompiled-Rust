// Decompiled with JetBrains decompiler
// Type: IPrefabPostProcess
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public interface IPrefabPostProcess
{
  void PostProcess(
    IPrefabProcessor preProcess,
    GameObject rootObj,
    string name,
    bool serverside,
    bool clientside,
    bool bundling);
}
