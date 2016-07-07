namespace AuxCrud.Model.Mapping
{
    using Entities;

    public class PersonMap : ModelBaseMap<Person>
    {
        public PersonMap()
        {
            Map(x => x.Firstname);
            Map(x => x.Prefix);
            Map(x => x.Lastname);
            Map(x => x.Street);
            Map(x => x.Housenumber);
            Map(x => x.Postcode);
            Map(x => x.City);
            Map(x => x.Phone);
            Map(x => x.Email);
            Map(x => x.Birthdate);
            References(x => x.Group).Cascade.None();
        }
    }
}