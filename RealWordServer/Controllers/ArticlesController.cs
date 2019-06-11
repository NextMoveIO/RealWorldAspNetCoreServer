using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealWordServer.Models;

namespace RealWordServer.Controllers
{
    public class ArticleDto
    {
        public int Id { get; set; }
        public string Title { get; set;  }
        public string Content { get; set; }
        public DateTime? VisibleFrom { get; set; }
        public PublishState State { get; set; }
    }

    public class PagedResponse<T>
    {
        public T[] Data { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "BearerAuthentication")]
    [EnableCors("RealWorldServerCorsPolicy")]
    public class ArticlesController : ApiControllerBase
    {
        public ArticlesController(BloggingContext context) : base(context)
        { }

        [HttpGet]
        public ActionResult<PagedResponse<ArticleDto>> Get()
        {
            var userId = GetUser().Id;
            var articles = Context.Articles.Where(_ => _.UserId == userId);

            var articlesDto = new List<ArticleDto>();
            foreach(var article in articles)
            {
                articlesDto.Add(new ArticleDto
                {
                    Id = article.ArticleId,
                    Title = article.Title,
                    Content = article.Content,
                    State = article.State
                });
            }
            return new PagedResponse<ArticleDto>
            {
                Data = articlesDto.ToArray()
            };
        }

        [HttpGet("{id}")]
        public ActionResult<ArticleDto> Get(int id)
        {
            var userId = GetUser().Id;
            var article = Context.Articles.Where(_ => _.UserId == userId && _.ArticleId == id).FirstOrDefault();

            if (article == null)
            {
                return NotFound();
            }

            return new ArticleDto
            {
                Id = article.ArticleId,
                Title = article.Title,
                Content = article.Content,
                State = article.State,
                VisibleFrom = article.VisibleFrom
            };
        }

        [HttpPost]
        public ActionResult<ArticleDto> Post(ArticleDto articleDto)
        {
            var article = new Article
            {
                UserId = GetUser().Id,
                Title = articleDto.Title,
                Content = articleDto.Content,
                State = articleDto.State,
                VisibleFrom = articleDto.VisibleFrom
            };

            Context.Articles.Add(article);
            Context.SaveChanges();

            return new ArticleDto
            {
                Id = article.ArticleId,
                Title = article.Title,
                Content = article.Content,
                State = article.State
            };
        }

        [HttpPut]
        public ActionResult<int> Put([FromBody] ArticleDto articleDto)
        {
            var userId = GetUser().Id;
            var article = Context.Articles.Where(_ => _.UserId == userId && _.ArticleId == articleDto.Id).FirstOrDefault();

            if (article == null)
            {
                return NotFound();
            }

            article.Title = articleDto.Title;
            article.Content = articleDto.Content;
            article.State = articleDto.State;
            article.VisibleFrom = articleDto.VisibleFrom;

            Context.SaveChanges();

            return article.ArticleId;
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var userId = GetUser().Id;
            var article = Context.Articles.Where(_ => _.UserId == userId && _.ArticleId == id).FirstOrDefault();

            if (article == null)
            {
                return NotFound();
            }

            try
            {
                Context.Articles.Remove(article);
                Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {

            }

            return Ok();
        }
    }
}
