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


            Input(x => x.Firstname, new StringInput(true) {Size = 4});
            Input(x => x.Prefix, new StringInput(false) {Size = 2, ShowLabel = false});
            Input(x => x.Lastname, new StringInput(true) {Size = 4, ShowLabel = false});

            Input(x => x.Street, new StringInput(true) {Size = 8});
            Input(x => x.Housenumber, new StringInput(true) {Size = 2, ShowLabel = false});
            Input(x => x.Postcode, new StringInput(true) {Size = 4, Pattern = StringInput.Patterns.Postcode, Message = "Vul een geldige postcode in" });
            Input(x => x.City, new StringInput(true) {Size = 4});

            Input(x => x.Phone, new StringInput(true) {Pattern = StringInput.Patterns.Phone, Message = "Vul een geldig telefoonnummer in" });
            Input(x => x.Email, new StringInput(true) {Pattern = StringInput.Patterns.Email, Message = "Vul een geldig emailadres in"});

            Input(x => x.Birthdate, new DateTimeInput(true));
        }

        public string Firstname { get; set; }
        public string Prefix { get; set; }
        public string Lastname { get; set; }


        public string Street { get; set; }
        public string Housenumber { get; set; }
        public string Postcode { get; set; }
        public string City { get; set; }

        [TableColumn(4, MappingField = "Phone")]
        public string Phone { get; set; }
        [TableColumn(5, MappingField = "Email")]
        public string Email { get; set; }

        public DateTime? Birthdate { get; set; }

        [TableColumn(1, MappingField = "Firstname")]
        public string Fullname => $"{Firstname} {Prefix} {Lastname}".Replace("  ", " ");

        [TableColumn(2, MappingField = "Street")]
        public string Address => $"{Street} {Housenumber}";

        [TableColumn(3, MappingField = "Postcode")]
        public string Address2 => $"{Postcode} {City}";

        public override string Readable => Fullname;
    }
}