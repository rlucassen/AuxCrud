namespace AuxCrud.Model.Mapping
{
    using Entities;

    public class GroupMap : ModelBaseMap<Group>
    {
        public GroupMap()
        {
            Map(x => x.Name);
            HasMany(x => x.Persons).Cascade.None();
        }
    }
}