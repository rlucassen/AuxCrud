namespace AuxCrud.ViewModel.ViewModel
{
    using Attributes;
    using Inputs;
    using Model.Entities;

    [ViewModel("groep", "groepen")]
    public class GroupViewModel : ViewModel<Group, GroupViewModel>
    {
        public GroupViewModel(Group owner) : base(owner)
        {
            Map(x => x.Name, y => y.Name, true);

            Form().Input(x => x.Name, new StringInput("Naam", true));
        }

        [TableColumn(4, MappingField = "Name", Title = "Naam")]
        public string Name { get; set; }

        public override string Readable => Name;
    }
}