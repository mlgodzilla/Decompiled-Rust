// Decompiled with JetBrains decompiler
// Type: ProceduralComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public abstract class ProceduralComponent : MonoBehaviour
{
  [InspectorFlags]
  public ProceduralComponent.Realm Mode;
  public string Description;

  public virtual bool RunOnCache
  {
    get
    {
      return false;
    }
  }

  public bool ShouldRun()
  {
    return (!World.Cached || this.RunOnCache) && (this.Mode & ProceduralComponent.Realm.Server) != (ProceduralComponent.Realm) 0;
  }

  public abstract void Process(uint seed);

  protected ProceduralComponent()
  {
    base.\u002Ector();
  }

  public enum Realm
  {
    Client = 1,
    Server = 2,
  }
}
