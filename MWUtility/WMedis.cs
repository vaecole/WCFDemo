using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;

namespace MWUtility
{
    public class WMedis
    {
        private object newCacheLock = new object();
        private Dictionary<string, string> cachesHashList = new Dictionary<string, string>();
        private string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dump.octdb");
        private int dumpPeriodMS = 1 * 60 * 1000;
        private bool updated = false;
        private static IFormatter formatter = new BinaryFormatter();

        private WMedis()
        {
            Recover();
            timer = new Timer(
                    (t) => { Dump2File(); },
                    null,
                    dumpPeriodMS,
                    Timeout.Infinite);
        }
        public static WMedis Instance = new WMedis();

        private Timer timer;

        public void Cache<T>(string entityName, T entity, bool persistenceNow = false) where T : new()
        {
            lock (newCacheLock)
            {
                if (!cachesHashList.ContainsKey(entityName))
                {
                    cachesHashList.Add(entityName, SerializeObject(entity));
                }
                else
                {
                    cachesHashList[entityName] = SerializeObject(entity);
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

        public TResult Peek<TResult>(string entityName) where TResult : class, new()
        {
            if (!cachesHashList.ContainsKey(entityName))
            {
                return default(TResult);
            }
            return DeserializeObject<TResult>(cachesHashList[entityName]);
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
                string dumpedString = SerializeObject(cachesHashList);
                var bytes = Encoding.UTF8.GetBytes(
                        dumpedString);
                try
                {
                    using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(bytes, 0, bytes.Length);
                    }
                    updated = false;
                    LogHelper.Debug("dumped: " + cachesHashList.Count);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex);
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
                        cachesHashList = DeserializeObject<Dictionary<string, string>>(dumpedString);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }

        // Create a User object and serialize it to a JSON stream.  
        public static string SerializeObject<T>(T entity)
        {
            MemoryStream ms = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            ser.WriteObject(ms, entity);
            byte[] json = ms.ToArray();
            ms.Close();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }

        public static TResult DeserializeObject<TResult>(string json)
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(TResult));
            var obj = (TResult)ser.ReadObject(ms);
            ms.Close();
            return obj;
        }
    }
}
