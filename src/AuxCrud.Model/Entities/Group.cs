namespace AuxCrud.Model.Entities
{
    using System.Collections.Generic;

    public class Group : ModelBase
    {
        public virtual string Name { get; set; }
        public virtual IList<Person> Persons { get; set; }
    }
}