// Decompiled with JetBrains decompiler
// Type: ResourceRef`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public class ResourceRef<T> where T : Object
{
  public string guid;
  private Object _cachedObject;

  public bool isValid
  {
    get
    {
      return !string.IsNullOrEmpty(this.guid);
    }
  }

  public T Get()
  {
    if (Object.op_Equality(this._cachedObject, (Object) null))
      this._cachedObject = GameManifest.GUIDToObject(this.guid);
    return this._cachedObject as T;
  }

  public string resourcePath
  {
    get
    {
      return GameManifest.GUIDToPath(this.guid);
    }
  }

  public uint resourceID
  {
    get
    {
      return StringPool.Get(this.resourcePath);
    }
  }
}
