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
    public class ChartMessageController : Controller
    {
        Youandi_DBContext Context;
        public ChartMessageController(Youandi_DBContext context)
        {
            Context = context;
        }
        /// <summary>
        /// 查看聊天信息
        /// </summary>
        /// <param name="activtiyId"></param>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpGet]
        [Authorize]
        public IActionResult showChartMessage(int activtiyId)
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                int userID = int.Parse(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.Sid))?.Value);
                // 判断申请人是否在成员里面
                bool isMember = Context.ApplyActivity.Where(x => x.activity_id == activtiyId).ToList().Exists(x => x.userid == userID && x.status == ApplyStatusConstant.PASS);
                if (isMember)
                {
                    List<View_ChartMessage> view_ChartMessage = Context.View_ChartMessage.Where(x => x.activity_id == activtiyId).ToList();
                    view_ChartMessage.ForEach(x => x.image = PathConstant.IMAGEPATH_USER + x.userid + "/" + x.image);
                    return Ok(view_ChartMessage);
                }
                else
                {
                    return BadRequest("不在此活动成员中");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 发送聊天消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpPost]
        [Authorize]
        public IActionResult addChartMessage(ChartMessageAddModel message)
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                int userID = int.Parse(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.Sid))?.Value);
                // 判断申请人是否在成员里面
                bool isMember = Context.ApplyActivity.ToList().Exists(x => x.userid == userID && x.status == ApplyStatusConstant.PASS);
                if (isMember)
                {
                    ChartMessage chartMessage = new ChartMessage();
                    chartMessage.userid = userID;
                    chartMessage.activity_id = message.activity_id;
                    chartMessage.createtime = DateTime.Now;
                    chartMessage.message = message.chartMessageInfo;
                    Context.ChartMessage.Add(chartMessage);
                    Context.SaveChanges();
                    return Ok("消息发送成功");
                }
                else
                {
                    return BadRequest("不在此活动成员中");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
