namespace AuxCrud.ViewModel.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class TableColumnAttribute : Attribute
    {
        public TableColumnAttribute(int order)
        {
            Order = order;
        }

        public string Title { get; set; }
        public int Order { get; set; }
        public string MappingField { get; set; }
        public string ClassName { get; set; }
        public bool Orderable { get; set; } = true;
    }
}