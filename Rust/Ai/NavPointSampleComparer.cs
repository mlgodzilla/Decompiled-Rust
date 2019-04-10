// Decompiled with JetBrains decompiler
// Type: Rust.Ai.NavPointSampleComparer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
  public class NavPointSampleComparer : IComparer<NavPointSample>
  {
    public int Compare(NavPointSample a, NavPointSample b)
    {
      if (Mathf.Approximately(a.Score, b.Score))
        return 0;
      return (double) a.Score > (double) b.Score ? -1 : 1;
    }
  }
}
