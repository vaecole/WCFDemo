using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyTest.Common
{
    public class Cacher
    {
        private object newCacheLock = new object();
        private Dictionary<string, string> cachesHashList = new Dictionary<string, string>();
        private string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dump.octdb");
        private int dumpPeriodMS = 30 * 60 * 1000;
        private bool updated = false;

        private Cacher()
        {
            Recover();
            timer = new Timer(
                    (t) => { Dump2File(); },
                    null,
                    dumpPeriodMS,
                    Timeout.Infinite);
        }
        public static Cacher Instance = new Cacher();

        private Timer timer;

        public void Cache<T>(string entityName, T entity, bool persistenceNow = false) where T : new()
        {
            lock (newCacheLock)
            {
                if (!cachesHashList.ContainsKey(entityName))
                {
                    cachesHashList.Add(entityName, JsonConvert.SerializeObject(entity));
                }
                else
                {
                    cachesHashList[entityName] = JsonConvert.SerializeObject(entity);
                }
                updated = true;
                if (persistenceNow)
                {
                    Dump2File();
                }
            }
        }

        public void Remove(string entityName)
        {
            if (cachesHashList.ContainsKey(entityName))
            {
                cachesHashList.Remove(entityName);
                Dump2File();
            }
        }

        public TResult Peek<TResult>(string entityName)
        {
            if (!cachesHashList.ContainsKey(entityName))
            {
                return default(TResult);
            }
            return JsonConvert.DeserializeObject<TResult>(cachesHashList[entityName]);
        }

        public bool IsUsed(string entityName)
        {
            return cachesHashList.ContainsKey(entityName);
        }

        private void Dump2File()
        {
            if (updated)
            {
                DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(path));
                if (!dir.Exists)
                {
                    dir.Create();
                }
                string dumpedString = JsonConvert.SerializeObject(cachesHashList);
                var bytes = Encoding.UTF8.GetBytes(
                        dumpedString);
                try
                {
                    using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(bytes, 0, bytes.Length);
                    }
                    updated = false;
                    LogHelper.LogDebug("dumped: " + cachesHashList.Count);
                }
                catch (Exception ex)
                {
                    LogHelper.LogError(ex);
                }
            }
            timer.Change(dumpPeriodMS, Timeout.Infinite);
        }

        private void Recover()
        {
            try
            {
                if (File.Exists(path))
                {
                    using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, (int)fs.Length);
                        var dumpedString = Encoding.UTF8.GetString(buffer);
                        cachesHashList = JsonConvert.DeserializeObject<Dictionary<string, string>>(dumpedString);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }
        }
    }

}
