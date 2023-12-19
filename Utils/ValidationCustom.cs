using System.ComponentModel.DataAnnotations;

namespace ECommerceApi.Utils;

public static class StringUtils
{
    public static string ToSnakeCase(this string str)
    {
        return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
    }
}

public class MinimalValidator
{
    public static ValidationResult Validate<T>(T model)
    {
        var result = new ValidationResult()
        {
            IsValid = true
        };
        var properties = typeof(T).GetProperties();
        foreach (var property in properties)
        {
            var customAttributes = property.GetCustomAttributes(typeof(ValidationAttribute), true);
            foreach (var attribute in customAttributes)
            {
                var validationAttribute = attribute != null ? attribute as ValidationAttribute : null;
                if (validationAttribute != null)
                {
                    var propertyValue = property.CanRead ? property.GetValue(model) : null;
                    var isValid = validationAttribute.IsValid(propertyValue);

                    if (!isValid)
                    {
                        var keyName = property.Name.ToSnakeCase();
                        if (result.Errors.ContainsKey(property.Name))
                        {
                            var errors = result.Errors[keyName].ToList();
                            errors.Add(validationAttribute.FormatErrorMessage(keyName));
                            result.Errors[keyName] = errors.ToArray();
                        }
                        else
                        {
                            result.Errors.Add(keyName, new string[] { validationAttribute.FormatErrorMessage(keyName) });
                        }

                        result.IsValid = false;
                    }
                }
            }
        }
        return result;
    }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public Dictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
}