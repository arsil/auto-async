using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace AutoAsync.Helpers
{
#if !SILVERLIGHT

	class FullFrameworkCustomAttributeBuilderCreator
	{
		public static CustomAttributeBuilder GetAttributeBuilder(CustomAttributeData orgAttribute,
			Action<List<PropertyInfo>, List<object>> onModifyProperties)
		{
			var propertyInfos = new List<PropertyInfo>();
			var propertyValues = new List<object>();

			var fieldInfos = new List<FieldInfo>();
			var fieldValues = new List<object>();

			var constructorArguments = orgAttribute.ConstructorArguments
				.Select(constructorArg => constructorArg.Value).ToList();

			if (orgAttribute.NamedArguments != null && orgAttribute.NamedArguments.Count > 0)
			{
				foreach (var na in orgAttribute.NamedArguments)
				{
					var propertyInfo = na.MemberInfo as PropertyInfo;
					var fieldInfo = na.MemberInfo as FieldInfo;

					if (propertyInfo != null)
					{
						propertyInfos.Add(propertyInfo);
						propertyValues.Add(na.TypedValue.Value);
					}
					else if (fieldInfo != null)
					{
						fieldInfos.Add(fieldInfo);
						fieldValues.Add(na.TypedValue.Value);
					}
				}
			}

			if (onModifyProperties != null)
				onModifyProperties(propertyInfos, propertyValues);

			return new CustomAttributeBuilder(orgAttribute.Constructor, constructorArguments.ToArray(),
				propertyInfos.ToArray(), propertyValues.ToArray(),
				fieldInfos.ToArray(), fieldValues.ToArray());
		}
	}

#endif
}
