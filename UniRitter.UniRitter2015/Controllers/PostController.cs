using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UniRitter.UniRitter2015.Models;

namespace UniRitter.UniRitter2015.Controllers
{
    public class PostController : ApiController
    {
        // GET: api/Post
        public IHttpActionResult Get()
        {
            var data = new string[] { "value1", "value2" };
            return Json(data);
        }

        // GET: api/Post/5
        public IHttpActionResult Get(Guid id)
        {
            return Json("value");
        }

        // POST: api/Post
        public IHttpActionResult Post([FromBody]PostModel post)
        {
            if (ModelState.IsValid)
            {
                post.id = Guid.NewGuid();
                return Created("posts/" + post.id, post);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        // PUT: api/Post/5
        public IHttpActionResult Put(Guid id, [FromBody]PostModel post)
        {
            if (ModelState.IsValid)
            {
                return Json(post);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        // DELETE: api/Post/5
        public void Delete(Guid id)
        {
        }
    }
}