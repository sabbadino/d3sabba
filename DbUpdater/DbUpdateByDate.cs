//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.ComponentModel.Design;
//using System.Configuration;
//using System.Data.SqlClient;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using System.Transactions;
//using System.Xml;
//using UnityInfrastructure.Logging;

//namespace DbUpdater
//{

	

//	public class DbUpdateByDate
//	{

//		private static Regex _DateVersionRegEx = new Regex("\\d+\\.\\d+\\.\\d+\\.\\d+\\.\\d+");

//		private const string C_VERSIONTABLE = "dbVersionByDate";
//		private const string C_SCHEMA = "soa";

//		private const string C_INSER_VERSION_ROW =
//			@"INSERT into [" + C_SCHEMA + "].[" + C_VERSIONTABLE + @"] ([ModuleID], [DateVersion],[Comment], [DateInsert]) 
//		VALUES(@ModuleID, @DateVersion,@Comment,@DateInsert) ";


//		private const string C_CREATESOASCHEMA = @"CREATE SCHEMA [" + C_SCHEMA + @"]";
//		private const string C_CREATEVERSIONTABLE = @"CREATE TABLE [" + C_SCHEMA +  @"].[" + C_VERSIONTABLE + @"](
//	[Id] [int] IDENTITY(1,1) NOT NULL,
//	[ModuleID] [varchar](100) NOT NULL,
//	[DateVersion] [datetime] NOT NULL,
//	[DateInsert] [datetime] NOT NULL,
//	[Comment] [varchar](max) NULL,
//	CONSTRAINT [PK_" + C_VERSIONTABLE + @"] PRIMARY KEY CLUSTERED (	[Id] ASC) , 
//	CONSTRAINT [IX_Unique_VerDate] UNIQUE NONCLUSTERED (	
//		[ModuleID] ASC,
//		[DateVersion] DESC
//  )
//)";

//		private readonly XmlDocument _configXml = new XmlDocument();
//		private readonly string[] _scriptsList;
//		private readonly string _targetEnvironment;
//		private readonly UpdateStrategy _updateStrategy;
//		public DbUpdateByDate(UpdateStrategy updateStrategy, string targetEnvironment, string configFile, string[] scriptsList)
//		{
//			using (var tc = new UnityTraceContext())
//			{
//				_updateStrategy = updateStrategy;
//				_targetEnvironment = targetEnvironment;
//				if(!File.Exists(configFile)) throw new Exception("File " + configFile + " does not exists");
//				_configXml.Load(configFile);
//				tc.TraceMessage(_configXml.OuterXml);
//				_scriptsList = scriptsList;
//				tc.TraceMessage("configFile=" + configFile + " _scriptsList.Length=" + scriptsList.Length);
//			}
//		}

//		public DbUpdateByDate(UpdateStrategy updateStrategy,string targetEnvironment, string configFile, string scriptsFolder)
//		{
//			using (var tc = new UnityTraceContext())
//			{
//				_updateStrategy = updateStrategy;
//				_targetEnvironment = targetEnvironment;
//				if (!File.Exists(configFile)) throw new Exception("File " + configFile + " does not exists");
//				_configXml.Load(configFile);
//				tc.TraceMessage(_configXml.OuterXml);
//				if (!Directory.Exists(scriptsFolder)) throw new Exception("Directory " + scriptsFolder + " does not exists");
//				_scriptsList = Directory.GetFiles(scriptsFolder,"*.sql",SearchOption.AllDirectories);
//				tc.TraceMessage("configFile=" + configFile + " _scriptsFolder=" + scriptsFolder);
//			}
//		}

//		public void Doupdate()
//		{
//			using (var tc = new UnityTraceContext())
//			{
//				var scriptsInfo = loadScriptInfo();
//				foreach (var scriptsForModule in scriptsInfo)
//				{
//					updateDbForModule(scriptsForModule);
//				}
//			}
//		}


//		private void updateDbForModule(KeyValuePair<string, List<ScriptInfoByDate>> scriptsForModule)
//		{
//			using (var tc = new UnityTraceContext())
//			{
//				foreach (var script in scriptsForModule.Value)
//				{
//					var cnstrings = getCnStringsFromScriptConfiguration(script);
//					foreach (var cnString in cnstrings)
//					{
//						tc.TraceMessage("cnstring=" + cnString);
//						using (var ts = new TransactionScope())
//						{
//							using (var cn = new SqlConnection(cnString))
//							{
//								cn.Open();
//								var scriptsInfoToExecute = new List<ScriptInfoByDate>();
//								if (_updateStrategy == UpdateStrategy.Full)
//								{
//									var dbcurUpdatesForModule = getDbExistingUpdates(cn, scriptsForModule.Key);
//									scriptsInfoToExecute =
//										scriptsForModule.Value.Where(v => !dbcurUpdatesForModule.Contains(v.DateVersion)).ToList();
//								}
//								else
//								{
//									var dbcurVer = getDbVersion(cn, scriptsForModule.Key);
//									scriptsInfoToExecute =
//										scriptsForModule.Value.Where(v => v.DateVersion > dbcurVer).ToList();
//								}
//								tc.TraceMessage("scriptsInfoToExecute.Count=" + scriptsInfoToExecute.Count + " for module " + script.Schema) ; 
//								if (scriptsInfoToExecute.Count > 0)
//								{
//									executeScripts(scriptsInfoToExecute, cn);
//								}
//							}
//							ts.Complete();
//						}
//					}
//				}
//			}
//		}

//		private List<string> getCnStringsFromScriptConfiguration(ScriptInfoByDate script)
//		{
//			using (var tc = new UnityTraceContext())
//			{
//				tc.TraceMessage("Processing script.ScriptName=" + script.ScriptName + "script.Configuration=" + script.Configuration);
//				var listcnstring = new List<string>();
//				var nodes =
//					_configXml.SelectNodes("/config/context_configurations/context_configuration[@name='" + script.Configuration +
//																 "']/context/@name");
//				if (nodes.Count == 0) throw new Exception("no configuration named " + script.Configuration + " was found");
//				var envnode = _configXml.SelectSingleNode("/config/environments/environment[@name='" + _targetEnvironment + "']");
//				if (envnode == null) throw new Exception("no environment named " + _targetEnvironment + " was found");
//				foreach (XmlNode node in nodes)
//				{
//					tc.TraceMessage("script is in context " + node.InnerText);
//					var cnstringAttr = envnode.SelectSingleNode("context[@name='" + node.InnerText + "']/@connection_string");
//					if (cnstringAttr != null)
//					{
//						tc.TraceMessage("Adding cnstring " + cnstringAttr.InnerText);
//						listcnstring.Add(cnstringAttr.InnerText);
//					}
//				}
//				return listcnstring;
//			}
//		}


//		private void updateDbVersion(SqlConnection cn, ScriptInfoByDate scriptInfoByDate)
//		{
//			using (var tc = new UnityTraceContext())
//			{
//				var cmd = cn.CreateCommand();
//				cmd.CommandText = C_INSER_VERSION_ROW;
				
//				var param = cmd.CreateParameter();
//				param.ParameterName = "ModuleID";
//				param.Value = scriptInfoByDate.Schema;
//				cmd.Parameters.Add(param);

//				param = cmd.CreateParameter();
//				param.ParameterName = "DateVersion";
//				param.Value = scriptInfoByDate.DateVersion;
//				cmd.Parameters.Add(param);

//				param = cmd.CreateParameter();
//				param.ParameterName = "Comment";
//				param.Value = scriptInfoByDate.Comment;
//				cmd.Parameters.Add(param);

//				param = cmd.CreateParameter();
//				param.ParameterName = "DateInsert";
//				param.Value = DateTime.Now;
//				cmd.Parameters.Add(param);

//				cmd.ExecuteNonQuery();
//			}
//		}

//		private void executeScripts(List<ScriptInfoByDate> scriptsInfoToExecute, SqlConnection cn)
//		{
//			using (var tc = new UnityTraceContext())
//			{
//				scriptsInfoToExecute.ForEach(script =>
//				{
//					var cmd = cn.CreateCommand();
//					cmd.CommandText = File.ReadAllText(script.Path);
//					cmd.ExecuteNonQuery();
//					updateDbVersion(cn, script);
//				});
//			}
//		}

//		private Dictionary<string,List<ScriptInfoByDate>> loadScriptInfo()
//		{
//			using (var tc = new UnityTraceContext())
//			{
//				var scriptInfos = new Dictionary<string,List<ScriptInfoByDate>>();
//				foreach(var scriptPath in _scriptsList)
//				{
//					tc.TraceMessage("Processing script " + scriptPath);
//					var scriptInfo = new ScriptInfoByDate();
//					scriptInfo.Path = scriptPath;
//					scriptInfo.ScriptName= Path.GetFileNameWithoutExtension(scriptPath);
//					var matches = _DateVersionRegEx.Match(scriptInfo.ScriptName);
//					if (matches.Length != 0)
//					{
//						var tmp = matches.Value.Split('.');
//						if (tmp.Length != 5)
//						{
//							tc.TraceMessage("Script " + scriptInfo + " not added to list since could not infer version from name (no 5 blocks of digits");
//							continue;
//						}

//						scriptInfo.DateVersion = new DateTime(int.Parse(tmp[0]),int.Parse(tmp[1]),int.Parse(tmp[2]),int.Parse(tmp[3]),int.Parse(tmp[4]),0);
//					}
//					else
//					{
//						tc.TraceMessage("Script " + scriptInfo + " not added to list since could not infer version from name (no regex match)");
//						continue;
//					}
//					int pos = scriptInfo.ScriptName.IndexOf('_');
//					if (pos != -1)
//					{
//						scriptInfo.Schema = scriptInfo.ScriptName.Substring(0, pos);
//					}
//					else
//					{
//						tc.TraceMessage("Script " + scriptInfo + " not added to list since could not infer the modulename");
//						continue;
//					}

//					int pos1 = scriptInfo.ScriptName.LastIndexOf('_');
//					if (pos1 == pos)
//					{
//						tc.TraceMessage("Script " + scriptInfo + " not added to list since could not infer the Configuration  name (only one _ )");
//						continue;
//					}
//					if (pos1 != -1)
//					{
//						if (pos1 == (scriptInfo.ScriptName.Length -1))
//						{
//							tc.TraceMessage("Script " + scriptInfo + " not added to list since could not infer the Configuration  name ( nothing after _ )");
//							continue;
//						}
//						scriptInfo.Configuration = scriptInfo.ScriptName.Substring(pos1 + 1, scriptInfo.ScriptName.Length - pos1 -1);
//					}
//					else
//					{
//						tc.TraceMessage("Script " + scriptInfo + " not added to list since could not infer the Configuration  name");
//						continue;
//					}

//					List<ScriptInfoByDate> scriptListforModule = null;
//					if (!scriptInfos.TryGetValue(scriptInfo.Schema, out scriptListforModule))
//					{
//						scriptListforModule  = new List<ScriptInfoByDate>();
//						scriptInfos.Add(scriptInfo.Schema, scriptListforModule);
//					}
//					if (scriptListforModule.Contains(scriptInfo))
//						throw new Exception("script with version " + scriptInfo.DateVersion + " has already been added to the list for module " + scriptInfo.Schema);
//					scriptListforModule.Add(scriptInfo);
//				};
//				foreach (var scriptset in scriptInfos.Values)
//					scriptset.Sort();
//				return scriptInfos;
//			}
//		}




//		private DateTime getDbVersion(SqlConnection cn, string moduleId)
//		{
//			using (var tc = new UnityTraceContext())
//			{
//				DateTime ver = DateTime.MinValue;
//				var lcmd = cn.CreateCommand();
//				lcmd.CommandText = "SELECT count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + C_SCHEMA +
//													 "' AND  TABLE_NAME = '" + C_VERSIONTABLE + "'";
//				if ((int) lcmd.ExecuteScalar() == 0)
//				{
//					createVersionTable(cn);
//				}
//				else
//				{
//					lcmd.CommandText = "SELECT top 1 * from " + C_SCHEMA + "." + C_VERSIONTABLE +
//														 " where moduleid=@moduleid order by DateVersion desc";
//					var parammoduleid = lcmd.CreateParameter();
//					parammoduleid.ParameterName = "moduleid";
//					parammoduleid.Value = moduleId;
//					lcmd.Parameters.Add(parammoduleid);
//					using (var rd = lcmd.ExecuteReader())
//					{
//						if (rd.HasRows)
//						{
//							rd.Read();
//							ver = (DateTime) rd["DateVersion"];
//						}
//					}
//				}
//				tc.TraceMessage("dbDateVersion=" + ver.ToString("yyyy-MM-dd hh.mm"));
//				return ver;
//			}
//		}

//		private List<DateTime> getDbExistingUpdates(SqlConnection cn, string moduleId)
//		{
//			using (var tc = new UnityTraceContext())
//			{
//				var updates = new List<DateTime>();
//				var lcmd = cn.CreateCommand();
//				lcmd.CommandText = "SELECT count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + C_SCHEMA +
//													 "' AND  TABLE_NAME = '" + C_VERSIONTABLE + "'";
//				if ((int)lcmd.ExecuteScalar() == 0)
//				{
//					createVersionTable(cn);
//				}
//				else
//				{
//					lcmd.CommandText = "SELECT * from " + C_SCHEMA + "." + C_VERSIONTABLE +
//														 " where moduleid=@moduleid order by DateVersion desc";
//					var parammoduleid = lcmd.CreateParameter();
//					parammoduleid.ParameterName = "moduleid";
//					parammoduleid.Value = moduleId;
//					lcmd.Parameters.Add(parammoduleid);
//					using (var rd = lcmd.ExecuteReader())
//					{
//						if (rd.HasRows)
//						{
//							while (rd.Read())
//							{
//								updates.Add((DateTime)rd["DateVersion"]);
//							}
//						}
//					}
//				}
//				tc.TraceMessage("updates.Count=" + updates.Count + " moduleId=" + moduleId);
//				return updates;
//			}
//		}

//		private void createVersionTable(SqlConnection cn)
//		{
//			using (var tc = new UnityTraceContext())
//			{
//				var lcmd = cn.CreateCommand();
//				lcmd.CommandText = C_CREATEVERSIONTABLE;
//				lcmd.ExecuteNonQuery();
//			}
//		}

//		private void createsoaSchema(SqlConnection cn)
//		{
//			using (var tc = new UnityTraceContext())
//			{
//				var lcmd = cn.CreateCommand();
//				lcmd.CommandText = C_CREATESOASCHEMA;
//				lcmd.ExecuteNonQuery();
//			}
//		}
//	}

//	public class ScriptInfoByDate : IComparable, IEquatable<ScriptInfoByDate>

//	{
//		public string Path;
//		public DateTime DateVersion;
//		public string ScriptName;
//		public string Schema;
//		public string Configuration;
//		public string Comment="";

//		public int CompareTo(object obj)
//		{
//			return this.DateVersion.CompareTo(((ScriptInfoByDate)obj).DateVersion);
//		}

//		public bool Equals(ScriptInfoByDate other)
//		{
//			return this.DateVersion == other.DateVersion;
//		}
//	}
//}
