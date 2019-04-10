// Decompiled with JetBrains decompiler
// Type: ConVar.Manifest
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;

namespace ConVar
{
  public class Manifest
  {
    [ClientVar]
    [ServerVar]
    public static object PrintManifest()
    {
      return (object) Application.Manifest;
    }

    [ClientVar]
    [ServerVar]
    public static object PrintManifestRaw()
    {
      return (object) Manifest.get_Contents();
    }
  }
}
