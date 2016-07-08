namespace AuxCrud.Model.Entities
{
    using System.Collections.Generic;

    public class Thing : ModelBase
    {
        public virtual string Name { get; set; }
        public virtual Person Person { get; set; }
    }
}