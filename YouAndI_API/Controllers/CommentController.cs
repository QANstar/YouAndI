using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YouAndI_API.Utils.Constant;
using YouAndI_Entity;
using YouAndI_Model;

namespace YouAndI_API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CommentController : Controller
    {
        Youandi_DBContext Context;
        public CommentController(Youandi_DBContext context)
        {
            Context = context;
        }
        /// <summary>
        /// 评论
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpPost]
        [Authorize]
        public IActionResult addComment(CommentAddModel comment)
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                int userID = int.Parse(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.Sid))?.Value);
                Comment commentNew = new Comment();
                commentNew.userid = userID;
                commentNew.comment1 = comment.commentInfo;
                commentNew.activity_id = comment.activity_id;
                commentNew.createtime = DateTime.Now;
                Context.Comment.Add(commentNew);
                Context.SaveChanges();
                return Ok("评论成功");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 查看评论
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpGet]
        [Authorize]
        public IActionResult showComment(int activtiyId)
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                int userID = int.Parse(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.Sid))?.Value);
                List<View_Comment> view_Comment = Context.View_Comment.Where(x => x.activity_id == activtiyId).ToList();
                view_Comment.ForEach(x => x.image = PathConstant.IMAGEPATH_USER + x.userid + "/" + x.image);
                return Ok(view_Comment);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
