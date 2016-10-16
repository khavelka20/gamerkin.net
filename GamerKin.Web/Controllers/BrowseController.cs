using GamerKin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GamerKin.Web.Controllers
{
    [RoutePrefix("api/Browse")]
    public class BrowseController : ApiController
    {
        GamerKinService _service = new GamerKinService();

        [Route("GetViewModel/{gamerId}/{genreId}")]
        [HttpGet]
        public BrowseViewModel GetViewModel(int gamerId, int genreId)
        {
            var viewModel = _service.GetBrowseViewModel(gamerId, genreId);
            return viewModel;
        }
    }
}
