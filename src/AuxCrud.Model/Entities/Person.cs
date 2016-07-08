namespace AuxCrud.Model.Entities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class Person : ModelBase
    {
        public virtual string Firstname { get; set; }
        public virtual string Prefix { get; set; }
        public virtual string Lastname { get; set; }
        public virtual string Street { get; set; }
        public virtual string Housenumber { get; set; }
        public virtual string Postcode { get; set; }
        public virtual string City { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Email { get; set; }
        public virtual DateTime? Birthdate { get; set; }

        public virtual Group Group { get; set; }

        public virtual IList<Thing> Things { get; set; }
    }
}