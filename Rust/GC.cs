// Decompiled with JetBrains decompiler
// Type: Rust.GC
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Rust
{
  public class GC : MonoBehaviour, IClientComponent
  {
    public static void Collect()
    {
      System.GC.Collect();
    }

    public GC()
    {
      base.\u002Ector();
    }
  }
}
