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
        internal JadeiteKind[] Kinds { get; }
        internal bool IsOptional { get; }

        internal AssertKindAttribute(bool isOptional, params JadeiteKind[] kinds)
        {
            IsOptional = isOptional;
            Kinds = kinds;
        }

        internal AssertKindAttribute(params JadeiteKind[] kinds)
        {
            Kinds = kinds;
        }
    }

    [Conditional("DEBUG")]
    [AttributeUsage(AttributeTargets.Class)]
    internal class NodeKindAttribute : Attribute
    {
        internal JadeiteKind[] Kinds { get; }

        internal NodeKindAttribute(params JadeiteKind[] oneOf)
        {
            Kinds = oneOf;
        }
    }

    [Conditional("DEBUG")]
    [AttributeUsage(AttributeTargets.Property)]
    internal class AssertNotNullAttribute : Attribute
    {
    }

    internal static class ParsingDebug
    {
        private class DebugTypeInfo
        {
            public JadeiteKind[] NodeKinds { get; set; }
            public List<DebugProperty> Properties { get; set; }
        }

        private class DebugProperty
        {
            public PropertyInfo Property { get; set; }
            public bool IsOptional { get; set; }
            public JadeiteKind[] Kinds { get; set; }
        }

        private static readonly object s_propertiesLock = new object();
        private static readonly Dictionary<Type, DebugTypeInfo> s_typeCache = new Dictionary<Type, DebugTypeInfo>();

        [Conditional("DEBUG")]
        internal static void AssertNodeIsValid(INode element)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            var custom = element as ICustomDebugNode;
            custom?.AssertIsValid();

            var info = GetTypeInfo(element.GetType());

            // make sure the node itself is of a valid type
            if (info.NodeKinds?.Length > 0)
            {
                var good = false;

                foreach (var k in info.NodeKinds)
                {
                    if (element.Kind == k)
                    {
                        good = true;
                        break;
                    }
                }

                if (!good)
                    throw new Exception($"JADEITE BUG: \"{element.GetType().Name}\" was kind {element.Kind}, but should have been one of: {string.Join(", ", info.NodeKinds)}");
            }

            foreach (var p in info.Properties)
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
                if (child == null)
                    throw new Exception($"JADEITE BUG: A child of {element.GetType().Name} was null.");

                var node = child as INode;
                if (node != null)
                    AssertNodeIsValid(node);
            }
        }

        private static DebugTypeInfo GetTypeInfo(Type type)
        {
            DebugTypeInfo info;
            if (s_typeCache.TryGetValue(type, out info))
                return info;

            lock (s_propertiesLock)
            {
                if (s_typeCache.TryGetValue(type, out info))
                    return info;

                // todo - verify type constructors are internal

                info = new DebugTypeInfo();
                var nodeAttr = type.GetCustomAttribute<NodeKindAttribute>(inherit: false);
                if (nodeAttr != null)
                {
                    if (nodeAttr.Kinds == null || nodeAttr.Kinds.Length == 0)
                        throw new Exception($"[NodeKind] attribute on {type.Name} is empty.");

                    // make sure all of the kinds are actually node kinds, and not token kinds
                    foreach (var k in nodeAttr.Kinds)
                    {
                        if (!SyntaxInfo.IsNodeKind(k))
                            throw new Exception($"{k} not a node kind. It cannot be used in the [NodeKind] attribute on {type.Name}.");
                    }

                    info.NodeKinds = nodeAttr.Kinds;
                }
                else if (typeof(INode).IsAssignableFrom(type))
                {
                    throw new Exception($"{type.Name} implements INode, but does not have a [NodeKind] attribute.");
                }

                // need to create the list
                var list = new List<DebugProperty>();
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

                info.Properties = list;

                s_typeCache[type] = info;
                return info;
            }
        }
        [Conditional("DEBUG")]
        internal static void AssertKindIsOneOf(JadeiteKind kind, params JadeiteKind[] oneOf)
        {
            foreach (var k in oneOf)
            {
                if (kind == k)
                    return;
            }

            throw new Exception($"Got kind {kind}. Expected one of: {string.Join(", ", oneOf)}");
        }

        [Conditional("DEBUG")]
        internal static void Assert(bool condition)
        {
            Debug.Assert(condition);
        }
    }
}