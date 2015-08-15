using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using UniRitter.UniRitter2015.Models;
using UniRitter.UniRitter2015.Services;

namespace UniRitter.UniRitter2015.Controllers
{
    abstract public class BaseController<TModel> : ApiController where TModel: class, IModel
    {
        protected IRepository<TModel> _repo;

        public BaseController()
        {
        }

        /*
        public BaseController(IRepository<TModel> repo)
        {
            _repo = repo;
        }
        */

        public async Task<IHttpActionResult> Get()
        {
            return Json(await _repo.GetAll());
        }

        public async Task<IHttpActionResult> Get(Guid id)
        {
            var data = await _repo.GetById(id);
            if (data != null)
            {
                return Json(data);
            }
            return NotFound();
        }

        public async Task<IHttpActionResult> Post([FromBody] TModel model)
        {
            if (ModelState.IsValid)
            {
                var data = await _repo.Add(model);
                return Json(data);
            }
            return BadRequest(ModelState);
        }

        public async Task<IHttpActionResult> Put(Guid id, [FromBody] TModel model)
        {
            var data = await _repo.Update(id, model);
            return Json(model);
        }

        public async Task<IHttpActionResult> Delete(Guid id)
        {
            await _repo.Delete(id);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
