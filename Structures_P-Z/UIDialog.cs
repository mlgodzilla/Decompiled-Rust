// Decompiled with JetBrains decompiler
// Type: UIDialog
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class UIDialog : ListComponent<UIDialog>
{
  public static bool isOpen
  {
    get
    {
      return ((ListHashSet<UIDialog>) ListComponent<UIDialog>.InstanceList).get_Count() > 0;
    }
  }

  public UIDialog()
  {
    base.\u002Ector();
  }
}
