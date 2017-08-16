using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class IOHelper
    {
        /// <summary>
        /// 在指定的目录中查找文件
        /// </summary>
        /// <param name="dir">目录</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static bool FindFile(string dir, string fileName)
        {
            if (dir == null || dir.Trim() == "" || fileName == null || fileName.Trim() == "" || !Directory.Exists(dir))
            {
                return false;
            }

            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            return FindFile(dirInfo, fileName);

        }


        public static bool FindFile(DirectoryInfo dir, string fileName)
        {
            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                if (File.Exists(d.FullName + "\\" + fileName))
                {
                    return true;
                }
                FindFile(d, fileName);
            }

            return false;
        }
        /// <summary>
        /// 在指定的目录找到文件路径
        /// </summary>
        /// <param name="dir">目录</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static Collection<FileInfo> GetFilePath(DirectoryInfo dir, string fileName)
        {
            if (dir == null || fileName == null || fileName.Trim() == "")
            {
                return null;
            }
            Collection<FileInfo> myfiles = new Collection<FileInfo>();
            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                foreach (FileInfo f in GetFilePath(d, fileName))
                {
                    myfiles.Add(f);
                }
            }

            foreach (FileInfo f in dir.GetFiles(fileName))
            {
                myfiles.Add(f);
            }
            return myfiles;
        }
        /// <summary>
        /// 在指定的目录找到文件路径
        /// </summary>
        /// <param name="dir">目录</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static Collection<string> GetFilePath(string dir, string fileName)
        {
            if (dir == null || dir.Trim() == "" || fileName == null || fileName.Trim() == "" || !Directory.Exists(dir))
            {
                return null;
            }

            Collection<string> myfiles = new Collection<string>();
            DirectoryInfo mydir = new DirectoryInfo(dir);
            foreach (DirectoryInfo d in mydir.GetDirectories())
            {
                foreach (FileInfo f in GetFilePath(d, fileName))
                {
                    myfiles.Add(f.FullName);
                }
            }

            foreach (FileInfo f in mydir.GetFiles(fileName))
            {
                myfiles.Add(f.FullName);
            }
            return myfiles;
        }
    }
}
