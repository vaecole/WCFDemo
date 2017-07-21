using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Octopus.Entity;
using Octopus.Utility;

namespace WCFDemo
{
    internal class SQLHelper
    {
        private static SQLHelper _instance = new SQLHelper();
        public static SQLHelper Instance { get { return _instance; } }
        SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder("server=123.57.16.246;database=CollectionData;uid=sa;password=skieer@2015;");
        public List<TaskInfoEntity> GetRealTimeTasks()
        {
            List<TaskInfoEntity> entityList = null;

            using (SqlConnection conn = new SqlConnection(scsb.ConnectionString))
            {
                MSSqlSPHelper spHelper = new MSSqlSPHelper(conn, "CollectionData.dbo.getRealTimeTask");

                using (SqlDataReader dataReader = spHelper.executeDataReader())
                {
                    entityList = new List<TaskInfoEntity>();

                    while (dataReader.Read())
                    {
                        TaskInfoEntity entity = new TaskInfoEntity();

                        entity.TaskId = dataReader["r_TaskId"].ToString().Trim();
                        entity.UserId = dataReader["r_CreationUserId"].ToString().Trim();

                        if (dataReader["r_TaskType"] != DBNull.Value)
                        {
                            entity.TaskType = Int32.Parse(dataReader["r_TaskType"].ToString().Trim());
                        }

                        entity.TaskName = dataReader["r_TaskName"].ToString().Trim();
                        entity.Description = dataReader["r_TaskDescription"].ToString().Trim();

                        if (dataReader["r_TaskStatus"] != DBNull.Value)
                        {
                            entity.Status = Int32.Parse(dataReader["r_TaskStatus"].ToString().Trim());
                        }

                        if (dataReader["r_EffectiveFrom"] != DBNull.Value)
                        {
                            entity.EffectiveFrom = DateTime.Parse(dataReader["r_EffectiveFrom"].ToString().Trim());
                        }

                        if (dataReader["r_EffectiveTo"] != DBNull.Value)
                        {
                            entity.EffectiveTo = DateTime.Parse(dataReader["r_EffectiveTo"].ToString().Trim());
                        }
                        if (dataReader["r_ScheduleType"] != DBNull.Value)
                        {
                            entity.ScheduleType = Int32.Parse(dataReader["r_ScheduleType"].ToString().Trim());
                        }

                        entity.ScheduleDate = dataReader["r_ScheduleDateList"].ToString().Trim();
                        entity.ScheduleTime = dataReader["r_ScheduleTimeList"].ToString().Trim();
                        entity.Comment = dataReader["r_Comment"].ToString().Trim();
                        entityList.Add(entity);
                    }
                }
            }


            return entityList;
        }
    }
}
