namespace AuxCrud.ViewModel.ViewModel
{
    using Model.Entities;

    public class GroupViewModel : ViewModel<Group, GroupViewModel>
    {
        public GroupViewModel(Group owner) : base(owner)
        {
            Map(x => x.Name, y => y.Name, true);
        }

        public string Name { get; set; }

        public override string Readable => Name;
    }
}