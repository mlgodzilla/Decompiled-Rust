// Decompiled with JetBrains decompiler
// Type: UIEscapeCapture
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine.Events;

public class UIEscapeCapture : ListComponent<UIEscapeCapture>
{
  public UnityEvent onEscape;

  public static bool EscapePressed()
  {
    using (IEnumerator<UIEscapeCapture> enumerator = ((ListHashSet<UIEscapeCapture>) ListComponent<UIEscapeCapture>.InstanceList).GetEnumerator())
    {
      if (enumerator.MoveNext())
      {
        enumerator.Current.onEscape.Invoke();
        return true;
      }
    }
    return false;
  }

  public UIEscapeCapture()
  {
    base.\u002Ector();
  }
}
