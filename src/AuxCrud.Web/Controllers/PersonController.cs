namespace AuxCrud.Web.Controllers
{
    using Model.Entities;
    using NHibernate;
    using ViewModel.ViewModel;

    public class PersonController : CrudController<Person, PersonViewModel>
    {
        public PersonController(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        protected override void FillPropertyBag()
        {
        }
    }

}