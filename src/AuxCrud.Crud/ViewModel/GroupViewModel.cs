namespace AuxCrud.ViewModel.ViewModel
{
    using Attributes;
    using Inputs;
    using Model.Entities;
    using NHibernate;

    [ViewModel("groep", "groepen")]
    public class GroupViewModel : ViewModel<Group>
    {
        public GroupViewModel()
        {
        }

        public GroupViewModel(Group owner) : base(owner)
        {
            Name = owner.Name;
        }

        public override Group Update(ISession session)
        {
            var item = session.Get<Group>(Id) ?? new Group();

            item.Name = Name;

            return item;
        }

        [TableColumn(1)]
        [StringInput(1, true)]
        [Mapping("Name", "Naam")]
        public string Name { get; set; }

        public override string Readable => Name;
    }
}