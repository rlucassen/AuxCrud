namespace AuxCrud.ViewModel.ViewModel
{
    using System;
    using Attributes;
    using Inputs;
    using Model.Entities;

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

            Input(x => x.Firstname, new StringInput("Voornaam", true) { Size = 4 });
            Input(x => x.Prefix, new StringInput("Tussenvoegsel", false) { Size = 2, ShowLabel = false });
            Input(x => x.Lastname, new StringInput("Achternaam", true) { Size = 4, ShowLabel = false });

            Input(x => x.Street, new StringInput("Straat", true) { Size = 8 });
            Input(x => x.Housenumber, new StringInput("Huisnummer", true) { Size = 2, ShowLabel = false });
            Input(x => x.Postcode, new StringInput("Postcode", true) { Size = 4, Pattern = BaseInput.Patterns.Postcode, Message = "Vul een geldige postcode in" });
            Input(x => x.City, new StringInput("Stad", true) { Size = 4 });

            Input(x => x.Phone, new StringInput("Telefoonnummer", true) { Pattern = BaseInput.Patterns.Phone, Message = "Vul een geldig telefoonnummer in" });
            Input(x => x.Email, new StringInput("Emailadres", true) { Pattern = BaseInput.Patterns.Email, Message = "Vul een geldig emailadres in" });

            Input(x => x.Birthdate, new DateTimeInput("Birthdate", true));

            Input(x => x.Group, new SelectInput("Groep", true));

            Form()
                .Row("Naam", 4, 2, 4)
                    .Input(x => x.Firstname, new StringInput("Voornaam", true))
                    .Input(x => x.Prefix, new StringInput("Tussenvoegsel", false))
                    .Input(x => x.Lastname, new StringInput("Achternaam", true))
                .Row("Adres", 8)
                    .Input(x => x.Street, new StringInput("Straat", true))
                    .Input(x => x.Housenumber, new StringInput("Huisnummer", true))
                .Row("", 4, 6)
                    .Input(x => x.Postcode, new StringInput("Postcode", true) { Size = 4, Pattern = BaseInput.Patterns.Postcode, Message = "Vul een geldige postcode in" })
                    .Input(x => x.City, new StringInput("Stad", true) { Size = 4 })
                .Row().Input(x => x.Phone, new StringInput("Telefoonnummer", true) { Pattern = BaseInput.Patterns.Phone, Message = "Vul een geldig telefoonnummer in" })
                .Row().Input(x => x.Email, new StringInput("Emailadres", true) { Pattern = BaseInput.Patterns.Email, Message = "Vul een geldig emailadres in" })
                .Row().Input(x => x.Birthdate, new DateTimeInput("Birthdate", true))
                .Row().Input(x => x.Group, new SelectInput("Groep", true));


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