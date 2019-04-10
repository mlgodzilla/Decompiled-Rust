// Decompiled with JetBrains decompiler
// Type: VendingMachineScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class VendingMachineScreen : MonoBehaviour
{
  public RawImage largeIcon;
  public RawImage blueprintIcon;
  public Text mainText;
  public Text lowerText;
  public Text centerText;
  public RawImage smallIcon;
  public VendingMachine vendingMachine;
  public Sprite outOfStockSprite;
  public Renderer fadeoutMesh;
  public CanvasGroup screenCanvas;
  public Renderer light1;
  public Renderer light2;
  public float nextImageTime;
  public int currentImageIndex;

  public VendingMachineScreen()
  {
    base.\u002Ector();
  }

  public enum vmScreenState
  {
    ItemScroll,
    Vending,
    Message,
    ShopName,
    OutOfStock,
  }
}
