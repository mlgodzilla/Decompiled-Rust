// Decompiled with JetBrains decompiler
// Type: SwapArrows
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using System;
using UnityEngine;

public class SwapArrows : MonoBehaviour, IClientComponent
{
  public GameObject[] arrowModels;
  [NonSerialized]
  private string curAmmoType;

  public void SelectArrowType(int iType)
  {
    this.HideAllArrowHeads();
    if (iType >= this.arrowModels.Length)
      return;
    this.arrowModels[iType].SetActive(true);
  }

  public void HideAllArrowHeads()
  {
    foreach (GameObject arrowModel in this.arrowModels)
      arrowModel.SetActive(false);
  }

  public void UpdateAmmoType(ItemDefinition ammoType)
  {
    if (this.curAmmoType == ammoType.shortname)
      return;
    this.curAmmoType = ammoType.shortname;
    string curAmmoType = this.curAmmoType;
    if (!(curAmmoType == "ammo_arrow"))
    {
      if (!(curAmmoType == "arrow.bone"))
      {
        if (!(curAmmoType == "arrow.fire"))
        {
          if (!(curAmmoType == "arrow.hv"))
          {
            if (!(curAmmoType == "ammo_arrow_poison"))
            {
              if (curAmmoType == "ammo_arrow_stone")
              {
                this.SelectArrowType(4);
                return;
              }
            }
            else
            {
              this.SelectArrowType(3);
              return;
            }
          }
          else
          {
            this.SelectArrowType(2);
            return;
          }
        }
        else
        {
          this.SelectArrowType(1);
          return;
        }
      }
      else
      {
        this.SelectArrowType(0);
        return;
      }
    }
    this.HideAllArrowHeads();
  }

  private void Cleanup()
  {
    this.HideAllArrowHeads();
    this.curAmmoType = "";
  }

  public void OnDisable()
  {
    if (Application.isQuitting != null)
      return;
    this.Cleanup();
  }

  public void OnEnable()
  {
    this.Cleanup();
  }

  public SwapArrows()
  {
    base.\u002Ector();
  }

  public enum ArrowType
  {
    One,
    Two,
    Three,
    Four,
  }
}
