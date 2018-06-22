
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace mw.Image2Url
{
    public class D3PictureServiceWrapper
    {
        private static string D3PictureLookupAddress
        {
            get
            {
                return "http://img.dataduoduo.com/filemanage/Image/Lookup/";
            }
        }

        private static string D3PictureUploadBaseAddress
        {
            get
            {
                return "http://10.105.40.118:8080/";// "http://img.dataduoduo.com/";// "http://10.105.40.118:8080/";
            }
        }

        private static string D3PictureUploadPath
        {
            get
            {
                return "filemanage/Image/Upload";
            }
        }

        /// <summary>
        /// 上传图片文件到服务器并返回其url
        /// </summary>
        /// <param name="imageBytes"></param>
        /// <param name="subPath"></param>
        /// <param name="fileExt">支持png</param>
        /// <param name="disposes">压缩参数，非必需，格式正则：(\d+)[xX](\d+)#([01])，组1表示宽，组2表示高，组3表示是否等比例压缩（0否1是）。如果选择等比例压缩模式，     最终的宽高会根据源图片与目标图片的宽高的比例大的那个进行汇算</param>
        /// <returns></returns>
        public static string UploadPicture(byte[] imageBytes, string subPath, string fileExt = ".png", string disposes = null)
        {
            if (imageBytes?.Length > 0)
            {
                var request = new RestRequest(D3PictureUploadPath, Method.POST);
                request.AddParameter("bucket", subPath);// "bazhuayu/ruleTemplate/id/0/当前城市");
                if (!string.IsNullOrWhiteSpace(disposes))
                {
                    request.AddParameter("disposes", disposes);// disposes 压缩参数，非必需，格式正则：(\d+)[xX](\d+)#([01])，组1表示宽，组2表示高，组3表示是否等比例压缩（0否1是）。如果选择等比例压缩模式，     最终的宽高会根据源图片与目标图片的宽高的比例大的那个进行汇算
                }
                request.AddFile("img", imageBytes, "fakefilename" + fileExt);
                var response = new RestClient(D3PictureUploadBaseAddress).Execute(request);

                if (response.ErrorException != null)
                {
                    throw response.ErrorException;
                }

                var obj = JsonConvert.DeserializeAnonymousType(response.Content, new { code = "1", reason = "ok", result = new { status = "1", message = "msg" } });
                return D3PictureLookupAddress + obj.result.message;
            }
            return string.Empty;
        }
    }
}
