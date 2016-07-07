namespace AuxCrud.Web.Controllers
{
    using NHibernate;

    public class HomeController : BaseController
    {
        public HomeController(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        public void Index()
        {
            
        }
    }
}