// Decompiled with JetBrains decompiler
// Type: PooledList`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System.Collections.Generic;

public class PooledList<T>
{
  public List<T> data;

  public void Alloc()
  {
    if (this.data != null)
      return;
    this.data = (List<T>) Pool.GetList<T>();
  }

  public void Free()
  {
    if (this.data == null)
      return;
    // ISSUE: cast to a reference type
    Pool.FreeList<T>((List<M0>&) ref this.data);
  }

  public void Clear()
  {
    if (this.data == null)
      return;
    this.data.Clear();
  }
}
