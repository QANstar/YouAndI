using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YouAndI_Entity;
using YouAndI_Model;
using YouAndI_API.Utils;
using YouAndI_API.Utils.Constant;
using System.Diagnostics;
using Activity = YouAndI_Entity.Activity;

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
                if (activity.Payment != null)
                {
                    Payment payment = new()
                    {
                        activityId = newActivity.id,
                        payment1 = activity.Payment.payment1,
                        type = activity.Payment.type,
                    };
                    newActivity.Payment = payment;
                }
                newActivity.ActivityTag = activity.tags.Select(x =>
                {
                    return new ActivityTag()
                    {
                        tagId = x.tagId,
                    };
                }).ToList();
                Context.Activity.Add(newActivity);
                Context.SaveChanges();
                Boolean isHave = Context.ApplyActivity.ToList().Exists(x => x.userid == userID && x.activity_id == newActivity.id && x.status != ApplyStatusConstant.UNPASS);
                if (newActivity.curnumber >= newActivity.maxnumber)
                {
                    return BadRequest("人数已满");
                }
                if (isHave)
                {
                    return BadRequest("你已申请过");
                }
                ApplyActivity applyActivityItme = new ApplyActivity();
                applyActivityItme.activity_id = newActivity.id;
                applyActivityItme.userid = userID;
                applyActivityItme.applytime = DateTime.Now;
                applyActivityItme.status = ApplyStatusConstant.PASS;
                Context.ApplyActivity.Add(applyActivityItme);
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
        [Authorize]
        public IActionResult getAllActivity()
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                int userID = int.Parse(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.Sid))?.Value);
                List<View_Activity> activityList = Context.View_Activity.ToList();
                List<ActivityShowModel> activities = activityList.Select(x => new ActivityShowModel()
                {
                    id = x.id,
                    userid = x.userid,
                    title = x.title,
                    x = x.x,
                    y = x.y,
                    image = PathConstant.IMAGEPATH_USER + x.userid + "/" + x.image,
                    location = x.location,
                    introduction = x.introduction,
                    maxnumber = x.maxnumber,
                    curnumber = x.curnumber,
                    starttime = x.starttime,
                    endtime = x.endtime,
                    type = x.type,
                    typeID = x.typeID,
                    username = x.username,
                    isStar = CheckIsStar(x.id, userID),
                    tags = Context.ActivityTag.Where(y => y.activityId == x.id).ToList()
                }).ToList();
                return Ok(activities);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        /// <summary>
        /// 根据id获取活动
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpGet]
        [Authorize]
        public IActionResult getActivityById(int activityId)
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                int userID = int.Parse(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.Sid))?.Value);
                View_Activity activity = Context.View_Activity.FirstOrDefault(x => x.id == activityId);
                if (activity == null)
                {
                    throw new Exception("活动不存在");
                }

                ActivityShowModel activityShow = new()
                {
                    id = activity.id,
                    userid = activity.userid,
                    title = activity.title,
                    x = activity.x,
                    y = activity.y,
                    image = PathConstant.IMAGEPATH_USER + activity.userid + "/" + activity.image,
                    location = activity.location,
                    introduction = activity.introduction,
                    maxnumber = activity.maxnumber,
                    curnumber = activity.curnumber,
                    starttime = activity.starttime,
                    endtime = activity.endtime,
                    type = activity.type,
                    typeID = activity.typeID,
                    username = activity.username,
                    isStar = CheckIsStar(activity.id, userID),
                    payment = Context.Payment.FirstOrDefault(y => y.activityId == activity.id),
                    tags = Context.ActivityTag.Where(x => x.activityId == activity.id).ToList()
                };
                return Ok(activityShow);
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
        [Authorize]
        public IActionResult getActivityByType(int typeId, int lastId)
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                int userID = int.Parse(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.Sid))?.Value);
                if (typeId == 0)
                {
                    List<View_Activity> activityList = Context.View_Activity.OrderByDescending(x => x.id).Where(x => DateTime.Compare(DateTime.Now, x.endtime) < 0).Where(x => lastId == 0 || x.id < lastId).Take(10).ToList();
                    List<ActivityShowModel> activities = activityList.Select(x => new ActivityShowModel()
                    {
                        id = x.id,
                        userid = x.userid,
                        title = x.title,
                        x = x.x,
                        y = x.y,
                        image = PathConstant.IMAGEPATH_USER + x.userid + "/" + x.image,
                        location = x.location,
                        introduction = x.introduction,
                        maxnumber = x.maxnumber,
                        curnumber = x.curnumber,
                        starttime = x.starttime,
                        endtime = x.endtime,
                        type = x.type,
                        typeID = x.typeID,
                        username = x.username,
                        isStar = CheckIsStar(x.id, userID),
                        payment = Context.Payment.FirstOrDefault(y => y.activityId == x.id),
                        tags = Context.ActivityTag.Where(y => y.activityId == x.id).ToList()
                    }).ToList();
                    return Ok(activities);
                }
                else
                {
                    List<View_Activity> activityList = Context.View_Activity.OrderByDescending(x => x.id).Where(x => x.typeID == typeId && DateTime.Compare(DateTime.Now, x.endtime) < 0).Where(x => lastId == 0 || x.id < lastId).Take(10).ToList();
                    List<ActivityShowModel> activities = activityList.Select(x => new ActivityShowModel()
                    {
                        id = x.id,
                        userid = x.userid,
                        title = x.title,
                        x = x.x,
                        y = x.y,
                        image = PathConstant.IMAGEPATH_USER + x.userid + "/" + x.image,
                        location = x.location,
                        introduction = x.introduction,
                        maxnumber = x.maxnumber,
                        curnumber = x.curnumber,
                        starttime = x.starttime,
                        endtime = x.endtime,
                        type = x.type,
                        typeID = x.typeID,
                        username = x.username,
                        isStar = CheckIsStar(x.id, userID),
                        payment = Context.Payment.FirstOrDefault(y => y.activityId == x.id),
                        tags = Context.ActivityTag.Where(y => y.activityId == x.id).ToList()
                    }).ToList();
                    return Ok(activities);
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
        [Authorize]
        public IActionResult getActivityByDistance(double x, double y, double distance)
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                int userID = int.Parse(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.Sid))?.Value);
                List<View_Activity> activityList = Context.View_Activity.Where(a => DateTime.Compare(DateTime.Now, a.endtime) < 0 && Math.Sqrt(Math.Pow(a.x - x, 2) + Math.Pow(a.y - y, 2)) < distance).OrderByDescending(x => x.id).Take(10).ToList();
                List<ActivityShowModel> activities = activityList.Select(x => new ActivityShowModel()
                {
                    id = x.id,
                    userid = x.userid,
                    title = x.title,
                    x = x.x,
                    y = x.y,
                    image = PathConstant.IMAGEPATH_USER + x.userid + "/" + x.image,
                    location = x.location,
                    introduction = x.introduction,
                    maxnumber = x.maxnumber,
                    curnumber = x.curnumber,
                    starttime = x.starttime,
                    endtime = x.endtime,
                    type = x.type,
                    typeID = x.typeID,
                    username = x.username,
                    isStar = CheckIsStar(x.id, userID),
                    payment = Context.Payment.FirstOrDefault(y => y.activityId == x.id),
                    tags = Context.ActivityTag.Where(y => y.activityId == x.id).ToList()
                }).ToList();
                return Ok(activities);
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
                Activity activity = Context.Activity.FirstOrDefault(x => x.userid == userID && x.id == activtyId);
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
                if (activity.curnumber >= activity.maxnumber)
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
        public IActionResult getApplyActivityOfCreator(int activityId, int applyState)
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
        /// <summary>
        /// 收藏活动
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpPost]
        [Authorize]
        public IActionResult starActivity(int activityId)
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                int userID = int.Parse(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.Sid))?.Value);
                Boolean isHave = Context.StarActivity.ToList().Exists(x => x.userId == userID && x.activityId == activityId);
                if (!isHave)
                {
                    StarActivity starActivity = new StarActivity();
                    starActivity.userId = userID;
                    starActivity.activityId = activityId;
                    Context.StarActivity.Add(starActivity);
                    Context.SaveChanges();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 删除收藏
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpDelete]
        [Authorize]
        public IActionResult unStarActivity(int activityId)
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                int userID = int.Parse(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.Sid))?.Value);
                StarActivity starActivity = Context.StarActivity.ToList().FirstOrDefault(x => x.userId == userID && x.activityId == activityId);
                if (starActivity != null)
                {
                    Context.StarActivity.Remove(starActivity);
                    Context.SaveChanges();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 获取收藏的活动
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpGet]
        [Authorize]
        public IActionResult getStarActivities()
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                int userID = int.Parse(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.Sid))?.Value);
                List<View_StarActivity> starActivities = Context.View_StarActivity.Where(x => x.userId == userID).ToList();
                starActivities.ForEach(x => x.image = PathConstant.IMAGEPATH_USER + x.createUerId + "/" + x.image);
                return Ok(starActivities);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 获取某活动收藏数量
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpGet]
        public IActionResult getStarActivityNum(int activityId)
        {
            try
            {
                List<StarActivity> starActivities = Context.StarActivity.Where(x => x.activityId == activityId).ToList();
                return Ok(starActivities.Count);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 获取某用户获得收藏的数量
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpGet]
        public IActionResult getStarActivityNumOfUser(int userId)
        {
            try
            {
                List<StarActivity> starActivities = Context.StarActivity.Where(x => x.userId == userId).ToList();
                return Ok(starActivities.Count);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 检查是否被收藏
        /// </summary>
        /// <returns></returns>
        private bool CheckIsStar(int activityId, int userId)
        {
            StarActivity starActivity = Context.StarActivity.FirstOrDefault(x => x.activityId == activityId && x.userId == userId);
            if (starActivity != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
