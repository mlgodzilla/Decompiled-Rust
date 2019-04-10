// Decompiled with JetBrains decompiler
// Type: AnimalSkin
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class AnimalSkin : MonoBehaviour, IClientComponent
{
  public SkinnedMeshRenderer[] animalMesh;
  public AnimalMultiSkin[] animalSkins;
  private Model model;
  public bool dontRandomizeOnStart;

  private void Start()
  {
    this.model = (Model) ((Component) this).get_gameObject().GetComponent<Model>();
    if (this.dontRandomizeOnStart)
      return;
    this.ChangeSkin(Mathf.FloorToInt((float) Random.Range(0, this.animalSkins.Length)));
  }

  public void ChangeSkin(int iSkin)
  {
    if (this.animalSkins.Length == 0)
      return;
    iSkin = Mathf.Clamp(iSkin, 0, this.animalSkins.Length - 1);
    foreach (SkinnedMeshRenderer skinnedMeshRenderer in this.animalMesh)
    {
      Material[] sharedMaterials = ((Renderer) skinnedMeshRenderer).get_sharedMaterials();
      if (sharedMaterials != null)
      {
        for (int index = 0; index < sharedMaterials.Length; ++index)
          sharedMaterials[index] = this.animalSkins[iSkin].multiSkin[index];
        ((Renderer) skinnedMeshRenderer).set_sharedMaterials(sharedMaterials);
      }
    }
    if (!Object.op_Inequality((Object) this.model, (Object) null))
      return;
    this.model.skin = iSkin;
  }

  public AnimalSkin()
  {
    base.\u002Ector();
  }
}
