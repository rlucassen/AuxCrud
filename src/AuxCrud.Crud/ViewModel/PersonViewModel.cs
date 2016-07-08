namespace AuxCrud.ViewModel.ViewModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Attributes;
    using Inputs;
    using Model.Entities;
    using NHibernate.Criterion;

    [ViewModel("persoon", "personen")]
    public class PersonViewModel : ViewModel<Person, PersonViewModel>
    {
        public PersonViewModel(Person owner) : base(owner)
        {
            Map(x => x.Firstname, y => y.Firstname, true);
            Map(x => x.Prefix, y => y.Prefix);
            Map(x => x.Lastname, y => y.Lastname, true);

            Map(x => x.Street, y => y.Street, true);
            Map(x => x.Housenumber, y => y.Housenumber);
            Map(x => x.Postcode, y => y.Postcode, true);
            Map(x => x.City, y => y.City);

            Map(x => x.Phone, y => y.Phone);
            Map(x => x.Email, y => y.Email);

            Map(x => x.Birthdate, y => y.Birthdate);

            Reference(x => x.Group, y => y.Group);

            HasMany(x => x.Things, y => y.Things);

            Form()
                .Row(Localization.Language.Person_Name, 4,2,4)
                    .Input(x => x.Firstname, new StringInput(true))
                    .Input(x => x.Prefix, new StringInput(false))
                    .Input(x => x.Lastname, new StringInput(true))
                .Row(Localization.Language.Person_Address, 8)
                    .Input(x => x.Street, new StringInput(true))
                    .Input(x => x.Housenumber, new StringInput(true))
                .Row("", 4, 6)
                    .Input(x => x.Postcode, new StringInput(true) { Pattern = BaseInput.Patterns.Postcode, Message = "Vul een geldige postcode in" })
                    .Input(x => x.City, new StringInput(true))
                .Row().Input(x => x.Phone, new StringInput(true) { Pattern = BaseInput.Patterns.Phone, Message = "Vul een geldig telefoonnummer in" })
                .Row().Input(x => x.Email, new StringInput(true) { Pattern = BaseInput.Patterns.Email, Message = "Vul een geldig emailadres in" })
                .Row().Input(x => x.Birthdate, new DateTimeInput(true))
                .Row().Input(x => x.Group, new SelectInput(true) {Criterions = new ICriterion[] {Restrictions.Like("Name", "Groep", MatchMode.Start)}});
        }

        public string Firstname { get; set; }
        public string Prefix { get; set; }
        public string Lastname { get; set; }

        public string Street { get; set; }
        public string Housenumber { get; set; }
        public string Postcode { get; set; }
        public string City { get; set; }

        [TableColumn(4, MappingField = "Phone", Title = "Telefoonnummer")]
        public string Phone { get; set; }
        [TableColumn(5, MappingField = "Email", Title = "Emailadres")]
        public string Email { get; set; }

        public DateTime? Birthdate { get; set; }

        public GroupViewModel Group { get; set; }

        public IList<ThingViewModel> Things { get; set; }

        [TableColumn(7, MappingField = "Things", Title = "ThingsCount")]
        public int ThingsCount => Things.Count;

        [TableColumn(6, MappingField = "Group", Title = "Groep")]
        public string GroupName => Group.Name;

        [TableColumn(1, MappingField = "Firstname", Title = "Naam")]
        public string Fullname => $"{Firstname} {Prefix} {Lastname}".Replace("  ", " ");

        [TableColumn(2, MappingField = "Street", Title = "Adres")]
        public string Address => $"{Street} {Housenumber}";

        [TableColumn(3, MappingField = "Postcode", Title = "Adres 2")]
        public string Address2 => $"{Postcode} {City}";

        public override string Readable => Fullname;
    }
}