namespace AuxCrud.ViewModel.ViewModel
{
    using System;
    using Attributes;
    using Inputs;
    using Model.Entities;

    [ViewModel("persoon", "personen")]
    public class PersonViewModel : ViewModel<Person>
    {
        public PersonViewModel()
        {
        }

        public PersonViewModel(Person owner) : base(owner)
        {
            if (owner.Group != null)
            {
                Group = new GroupViewModel(owner.Group);
            }
        }

        [SelectInput(11, typeof(Group), ObjectDtoType = typeof(GroupViewModel))]
        [Mapping("Group.Id", "Groep id", false)]
        public int GroupId { get; set; }

        public GroupViewModel Group { get; }

        [SearchField]
        [TableColumn(6)]
        [Mapping("Group.Name", "Groep naam", false)]
        public string GroupName { get; set; }

        [SearchField]
        [StringInput(1, true, Size = 4)]
        [Mapping("Firstname", "Voornaam")]
        public string Firstname { get; set; }

        [StringInput(2, Size = 2, ShowLabel = false)]
        [Mapping("Prefix", "Tussenvoegsel")]
        public string Prefix { get; set; }

        [SearchField]
        [StringInput(3, true, Size = 4, ShowLabel = false)]
        [Mapping("Lastname", "Achternaam")]
        public string Lastname { get; set; }

        [SearchField]
        [StringInput(4, true, Size = 8)]
        [Mapping("Street", "Straat")]
        public string Street { get; set; }

        [StringInput(5, true, Size = 2, ShowLabel = false)]
        [Mapping("Housenumber", "Huisnummer")]
        public string Housenumber { get; set; }

        [SearchField]
        [StringInput(6, true, Pattern = StringInputAttribute.Patterns.Postcode, Message = "Vul een geldige postcode in", Size = 4)]
        [Mapping("Postcode", "Postcode")]
        public string Postcode { get; set; }

        [StringInput(7, true, Size = 4)]
        [Mapping("City", "Stad")]
        public string City { get; set; }

        [TableColumn(4)]
        [StringInput(9, true, Pattern = StringInputAttribute.Patterns.Phone, Message = "Vul een geldig telefoonnummer in")]
        [Mapping("Phone", "Telefoon")]
        public string Phone { get; set; }

        [TableColumn(5)]
        [StringInput(8, true, Pattern = StringInputAttribute.Patterns.Email, Message = "Vul een geldig emailadres in")]
        [Mapping("Email", "Email")]
        public string Email { get; set; }

        [DateTimeInput(10, true)]
        [Mapping("Birthdate", "Geboortedatum")]
        public DateTime? Birthdate { get; set; }

        [TableColumn(1, MappingField = "Firstname")]
        public string Fullname => $"{Firstname} {Prefix} {Lastname}".Replace("  ", " ");

        [TableColumn(2, MappingField = "Street")]
        public string Address => $"{Street} {Housenumber}";

        [TableColumn(3, MappingField = "Postcode")]
        public string Address2 => $"{Postcode} {City}";

        public string FullAddress => $"{Address}, {Postcode}, {City}";

        public override string Readable => Fullname;
    }
}