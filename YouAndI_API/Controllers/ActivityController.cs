using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YouAndI_Entity;
using YouAndI_Model;
using YouAndI_API.Utils;
using YouAndI_API.Utils.Constant;

namespace YouAndI_API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ActivityController : Controller
    {
        Youandi_DBContext Context;
        public ActivityController(Youandi_DBContext context)
        {
            Context = context;
        }
        /// <summary>
        /// 新增活动
        /// </summary>
        [EnableCors("any")]
        [HttpPost]
        [Authorize]
        public IActionResult addActivity(ActivityAddModel activity)
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                int userID = int.Parse(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.Sid))?.Value);
                Activity newActivity = new Activity();
                newActivity.userid = userID;
                newActivity.title = activity.title;
                newActivity.x = activity.x;
                newActivity.y = activity.y;
                newActivity.location = activity.location;
                newActivity.type = activity.type;
                newActivity.introduction = activity.introduction;
                newActivity.maxnumber = activity.maxnumber;
                newActivity.curnumber = 0;
                newActivity.starttime = activity.starttime;
                newActivity.endtime = activity.endtime;
                Context.Activity.Add(newActivity);
                Context.SaveChanges();
                return Ok("新增成功");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 获取所有活动
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpGet]
        //[Authorize]
        public IActionResult getAllActivity()
        {
            try
            {
                List<View_Activity> activityList = Context.View_Activity.ToList();
                activityList.ForEach(x => x.image = PathConstant.IMAGEPATH_USER + x.userid + "/" + x.image);
                return Ok(activityList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        /// <summary>
        /// 根据类型id筛选
        /// 过滤掉已结束
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpGet]
        public IActionResult getActivityByType(int typeId)
        {
            try
            {
                if(typeId == 0)
                {
                    List<View_Activity> activityList = Context.View_Activity.Where(x =>  DateTime.Compare(DateTime.Now, x.endtime) < 0).ToList();
                    activityList.ForEach(x => x.image = PathConstant.IMAGEPATH_USER + x.userid + "/" + x.image);
                    return Ok(activityList);
                }
                else
                {
                    List<View_Activity> activityList = Context.View_Activity.Where(x => x.typeID == typeId && DateTime.Compare(DateTime.Now, x.endtime) < 0).ToList();
                    activityList.ForEach(x => x.image = PathConstant.IMAGEPATH_USER + x.userid + "/" + x.image);
                    return Ok(activityList);
                }
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 根据距离筛选
        /// 过滤掉已结束
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpGet]
        public IActionResult getActivityByDistance(double x, double y, double distance)
        {
            try
            {
                List<View_Activity> activityList = Context.View_Activity.Where(a => DateTime.Compare(DateTime.Now, a.endtime) < 0 && Math.Sqrt(Math.Pow(a.x - x, 2) + Math.Pow(a.y - y, 2)) < distance).ToList();
                activityList.ForEach(x => x.image = PathConstant.IMAGEPATH_USER + x.userid + "/" + x.image);
                return Ok(activityList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 获取某个用户创建的活动
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpGet]
        [Authorize]
        public IActionResult getActivityByCreator()
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                int userID = int.Parse(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.Sid))?.Value);
                List<View_Activity> activityList = Context.View_Activity.Where(x => x.userid == userID).ToList();
                activityList.ForEach(x => x.image = PathConstant.IMAGEPATH_USER + x.userid + "/" + x.image);
                return Ok(activityList);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 用户删除活动
        /// </summary>
        /// <param name="activtyId"></param>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpDelete]
        [Authorize]
        public IActionResult deletActivity(int activtyId)
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                int userID = int.Parse(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.Sid))?.Value);
                Activity activity = Context.Activity.FirstOrDefault(x => x.userid ==userID && x.id == activtyId);
                Context.Activity.Remove(activity);
                Context.SaveChanges();
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 申请加入活动
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpPost]
        [Authorize]
        public IActionResult applyActivity(int activityId)
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                int userID = int.Parse(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.Sid))?.Value);
                Activity activity = Context.Activity.FirstOrDefault(x => x.id == activityId);
                Boolean isHave = Context.ApplyActivity.ToList().Exists(x => x.userid == userID && x.activity_id == activityId && x.status != ApplyStatusConstant.UNPASS);
                if(activity.curnumber >= activity.maxnumber)
                {
                    return BadRequest("人数已满");
                }
                if (isHave)
                {
                    return BadRequest("你已申请过");
                }
                ApplyActivity applyActivityItme = new ApplyActivity();
                applyActivityItme.activity_id = activityId;
                applyActivityItme.userid = userID;
                applyActivityItme.applytime = DateTime.Now;
                applyActivityItme.status = ApplyStatusConstant.APPLYING;
                Context.ApplyActivity.Add(applyActivityItme);
                Context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 获取该用户所有申请的活动
        /// 根据申请状态返回
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpGet]
        [Authorize]
        public IActionResult getApplyActivityOfApplicant(int applyState)
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                int userID = int.Parse(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.Sid))?.Value);
                List<View_ApplyActivity> applyActivities = Context.View_ApplyActivity.Where(x => x.userid == userID && x.status == applyState).ToList();
                applyActivities.ForEach(x => { x.CreatorImage = PathConstant.IMAGEPATH_USER + x.CreatorId + "/" + x.CreatorImage; x.userImage = PathConstant.IMAGEPATH_USER + x.userid + "/" + x.userImage; });
                return Ok(applyActivities);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 获取该用户所有通过的进行中的活动
        /// 过滤已经过期
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpGet]
        [Authorize]
        public IActionResult getGoingActivityOfApplicant()
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                int userID = int.Parse(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.Sid))?.Value);
                List<View_ApplyActivity> applyActivities = Context.View_ApplyActivity.Where(x => x.userid == userID && DateTime.Compare(DateTime.Now, x.endtime) < 0 && x.status == ApplyStatusConstant.PASS).ToList();
                applyActivities.ForEach(x => { x.CreatorImage = PathConstant.IMAGEPATH_USER + x.CreatorId + "/" + x.CreatorImage; x.userImage = PathConstant.IMAGEPATH_USER + x.userid + "/" + x.userImage; });
                return Ok(applyActivities);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 获取创建者此项目的申请
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpGet]
        [Authorize]
        public IActionResult getApplyActivityOfCreator(int activityId,int applyState)
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                int userID = int.Parse(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.Sid))?.Value);
                // 判断项目是否属于创建者
                bool isCreator = Context.Activity.FirstOrDefault(x => x.id == activityId).userid == userID;
                if (isCreator)
                {
                    List<View_ApplyActivity> applyActivities = Context.View_ApplyActivity.Where(x => x.activity_id == activityId && x.status == applyState).ToList();
                    applyActivities.ForEach(x => { x.CreatorImage = PathConstant.IMAGEPATH_USER + x.CreatorId + "/" + x.CreatorImage; x.userImage = PathConstant.IMAGEPATH_USER + x.userid + "/" + x.userImage; });
                    return Ok(applyActivities);
                }
                else
                {
                    return BadRequest("没有此权限");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 通过活动申请
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpPut]
        [Authorize]
        public IActionResult passApply(int applyId)
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                int userID = int.Parse(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.Sid))?.Value);
                ApplyActivity apply = Context.ApplyActivity.FirstOrDefault(x => x.id == applyId);
                Activity activity = Context.Activity.FirstOrDefault(x => x.id == apply.activity_id);
                // 判断用户是否为创建者
                if (activity.userid == userID)
                {
                    if (activity.curnumber >= activity.maxnumber)
                    {
                        return BadRequest("人数已满");
                    }
                    apply.status = ApplyStatusConstant.PASS;
                    activity.curnumber++;
                    Context.SaveChanges();
                    return Ok();
                }
                else
                {
                    return BadRequest("权限错误");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 拒绝活动申请
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpPut]
        [Authorize]
        public IActionResult unpassApply(int applyId)
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                int userID = int.Parse(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.Sid))?.Value);
                ApplyActivity apply = Context.ApplyActivity.FirstOrDefault(x => x.id == applyId);
                Activity activity = Context.Activity.FirstOrDefault(x => x.id == apply.activity_id);
                // 判断用户是否为创建者
                if (activity.userid == userID)
                {
                    apply.status = ApplyStatusConstant.UNPASS;
                    Context.SaveChanges();
                    return Ok();
                }
                else
                {
                    return BadRequest("权限错误");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 获取所有活动类型
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpGet]
        public IActionResult getActivityType()
        {
            try
            {
                return Ok(Context.Type);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 返回所有申请状态
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpGet]
        public IActionResult getApplyStatus()
        {
            try
            {
                return Ok(Context.ApplyStatus);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
