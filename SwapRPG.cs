// Decompiled with JetBrains decompiler
// Type: SwapRPG
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class SwapRPG : MonoBehaviour
{
  public GameObject[] rpgModels;
  [NonSerialized]
  private string curAmmoType;

  public void SelectRPGType(int iType)
  {
    foreach (GameObject rpgModel in this.rpgModels)
      rpgModel.SetActive(false);
    this.rpgModels[iType].SetActive(true);
  }

  public void UpdateAmmoType(ItemDefinition ammoType)
  {
    if (this.curAmmoType == ammoType.shortname)
      return;
    this.curAmmoType = ammoType.shortname;
    string curAmmoType = this.curAmmoType;
    if (!(curAmmoType == "ammo.rocket.basic"))
    {
      if (!(curAmmoType == "ammo.rocket.fire"))
      {
        if (!(curAmmoType == "ammo.rocket.hv"))
        {
          if (curAmmoType == "ammo.rocket.smoke")
          {
            this.SelectRPGType(3);
            return;
          }
        }
        else
        {
          this.SelectRPGType(2);
          return;
        }
      }
      else
      {
        this.SelectRPGType(1);
        return;
      }
    }
    this.SelectRPGType(0);
  }

  private void Start()
  {
  }

  public SwapRPG()
  {
    base.\u002Ector();
  }

  public enum RPGType
  {
    One,
    Two,
    Three,
    Four,
  }
}
