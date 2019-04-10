// Decompiled with JetBrains decompiler
// Type: ProcessProceduralObjects
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class ProcessProceduralObjects : ProceduralComponent
{
  public override void Process(uint seed)
  {
    List<ProceduralObject> proceduralObjects = ((WorldSetup) SingletonComponent<WorldSetup>.Instance).ProceduralObjects;
    if (!World.Cached)
    {
      for (int index = 0; index < proceduralObjects.Count; ++index)
      {
        ProceduralObject proceduralObject = proceduralObjects[index];
        if (Object.op_Implicit((Object) proceduralObject))
          proceduralObject.Process();
      }
    }
    proceduralObjects.Clear();
  }

  public override bool RunOnCache
  {
    get
    {
      return true;
    }
  }
}
