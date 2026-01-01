using System.Reflection;

namespace Mess.Api.Data;

public class ObjectConverter
{
    
    public static NewMom ConvertCopy<NewMom, YourMom>(YourMom toConvert) where NewMom : new()
    {
        NewMom converted = new NewMom();
        foreach (FieldInfo father in typeof(YourMom).GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
        {
            try
            {
                if (typeof(NewMom).GetField(father.Name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public) != null) typeof(NewMom).GetField(father.Name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).SetValue(converted, father.GetValue(toConvert));
            }
            catch { }
        }
        return converted;
    }
}