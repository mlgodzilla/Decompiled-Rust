// Decompiled with JetBrains decompiler
// Type: CreateEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class CreateEffect : MonoBehaviour
{
  public GameObjectRef EffectToCreate;

  public void OnEnable()
  {
    Effect.client.Run(this.EffectToCreate.resourcePath, ((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_up(), ((Component) this).get_transform().get_forward());
  }

  public CreateEffect()
  {
    base.\u002Ector();
  }
}
