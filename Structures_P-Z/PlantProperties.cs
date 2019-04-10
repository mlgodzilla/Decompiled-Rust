// Decompiled with JetBrains decompiler
// Type: PlantProperties
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Plant Properties")]
public class PlantProperties : ScriptableObject
{
  [ArrayIndexIsEnum(enumType = typeof (PlantProperties.State))]
  public PlantProperties.Stage[] stages;
  [Header("Metabolism")]
  public AnimationCurve timeOfDayHappiness;
  public AnimationCurve temperatureHappiness;
  public AnimationCurve fruitCurve;
  public int maxSeasons;
  public int maxHeldWater;
  public int lifetimeWaterConsumption;
  public float waterConsumptionLifetime;
  public int waterYieldBonus;
  [Header("Harvesting")]
  public BaseEntity.Menu.Option pickOption;
  public ItemDefinition pickupItem;
  public int pickupAmount;
  public GameObjectRef pickEffect;
  public int maxHarvests;
  public bool disappearAfterHarvest;
  [Header("Cloning")]
  public BaseEntity.Menu.Option cloneOption;
  public ItemDefinition cloneItem;
  public int maxClones;

  public PlantProperties()
  {
    base.\u002Ector();
  }

  public enum State
  {
    Seed,
    Seedling,
    Sapling,
    Mature,
    Fruiting,
    Dying,
  }

  [Serializable]
  public struct Stage
  {
    public PlantProperties.State nextState;
    public float lifeLength;
    public float health;
    public float resources;
    public GameObjectRef skinObject;
  }
}
