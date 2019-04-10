// Decompiled with JetBrains decompiler
// Type: ExplosionDemoReactivator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ExplosionDemoReactivator : MonoBehaviour
{
  public float TimeDelayToReactivate;

  private void Start()
  {
    this.InvokeRepeating("Reactivate", 0.0f, this.TimeDelayToReactivate);
  }

  private void Reactivate()
  {
    foreach (Transform componentsInChild in (Transform[]) ((Component) this).GetComponentsInChildren<Transform>())
    {
      ((Component) componentsInChild).get_gameObject().SetActive(false);
      ((Component) componentsInChild).get_gameObject().SetActive(true);
    }
  }

  public ExplosionDemoReactivator()
  {
    base.\u002Ector();
  }
}
