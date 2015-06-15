using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using UnityInfrastructure.Logging;

namespace DbUpdater
{
	public class DbUpdate
	{

		private static Regex _VersionRegEx= new Regex("\\d+\\.\\d+\\.\\d+\\.\\d+");

		private const string C_VERSIONTABLE ="dbversion";
		private const string C_SCHEMA = "dbo";

		private const string C_INSER_VERSION_ROW =
			@"INSERT into [dbo].[dbVersion] ([ModuleID], [Major],[Minor],[Build],[Revision],[Comment], [DateInsert]) 
		VALUES(@ModuleID, @Major,@Minor,@Build,@Revision,@Comment,@DateInsert) ";

		private const string C_CREATEVERSIONTABLE = @"CREATE TABLE [dbo].[dbVersion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ModuleID] [varchar](100) NOT NULL,
	[Major] [int] NOT NULL,
	[Minor] [int] NOT NULL,
	[Build] [int] NOT NULL,
	[Revision] [int] NOT NULL,
	[DateInsert] [datetime] NOT NULL,
	[Comment] [varchar](max) NULL,
	CONSTRAINT [PK_module_Id] PRIMARY KEY CLUSTERED (	[Id] ASC) , 
	CONSTRAINT [IX_Unique_Ver] UNIQUE NONCLUSTERED (	
		[ModuleID] ASC,
		[Major] DESC,
		[Minor] DESC,
		[Build] DESC,
		[Revision] DESC
  )
)";

		private readonly string _cnString ;
		private readonly string[] _scriptsList;


		public DbUpdate(string cnString, string[] scriptsList)
		{
			using (var tc = new UnityTraceContext())
			{
				_cnString = cnString;
				_scriptsList = scriptsList;
			}
		}

		public DbUpdate(string cnString, string scriptsFolder)
		{
			using (var tc = new UnityTraceContext())
			{
				_cnString = cnString;
				if (!Directory.Exists(scriptsFolder)) throw new Exception("Directory " + scriptsFolder + " does not exists");
				_scriptsList = Directory.GetFiles(scriptsFolder,"*.sql",SearchOption.AllDirectories);
				tc.TraceMessage("_cnString=" + _cnString + " _scriptsFolder=" + scriptsFolder);
			}
		}

		public void Doupdate()
		{
			using (var tc = new UnityTraceContext())
			{
				var scriptsInfo = loadScriptInfo();
				foreach (var scriptsForModule in scriptsInfo)
				{
					updateDbForModule(scriptsForModule);
				}
			}
		}


		private void updateDbForModule(KeyValuePair<string, List<ScriptInfo>> scriptsForModule)
		{
			using (var ts = new TransactionScope())
			{
				using (var cn = new SqlConnection(_cnString))
				{
					cn.Open();
					var dbcurverForModule = getDbVersion(cn, scriptsForModule.Key);
					var scriptsInfoToExecute = scriptsForModule.Value.Where(v => v.Version > dbcurverForModule).ToList();
					if (scriptsInfoToExecute.Count > 0)
					{
						executeScripts(scriptsInfoToExecute, cn);
					}
				}
				ts.Complete();
			}
		}


		private void updateDbVersion(SqlConnection cn, ScriptInfo scriptInfo)
		{
			using (var tc = new UnityTraceContext())
			{
				var cmd = cn.CreateCommand();
				cmd.CommandText = C_INSER_VERSION_ROW;
				
				var param = cmd.CreateParameter();
				param.ParameterName = "ModuleID";
				param.Value = scriptInfo.Module;
				cmd.Parameters.Add(param);

				param = cmd.CreateParameter();
				param.ParameterName = "Major"; 
				param.Value  = scriptInfo.Version.Major;
				cmd.Parameters.Add(param);

				param = cmd.CreateParameter();
				param.ParameterName = "Minor"; 
				param.Value  = scriptInfo.Version.Minor;
				cmd.Parameters.Add(param);
				
				param = cmd.CreateParameter();
				param.ParameterName = "Build"; 
				param.Value  = scriptInfo.Version.Build;
				cmd.Parameters.Add(param);

				param = cmd.CreateParameter();
				param.ParameterName = "Revision"; 
				param.Value  = scriptInfo.Version.Revision;
				cmd.Parameters.Add(param);

				param = cmd.CreateParameter();
				param.ParameterName = "Comment";
				param.Value = scriptInfo.Comment;
				cmd.Parameters.Add(param);

				param = cmd.CreateParameter();
				param.ParameterName = "DateInsert";
				param.Value = DateTime.Now;
				cmd.Parameters.Add(param);

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
					updateDbVersion(cn, script);
				});
			}
		}

		private Dictionary<string,List<ScriptInfo>> loadScriptInfo()
		{
			using (var tc = new UnityTraceContext())
			{
				var scriptInfos = new Dictionary<string,List<ScriptInfo>>();
				foreach(var scriptPath in _scriptsList)
				{
					tc.TraceMessage("Processing script " + scriptPath);
					var scriptInfo = new ScriptInfo();
					scriptInfo.Path = scriptPath;
					scriptInfo.ScriptName= Path.GetFileName(scriptPath);
					var matches = _VersionRegEx.Match(Path.GetFileName(scriptPath));
					if (matches.Length != 0)
					{
						scriptInfo.Version = new Version(matches.Value);
					}
					else
					{
						tc.TraceMessage("Script " + scriptInfo + " not added to list since could not infer version from name");
						continue;
					}
					int pos = Path.GetFileName(scriptPath).IndexOf('_');
					if (pos != -1)
					{
						scriptInfo.Module = Path.GetFileName(scriptPath).Substring(0, pos);
					}
					else
					{
						tc.TraceMessage("Script " + scriptInfo + " not added to list since could not infer the module name");
						continue;
					}
					List<ScriptInfo> scriptListforModule = null;
					if (!scriptInfos.TryGetValue(scriptInfo.Module, out scriptListforModule))
					{
						scriptListforModule  = new List<ScriptInfo>();
						scriptInfos.Add(scriptInfo.Module, scriptListforModule);
					}
					if (scriptListforModule.Contains(scriptInfo))
						throw new Exception("script with version " + scriptInfo.Version + " has already been added to the list for module " + scriptInfo.Module);
					scriptListforModule.Add(scriptInfo);
				};
				foreach (var scriptset in scriptInfos.Values)
					scriptset.Sort();
				return scriptInfos;
			}
		}


	

		private Version getDbVersion(SqlConnection cn, string moduleId)
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
					lcmd.CommandText = "SELECT top 1 * from " + C_SCHEMA + "." + C_VERSIONTABLE + " where moduleid=@moduleid order by major desc,minor desc,build desc, revision desc";
					var parammoduleid = lcmd.CreateParameter();
					parammoduleid.ParameterName = "moduleid";
					parammoduleid.Value = moduleId;
					lcmd.Parameters.Add(parammoduleid);
					using (var rd = lcmd.ExecuteReader())
					{
						if (rd.HasRows)
						{
							rd.Read();
							ver = new Version((int) rd["major"], (int) rd["minor"], (int) rd["build"], (int) rd["revision"]);
						}
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

	public class ScriptInfo : IComparable, IEquatable<ScriptInfo>

	{
		public string Path;
		public Version Version;
		public string ScriptName;
		public string Module;
		public string Comment="";

		public int CompareTo(object obj)
		{
			return this.Version.CompareTo(((ScriptInfo)obj).Version);
		}

		public bool Equals(ScriptInfo other)
		{
			return this.Version == other.Version;
		}
	}
}
