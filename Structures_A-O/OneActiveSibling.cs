// Decompiled with JetBrains decompiler
// Type: OneActiveSibling
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class OneActiveSibling : MonoBehaviour
{
  [ComponentHelp("This component will disable all of its siblings when it becomes enabled. This can be useful in situations where you only ever want one of the children active - but don't want to manage turning each one off.")]
  private void OnEnable()
  {
    using (List<Transform>.Enumerator enumerator = ((Component) this).get_transform().GetSiblings<Transform>(false).GetEnumerator())
    {
      while (enumerator.MoveNext())
        ((Component) enumerator.Current).get_gameObject().SetActive(false);
    }
  }

  public OneActiveSibling()
  {
    base.\u002Ector();
  }
}
