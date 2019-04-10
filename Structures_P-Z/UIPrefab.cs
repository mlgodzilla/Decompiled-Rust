// Decompiled with JetBrains decompiler
// Type: UIPrefab
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using UnityEngine;

public class UIPrefab : MonoBehaviour
{
  public GameObject prefabSource;
  internal GameObject createdGameObject;

  private void Awake()
  {
    if (Object.op_Equality((Object) this.prefabSource, (Object) null) || Object.op_Inequality((Object) this.createdGameObject, (Object) null))
      return;
    this.createdGameObject = Instantiate.GameObject(this.prefabSource, (Transform) null);
    ((Object) this.createdGameObject).set_name(((Object) this.prefabSource).get_name());
    this.createdGameObject.get_transform().SetParent(((Component) this).get_transform(), false);
    this.createdGameObject.Identity();
  }

  public void SetVisible(bool visible)
  {
    if (Object.op_Equality((Object) this.createdGameObject, (Object) null) || this.createdGameObject.get_activeSelf() == visible)
      return;
    this.createdGameObject.SetActive(visible);
  }

  public UIPrefab()
  {
    base.\u002Ector();
  }
}
