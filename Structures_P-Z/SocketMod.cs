// Decompiled with JetBrains decompiler
// Type: SocketMod
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;

public class SocketMod : PrefabAttribute
{
  [NonSerialized]
  public Socket_Base baseSocket;
  public Translate.Phrase FailedPhrase;

  public virtual bool DoCheck(Construction.Placement place)
  {
    return false;
  }

  public virtual void ModifyPlacement(Construction.Placement place)
  {
  }

  protected override System.Type GetIndexedType()
  {
    return typeof (SocketMod);
  }
}
