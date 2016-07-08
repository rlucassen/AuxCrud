namespace AuxCrud.ViewModel.ViewModel
{
    using Attributes;
    using Inputs;
    using Model.Entities;

    [ViewModel("groep", "groepen")]
    public class ThingViewModel : ViewModel<Thing, ThingViewModel>
    {
        public ThingViewModel(Thing owner) : base(owner)
        {
            Map(x => x.Name, y => y.Name, true);

            Form().Input(x => x.Name, new StringInput(true));
        }

        [TableColumn(4, MappingField = "Name", Title = "Naam")]
        public string Name { get; set; }

        public override string Readable => Name;
    }
}