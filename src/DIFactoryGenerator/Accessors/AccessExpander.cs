namespace DIFactoryGenerator.Accessors
{
    public static class AccessExpander
    {
        public static void SetValueField<TObject, TField>(this TObject instance, string fieldName, TField value)
        {
            PrivateFieldAccessor<TObject>.SetField(instance, fieldName, value);
        }
    }
}
