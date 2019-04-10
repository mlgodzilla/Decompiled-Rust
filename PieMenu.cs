// Decompiled with JetBrains decompiler
// Type: PieMenu
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class PieMenu : UIBehaviour
{
  private static AnimationCurve easePunch = new AnimationCurve(new Keyframe[9]
  {
    new Keyframe(0.0f, 0.0f),
    new Keyframe(0.112586f, 0.9976035f),
    new Keyframe(0.3120486f, 0.01720615f),
    new Keyframe(0.4316337f, 0.1703068f),
    new Keyframe(0.5524869f, 0.03141804f),
    new Keyframe(0.6549395f, 0.002909959f),
    new Keyframe(0.770987f, 0.009817753f),
    new Keyframe(0.8838775f, 0.001939224f),
    new Keyframe(1f, 0.0f)
  });
  public static PieMenu Instance;
  public Image middleBox;
  public PieShape pieBackgroundBlur;
  public PieShape pieBackground;
  public PieShape pieSelection;
  public GameObject pieOptionPrefab;
  public GameObject optionsCanvas;
  public PieMenu.MenuOption[] options;
  public GameObject scaleTarget;
  public float sliceGaps;
  [Range(0.0f, 1f)]
  public float outerSize;
  [Range(0.0f, 1f)]
  public float innerSize;
  [Range(0.0f, 1f)]
  public float iconSize;
  [Range(0.0f, 360f)]
  public float startRadius;
  [Range(0.0f, 360f)]
  public float radiusSize;
  public Image middleImage;
  public Text middleTitle;
  public Text middleDesc;
  public Text middleRequired;
  public Color colorIconActive;
  public Color colorIconHovered;
  public Color colorIconDisabled;
  public Color colorBackgroundDisabled;
  public SoundDefinition clipOpen;
  public SoundDefinition clipCancel;
  public SoundDefinition clipChanged;
  public SoundDefinition clipSelected;
  public PieMenu.MenuOption defaultOption;
  private bool isClosing;
  private CanvasGroup canvasGroup;
  public bool IsOpen;
  internal PieMenu.MenuOption selectedOption;

  protected virtual void Start()
  {
    base.Start();
    PieMenu.Instance = this;
    this.canvasGroup = (CanvasGroup) ((Component) this).GetComponentInChildren<CanvasGroup>();
    this.canvasGroup.set_alpha(0.0f);
    this.canvasGroup.set_interactable(false);
    this.canvasGroup.set_blocksRaycasts(false);
    this.IsOpen = false;
    this.isClosing = true;
  }

  public void Clear()
  {
    this.options = new PieMenu.MenuOption[0];
  }

  public void AddOption(PieMenu.MenuOption option)
  {
    List<PieMenu.MenuOption> list = ((IEnumerable<PieMenu.MenuOption>) this.options).ToList<PieMenu.MenuOption>();
    list.Add(option);
    this.options = list.ToArray();
  }

  public void FinishAndOpen()
  {
    this.IsOpen = true;
    this.isClosing = false;
    this.SetDefaultOption();
    this.Rebuild();
    this.UpdateInteraction(false);
    this.PlayOpenSound();
    LeanTween.cancel(((Component) this).get_gameObject());
    LeanTween.cancel(this.scaleTarget);
    ((CanvasGroup) ((Component) this).GetComponent<CanvasGroup>()).set_alpha(0.0f);
    LeanTween.alphaCanvas((CanvasGroup) ((Component) this).GetComponent<CanvasGroup>(), 1f, 0.1f).setEase((LeanTweenType) 21);
    this.scaleTarget.get_transform().set_localScale(Vector3.op_Multiply(Vector3.get_one(), 1.5f));
    LeanTween.scale(this.scaleTarget, Vector3.get_one(), 0.1f).setEase((LeanTweenType) 24);
  }

  protected virtual void OnEnable()
  {
    base.OnEnable();
    this.Rebuild();
  }

  public void SetDefaultOption()
  {
    this.defaultOption = (PieMenu.MenuOption) null;
    for (int index = 0; index < this.options.Length; ++index)
    {
      if (!this.options[index].disabled)
      {
        if (this.defaultOption == null)
          this.defaultOption = this.options[index];
        if (this.options[index].selected)
        {
          this.defaultOption = this.options[index];
          break;
        }
      }
    }
  }

  public void PlayOpenSound()
  {
  }

  public void PlayCancelSound()
  {
  }

  public void Close(bool success = false)
  {
    if (this.isClosing)
      return;
    this.isClosing = true;
    NeedsCursor component = (NeedsCursor) ((Component) this).GetComponent<NeedsCursor>();
    if (Object.op_Inequality((Object) component, (Object) null))
      ((Behaviour) component).set_enabled(false);
    LeanTween.cancel(((Component) this).get_gameObject());
    LeanTween.cancel(this.scaleTarget);
    LeanTween.alphaCanvas((CanvasGroup) ((Component) this).GetComponent<CanvasGroup>(), 0.0f, 0.2f).setEase((LeanTweenType) 21);
    LeanTween.scale(this.scaleTarget, Vector3.op_Multiply(Vector3.get_one(), success ? 1.5f : 0.5f), 0.2f).setEase((LeanTweenType) 21);
    this.IsOpen = false;
  }

  private void Update()
  {
    if (!Application.get_isPlaying())
      this.Rebuild();
    if ((double) this.pieBackground.innerSize != (double) this.innerSize || (double) this.pieBackground.outerSize != (double) this.outerSize || ((double) this.pieBackground.startRadius != (double) this.startRadius || (double) this.pieBackground.endRadius != (double) this.startRadius + (double) this.radiusSize))
    {
      this.pieBackground.startRadius = this.startRadius;
      this.pieBackground.endRadius = this.startRadius + this.radiusSize;
      this.pieBackground.innerSize = this.innerSize;
      this.pieBackground.outerSize = this.outerSize;
      this.pieBackground.SetVerticesDirty();
    }
    this.UpdateInteraction(true);
    if (!this.IsOpen)
      return;
    CursorManager.HoldOpen(false);
    IngameMenuBackground.Enabled = true;
  }

  public void Rebuild()
  {
    this.options = ((IEnumerable<PieMenu.MenuOption>) this.options).OrderBy<PieMenu.MenuOption, int>((Func<PieMenu.MenuOption, int>) (x => x.order)).ToArray<PieMenu.MenuOption>();
    while (this.optionsCanvas.get_transform().get_childCount() > 0)
      GameManager.DestroyImmediate(((Component) this.optionsCanvas.get_transform().GetChild(0)).get_gameObject(), true);
    float sliceSize = this.radiusSize / (float) this.options.Length;
    for (int index = 0; index < this.options.Length; ++index)
    {
      GameObject gameObject = Instantiate.GameObject(this.pieOptionPrefab, (Transform) null);
      gameObject.get_transform().SetParent(this.optionsCanvas.get_transform(), false);
      this.options[index].option = (PieOption) gameObject.GetComponent<PieOption>();
      this.options[index].option.UpdateOption((float) ((double) this.startRadius + (double) index * (double) sliceSize - (double) sliceSize * 0.25), sliceSize, this.sliceGaps, this.options[index].name, this.outerSize, this.innerSize, this.iconSize, this.options[index].sprite);
    }
    this.selectedOption = (PieMenu.MenuOption) null;
  }

  public void UpdateInteraction(bool allowLerp = true)
  {
    if (this.isClosing)
      return;
    Vector3 vector3 = Vector3.op_Subtraction(Input.get_mousePosition(), new Vector3((float) Screen.get_width() / 2f, (float) Screen.get_height() / 2f, 0.0f));
    float num1 = Mathf.Atan2((float) vector3.x, (float) vector3.y) * 57.29578f;
    if ((double) num1 < 0.0)
      num1 += 360f;
    float num2 = this.radiusSize / (float) this.options.Length;
    for (int index = 0; index < this.options.Length; ++index)
    {
      float num3 = (float) ((double) this.startRadius + (double) index * (double) num2 + (double) num2 * 0.5 - (double) num2 * 0.25);
      if ((double) ((Vector3) ref vector3).get_magnitude() < 32.0 && this.options[index] == this.defaultOption || (double) ((Vector3) ref vector3).get_magnitude() >= 32.0 && (double) Mathf.Abs(Mathf.DeltaAngle(num1, num3)) < (double) num2 * 0.5)
      {
        if (allowLerp)
        {
          this.pieSelection.startRadius = Mathf.MoveTowardsAngle(this.pieSelection.startRadius, this.options[index].option.background.startRadius, Time.get_deltaTime() * Mathf.Abs((float) ((double) Mathf.DeltaAngle(this.pieSelection.startRadius, this.options[index].option.background.startRadius) * 30.0 + 10.0)));
          this.pieSelection.endRadius = Mathf.MoveTowardsAngle(this.pieSelection.endRadius, this.options[index].option.background.endRadius, Time.get_deltaTime() * Mathf.Abs((float) ((double) Mathf.DeltaAngle(this.pieSelection.endRadius, this.options[index].option.background.endRadius) * 30.0 + 10.0)));
        }
        else
        {
          this.pieSelection.startRadius = this.options[index].option.background.startRadius;
          this.pieSelection.endRadius = this.options[index].option.background.endRadius;
        }
        this.pieSelection.SetVerticesDirty();
        this.middleImage.set_sprite(this.options[index].sprite);
        this.middleTitle.set_text(this.options[index].name);
        this.middleDesc.set_text(this.options[index].desc);
        this.middleRequired.set_text("");
        string requirements = this.options[index].requirements;
        if (requirements != null)
          this.middleRequired.set_text(requirements.Replace("[e]", "<color=#CD412B>").Replace("[/e]", "</color>"));
        ((Graphic) this.options[index].option.imageIcon).set_color(this.colorIconHovered);
        if (this.selectedOption != this.options[index])
        {
          if (this.selectedOption != null && !this.options[index].disabled)
          {
            this.scaleTarget.get_transform().set_localScale(Vector3.get_one());
            LeanTween.scale(this.scaleTarget, Vector3.op_Multiply(Vector3.get_one(), 1.03f), 0.2f).setEase(PieMenu.easePunch);
          }
          this.selectedOption = this.options[index];
        }
      }
      else
        ((Graphic) this.options[index].option.imageIcon).set_color(this.colorIconActive);
      if (this.options[index].disabled)
      {
        ((Graphic) this.options[index].option.imageIcon).set_color(this.colorIconDisabled);
        this.options[index].option.background.set_color(this.colorBackgroundDisabled);
      }
    }
  }

  public bool DoSelect()
  {
    return true;
  }

  public PieMenu()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class MenuOption
  {
    public string name;
    public string desc;
    public string requirements;
    public Sprite sprite;
    public bool disabled;
    public int order;
    [NonSerialized]
    public Action<BasePlayer> action;
    [NonSerialized]
    public PieOption option;
    [NonSerialized]
    public bool selected;
  }
}
