// Decompiled with JetBrains decompiler
// Type: ExplosionsShaderQueue
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ExplosionsShaderQueue : MonoBehaviour
{
  public int AddQueue;
  private Renderer rend;

  private void Start()
  {
    this.rend = (Renderer) ((Component) this).GetComponent<Renderer>();
    if (Object.op_Inequality((Object) this.rend, (Object) null))
    {
      Material sharedMaterial = this.rend.get_sharedMaterial();
      sharedMaterial.set_renderQueue(sharedMaterial.get_renderQueue() + this.AddQueue);
    }
    else
      this.Invoke("SetProjectorQueue", 0.1f);
  }

  private void SetProjectorQueue()
  {
    Material material = ((Projector) ((Component) this).GetComponent<Projector>()).get_material();
    material.set_renderQueue(material.get_renderQueue() + this.AddQueue);
  }

  private void OnDisable()
  {
    if (!Object.op_Inequality((Object) this.rend, (Object) null))
      return;
    this.rend.get_sharedMaterial().set_renderQueue(-1);
  }

  public ExplosionsShaderQueue()
  {
    base.\u002Ector();
  }
}
