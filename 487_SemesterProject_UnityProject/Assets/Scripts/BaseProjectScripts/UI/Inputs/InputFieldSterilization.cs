using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class InputFieldSterilization : MonoBehaviour
{
    public enum ContentType { NONE, custom, lowerLetters, upperLetters, numbers, space, symbols }
    public enum FormatType { NONE, upperCase, lowerCase, firstLetterCapitalized }

    public class ContentFilter
    {
        public ContentType contentType;
        public string allowedCharacterString;
        public ContentFilter(ContentType newContentType, string newAllowedCharacterString)
        {
            contentType = newContentType;
            allowedCharacterString = newAllowedCharacterString;
        }
    }
    public InputField inputField;
    List<ContentFilter> contentFilters = new List<ContentFilter>();

    [Header("Allowed Content List")]
    [SerializeField] List<ContentType> contentTypes = new List<ContentType>() { ContentType.NONE };

    [Header("Formatting Type")]
    public FormatType formatType = FormatType.NONE;
    
    [Header("Allowed Characters For Custom Content Type")]
    // This string cannot be changed in the inspector during runtime
    [SerializeField] string m_allowedCharactersCustom;
    public string allowedCharactersCustom
    {
        get
        {
            return m_allowedCharactersCustom;
        }
        set
        {
            m_allowedCharactersCustom = value;
            contentFilters[0].allowedCharacterString = value;
        }
    }

    // If you want to add a new content filter, create an ContentType enum value for it, and a string here that contains
    // all of the characters you want to be allowed for this content type
    static string allowedCharactersLowercaseLetters = "abcdefghijklmnopqrstuvwxyz";
    static string allowedCharactersUppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    static string allowedCharactersNumbers = "0123456789";
    static string allowedCharactersSpace = " ";
    static string allowedCharactersSymbols = "!@#$%^&*()_+{}|:\"<>?-=[]\\;',./~`";

    // If you want to add a new content filter, create a ContentType enum value and a string for it,
    // and then add it to the contentFilters list here in Awake()
    void Awake()
    {
        contentFilters.Add(new ContentFilter(ContentType.NONE, ""));
        contentFilters.Add(new ContentFilter(ContentType.custom, allowedCharactersCustom));
        contentFilters.Add(new ContentFilter(ContentType.lowerLetters, allowedCharactersLowercaseLetters));
        contentFilters.Add(new ContentFilter(ContentType.upperLetters, allowedCharactersUppercaseLetters));
        contentFilters.Add(new ContentFilter(ContentType.numbers, allowedCharactersNumbers));
        contentFilters.Add(new ContentFilter(ContentType.space, allowedCharactersSpace));
        contentFilters.Add(new ContentFilter(ContentType.symbols, allowedCharactersSymbols));        
    }

    void Start()
    {
        inputField.onValidateInput += delegate (string input, int charIndex, char addedChar) { return ValidateChar(addedChar); };
    }

    /// <summary>
    /// ValidateChar takes in a char and returns either the same char or an empty char
    /// based on whether it meets the contentTypes listed in InputFieldSerialization
    /// (This  function is intended to be used as a delegate for an InputField's onValidateInput only)
    /// </summary>
    /// <param name="addedChar">character to be validated</param>
    /// <returns></returns>
    public char ValidateChar(char addedChar)
    {
        bool isValidChar = false;
        foreach (ContentType content in contentTypes)
        {
            if (content == ContentType.NONE)
            {
                isValidChar = true;
                break;
            }
            if (GetContentFilterFromContentType(content).allowedCharacterString.Contains(addedChar))
            {
                isValidChar = true;
            }
        }
        if (isValidChar)
        {
            return addedChar;
        }
        else
        {
            return '\0';
        }
    }

    public ContentFilter GetContentFilterFromContentType(ContentType contentTypeToSearch)
    {
        foreach (ContentFilter filter in contentFilters)
        {
            if (filter.contentType == contentTypeToSearch)
            {
                return filter;
            }
        }
        Debug.LogError("ContentType (" + contentTypeToSearch + ") was not found in ContentFilters List");
        return null;
    }

    public void OnValueChanged()
    {
        if (FormatString(inputField.text) != inputField.text)
        {
            inputField.text = FormatString(inputField.text);
        }
    }

    // If you want to add a new format type that auto-formats the input field while the user is typing, 
    // create a function that takes in a string and returns a string formatted to your liking,
    // and then add a FormatType enum value for that format and create a case for that enum value in the format of 
    // 
    // case FormatType.yourNewFormatTypeHere:
    //     return YourNewFunctionHere(toFormat);
    //     break;
    // 
    // in the FormatString function below

    /// <summary>
    /// FormatString takes in a string and returns it formatted based on the function 
    /// associated with the currently selected formatType specified in InputFieldSerialization
    /// </summary>
    /// <param name="toFormat">String to be formatted</param>
    /// <returns>wow nick TODO</returns>
    public string FormatString(string toFormat)
    {
        switch (formatType)
        {
            case FormatType.lowerCase:
                return FormatStringLowerCase(toFormat);
            case FormatType.upperCase:
                return FormatStringUpperCase(toFormat);
            case FormatType.firstLetterCapitalized:
                return FormatCapitalizeFirstLetter(toFormat);
            case FormatType.NONE:
                return toFormat;
            default:
                return null;
        }
    }

    // Create Formatting functions here: They must take in one string as a parameter and return a string formatted any way you like
    public string FormatStringLowerCase(string toFormat)
    {
        return toFormat.ToLower();
    }

    public string FormatStringUpperCase(string toFormat)
    {
        return toFormat.ToUpper();
    }

    public string FormatCapitalizeFirstLetter(string toFormat)
    {
        string retString = toFormat;
        if (toFormat.Length > 0)
        {
            retString = (retString[0] + "").ToUpper() + retString.Substring(1, retString.Length - 1);
            for (int i = 1; i < toFormat.Length; i++)
            {
                if (retString[i - 1] == ' ')
                {
                    retString = retString.Substring(0, i) + retString.Substring(i, 1).ToUpper() + retString.Substring(i + 1, retString.Length - i - 1);
                }
            }
        }
        return retString;
    }
}
