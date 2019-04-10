// Decompiled with JetBrains decompiler
// Type: GameAnalyticsSDK.Validators.GAValidator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using GameAnalyticsSDK.State;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GameAnalyticsSDK.Validators
{
  internal static class GAValidator
  {
    public static bool StringMatch(string s, string pattern)
    {
      if (s == null || pattern == null)
        return false;
      return Regex.IsMatch(s, pattern);
    }

    public static bool ValidateBusinessEvent(
      string currency,
      int amount,
      string cartType,
      string itemType,
      string itemId)
    {
      if (!GAValidator.ValidateCurrency(currency))
      {
        Debug.Log((object) ("Validation fail - business event - currency: Cannot be (null) and need to be A-Z, 3 characters and in the standard at openexchangerates.org. Failed currency: " + currency));
        return false;
      }
      if (!GAValidator.ValidateShortString(cartType, true))
      {
        Debug.Log((object) ("Validation fail - business event - cartType. Cannot be above 32 length. String: " + cartType));
        return false;
      }
      if (!GAValidator.ValidateEventPartLength(itemType, false))
      {
        Debug.Log((object) ("Validation fail - business event - itemType: Cannot be (null), empty or above 64 characters. String: " + itemType));
        return false;
      }
      if (!GAValidator.ValidateEventPartCharacters(itemType))
      {
        Debug.Log((object) ("Validation fail - business event - itemType: Cannot contain other characters than A-z, 0-9, -_., ()!?. String: " + itemType));
        return false;
      }
      if (!GAValidator.ValidateEventPartLength(itemId, false))
      {
        Debug.Log((object) ("Validation fail - business event - itemId. Cannot be (null), empty or above 64 characters. String: " + itemId));
        return false;
      }
      if (GAValidator.ValidateEventPartCharacters(itemId))
        return true;
      Debug.Log((object) ("Validation fail - business event - itemId: Cannot contain other characters than A-z, 0-9, -_., ()!?. String: " + itemId));
      return false;
    }

    public static bool ValidateResourceEvent(
      GAResourceFlowType flowType,
      string currency,
      float amount,
      string itemType,
      string itemId)
    {
      if (string.IsNullOrEmpty(currency))
      {
        Debug.Log((object) "Validation fail - resource event - currency: Cannot be (null)");
        return false;
      }
      if (flowType == GAResourceFlowType.Undefined)
        Debug.Log((object) "Validation fail - resource event - flowType: Invalid flowType");
      if (!GAState.HasAvailableResourceCurrency(currency))
      {
        Debug.Log((object) ("Validation fail - resource event - currency: Not found in list of pre-defined resource currencies. String: " + currency));
        return false;
      }
      if ((double) amount <= 0.0)
      {
        Debug.Log((object) ("Validation fail - resource event - amount: Float amount cannot be 0 or negative. Value: " + (object) amount));
        return false;
      }
      if (string.IsNullOrEmpty(itemType))
      {
        Debug.Log((object) "Validation fail - resource event - itemType: Cannot be (null)");
        return false;
      }
      if (!GAValidator.ValidateEventPartLength(itemType, false))
      {
        Debug.Log((object) ("Validation fail - resource event - itemType: Cannot be (null), empty or above 64 characters. String: " + itemType));
        return false;
      }
      if (!GAValidator.ValidateEventPartCharacters(itemType))
      {
        Debug.Log((object) ("Validation fail - resource event - itemType: Cannot contain other characters than A-z, 0-9, -_., ()!?. String: " + itemType));
        return false;
      }
      if (!GAState.HasAvailableResourceItemType(itemType))
      {
        Debug.Log((object) ("Validation fail - resource event - itemType: Not found in list of pre-defined available resource itemTypes. String: " + itemType));
        return false;
      }
      if (!GAValidator.ValidateEventPartLength(itemId, false))
      {
        Debug.Log((object) ("Validation fail - resource event - itemId: Cannot be (null), empty or above 64 characters. String: " + itemId));
        return false;
      }
      if (GAValidator.ValidateEventPartCharacters(itemId))
        return true;
      Debug.Log((object) ("Validation fail - resource event - itemId: Cannot contain other characters than A-z, 0-9, -_., ()!?. String: " + itemId));
      return false;
    }

    public static bool ValidateProgressionEvent(
      GAProgressionStatus progressionStatus,
      string progression01,
      string progression02,
      string progression03)
    {
      if (progressionStatus == GAProgressionStatus.Undefined)
      {
        Debug.Log((object) "Validation fail - progression event: Invalid progression status.");
        return false;
      }
      if (!string.IsNullOrEmpty(progression03) && string.IsNullOrEmpty(progression02) && !string.IsNullOrEmpty(progression01))
      {
        Debug.Log((object) "Validation fail - progression event: 03 found but 01+02 are invalid. Progression must be set as either 01, 01+02 or 01+02+03.");
        return false;
      }
      if (!string.IsNullOrEmpty(progression02) && string.IsNullOrEmpty(progression01))
      {
        Debug.Log((object) "Validation fail - progression event: 02 found but not 01. Progression must be set as either 01, 01+02 or 01+02+03");
        return false;
      }
      if (string.IsNullOrEmpty(progression01))
      {
        Debug.Log((object) "Validation fail - progression event: progression01 not valid. Progressions must be set as either 01, 01+02 or 01+02+03");
        return false;
      }
      if (!GAValidator.ValidateEventPartLength(progression01, false))
      {
        Debug.Log((object) ("Validation fail - progression event - progression01: Cannot be (null), empty or above 64 characters. String: " + progression01));
        return false;
      }
      if (!GAValidator.ValidateEventPartCharacters(progression01))
      {
        Debug.Log((object) ("Validation fail - progression event - progression01: Cannot contain other characters than A-z, 0-9, -_., ()!?. String: " + progression01));
        return false;
      }
      if (!string.IsNullOrEmpty(progression02))
      {
        if (!GAValidator.ValidateEventPartLength(progression02, true))
        {
          Debug.Log((object) ("Validation fail - progression event - progression02: Cannot be empty or above 64 characters. String: " + progression02));
          return false;
        }
        if (!GAValidator.ValidateEventPartCharacters(progression02))
        {
          Debug.Log((object) ("Validation fail - progression event - progression02: Cannot contain other characters than A-z, 0-9, -_., ()!?. String: " + progression02));
          return false;
        }
      }
      if (!string.IsNullOrEmpty(progression03))
      {
        if (!GAValidator.ValidateEventPartLength(progression03, true))
        {
          Debug.Log((object) ("Validation fail - progression event - progression03: Cannot be empty or above 64 characters. String: " + progression03));
          return false;
        }
        if (!GAValidator.ValidateEventPartCharacters(progression03))
        {
          Debug.Log((object) ("Validation fail - progression event - progression03: Cannot contain other characters than A-z, 0-9, -_., ()!?. String: " + progression03));
          return false;
        }
      }
      return true;
    }

    public static bool ValidateDesignEvent(string eventId)
    {
      if (!GAValidator.ValidateEventIdLength(eventId))
      {
        Debug.Log((object) ("Validation fail - design event - eventId: Cannot be (null) or empty. Only 5 event parts allowed seperated by :. Each part need to be 32 characters or less. String: " + eventId));
        return false;
      }
      if (GAValidator.ValidateEventIdCharacters(eventId))
        return true;
      Debug.Log((object) ("Validation fail - design event - eventId: Non valid characters. Only allowed A-z, 0-9, -_., ()!?. String: " + eventId));
      return false;
    }

    public static bool ValidateErrorEvent(GAErrorSeverity severity, string message)
    {
      if (severity == GAErrorSeverity.Undefined)
      {
        Debug.Log((object) "Validation fail - error event - severity: Severity was unsupported value.");
        return false;
      }
      if (GAValidator.ValidateLongString(message, true))
        return true;
      Debug.Log((object) "Validation fail - error event - message: Message cannot be above 8192 characters.");
      return false;
    }

    public static bool ValidateSdkErrorEvent(
      string gameKey,
      string gameSecret,
      GAErrorSeverity type)
    {
      if (!GAValidator.ValidateKeys(gameKey, gameSecret))
        return false;
      if (type != GAErrorSeverity.Undefined)
        return true;
      Debug.Log((object) "Validation fail - sdk error event - type: Type was unsupported value.");
      return false;
    }

    public static bool ValidateKeys(string gameKey, string gameSecret)
    {
      return GAValidator.StringMatch(gameKey, "^[A-z0-9]{32}$") && GAValidator.StringMatch(gameSecret, "^[A-z0-9]{40}$");
    }

    public static bool ValidateCurrency(string currency)
    {
      return !string.IsNullOrEmpty(currency) && GAValidator.StringMatch(currency, "^[A-Z]{3}$");
    }

    public static bool ValidateEventPartLength(string eventPart, bool allowNull)
    {
      return allowNull && string.IsNullOrEmpty(eventPart) || !string.IsNullOrEmpty(eventPart) && eventPart.Length <= 64;
    }

    public static bool ValidateEventPartCharacters(string eventPart)
    {
      return GAValidator.StringMatch(eventPart, "^[A-Za-z0-9\\s\\-_\\.\\(\\)\\!\\?]{1,64}$");
    }

    public static bool ValidateEventIdLength(string eventId)
    {
      return !string.IsNullOrEmpty(eventId) && GAValidator.StringMatch(eventId, "^[^:]{1,64}(?::[^:]{1,64}){0,4}$");
    }

    public static bool ValidateEventIdCharacters(string eventId)
    {
      return !string.IsNullOrEmpty(eventId) && GAValidator.StringMatch(eventId, "^[A-Za-z0-9\\s\\-_\\.\\(\\)\\!\\?]{1,64}(:[A-Za-z0-9\\s\\-_\\.\\(\\)\\!\\?]{1,64}){0,4}$");
    }

    public static bool ValidateBuild(string build)
    {
      return GAValidator.ValidateShortString(build, false);
    }

    public static bool ValidateUserId(string uId)
    {
      if (GAValidator.ValidateString(uId, false))
        return true;
      Debug.Log((object) "Validation fail - user id: id cannot be (null), empty or above 64 characters.");
      return false;
    }

    public static bool ValidateShortString(string shortString, bool canBeEmpty)
    {
      return canBeEmpty && string.IsNullOrEmpty(shortString) || !string.IsNullOrEmpty(shortString) && shortString.Length <= 32;
    }

    public static bool ValidateString(string s, bool canBeEmpty)
    {
      return canBeEmpty && string.IsNullOrEmpty(s) || !string.IsNullOrEmpty(s) && s.Length <= 64;
    }

    public static bool ValidateLongString(string longString, bool canBeEmpty)
    {
      return canBeEmpty && string.IsNullOrEmpty(longString) || !string.IsNullOrEmpty(longString) && longString.Length <= 8192;
    }

    public static bool ValidateConnectionType(string connectionType)
    {
      return GAValidator.StringMatch(connectionType, "^(wwan|wifi|lan|offline)$");
    }

    public static bool ValidateCustomDimensions(params string[] customDimensions)
    {
      return GAValidator.ValidateArrayOfStrings(20L, 32L, false, "custom dimensions", customDimensions);
    }

    public static bool ValidateResourceCurrencies(params string[] resourceCurrencies)
    {
      if (!GAValidator.ValidateArrayOfStrings(20L, 64L, false, "resource currencies", resourceCurrencies))
        return false;
      foreach (string resourceCurrency in resourceCurrencies)
      {
        if (!GAValidator.StringMatch(resourceCurrency, "^[A-Za-z]+$"))
        {
          Debug.Log((object) ("resource currencies validation failed: a resource currency can only be A-Z, a-z. String was: " + resourceCurrency));
          return false;
        }
      }
      return true;
    }

    public static bool ValidateResourceItemTypes(params string[] resourceItemTypes)
    {
      if (!GAValidator.ValidateArrayOfStrings(20L, 32L, false, "resource item types", resourceItemTypes))
        return false;
      foreach (string resourceItemType in resourceItemTypes)
      {
        if (!GAValidator.ValidateEventPartCharacters(resourceItemType))
        {
          Debug.Log((object) ("resource item types validation failed: a resource item type cannot contain other characters than A-z, 0-9, -_., ()!?. String was: " + resourceItemType));
          return false;
        }
      }
      return true;
    }

    public static bool ValidateDimension01(string dimension01)
    {
      if (string.IsNullOrEmpty(dimension01))
      {
        Debug.Log((object) "Validation failed - custom dimension01 - value cannot be empty.");
        return false;
      }
      if (GAState.HasAvailableCustomDimensions01(dimension01))
        return true;
      Debug.Log((object) ("Validation failed - custom dimension 01 - value was not found in list of custom dimensions 01 in the Settings object. \nGiven dimension value: " + dimension01));
      return false;
    }

    public static bool ValidateDimension02(string dimension02)
    {
      if (string.IsNullOrEmpty(dimension02))
      {
        Debug.Log((object) "Validation failed - custom dimension01 - value cannot be empty.");
        return false;
      }
      if (GAState.HasAvailableCustomDimensions02(dimension02))
        return true;
      Debug.Log((object) ("Validation failed - custom dimension 02 - value was not found in list of custom dimensions 02 in the Settings object. \nGiven dimension value: " + dimension02));
      return false;
    }

    public static bool ValidateDimension03(string dimension03)
    {
      if (string.IsNullOrEmpty(dimension03))
      {
        Debug.Log((object) "Validation failed - custom dimension01 - value cannot be empty.");
        return false;
      }
      if (GAState.HasAvailableCustomDimensions03(dimension03))
        return true;
      Debug.Log((object) ("Validation failed - custom dimension 03 - value was not found in list of custom dimensions 03 in the Settings object. \nGiven dimension value: " + dimension03));
      return false;
    }

    public static bool ValidateArrayOfStrings(
      long maxCount,
      long maxStringLength,
      bool allowNoValues,
      string logTag,
      params string[] arrayOfStrings)
    {
      string str = logTag;
      if (string.IsNullOrEmpty(str))
        str = "Array";
      if (arrayOfStrings == null)
      {
        Debug.Log((object) (str + " validation failed: array cannot be null. "));
        return false;
      }
      if (!allowNoValues && arrayOfStrings.Length == 0)
      {
        Debug.Log((object) (str + " validation failed: array cannot be empty. "));
        return false;
      }
      if (maxCount > 0L && (long) arrayOfStrings.Length > maxCount)
      {
        Debug.Log((object) (str + " validation failed: array cannot exceed " + (object) maxCount + " values. It has " + (object) arrayOfStrings.Length + " values."));
        return false;
      }
      foreach (string arrayOfString in arrayOfStrings)
      {
        int num = arrayOfString == null ? 0 : arrayOfString.Length;
        if (num == 0)
        {
          Debug.Log((object) (str + " validation failed: contained an empty string."));
          return false;
        }
        if (maxStringLength > 0L && (long) num > maxStringLength)
        {
          Debug.Log((object) (str + " validation failed: a string exceeded max allowed length (which is: " + (object) maxStringLength + "). String was: " + arrayOfString));
          return false;
        }
      }
      return true;
    }

    public static bool ValidateFacebookId(string facebookId)
    {
      if (GAValidator.ValidateString(facebookId, false))
        return true;
      Debug.Log((object) "Validation fail - facebook id: id cannot be (null), empty or above 64 characters.");
      return false;
    }

    public static bool ValidateGender(string gender)
    {
      if (!(gender == ""))
      {
        string str1 = gender;
        GAGender gaGender = GAGender.male;
        string str2 = gaGender.ToString();
        if (!(str1 == str2))
        {
          string str3 = gender;
          gaGender = GAGender.female;
          string str4 = gaGender.ToString();
          if (!(str3 == str4))
            goto label_3;
        }
        return true;
      }
label_3:
      Debug.Log((object) ("Validation fail - gender: Has to be 'male' or 'female'.Given gender:" + gender));
      return false;
    }

    public static bool ValidateBirthyear(int birthYear)
    {
      if (birthYear >= 0 && birthYear <= 9999)
        return true;
      Debug.Log((object) "Validation fail - birthYear: Cannot be (null) or invalid range.");
      return false;
    }

    public static bool ValidateClientTs(long clientTs)
    {
      return clientTs >= -9223372036854775807L && clientTs <= 9223372036854775806L;
    }
  }
}
