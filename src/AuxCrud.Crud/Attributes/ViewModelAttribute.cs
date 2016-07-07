namespace AuxCrud.ViewModel.Attributes
{
    using System;

    public class ViewModelAttribute : Attribute
    {
        public ViewModelAttribute(string name)
        {
            Name = name;
            PluralName = $"{name}s";
        }

        public ViewModelAttribute(string name, string pluralName)
        {
            Name = name;
            PluralName = pluralName;
        }

        public string Name { get; set; }
        public string PluralName { get; set; }
    }
}