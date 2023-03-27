using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json;
using YouAndI_API.Utils;
using YouAndI_API.Utils.Constant;
using YouAndI_Entity;
using YouAndI_Model;

namespace YouAndI_API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : Controller
    {
        Youandi_DBContext Context;
        public UserController(Youandi_DBContext context)
        {
            Context = context;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpPost]
        public IActionResult userSignUp(SignUpModel userInfo)
        {
            const int defaultAvatarNum = 79; // 随机的图片数量
            Random rd = new Random();
            try
            {
                bool isEmailHave = Context.User.ToList().Exists(x => x.email == userInfo.email);
                bool isNameHave = Context.User.ToList().Exists(x => x.username == userInfo.username);
                if (isEmailHave)
                {
                    return BadRequest("邮箱已被注册");
                }
                if (isNameHave)
                {
                    return BadRequest("用户名已被注册");
                }
                string imageName = "default (" + rd.Next(1, defaultAvatarNum + 1) + ").png";
                // 添加用户
                User user = new User();
                user.username = userInfo.username;
                user.password = userInfo.password;
                user.email = userInfo.email;
                user.image = imageName;
                Context.User.Add(user);
                Context.SaveChanges();
                User result = Context.User.FirstOrDefault(x => x.email == user.email && x.password == user.password);
                // 创建默认头像
                FileUtils.createImage(result.id, imageName);
                // 创建用户信息
                UserInformation userInformation = new UserInformation();
                // 初始化
                userInformation.id = result.id;
                userInformation.sex = "男";
                userInformation.age = 18;
                userInformation.hobby = "";
                userInformation.brief = "";
                userInformation.x = 0;
                userInformation.y = 0;
                userInformation.location = "";

                Context.UserInformation.Add(userInformation);
                Context.SaveChanges();
                return Ok("注册成功");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpPost]
        public IActionResult login(LoginModel user)
        {
            User result = Context.User.FirstOrDefault(x => x.email == user.email && x.password == user.password);
            if (result != null)
            {
                // 配置token
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                    new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddMinutes(60*24*7)).ToUnixTimeSeconds()}"),
                    new Claim(ClaimTypes.Sid, result.id.ToString())
                };
                var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("QANstarAndSuoMi1931"));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: "QANstar",
                    audience: "QANstar",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(60 * 24 * 7),
                    signingCredentials: creds);

                var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(jwtToken);
            }
            else
            {
                return BadRequest();
            }
        }
        /// <summary>
        /// 上传头像
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> uploadHeadImg(IFormFile image)
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                int userID = int.Parse(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.Sid))?.Value);
                Boolean isSave = await FileUtils.saveImage(image, userID);
                if (isSave)
                {
                    User user = Context.User.FirstOrDefault(x => x.id == userID);
                    user.image = image.FileName;
                    Context.SaveChanges();
                    return Ok(PathConstant.IMAGEPATH_USER + user.id + "/" + user.image);
                }
                else
                {
                    return BadRequest("上传失败");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpPost]
        [Authorize]
        public IActionResult editUserInfo(UserInfoEditModel userInfo)
        {
            var auth = HttpContext.AuthenticateAsync();
            int userID = int.Parse(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.Sid))?.Value);
            UserInformation user = Context.UserInformation.FirstOrDefault(x => x.id == userID);
            if (user != null)
            {
                user.sex = userInfo.sex;
                user.age = userInfo.age;
                user.hobby = userInfo.hobby;
                user.brief = userInfo.brief;
                user.x = userInfo.x;
                user.y = userInfo.y;
                user.location = userInfo.location;
                Context.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpGet]
        [Authorize]
        public IActionResult getUserInfo()
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                int userID = int.Parse(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.Sid))?.Value);
                View_User view_User = Context.View_User.FirstOrDefault(x => x.id == userID);
                view_User.image = PathConstant.IMAGEPATH_USER + view_User.id + "/" + view_User.image;
                return Ok(view_User);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        /// <summary>
        /// 通过id获取用户信息
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpGet]
        [Authorize]
        public IActionResult getUserInfoById(int userId)
        {
            try
            {
                View_User view_User = Context.View_User.FirstOrDefault(x => x.id == userId);
                view_User.image = PathConstant.IMAGEPATH_USER + view_User.id + "/" + view_User.image;
                return Ok(view_User);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        /// <summary>
        /// 添加tag
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpPost]
        [Authorize]
        public IActionResult AddUserTag(int[] tagIdList)
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                int userID = int.Parse(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.Sid))?.Value);
                tagIdList.ToList().ForEach(tagId =>
                {
                    Boolean isHave = Context.UserTag.ToList().Exists(x => x.userId == userID && x.tagId == tagId);
                    if (!isHave)
                    {
                        UserTag userTag = new UserTag();
                        userTag.userId = userID;
                        userTag.tagId = tagId;
                        Context.UserTag.Add(userTag);
                    }
                });
                Context.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 覆盖tag
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpPost]
        [Authorize]
        public IActionResult CoverUserTag(int[] tagIdList)
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                int userID = int.Parse(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.Sid))?.Value);
                List<UserTag> oldTags = Context.UserTag.Where(x => x.userId == userID).ToList();
                oldTags.ForEach(tag =>
                {
                    Context.UserTag.Remove(tag);
                });
                tagIdList.ToList().ForEach(tagId =>
                {
                    Boolean isHave = Context.UserTag.ToList().Exists(x => x.userId == userID && x.tagId == tagId);
                    if (!isHave)
                    {
                        UserTag userTag = new UserTag();
                        userTag.userId = userID;
                        userTag.tagId = tagId;
                        Context.UserTag.Add(userTag);
                    }
                });
                Context.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 删除tag
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpDelete]
        [Authorize]
        public IActionResult delUserTag(int tagId)
        {
            try
            {
                var auth = HttpContext.AuthenticateAsync();
                int userID = int.Parse(auth.Result.Principal.Claims.First(t => t.Type.Equals(ClaimTypes.Sid))?.Value);
                UserTag userTag = Context.UserTag.ToList().FirstOrDefault(x => x.userId == userID && x.tagId == tagId);
                if (userTag != null)
                {
                    Context.UserTag.Remove(userTag);
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
        /// 获取某用户tag
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpGet]
        public IActionResult getUserTag(int userId)
        {
            try
            {
                List<View_UserTag> view_UserTag = Context.View_UserTag.Where(x => x.userId == userId).ToList();
                return Ok(view_UserTag);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 获取所以tag
        /// </summary>
        /// <returns></returns>
        [EnableCors("any")]
        [HttpGet]
        public IActionResult GetAllTag()
        {
            try
            {
                List<RootTag> rootTag = Context.RootTag.Include(x => x.Tag).ToList();
                var json = JsonUtils.ToJson(rootTag);
                return Ok(json);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
