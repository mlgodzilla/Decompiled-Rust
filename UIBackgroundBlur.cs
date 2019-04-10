// Decompiled with JetBrains decompiler
// Type: UIBackgroundBlur
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class UIBackgroundBlur : ListComponent<UIBackgroundBlur>, IClientComponent
{
  public float amount;

  public static float currentMax
  {
    get
    {
      if (((ListHashSet<UIBackgroundBlur>) ListComponent<UIBackgroundBlur>.InstanceList).get_Count() == 0)
        return 0.0f;
      float num = 0.0f;
      for (int index = 0; index < ((ListHashSet<UIBackgroundBlur>) ListComponent<UIBackgroundBlur>.InstanceList).get_Count(); ++index)
        num = Mathf.Max(((ListHashSet<UIBackgroundBlur>) ListComponent<UIBackgroundBlur>.InstanceList).get_Item(index).amount, num);
      return num;
    }
  }

  public UIBackgroundBlur()
  {
    base.\u002Ector();
  }
}
