using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Selenium_Chrome_HTTP_Private_Proxy
{
    public class ExtensionHelper
    {
        //Selenium + Chrome 使用用户名密码认证的代理封装
        static string CHROME_PROXY_HELPER_DIR = "Chrome-proxy-helper";  //Chrome代理模板插件目录
        static string CUSTOM_CHROME_PROXY_EXTENSIONS_DIR = "chrome-proxy-extensions";  //存储自定义Chrome代理扩展文件的目录
        
        /// <summary>
        /// 创建插件
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static Tuple<bool,string> CreateExtension(string username,string password,string ip,string port)
        {
            try
            {
                //创建一个定制Chrome代理扩展(zip文件)
                if (!Directory.Exists(CHROME_PROXY_HELPER_DIR))
                {
                    Directory.CreateDirectory(CHROME_PROXY_HELPER_DIR);
                    return new Tuple<bool, string>(false, "Please add manifest.json and background.js");
                }
                if (!Directory.Exists(CUSTOM_CHROME_PROXY_EXTENSIONS_DIR))
                {
                    Directory.CreateDirectory(CUSTOM_CHROME_PROXY_EXTENSIONS_DIR);
                }
                string extenFilePath = CUSTOM_CHROME_PROXY_EXTENSIONS_DIR+"\\"+ip+port+".zip";
                using (FileStream fsOut = File.Create(@extenFilePath))
                {
                    using (ZipOutputStream zipStream = new ZipOutputStream(fsOut))
                    {

                        //准备把Chrome代理模板插件目录下的manifest.json文件添加到压缩包中。
                        string fileName = CHROME_PROXY_HELPER_DIR + "\\manifest.json";
                        if (!File.Exists(fileName))
                            return new Tuple<bool, string>(false, "lack manifest.json");
                        FileInfo fi = new FileInfo(fileName);
                        //entryName就是压缩包中文件的名称。
                        string entryName = "manifest.json";
                        //ZipEntry类代表了一个压缩包中的一个项，可以是一个文件，也可以是一个目录。
                        ZipEntry newEntry = new ZipEntry(entryName);
                        newEntry.DateTime = fi.LastWriteTime;
                        newEntry.Size = fi.Length;
                        //把压缩项的信息添加到ZipOutputStream中。
                        zipStream.PutNextEntry(newEntry);
                        byte[] buffer = new byte[4096];
                        //把需要压缩文件以文件流的方式复制到ZipOutputStream中。

                        using (FileStream streamReader = File.OpenRead(fileName))
                        {
                            StreamUtils.Copy(streamReader, zipStream, buffer);
                        }
                        zipStream.CloseEntry();
                        ZipEntry entry = new ZipEntry("background.js");
                        entry.DateTime = DateTime.Now;
                        zipStream.PutNextEntry(entry);
                        fileName = CHROME_PROXY_HELPER_DIR + "\\background.js";
                        if (!File.Exists(fileName))
                            return new Tuple<bool, string>(false, "lack background.js");
                        string backGroundContent = File.ReadAllText(fileName);
                        backGroundContent = backGroundContent.Replace("%proxy_host", ip);
                        backGroundContent = backGroundContent.Replace("%proxy_port", port);
                        backGroundContent = backGroundContent.Replace("%username", username);
                        backGroundContent = backGroundContent.Replace("%password", password);
                        byte[] data = System.Text.Encoding.Default.GetBytes(backGroundContent);
                        zipStream.Write(data, 0, data.Length);
                        zipStream.CloseEntry();
                        zipStream.IsStreamOwner = false;
                        zipStream.Finish();
                        zipStream.Close();
                        return new Tuple<bool, string>(true, extenFilePath);

                    }
                }

            }
            catch(Exception ex)
            {
                return new Tuple<bool, string>(false, ex.ToString());
            }


           
        }
        
    }
}
