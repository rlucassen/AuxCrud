﻿namespace AuxCrud.ViewModel.ViewModel
{
    using Attributes;
    using Inputs;
    using Model.Entities;

    public class ThingViewModel : ViewModel<Thing, ThingViewModel>
    {
        public ThingViewModel(Thing owner) : base(owner)
        {
            Map(x => x.Name, y => y.Name, true);

            Form().Input(x => x.Name, new StringInput(true));
        }

        [TableColumn(4, MappingField = "Name")]
        public string Name { get; set; }

        public override string Readable => Name;
    }
}