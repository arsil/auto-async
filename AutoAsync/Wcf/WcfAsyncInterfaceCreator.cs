using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.ServiceModel;
using System.Threading;

using AutoAsync.Helpers;

namespace AutoAsync.Wcf
{
	/// <summary>
	/// Generates asynchronous interface based on synchronous one.
	/// </summary>
	class WcfAsyncInterfaceCreator
	{
		public static void GenerateAsyncInterface(
			Type orgInterfaceType, out Type asyncInterfaceType, 
			IDictionary<MethodInfo, WcfAsyncMethodsHolder> orgMethodToAsyncMap)
		{
			var methodsList = new List<MethodDescTmp>();

			var moduleBuilder = GetModuleBuilder();

			// name of the new async interface
			var asyncInterfaceName = orgInterfaceType.Name + "AsyncGenerated";

			// new interface type
			var interf = moduleBuilder.DefineType(asyncInterfaceName,
				TypeAttributes.Interface
				| TypeAttributes.Public
				| TypeAttributes.Abstract
				| TypeAttributes.AnsiClass
				| TypeAttributes.AutoLayout
				);

			CreateInterfeaceAttributes(orgInterfaceType, interf);

// todo: generic methods handling!!!!!!  ? --------------------------------------------------------------------------------------------------------------------239849023849028390482903849028390489023849028390482903849028390
			var orgInterfaceMethods = orgInterfaceType.GetMethods();
			foreach (var mi in orgInterfaceMethods)
			{
				var syncCallParameters = mi.GetParameters();
				var beginAsyncParameters = (from p in syncCallParameters select p.ParameterType)
					.Concat(new[] { typeof(AsyncCallback), typeof(object) }).ToArray();

				var beginMethodName = "Begin" + mi.Name;
				var beginMethodBuilder = interf.DefineMethod(beginMethodName,
					MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual |
					MethodAttributes.NewSlot | MethodAttributes.HideBySig,
					typeof(IAsyncResult), beginAsyncParameters);

				int index = 1;
				foreach (var param in syncCallParameters)
					beginMethodBuilder.DefineParameter(index++, ParameterAttributes.None, param.Name);

				CreateBeginMethodAttributes(mi, beginMethodBuilder);

				var endAsyncOutParameters = (from p in syncCallParameters where p.ParameterType.IsByRef select p);
				var endAsyncParameters = endAsyncOutParameters.Select(p => p.ParameterType)
					.Concat(new[] { typeof(IAsyncResult) }).ToArray();

				//end method
				var endMethodName = "End" + mi.Name;
				var endMethodBuilder = interf.DefineMethod(endMethodName,
					MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual |
					MethodAttributes.NewSlot | MethodAttributes.HideBySig,
					mi.ReturnType, endAsyncParameters);

				index = 1;
				foreach (var param in endAsyncOutParameters)
					endMethodBuilder.DefineParameter(index++, ParameterAttributes.None, param.Name);

				methodsList.Add(new MethodDescTmp(
					mi, beginMethodName, beginAsyncParameters, endMethodName, endAsyncParameters));
			}

			var result = interf.CreateType();
			//			newAssembly.Save("DynamicModule.dll"); // debug!

			foreach (var m in methodsList)
			{
				var beginMethod = result.GetMethod(m.BeginMethodName, m.BeginMethodArgTypes);
				var endMethod = result.GetMethod(m.EndMethodName, m.EndMethodArgTypes);

				orgMethodToAsyncMap[m.SyncMethodInfo] =
					new WcfAsyncMethodsHolder(result, beginMethod, m.BeginMethodArgTypes, endMethod, m.EndMethodArgTypes);
			}

			asyncInterfaceType = result;
		}

		private static void CreateInterfeaceAttributes(Type orgInterfaceType, TypeBuilder interf)
		{
#if SILVERLIGHT
			CreateInterfaceAttributes(orgInterfaceType, interf, orgInterfaceType.GetCustomAttributes(false).Cast<Attribute>(),
				attr => attr.GetType() == typeof (ServiceContractAttribute),
				SilverlightCustomAttributeBuilderCreator.GetAttributeBuilder);
#else
			CreateInterfaceAttributes(orgInterfaceType, interf, CustomAttributeData.GetCustomAttributes(orgInterfaceType),
				attr => attr.Constructor.DeclaringType == typeof(ServiceContractAttribute),
				FullFrameworkCustomAttributeBuilderCreator.GetAttributeBuilder);
#endif
		}

		private static void CreateInterfaceAttributes<T>(
			Type orgInterfaceType,
			TypeBuilder interf,
			IEnumerable<T> attributes,
			Func<T, bool> selector,
			Func<T, Action<List<PropertyInfo>, List<object>>, CustomAttributeBuilder> customAttributeBuilderCreator)
			where T : class
		{
			foreach (T attr in attributes)
			{
				if (selector(attr))
				{
					interf.SetCustomAttribute(customAttributeBuilderCreator(attr, (propList, propValues) =>
					{
						var nameProp = typeof(ServiceContractAttribute).GetProperty("Name");
						if (propList.FindIndex(a => a == nameProp) == -1) //!Exists
						{
							propList.Add(nameProp);
							propValues.Add(orgInterfaceType.Name);
						}

					}));
				}
				else
				{
					interf.SetCustomAttribute(customAttributeBuilderCreator(attr, null));
				}
			}
		}

		private static void CreateBeginMethodAttributes(MethodInfo mi, MethodBuilder methodBuilder)
		{
#if SILVERLIGHT
			CreateBeginMethodAttributes(methodBuilder, mi.GetCustomAttributes(false).Cast<Attribute>(),
				a => a.GetType() == typeof (OperationContractAttribute),
				SilverlightCustomAttributeBuilderCreator.GetAttributeBuilder
				);
#else
			CreateBeginMethodAttributes(methodBuilder, CustomAttributeData.GetCustomAttributes(mi),
				a => a.Constructor.DeclaringType == typeof(OperationContractAttribute),
				FullFrameworkCustomAttributeBuilderCreator.GetAttributeBuilder
				);
#endif
		}

		private static void CreateBeginMethodAttributes<T>(
			MethodBuilder methodBuilder,
			IEnumerable<T> attributes,
			Func<T, bool> selector,
			Func<T, Action<List<PropertyInfo>, List<object>>, CustomAttributeBuilder> customAttributeBuilderCreator)
			where T : class
		{
			foreach (var attr in attributes.Where(a => !selector(a)))
				methodBuilder.SetCustomAttribute(customAttributeBuilderCreator(attr, null));

			// property for async pattern
			var asyncPatternProperty = typeof(OperationContractAttribute).GetProperty("AsyncPattern");

			// looking for the operation contract attribute
			var oca = attributes.FirstOrDefault(selector);
			if (oca != null)
			{
				// found!
				methodBuilder.SetCustomAttribute(customAttributeBuilderCreator(oca, (propList, propValues) =>
					{
						var existingPropertyIndex = propList.FindIndex(p => p == asyncPatternProperty);
						if (existingPropertyIndex >= 0)
						{
							propValues[existingPropertyIndex] = true;
						}
						else
						{
							propList.Add(asyncPatternProperty);
							propValues.Add(true);
						}
					}));
			}
			else
			{
				// does not exist
				var mCi = typeof(OperationContractAttribute).GetConstructor(Type.EmptyTypes);
				var mAb = new CustomAttributeBuilder(mCi, new object[0], new[] { asyncPatternProperty }, new object[] { true });

				methodBuilder.SetCustomAttribute(mAb);
			}
		}

		private static ModuleBuilder GetModuleBuilder()
		{
			lock (syncRoot)
			{
				if (mb != null)
					return mb;

				var assemblyName = new AssemblyName { Name = "AsyncInterfaceTemporaryAssembly" };

				var newAssembly = Thread.GetDomain().DefineDynamicAssembly(assemblyName,
					AssemblyBuilderAccess.Run);

				mb = newAssembly.DefineDynamicModule("DynamicModule.dll", true); // true - symbol info
				return mb;
			}
		}

		private static ModuleBuilder mb;
		private static readonly object syncRoot = new object();

		class MethodDescTmp
		{
			public MethodDescTmp(
				MethodInfo syncMethodInfo, string beginMethodName, Type[] beginMethodArgTypes, string endMethodName, Type[] endMethodArgTypes)
			{
				SyncMethodInfo = syncMethodInfo;
				BeginMethodName = beginMethodName;
				BeginMethodArgTypes = beginMethodArgTypes;
				EndMethodName = endMethodName;
				EndMethodArgTypes = endMethodArgTypes;
			}

			public MethodInfo SyncMethodInfo { get; private set; }
			public string BeginMethodName { get; private set; }
			public Type[] BeginMethodArgTypes { get; private set; }
			public string EndMethodName { get; private set; }
			public Type[] EndMethodArgTypes { get; private set; }
		}
	}


}
