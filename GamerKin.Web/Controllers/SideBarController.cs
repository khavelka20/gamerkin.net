using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GamerKin.ViewModels;
using GamerKin;

namespace GamerKin.Web.Controllers
{
    [RoutePrefix("api/SideBar")]
    public class SideBarController : ApiController
    {
        GamerKinService _service = new GamerKinService();

        [Route("GetViewModel")]
        [HttpGet]
        public SideBarViewModel GetViewModel()
        {
            var viewModel = _service.GetSideBarViewModel();
            return viewModel; 
        }

        [Route("UpdateViewModel")]
        [HttpPost]
        public SideBarViewModel UpdateViewModel(SideBarViewModel vm)
        {
            var viewModel = _service.UpdateSideBarViewModel(vm);
            return viewModel;
        }

    }
}
