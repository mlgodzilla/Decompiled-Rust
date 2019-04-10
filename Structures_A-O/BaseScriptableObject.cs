// Decompiled with JetBrains decompiler
// Type: BaseScriptableObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class BaseScriptableObject : ScriptableObject
{
  [HideInInspector]
  public uint FilenameStringId;

  public string LookupFileName()
  {
    return StringPool.Get(this.FilenameStringId);
  }

  public static bool operator ==(BaseScriptableObject a, BaseScriptableObject b)
  {
    if ((object) a == (object) b)
      return true;
    if ((object) a == null || (object) b == null)
      return false;
    return (int) a.FilenameStringId == (int) b.FilenameStringId;
  }

  public static bool operator !=(BaseScriptableObject a, BaseScriptableObject b)
  {
    return !(a == b);
  }

  public virtual int GetHashCode()
  {
    return (int) this.FilenameStringId;
  }

  public virtual bool Equals(object o)
  {
    if (o != null && (object) (o as BaseScriptableObject) != null)
      return o as BaseScriptableObject == this;
    return false;
  }

  public BaseScriptableObject()
  {
    base.\u002Ector();
  }
}
