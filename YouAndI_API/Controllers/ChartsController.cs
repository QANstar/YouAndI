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
    public class ChartsController : Controller
    {
        Youandi_DBContext Context;
        public ChartsController(Youandi_DBContext context)
        {
            Context = context;
        }
        /// <summary>
        /// 获取用户总数
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpGet]
        public IActionResult GetUserNum()
        {
            try
            {
                int num = Context.User.Count();
                return Ok(num);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        /// <summary>
        /// 获取活动总数
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpGet]
        public IActionResult GetActivitiesNum()
        {
            try
            {
                int num = Context.Activity.Count();
                int runNum = Context.Activity.Where(x => DateTime.Compare(DateTime.Now, x.endtime) < 0).Count();
                ActivityNumModel activityNum = new()
                {
                    activityNum = num,
                    activityRunningNum = runNum
                };
                return Ok(activityNum);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 获取用户标签数量
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpGet]
        public IActionResult GetTagNum()
        {
            try
            {
                var tagNums = Context.View_UserTag.GroupBy(z => z.name, (x, y) => new
                {
                    num = y.Count(),
                    name = x
                });
                return Ok(tagNums);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// /获取每个类型的活动数量
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpGet]
        public IActionResult GetActivityTypeNum()
        {
            try
            {
                var typeNums = Context.View_Activity.GroupBy(z => z.type, (x, y) => new
                {
                    num = y.Count(),
                    type = x
                });
                return Ok(typeNums);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 收藏类型数量
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpGet]
        public IActionResult GetActivityStarNum()
        {
            try
            {
                var starNums = Context.View_StarActivity.GroupBy(z => z.type, (x, y) => new
                {
                    num = y.Count(),
                    type = x
                });
                return Ok(starNums);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 日期数量
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpGet]
        public IActionResult GetActivityDateNum()
        {
            try
            {
                var dateNums = Context.View_Activity.OrderBy(x => x.starttime).GroupBy(z => new
                {
                    year = z.starttime.Year,
                    month = z.starttime.Month,
                }, (x, y) => new
                {
                    num = y.Count(),
                    x.year,
                    x.month,
                    date = x.year + "-" + (x.month >= 10 ? x.month.ToString() : "0" + x.month)
                }).OrderBy(x => x.year).ThenBy(x => x.month);
                return Ok(dateNums);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
