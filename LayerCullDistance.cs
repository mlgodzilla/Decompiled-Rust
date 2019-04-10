// Decompiled with JetBrains decompiler
// Type: LayerCullDistance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class LayerCullDistance : MonoBehaviour
{
  public string Layer;
  public float Distance;

  protected void OnEnable()
  {
    M0 component = ((Component) this).GetComponent<Camera>();
    float[] layerCullDistances = ((Camera) component).get_layerCullDistances();
    layerCullDistances[LayerMask.NameToLayer(this.Layer)] = this.Distance;
    ((Camera) component).set_layerCullDistances(layerCullDistances);
  }

  public LayerCullDistance()
  {
    base.\u002Ector();
  }
}
