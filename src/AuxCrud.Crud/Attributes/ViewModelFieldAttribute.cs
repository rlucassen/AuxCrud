namespace AuxCrud.ViewModel.Attributes
{
    using System;
    using NHibernate;

    public class ViewModelFieldAttribute : Attribute
    {
        public ViewModelFieldAttribute(string name)
        {
            Name = name;
        }

        public ViewModelFieldAttribute(string name, string mappingTree, bool mappingUpdate)
        {
            Name = name;
            MappingTree = mappingTree;
            MappingUpdate = mappingUpdate;
        }

        public ViewModelFieldAttribute(string name, string mappingTree, bool mappingUpdate, bool searchField)
        {
            Name = name;
            MappingTree = mappingTree;
            MappingUpdate = mappingUpdate;
            SearchField = searchField;
        }

        public string Name { get; set; }
        public bool SearchField { get; set; } = false;

        public bool IsColumn { get; set; } = false;
        public int ColumnOrder { get; set; }
        public string ColumnClass { get; set; }
        public bool ColumnOrderable { get; set; } = true;

        public string MappingTree { get; set; }
        public bool MappingUpdate { get; set; } = true;

    }
}