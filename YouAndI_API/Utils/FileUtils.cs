namespace YouAndI_API.Utils
{
    public static class FileUtils
    {
        // 开发环境
        //private const string IMAGE_LOCAL_PATH = "E:/Web/image/";
        // 部署环境
        private const string IMAGE_LOCAL_PATH = "C:/QANstar/image/";

        private const string IMAGE_LOCAL_USER_PATH = IMAGE_LOCAL_PATH + "user/";

        /// <summary>
        /// 注册创建图片
        /// </summary>
        /// <param name="username"></param>
        public static void createImage(int id)
        {
            string userImgPath = IMAGE_LOCAL_USER_PATH + id;
            if (!Directory.Exists(userImgPath))

            {
                Directory.CreateDirectory(userImgPath);
            }
            System.IO.File.Copy(IMAGE_LOCAL_USER_PATH + "default.jpg", userImgPath + "/default.jpg", true);

        }
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="image"></param>
        public static async Task<Boolean> saveImage(IFormFile image,int userid)
        {
            if (image != null)
            {
                //文件后缀
                var fileExtension = Path.GetExtension(image.FileName);

                //判断后缀是否是图片
                const string fileFilt = ".gif|.jpg|.jpeg|.png";
                if (fileExtension == null)
                {

                    return false;
                }
                if (fileFilt.IndexOf(fileExtension.ToLower(), StringComparison.Ordinal) <= -1)
                {
                    return false;
                }

                //插入图片数据                 
                using (var stream = System.IO.File.Create(IMAGE_LOCAL_USER_PATH+userid+"/"+ image.FileName))
                {
                   await image.CopyToAsync(stream);
                }
            }
            else
            {
                return false;
            }
            return true;

        }
    }
}
