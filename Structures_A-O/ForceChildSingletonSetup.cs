// Decompiled with JetBrains decompiler
// Type: ForceChildSingletonSetup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ForceChildSingletonSetup : MonoBehaviour
{
  [ComponentHelp("Any child objects of this object that contain SingletonComponents will be registered - even if they're not enabled")]
  private void Awake()
  {
    foreach (SingletonComponent componentsInChild in (SingletonComponent[]) ((Component) this).GetComponentsInChildren<SingletonComponent>(true))
      componentsInChild.Setup();
  }

  public ForceChildSingletonSetup()
  {
    base.\u002Ector();
  }
}
