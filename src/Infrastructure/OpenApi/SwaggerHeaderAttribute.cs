namespace Infrastructure.OpenApi;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class SwaggerHeaderAttribute(string headerName, string defaultValue, string description, bool isRequired) : Attribute
{
    public string HeaderName { get; set; } = headerName;
    public string Description { get; set; } = description;
    public string DefaultValue { get; set; } = defaultValue;
    public bool IsRequired { get; set; } = isRequired;
}
