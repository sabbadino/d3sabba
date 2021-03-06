﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml;
using UnityInfrastructure.Logging;

namespace DbUpdater
{

	

	public class DbUpdateByVersion
	{

		private static Regex _VersionRegEx= new Regex("\\d+\\.\\d+\\.\\d+\\.\\d+");

		private const string C_VERSIONTABLE ="dbversion";
		private const string C_SCHEMA = "#service#";
		private const string C_SERVICE = "#service#";
		private const string C_SEAPARATOR = "-";


		private const string C_INSERT_VERSION_ROW =
			@"INSERT into [" + C_SCHEMA + "].[" + C_VERSIONTABLE + @"] ([Major],[Minor],[Build],[Revision],[Comment], [DateInsert]) 
		VALUES(@Major,@Minor,@Build,@Revision,@Comment,@DateInsert) ";
		private const string C_CREATESOASCHEMA = @"CREATE SCHEMA [" + C_SCHEMA + @"]";
		private const string C_CREATEVERSIONTABLE = @"CREATE TABLE [" + C_SCHEMA + "].[" + C_VERSIONTABLE + @"](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Major] [int] NOT NULL,
	[Minor] [int] NOT NULL,
	[Build] [int] NOT NULL,
	[Revision] [int] NOT NULL,
	[DateInsert] [datetime] NOT NULL,
	[Comment] [varchar](max) NULL,
	CONSTRAINT [PK_" + C_VERSIONTABLE + @"] PRIMARY KEY CLUSTERED (	[Id] ASC) , 
	CONSTRAINT [IX_Unique_Ver] UNIQUE NONCLUSTERED (	
		[Major] DESC,
		[Minor] DESC,
		[Build] DESC,
		[Revision] DESC
  )
)";

		private readonly XmlDocument _configXml = new XmlDocument();
		private readonly string[] _scriptsList;
		private readonly string _targetEnvironment;
		private readonly UpdateStrategy _updateStrategy;
		public DbUpdateByVersion(UpdateStrategy updateStrategy, string targetEnvironment, string configFile, string[] scriptsList)
		{
			using (var tc = new UnityTraceContext())
			{
				_updateStrategy = updateStrategy;
				_targetEnvironment = targetEnvironment;
				if(!File.Exists(configFile)) throw new Exception("File " + configFile + " does not exists");
				_configXml.Load(configFile);
				tc.TraceMessage(_configXml.OuterXml);
				_scriptsList = scriptsList;
				tc.TraceMessage("configFile=" + configFile + " _scriptsList.Length=" + scriptsList.Length);
			}
		}

		public DbUpdateByVersion(UpdateStrategy updateStrategy , string targetEnvironment, string configFile, string scriptsFolder)
		{
			using (var tc = new UnityTraceContext())
			{
				_updateStrategy = updateStrategy;
				_targetEnvironment = targetEnvironment;
				if (!File.Exists(configFile)) throw new Exception("File " + configFile + " does not exists");
				_configXml.Load(configFile);
				tc.TraceMessage(_configXml.OuterXml);
				if (!Directory.Exists(scriptsFolder)) throw new Exception("Directory " + scriptsFolder + " does not exists");
				_scriptsList = Directory.GetFiles(scriptsFolder,"*.sql",SearchOption.AllDirectories);
				tc.TraceMessage("configFile=" + configFile + " _scriptsFolder=" + scriptsFolder);
			}
		}

		public void Doupdate()
		{
			using (var tc = new UnityTraceContext())
			{
				var scriptsInfo = loadScriptInfo();
				foreach (var scriptsForSchema in scriptsInfo)
				{
					updateDbForSchema(scriptsForSchema);
				}
			}
		}


		private void updateDbForSchema(KeyValuePair<string, List<ScriptInfoByVersion>> scriptsForSchema)
		{
			using (var tc = new UnityTraceContext())
			{
				// get all the db potententially involved in the update
				var cnstrings = getCnStringsFromScriptConfiguration(scriptsForSchema.Value);
				foreach (var cnString in cnstrings)
				{
						tc.TraceMessage("cnstring=" + cnString);
						using (var ts = new TransactionScope())
						{
							using (var cn = new SqlConnection(cnString))
							{
								cn.Open();
								var scriptsInfoToExecute = new List<ScriptInfoByVersion>();
								// cerco gli script da eseguire per quel db .. escludo quelli script che su quel db non deono andare ( v.ConnectionStrings.Contains(cnString) )
								if (_updateStrategy == UpdateStrategy.Full)
								{
									var dbcurUpdatesForSchema = getDbExistingUpdates(cn, scriptsForSchema.Key);
									scriptsInfoToExecute =
										scriptsForSchema.Value.Where(
											v => !dbcurUpdatesForSchema.Contains(v.Version) && v.ConnectionStrings.Contains(cnString)).ToList();
								}
								else
								{
									var dbcurVer = getDbVersion(cn, scriptsForSchema.Key);
									scriptsInfoToExecute =
										scriptsForSchema.Value.Where(v => v.Version > dbcurVer && v.ConnectionStrings.Contains(cnString)).ToList();
								}

								if (scriptsInfoToExecute.Count > 0)
								{
									executeScripts(scriptsInfoToExecute, cn);
								}
							}
							ts.Complete();
						}
					
				}
			}
		}

		private List<string> getCnStringsFromScriptConfiguration(List<ScriptInfoByVersion> scripts)
		{
			using (var tc = new UnityTraceContext())
			{
				var listcnstring = new List<string>();
				foreach (var script in scripts)
				{
					tc.TraceMessage("Processing script.ScriptName=" + script.ScriptName + "script.Configuration=" +
					                script.Configuration);
					var nodes =
						_configXml.SelectNodes("/config/context_configurations/context_configuration[@name='" + script.Configuration +
						                       "']/context/@name");
					if (nodes.Count == 0) throw new Exception("no configuration named " + script.Configuration + " was found");
					var envnode = _configXml.SelectSingleNode("/config/environments/environment[@name='" + _targetEnvironment + "']");
					if (envnode == null) throw new Exception("no environment named " + _targetEnvironment + " was found");
					foreach (XmlNode node in nodes)
					{
						tc.TraceMessage("script is in context " + node.InnerText);
						var cnstringAttr = envnode.SelectSingleNode("context[@name='" + node.InnerText + "']/@connection_string");
						if (cnstringAttr != null)
						{
							script.ConnectionStrings.Add(cnstringAttr.InnerText);
							if (!listcnstring.Contains(cnstringAttr.InnerText))
							{
								tc.TraceMessage("Adding cnstring " + cnstringAttr.InnerText);
								listcnstring.Add(cnstringAttr.InnerText);
							}
						}
					}
				}
				return listcnstring;
			}
		}


		private void updateDbVersion(SqlConnection cn, ScriptInfoByVersion scriptInfoByVersion)
		{
			using (var tc = new UnityTraceContext())
			{
				var cmd = cn.CreateCommand();
				cmd.CommandText = C_INSERT_VERSION_ROW.Replace(C_SERVICE, scriptInfoByVersion.Schema);
				

				var param = cmd.CreateParameter();
				param.ParameterName = "Major"; 
				param.Value  = scriptInfoByVersion.Version.Major;
				cmd.Parameters.Add(param);

				param = cmd.CreateParameter();
				param.ParameterName = "Minor"; 
				param.Value  = scriptInfoByVersion.Version.Minor;
				cmd.Parameters.Add(param);
				
				param = cmd.CreateParameter();
				param.ParameterName = "Build"; 
				param.Value  = scriptInfoByVersion.Version.Build;
				cmd.Parameters.Add(param);

				param = cmd.CreateParameter();
				param.ParameterName = "Revision"; 
				param.Value  = scriptInfoByVersion.Version.Revision;
				cmd.Parameters.Add(param);

				param = cmd.CreateParameter();
				param.ParameterName = "Comment";
				param.Value = scriptInfoByVersion.Comment;
				cmd.Parameters.Add(param);

				param = cmd.CreateParameter();
				param.ParameterName = "DateInsert";
				param.Value = DateTime.Now;
				cmd.Parameters.Add(param);

				cmd.ExecuteNonQuery();
			}
		}

		private void executeScripts(List<ScriptInfoByVersion> scripts, SqlConnection cn)
		{
			using (var tc = new UnityTraceContext())
			{
				scripts.ForEach(script =>
				{
					var cmd = cn.CreateCommand();
					cmd.CommandText = File.ReadAllText(script.Path);
					cmd.ExecuteNonQuery();
					updateDbVersion(cn, script);
				});
		}
		}


		private Dictionary<string,List<ScriptInfoByVersion>> loadScriptInfo()
		{
			using (var tc = new UnityTraceContext())
			{
				var scriptInfos = new Dictionary<string,List<ScriptInfoByVersion>>();
				foreach(var scriptPath in _scriptsList)
				{
					tc.TraceMessage("Processing script " + scriptPath);
					var scriptInfo = new ScriptInfoByVersion();
					scriptInfo.Path = scriptPath;
					scriptInfo.ScriptName= Path.GetFileNameWithoutExtension(scriptPath);
					var matches = _VersionRegEx.Match(scriptInfo.ScriptName);
					if (matches.Length != 0)
					{
						scriptInfo.Version = new Version(matches.Value);
					}
					else
					{
						throw new Exception("Script " + scriptInfo + " not added to list since could not infer version from name");
						//continue;
					}
					int pos = scriptInfo.ScriptName.IndexOf('-');
					if (pos != -1)
					{
						scriptInfo.Schema = scriptInfo.ScriptName.Substring(0, pos);
					}
					else
					{
						throw new Exception("Script " + scriptInfo + " not added to list since could not infer the schema");
						//continue;
					}

					int pos1 = scriptInfo.ScriptName.LastIndexOf('-');
					if (pos1 == pos)
					{
						throw new Exception("Script " + scriptInfo + " not added to list since could not infer the Configuration  name (only one _ )");
						//continue;
					}
					if (pos1 != -1)
					{
						if (pos1 == (scriptInfo.ScriptName.Length -1))
						{
							tc.TraceMessage("Script " + scriptInfo + " not added to list since could not infer the Configuration  name ( nothing after _ )");
							continue;
						}
						scriptInfo.Configuration = scriptInfo.ScriptName.Substring(pos1 + 1, scriptInfo.ScriptName.Length - pos1 -1);
					}
					else
					{
						tc.TraceMessage("Script " + scriptInfo + " not added to list since could not infer the Configuration  name");
						continue;
					}

					List<ScriptInfoByVersion> scriptListforSchema = null;
					if (!scriptInfos.TryGetValue(scriptInfo.Schema, out scriptListforSchema))
					{
						scriptListforSchema  = new List<ScriptInfoByVersion>();
						scriptInfos.Add(scriptInfo.Schema, scriptListforSchema);
					}
					if (scriptListforSchema.Contains(scriptInfo))
						throw new Exception("script with version " + scriptInfo.Version + " has already been added to the list for schema" + scriptInfo.Schema);
					scriptListforSchema.Add(scriptInfo);
				};
				foreach (var scriptset in scriptInfos.Values)
					scriptset.Sort();
				return scriptInfos;
			}
		}



		private List<Version> getDbExistingUpdates(SqlConnection cn, string schema)
		{
			using (var tc = new UnityTraceContext())
			{
				var updates = new List<Version>();
				var lcmd = cn.CreateCommand();

				lcmd.CommandText = "SELECT count(*) FROM INFORMATION_SCHEMA.schemata WHERE SCHEMA_NAME = '" + C_SCHEMA.Replace(C_SERVICE,schema) + "'";
				if ((int)lcmd.ExecuteScalar() == 0)
				{
					createsoaSchema(cn,schema);
				}
				lcmd.CommandText = "SELECT count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + C_SCHEMA.Replace(C_SERVICE, schema) + "' AND  TABLE_NAME = '" + C_VERSIONTABLE + "'";
				if ((int)lcmd.ExecuteScalar() == 0)
				{
					createVersionTable(cn,schema);
				}
				else
				{
					lcmd.CommandText = "SELECT * from " + C_SCHEMA.Replace(C_SERVICE, schema) + "." + C_VERSIONTABLE ;
					using (var rd = lcmd.ExecuteReader())
					{
						if (rd.HasRows)
						{
							while (rd.Read())
							{
								updates.Add(new Version((int)rd["major"], (int)rd["minor"], (int)rd["build"], (int)rd["revision"]));
							}
						}
					}
				}
				tc.TraceMessage("updates.Count=" + updates.Count() + " schema=" + schema);
				return updates;
			}
		}

		private Version getDbVersion(SqlConnection cn, string schema)
		{
			using (var tc = new UnityTraceContext())
			{
				var ver = new Version(0, 0, 0, 0);
				var lcmd = cn.CreateCommand();
				lcmd.CommandText = "SELECT count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + C_SCHEMA.Replace(C_SERVICE, schema) + "' AND  TABLE_NAME = '" + C_VERSIONTABLE + "'";
				if ((int) lcmd.ExecuteScalar() == 0)
				{
					createVersionTable(cn,schema);
				}
				else
				{
					lcmd.CommandText = "SELECT top 1 * from " + C_SCHEMA + "." + C_VERSIONTABLE + " order by major desc,minor desc,build desc, revision desc";
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

		private void createVersionTable(SqlConnection cn, string schema)
		{
			using (var tc = new UnityTraceContext())
			{
				var lcmd = cn.CreateCommand();
				lcmd.CommandText = C_CREATEVERSIONTABLE.Replace(C_SERVICE, schema);
				lcmd.ExecuteNonQuery();
			}
		}

		private void createsoaSchema(SqlConnection cn, string schema)
		{
			using (var tc = new UnityTraceContext())
			{
				var lcmd = cn.CreateCommand();
				lcmd.CommandText = C_CREATESOASCHEMA.Replace(C_SERVICE, schema);
				lcmd.ExecuteNonQuery();
			}
		}

	}

	public class ScriptInfoByVersion : IComparable, IEquatable<ScriptInfoByVersion>

	{
		public string Path;
		public Version Version;
		public string ScriptName;
		public string Schema;
		public string Configuration;
		public string Comment="";
		public List<string> ConnectionStrings = new List<string>();

		public int CompareTo(object obj)
		{
			return this.Version.CompareTo(((ScriptInfoByVersion)obj).Version);
		}


		public bool Equals(ScriptInfoByVersion other)
		{
			return this.Version == other.Version;
		}
	}

}
