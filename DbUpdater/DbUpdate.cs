using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using UnityInfrastructure.Logging;

namespace DbUpdater
{
	public class DbUpdate
	{
		private const string C_VERSIONTABLE ="dbversion";
		private const string C_SCHEMA = "dbo";

		private const string C_CREATEVERSIONTABLE = @"CREATE TABLE [dbo].[dbVersion] (
			[ModuleID] [varchar](100) NOT NULL,
			[Major] [tinyint] NOT NULL,
			[Minor] [tinyint] NOT NULL,
			[Build] [tinyint] NOT NULL,
			[Revision] [smallint] NOT NULL,
		  [Revision] [date] NOT NULL,
			[Comment] varchar(MAX) NULL
		 CONSTRAINT [PK_module_Id] PRIMARY KEY CLUSTERED (	[ModuleID] ASC ) ) ";

		private string _cnString ;
		private string _scriptsFolder ;
		private string _moduleId;
		public DbUpdate(string cnString, string scriptsFolder, string moduleId)
		{
			using (var tc = new UnityTraceContext())
			{
				_moduleId = moduleId ;
				_cnString = cnString;
				_scriptsFolder = scriptsFolder;
				tc.TraceMessage("_cnString=" + _cnString + " _scriptsFolder=" + _scriptsFolder + " _module=" + _module);
			}
		}

		public void Doupdate()
		{
			using (var tc = new UnityTraceContext())
			{
				if(!Directory.Exists(_scriptsFolder)) throw new Exception("Directory " + _scriptsFolder + " does not exists");
				using (var ts = new TransactionScope())
				{
					using (var cn = new SqlConnection(_cnString))
					{
						cn.Open();
						Version ldbCurVer = getDbVersion(cn);
						var scriptsInfo = loadScriptInfo(_scriptsFolder);
						var scriptsInfoToExecute = scriptsInfo.Where(v => v.Version > ldbCurVer).ToList();
						if (scriptsInfoToExecute.Count > 0)
						{
							executeScripts(scriptsInfoToExecute, cn);
							updateDbVersion(cn, scriptsInfoToExecute.Last());
						}
					}
					ts.Complete();
				}
			}
		}

		private void updateDbVersion(SqlConnection cn, ScriptInfo scriptInfo)
		{
			using (var tc = new UnityTraceContext())
			{
				var cmd = cn.CreateCommand();
				cmd.CommandText = String.Format(C_UpdateVersion, scriptInfo.Version.Major, scriptInfo.Version.Minor, scriptInfo.Version.Build, scriptInfo.Version.Revision);
				cmd.ExecuteNonQuery();
			}
		}

		private void executeScripts(List<ScriptInfo> scriptsInfoToExecute, SqlConnection cn)
		{
			using (var tc = new UnityTraceContext())
			{
				scriptsInfoToExecute.ForEach(script =>
				{
					var cmd = cn.CreateCommand();
					cmd.CommandText = File.ReadAllText(script.Path);
					cmd.ExecuteNonQuery();
				});
			}
		}

		private IList<ScriptInfo> loadScriptInfo( string folderPath)
		{
			var x = new List<ScriptInfo>();
			x.Sort();
			return x;
		}

		private Version getDbVersion(SqlConnection cn)
		{
			using (var tc = new UnityTraceContext())
			{
				var ver = new Version(0, 0, 0, 0);
				var lcmd = cn.CreateCommand();
				lcmd.CommandText = "SELECT count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + C_SCHEMA + "' AND  TABLE_NAME = '" + C_VERSIONTABLE + "'";
				if ((int) lcmd.ExecuteScalar() == 0)
				{
					createVersionTable(cn);
				}
				else
				{
					lcmd.CommandText = "SELECT top 1 from " + C_SCHEMA + "." + C_VERSIONTABLE + " where moduleid=@moduleid order by maior,minor,build,revision desc";
					var parammoduleid = lcmd.CreateParameter();
					parammoduleid.ParameterName = "moduleid";
					parammoduleid.Value = _moduleId;
					lcmd.Parameters.Add(parammoduleid);
					var rd = lcmd.ExecuteReader();
					if (rd.HasRows)
					{
						ver = new Version((int) rd["maior"], (int) rd["minor"], (int) rd["build"], (int) rd["revision"]);
					}
				}
				tc.TraceMessage("dbVersion=" + ver.ToString());
				return ver;
			}
		}

		private void createVersionTable(SqlConnection cn)
		{
			using (var tc = new UnityTraceContext())
			{
				var lcmd = cn.CreateCommand();
				lcmd.CommandText = C_CREATEVERSIONTABLE;
				lcmd.ExecuteNonQuery();
			}
		}
	}

	public class ScriptInfo : IComparable<Version> 
	{
		public string Path;
		public Version Version;
		public string Name;

		public int CompareTo(Version other)
		{
			return this.Version.CompareTo(other);
		}
	}
}
