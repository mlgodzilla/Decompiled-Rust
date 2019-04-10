// Decompiled with JetBrains decompiler
// Type: NeedsMouseWheel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class NeedsMouseWheel : ListComponent<NeedsMouseWheel>
{
  public static bool AnyActive()
  {
    return ((ListHashSet<NeedsMouseWheel>) ListComponent<NeedsMouseWheel>.InstanceList).get_Count() > 0;
  }

  public NeedsMouseWheel()
  {
    base.\u002Ector();
  }
}
