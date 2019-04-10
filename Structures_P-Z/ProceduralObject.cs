// Decompiled with JetBrains decompiler
// Type: ProceduralObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public abstract class ProceduralObject : MonoBehaviour
{
  protected void Awake()
  {
    if (Object.op_Equality((Object) SingletonComponent<WorldSetup>.Instance, (Object) null))
      return;
    if (((WorldSetup) SingletonComponent<WorldSetup>.Instance).ProceduralObjects == null)
      Debug.LogError((object) "WorldSetup.Instance.ProceduralObjects is null.", (Object) this);
    else
      ((WorldSetup) SingletonComponent<WorldSetup>.Instance).ProceduralObjects.Add(this);
  }

  public abstract void Process();

  protected ProceduralObject()
  {
    base.\u002Ector();
  }
}
