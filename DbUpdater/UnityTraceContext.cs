using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using log4net;
using log4net.Core;
using System.Collections;
using System.Collections.Generic;
using System.Threading;


namespace UnityInfrastructure.Logging
{
	public sealed class UnityTraceContext : IUnityTraceContext
	{
		#region Fields

		private static readonly ILogger _Logger;

		private const char DEFAULTVALUE_TRACE_NESTING_STRING = ' ';

		private readonly DateTime _startTime = DateTime.Now;
		private string _className, _procedureName;
		private Assembly _assembly;
		private string _assemblyName;
		private Type _type;
		private int _nestingLevel;
		[ThreadStatic]
		private static int _CurrentNestingLevel;

		private const string UNKNOWN_CLASS = "unknownclass";
		private const short ASSEMBLY_NAME_MAX_LENGTH = 50;

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "UNITYEXCEPTIONSTACKTRACE")]
		public const string UNITYEXCEPTIONSTACKTRACE = "UnityExceptionStackTrace";
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "UNITYEXCEPTIONTYPE")]
		public const string UNITYEXCEPTIONTYPE = "UnityExceptionStackTrace";

		//private Dictionary<string, object> m_extrainfo = new Dictionary<string, object>();
		//private string m_extrainfo = "";
		private bool _writeEnterExitLogrows = true;

		private Level _defaultLevel = Level.Debug;

		private int _StackTrRaceUp = 2;

		#endregion

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "GenyaTraceContext")]
		static UnityTraceContext()
		{
			try
			{
				_Logger = LogManager.GetLogger("DbUpdater").Logger;
			}
			catch (Exception ex)
			{
				Console.WriteLine("static GenyaTraceContext=" + ex);
				Debug.WriteLine("static GenyaTraceContext=" + ex);
				throw;
			}
		}




		private static void LogEntrySafe(string msg, Level level)
		{
			try
			{
				_Logger.Log(typeof(UnityTraceContext), level, msg, null);
			}
			catch (Exception ex)
			{
				Console.WriteLine("LogEntrySafe=" + ex);
				Debug.WriteLine("LogEntrySafe=" + ex);
			}
		}

		private readonly static string _Machiname = Environment.MachineName;
		private readonly static string _ProcessId = Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture);

		private static void addBuiltin(IDictionary dictionary)
		{
			dictionary.Add("tid", Thread.CurrentThread.ManagedThreadId);
			dictionary.Add("MachineName", _Machiname);
			dictionary.Add("pid", _ProcessId);
		}


		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		private void processEnter(bool writeEnterExitLogrows, Level defaultLevel, Assembly assembly, Type type, string procedureName)
		{
			try
			{
				_writeEnterExitLogrows = writeEnterExitLogrows;
				_defaultLevel = defaultLevel;

				if (assembly != null)
				{
					_assembly = assembly;
				}
				if (type != null)
				{
					_type = type;
				}
				if (procedureName != null)
				{
					_procedureName = procedureName;
				}

				//bool bIsStatic = false;
				if (_assembly == null || _type == null || _procedureName == null)
				{
					System.Diagnostics.StackTrace st = null;
					System.Diagnostics.StackFrame sf = null;
					MethodBase mb = null;
					st = new System.Diagnostics.StackTrace();
					if (st != null)
					{
						sf = st.GetFrame(_StackTrRaceUp);
					}
					if (sf != null)
					{
						mb = sf.GetMethod();
					}
					if (mb != null)
					{
						//bIsStatic = mb.IsStatic;
						if (_assembly == null)
						{
							_assembly = mb.Module.Assembly;
						}
						if (_type == null)
						{
							_type = mb.DeclaringType;
						}
						if (_procedureName == null)
						{
							_procedureName = mb.Name;
						}
					}
				}

				if (_type != null)
				{
					_className = _type.Name;
				}
				if (_className == null)
				{
					if (_procedureName.Contains("."))
					{
						string[] _aProcedureName = _procedureName.Split('.');
						_className = _aProcedureName[0];
						_procedureName = _aProcedureName[1];
					}
					else
					{
						_className = UNKNOWN_CLASS;
					}
				}

				_nestingLevel = _CurrentNestingLevel;

				_assemblyName = _assembly.GetName().Name.PadRight(ASSEMBLY_NAME_MAX_LENGTH);
				if (_assemblyName.Length > ASSEMBLY_NAME_MAX_LENGTH)
					_assemblyName = _assemblyName.Substring(_assemblyName.Length - ASSEMBLY_NAME_MAX_LENGTH);
				if (_writeEnterExitLogrows)
				{
					string lEnterMessage = buildMessage("-> " + _className + "." + _procedureName);
					LogEntrySafe(lEnterMessage, defaultLevel);
					_nestingLevel = _CurrentNestingLevel += 1;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}

		private string buildMessage(string pRowMessage)
		{
			var lMessage = new StringBuilder();
			lMessage.Append(_assemblyName + "\t");
			lMessage.Append(DEFAULTVALUE_TRACE_NESTING_STRING, _nestingLevel);
			lMessage.Append(pRowMessage);
			return lMessage.ToString();
		}


		public UnityTraceContext()
		{
			// DO NOT REFACTOR de different constrctors overload : processEnter suppose to be a fixed number of steps down the caller of the constructor
			processEnter(true, Level.Debug, null, null, null);
		}

		public UnityTraceContext(bool writeEnterExitLogRows)
		{
			processEnter(writeEnterExitLogRows, Level.Debug, null, null, null);
		}

		public UnityTraceContext(bool writeEnterExitLogRows, Level defaultLevel, Assembly callingAssembly, Type callingType, string procedureName)
		{
			processEnter(writeEnterExitLogRows, defaultLevel, callingAssembly, callingType, procedureName);
		}


		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "TraceMessage"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public void TraceMessage([Localizable(false)] string message, Level level)
		{
			try
			{
				var msg = buildMessage(message);
				LogEntrySafe(msg, level);
			}
			catch (Exception ex1)
			{
				Console.WriteLine("TraceMessage=" + ex1.ToString());
				Debug.WriteLine("TraceMessage=" + ex1.ToString());
			}
		}

		public void TraceMessage([Localizable(false)] string message)
		{
			TraceMessage(message, _defaultLevel);
		}


		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "TraceError"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public void TraceError(Exception ex, Level level)
		{
			try
			{
				if (ex == null) 
					return;
				
				LogEntrySafe(ex.Message, level);
			}
			catch (Exception ex1)
			{
				Console.WriteLine("TraceError=" + ex1.ToString());
				Debug.WriteLine("TraceError=" + ex1.ToString());
			}
		}

		public void TraceError(Exception ex)
		{
			TraceError(ex, _defaultLevel);
		}


		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public void Dispose()
		{
			try
			{
				if (_writeEnterExitLogrows)
				{
					_nestingLevel = _CurrentNestingLevel -= 1;
					string lExitMessage =
						buildMessage("<- " + "( " + (DateTime.Now - _startTime).TotalSeconds + " .sec) " + _className + "." +
												 _procedureName);
					var entry = lExitMessage;
					LogEntrySafe(entry, _defaultLevel);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}
	}


	public interface IUnityTraceContext : IDisposable
	{
		void TraceMessage(string message, Level level);
		void TraceMessage(string message);
		void TraceError(Exception ex, Level level);
		void TraceError(Exception ex);
	}
}