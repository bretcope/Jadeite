using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Jadeite.Parsing
{

    [Conditional("DEBUG")]
    [AttributeUsage(AttributeTargets.Property)]
    internal class AssertKindAttribute : Attribute
    {
        public JadeiteSyntaxKind[] Kinds { get; }
        public bool IsOptional { get; }

        public AssertKindAttribute(bool isOptional, params JadeiteSyntaxKind[] kinds)
        {
            IsOptional = isOptional;
            Kinds = kinds;
        }

        public AssertKindAttribute(params JadeiteSyntaxKind[] kinds)
        {
            Kinds = kinds;
        }
    }

    [Conditional("DEBUG")]
    [AttributeUsage(AttributeTargets.Property)]
    internal class AssertNotNullAttribute : Attribute
    {
    }

    public static class ParsingDebug
    {
        private class DebugProperty
        {
            public PropertyInfo Property;
            public bool IsOptional;
            public JadeiteSyntaxKind[] Kinds;
        }

        private static readonly object s_propertiesLock = new object();
        private static readonly Dictionary<Type, List<DebugProperty>> s_propertiesCache = new Dictionary<Type, List<DebugProperty>>();

        [Conditional("DEBUG")]
        public static void AssertNodeIsValid(INode element)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            var custom = element as ICustomDebugNode;
            custom?.AssertIsValid();

            var props = GetDebugPropertyList(element.GetType());
            foreach (var p in props)
            {
                var val = (ISyntaxElement)p.Property.GetValue(element);
                if (val == null)
                {
                    if (p.IsOptional)
                        continue;

                    throw new Exception($"JADEITE BUG: Property \"{element.GetType().Name}.{p.Property.Name}\" should not have been null.");
                }

                if (p.Kinds == null || p.Kinds.Length == 0) // can be any kind
                    continue;

                var good = false;
                foreach (var k in p.Kinds)
                {
                    if (k == val.Kind)
                    {
                        good = true;
                        break;
                    }
                }

                if (!good)
                    throw new Exception($"JADEITE BUG: Property \"{element.GetType().Name}.{p.Property.Name}\" was kind {val.Kind}, but should have been one of: {string.Join(", ", p.Kinds)}");
            }

            // todo - could also verify that every ISyntaxElement property is included in the GetChildren call

            foreach (var child in element.GetChildren())
            {
                var node = child as INode;
                if (node != null)
                    AssertNodeIsValid(node);
            }
        }

        private static List<DebugProperty> GetDebugPropertyList(Type type)
        {
            List<DebugProperty> list;
            if (s_propertiesCache.TryGetValue(type, out list))
                return list;

            lock (s_propertiesLock)
            {
                if (s_propertiesCache.TryGetValue(type, out list))
                    return list;

                // need to create the list
                list = new List<DebugProperty>();
                var props = type.GetProperties();
                var iSyntaxType = typeof(ISyntaxElement);
                foreach (var p in props)
                {
                    if (iSyntaxType.IsAssignableFrom(p.PropertyType))
                    {
                        var kindAttr = p.GetCustomAttribute<AssertKindAttribute>();
                        var notNullAttr = p.GetCustomAttribute<AssertNotNullAttribute>();
                        if (kindAttr != null || notNullAttr != null)
                        {
                            var debugProp = new DebugProperty {Property = p, Kinds = kindAttr?.Kinds};
                            debugProp.IsOptional = kindAttr?.IsOptional == true && notNullAttr == null;

                            list.Add(debugProp);
                        }
                    }
                }

                s_propertiesCache[type] = list;
                return list;
            }
        }
        [Conditional("DEBUG")]
        public static void AssertKindIsOneOf(JadeiteSyntaxKind kind, params JadeiteSyntaxKind[] oneOf)
        {
            foreach (var k in oneOf)
            {
                if (kind == k)
                    return;
            }

            throw new Exception($"Got kind {kind}. Expected one of: {string.Join(", ", oneOf)}");
        }

        [Conditional("DEBUG")]
        public static void Assert(bool condition)
        {
            Debug.Assert(condition);
        }
    }
}