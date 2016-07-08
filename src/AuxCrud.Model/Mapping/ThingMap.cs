namespace AuxCrud.Model.Mapping
{
    using Entities;

    public class ThingMap : ModelBaseMap<Thing>
    {
        public ThingMap()
        {
            Map(x => x.Name);
            References(x => x.Person);
        }
    }
}