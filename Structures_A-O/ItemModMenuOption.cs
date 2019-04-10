// Decompiled with JetBrains decompiler
// Type: ItemModMenuOption
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ItemModMenuOption : ItemMod
{
  [Tooltip("If true, this is the command that will run when an item is 'selected' on the toolbar")]
  public bool isPrimaryOption = true;
  public string commandName;
  public ItemMod actionTarget;
  public BaseEntity.Menu.Option option;

  public override void ServerCommand(Item item, string command, BasePlayer player)
  {
    if (command != this.commandName || !this.actionTarget.CanDoAction(item, player))
      return;
    this.actionTarget.DoAction(item, player);
  }

  private void OnValidate()
  {
    if (Object.op_Equality((Object) this.actionTarget, (Object) null))
      Debug.LogWarning((object) "ItemModMenuOption: actionTarget is null!", (Object) ((Component) this).get_gameObject());
    if (string.IsNullOrEmpty(this.commandName))
      Debug.LogWarning((object) "ItemModMenuOption: commandName can't be empty!", (Object) ((Component) this).get_gameObject());
    if (!Object.op_Equality((Object) this.option.icon, (Object) null))
      return;
    Debug.LogWarning((object) ("No icon set for ItemModMenuOption " + ((Object) ((Component) this).get_gameObject()).get_name()), (Object) ((Component) this).get_gameObject());
  }
}
