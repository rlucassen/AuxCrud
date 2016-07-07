namespace AuxCrud.ViewModel.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class MappingAttribute : Attribute
    {
        public MappingAttribute(string propertyTree, string name)
        {
            PropertyTree = propertyTree;
            Name = name;
            Update = true;
        }

        public MappingAttribute(string propertyTree, string name, bool update)
        {
            PropertyTree = propertyTree;
            Name = name;
            Update = update;
        }

        public string PropertyTree { get; }
        public bool Update { get; }
        public string Name { get; }
    }
}