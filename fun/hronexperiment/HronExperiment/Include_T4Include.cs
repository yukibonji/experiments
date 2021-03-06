
// ############################################################################
// #                                                                          #
// #        ---==>  T H I S  F I L E  I S   G E N E R A T E D  <==---         #
// #                                                                          #
// # This means that any edits to the .cs file will be lost when its          #
// # regenerated. Changes should instead be applied to the corresponding      #
// # text template file (.tt)                                                      #
// ############################################################################



// ############################################################################
// @@@ INCLUDING: https://raw.github.com/mrange/T4Include/master/Extensions/BasicExtensions.cs
// @@@ INCLUDE_FOUND: ../Common/Array.cs
// @@@ INCLUDE_FOUND: ../Common/Config.cs
// @@@ INCLUDE_FOUND: ../Common/Log.cs
// @@@ INCLUDING: https://raw.github.com/mrange/T4Include/master/Hron/HRONObjectSerializer.cs
// @@@ INCLUDE_FOUND: HRONSerializer.cs
// @@@ INCLUDE_FOUND: ../Extensions/ParseExtensions.cs
// @@@ INCLUDE_FOUND: ../Reflection/ClassDescriptor.cs
// @@@ INCLUDE_FOUND: ../Reflection/StaticReflection.cs
// @@@ INCLUDING: https://raw.github.com/mrange/T4Include/master/Hron/HRONDynamicObjectSerializer.cs
// @@@ INCLUDE_FOUND: HRONSerializer.cs
// @@@ INCLUDE_FOUND: ../Extensions/EnumParseExtensions.cs
// @@@ INCLUDE_FOUND: ../Extensions/ParseExtensions.cs
// @@@ INCLUDING: https://raw.github.com/mrange/T4Include/master/Common/Array.cs
// @@@ INCLUDING: https://raw.github.com/mrange/T4Include/master/Common/Config.cs
// @@@ INCLUDING: https://raw.github.com/mrange/T4Include/master/Common/Log.cs
// @@@ INCLUDE_FOUND: Config.cs
// @@@ INCLUDE_FOUND: Generated_Log.cs
// @@@ INCLUDING: https://raw.github.com/mrange/T4Include/master/Hron/HRONSerializer.cs
// @@@ INCLUDE_FOUND: ../Common/Array.cs
// @@@ INCLUDE_FOUND: ../Common/Config.cs
// @@@ INCLUDE_FOUND: ../Common/SubString.cs
// @@@ INCLUDING: https://raw.github.com/mrange/T4Include/master/Extensions/ParseExtensions.cs
// @@@ INCLUDE_FOUND: ../Common/Config.cs
// @@@ INCLUDING: https://raw.github.com/mrange/T4Include/master/Reflection/ClassDescriptor.cs
// @@@ INCLUDING: https://raw.github.com/mrange/T4Include/master/Reflection/StaticReflection.cs
// @@@ SKIPPING (Already seen): https://raw.github.com/mrange/T4Include/master/Hron/HRONSerializer.cs
// @@@ INCLUDING: https://raw.github.com/mrange/T4Include/master/Extensions/EnumParseExtensions.cs
// @@@ INCLUDE_FOUND: ../Reflection/StaticReflection.cs
// @@@ SKIPPING (Already seen): https://raw.github.com/mrange/T4Include/master/Extensions/ParseExtensions.cs
// @@@ SKIPPING (Already seen): https://raw.github.com/mrange/T4Include/master/Common/Config.cs
// @@@ INCLUDING: https://raw.github.com/mrange/T4Include/master/Common/Generated_Log.cs
// @@@ SKIPPING (Already seen): https://raw.github.com/mrange/T4Include/master/Common/Array.cs
// @@@ SKIPPING (Already seen): https://raw.github.com/mrange/T4Include/master/Common/Config.cs
// @@@ INCLUDING: https://raw.github.com/mrange/T4Include/master/Common/SubString.cs
// @@@ SKIPPING (Already seen): https://raw.github.com/mrange/T4Include/master/Common/Config.cs
// @@@ SKIPPING (Already seen): https://raw.github.com/mrange/T4Include/master/Reflection/StaticReflection.cs
// ############################################################################
// Certains directives such as #define and // Resharper comments has to be 
// moved to top in order to work properly    
// ############################################################################
// ReSharper disable InconsistentNaming
// ReSharper disable PartialMethodWithSinglePart
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantCaseLabel
// ReSharper disable RedundantIfElseBlock
// ReSharper disable RedundantNameQualifier
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: https://raw.github.com/mrange/T4Include/master/Extensions/BasicExtensions.cs
namespace HronExperiment
{
    // ----------------------------------------------------------------------------------------------
    // Copyright (c) M�rten R�nge.
    // ----------------------------------------------------------------------------------------------
    // This source code is subject to terms and conditions of the Microsoft Public License. A 
    // copy of the license can be found in the License.html file at the root of this distribution. 
    // If you cannot locate the  Microsoft Public License, please send an email to 
    // dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
    //  by the terms of the Microsoft Public License.
    // ----------------------------------------------------------------------------------------------
    // You must not remove this notice, or any other, from this software.
    // ----------------------------------------------------------------------------------------------
    
    
    
    namespace Source.Extensions
    {
        using System;
        using System.Collections.Generic;
        using System.Globalization;
        using System.IO;
        using System.Reflection;
    
        using Source.Common;
    
        static partial class BasicExtensions
        {
            public static bool IsNullOrWhiteSpace (this string v)
            {
                return string.IsNullOrWhiteSpace (v);
            }
    
            public static bool IsNullOrEmpty (this string v)
            {
                return string.IsNullOrEmpty (v);
            }
    
            public static T FirstOrReturn<T>(this T[] values, T defaultValue)
            {
                if (values == null)
                {
                    return defaultValue;
                }
    
                if (values.Length == 0)
                {
                    return defaultValue;
                }
    
                return values[0];
            }
    
            public static T FirstOrReturn<T>(this IEnumerable<T> values, T defaultValue)
            {
                if (values == null)
                {
                    return defaultValue;
                }
    
                foreach (var value in values)
                {
                    return value;
                }
    
                return defaultValue;
            }
    
            public static void Shuffle<T>(this T[] values, Random random)
            {
                if (values == null)
                {
                    return;
                }
    
                if (random == null)
                {
                    return;
                }
    
                for (var iter = 0; iter < values.Length; ++iter)
                {
                    var swapWith = random.Next (iter, values.Length);
    
                    var tmp = values[iter];
    
                    values[iter] = values[swapWith];
                    values[swapWith] = tmp;
                }
    
            }
    
            public static string DefaultTo (this string v, string defaultValue = null)
            {
                return !v.IsNullOrEmpty () ? v : (defaultValue ?? "");
            }
    
            public static IEnumerable<T> DefaultTo<T>(
                this IEnumerable<T> values, 
                IEnumerable<T> defaultValue = null
                )
            {
                return values ?? defaultValue ?? Array<T>.Empty;
            }
    
            public static T[] DefaultTo<T>(this T[] values, T[] defaultValue = null)
            {
                return values ?? defaultValue ?? Array<T>.Empty;
            }
    
            public static T DefaultTo<T>(this T v, T defaultValue = default (T))
                where T : struct, IEquatable<T>
            {
                return !v.Equals (default (T)) ? v : defaultValue;
            }
    
            public static string FormatWith (this string format, CultureInfo cultureInfo, params object[] args)
            {
                return string.Format (cultureInfo, format ?? "", args.DefaultTo ());
            }
    
            public static string FormatWith (this string format, params object[] args)
            {
                return format.FormatWith (Config.DefaultCulture, args);
            }
    
            public static TValue Lookup<TKey, TValue>(
                this IDictionary<TKey, TValue> dictionary, 
                TKey key, 
                TValue defaultValue = default (TValue))
            {
                if (dictionary == null)
                {
                    return defaultValue;
                }
    
                TValue value;
                return dictionary.TryGetValue (key, out value) ? value : defaultValue;
            }
    
            public static TValue GetOrAdd<TKey, TValue>(
                this IDictionary<TKey, TValue> dictionary, 
                TKey key, 
                TValue defaultValue = default (TValue))
            {
                if (dictionary == null)
                {
                    return defaultValue;
                }
    
                TValue value;
                if (!dictionary.TryGetValue (key, out value))
                {
                    value = defaultValue;
                    dictionary[key] = value;
                }
    
                return value;
            }
    
            public static TValue GetOrAdd<TKey, TValue>(
                this IDictionary<TKey, TValue> dictionary,
                TKey key,
                Func<TValue> valueCreator
                )
            {
                if (dictionary == null)
                {
                    return valueCreator ();
                }
    
                TValue value;
                if (!dictionary.TryGetValue (key, out value))
                {
                    value = valueCreator ();
                    dictionary[key] = value;
                }
    
                return value;
            }
    
            public static void DisposeNoThrow (this IDisposable disposable)
            {
                try
                {
                    if (disposable != null)
                    {
                        disposable.Dispose ();
                    }
                }
                catch (Exception exc)
                {
                    Log.Exception ("DisposeNoThrow: Dispose threw: {0}", exc);
                }
            }
    
            public static TTo CastTo<TTo> (this object value, TTo defaultValue)
            {
                return value is TTo ? (TTo) value : defaultValue;
            }
    
            public static string Concatenate (this IEnumerable<string> values, string delimiter = null, int capacity = 16)
            {
                values = values ?? Array<string>.Empty;
                delimiter = delimiter ?? ", ";
    
                return string.Join (delimiter, values);
            }
    
            public static string GetResourceString (this Assembly assembly, string name, string defaultValue = null)
            {
                defaultValue = defaultValue ?? "";
    
                if (assembly == null)
                {
                    return defaultValue;
                }
    
                var stream = assembly.GetManifestResourceStream (name ?? "");
                if (stream == null)
                {
                    return defaultValue;
                }
    
                using (stream)
                using (var streamReader = new StreamReader (stream))
                {
                    return streamReader.ReadToEnd ();
                }
            }
    
            public static IEnumerable<string> ReadLines (this TextReader textReader)
            {
                if (textReader == null)
                {
                    yield break;
                }
    
                string line;
    
                while ((line = textReader.ReadLine ()) != null)
                {
                    yield return line;
                }
            }
    
    #if !NETFX_CORE
            public static IEnumerable<Type> GetInheritanceChain (this Type type)
            {
                while (type != null)
                {
                    yield return type;
                    type = type.BaseType;
                }
            }
    #endif
        }
    }
}
// @@@ END_INCLUDE: https://raw.github.com/mrange/T4Include/master/Extensions/BasicExtensions.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: https://raw.github.com/mrange/T4Include/master/Hron/HRONObjectSerializer.cs
namespace HronExperiment
{
    // ----------------------------------------------------------------------------------------------
    // Copyright (c) M�rten R�nge.
    // ----------------------------------------------------------------------------------------------
    // This source code is subject to terms and conditions of the Microsoft Public License. A 
    // copy of the license can be found in the License.html file at the root of this distribution. 
    // If you cannot locate the  Microsoft Public License, please send an email to 
    // dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
    //  by the terms of the Microsoft Public License.
    // ----------------------------------------------------------------------------------------------
    // You must not remove this notice, or any other, from this software.
    // ----------------------------------------------------------------------------------------------
    
    
    
    namespace Source.HRON
    {
        using System;
        using System.Collections;
        using System.Collections.Generic;
        using System.Collections.Concurrent;
        using System.Linq;
        using System.Text;
        using Source.Common;
        using Source.Extensions;
        using Source.Reflection;
        using System.Runtime.Serialization;
    
        partial class HRONObjectParseError
        {
            public readonly int LineNo;
            public readonly string Line;
            public readonly HRONSerializer.ParseError ParseError;
    
            public HRONObjectParseError(int lineNo, string line, HRONSerializer.ParseError parseError)
            {
                LineNo = lineNo;
                Line = line;
                ParseError = parseError;
            }
        }
    
        sealed partial class HRONObjectBuilderVisitor<T> : IHRONVisitor
        {
            partial struct Item
            {
                public readonly ClassDescriptor ClassDescriptor;
                public readonly object Value;
    
                public Item(Type type)
                    : this()
                {
                    if (type == null)
                    {
                        return;
                    }
    
                    ClassDescriptor = type.GetClassDescriptor();
                    Value = ClassDescriptor.Creator();
                }
            }
    
            readonly Stack<Item> m_stack = new Stack<Item>();
            public readonly List<HRONObjectParseError> Errors = new List<HRONObjectParseError>();
            public readonly int MaxErrors;
            readonly StringBuilder m_value = new StringBuilder(128);
            bool m_firstLine = true;
    
            public HRONObjectBuilderVisitor(int maxErrorCount)
            {
                MaxErrors = maxErrorCount;
                m_stack.Push(new Item(typeof(T)));
            }
    
            Item Top
            {
                get { return m_stack.Peek(); }
            }
    
            public T Instance
            {
                get { return (T)Top.Value; }
            }
    
            public void Document_Begin()
            {
            }
    
            public void Document_End()
            {
            }
    
            public void PreProcessor(SubString line)
            {
            }
    
            public void Empty(SubString line)
            {
                
            }
    
            public void Comment(int indent, SubString comment)
            {
            }
    
            public void Value_Begin(SubString name)
            {
                if (Top.Value == null)
                {
                    return;
                }
                m_firstLine = true;
                m_value.Clear();
            }
    
            public void Value_Line(SubString value)
            {
                if (Top.Value == null)
                {
                    return;
                }
                if (m_firstLine)
                {
                    m_firstLine = false;
                }
                else
                {
                    m_value.AppendLine();
                }
                m_value.Append(value);
            }
    
            static bool IsAssignableFromString(Type type)
            {
                return
                        type == typeof(string)
                    || type == typeof(object)
                    ;
            }
    
            public void Value_End(SubString name)
            {
                var top = Top;
                if (top.Value == null)
                {
                    return;
                }
    
                var value = m_value.ToString();
    
                var classDescriptor = top.ClassDescriptor;
                var itemName = name.ToString();
                var memberDescriptor = classDescriptor.FindMember(itemName);
    
                if (memberDescriptor == null)
                {
                    if (classDescriptor.IsDictionaryLike)
                    {
                        if (!IsAssignableFromString(classDescriptor.DictionaryKeyType))
                        {
                            // TODO: Log?
                            return;
                        }
    
                        var dictionary = (IDictionary)top.Value;
    
                        if (dictionary.Contains(itemName))
                        {
                            // TODO: Log?
                            return;
                        }
    
                        var itemType = classDescriptor.DictionaryValueType;
                        if (IsAssignableFromString(itemType))
                        {
                            dictionary.Add(itemName, value);
                            return;
                        }
    
                        var parsedValue = value.Parse(
                            Config.DefaultCulture,
                            itemType,
                            null
                            );
    
                        if (parsedValue == null)
                        {
                            // TODO: Log?
                            return;
                        }
    
                        dictionary.Add(itemName, parsedValue);
                    }
                    else if (classDescriptor.IsListLike)
                    {
                        var list = (IList)top.Value;
    
                        if (list.IsReadOnly)
                        {
                            // TODO: Log?
                            return;
                        }
    
                        if (IsAssignableFromString(classDescriptor.ListItemType))
                        {
                            list.Add (value);
                        }
    
                        var itemType = classDescriptor.ListItemType;
    
                        var parsedValue = value.Parse(
                            Config.DefaultCulture,
                            itemType,
                            null
                            );
        
                        if (parsedValue == null)
                        {
                            // TODO: Log?
                            return;
                        }
    
                        list.Add (parsedValue);
                    }
                    else
                    {
                        // TODO: Log?
                        return;
                    }
    
                    return;
                }
    
                var memberClassDescriptor = memberDescriptor.ClassDescriptor;
    
                if (memberClassDescriptor.IsListLike)
                {
                    var list = (IList)memberDescriptor.Getter(top.Value);
    
                    if (list == null)
                    {
                        if (!memberDescriptor.HasSetter)
                        {
                            // TODO: Log?
                            return;
                        }
    
                        if (!memberClassDescriptor.HasCreator)
                        {
                            // TODO: Log?
                            return;
                        }
    
                        list = (IList)memberClassDescriptor.Creator();
    
                        if (list.IsReadOnly)
                        {
                            // TODO: Log?
                            return;
                        }
    
                        memberDescriptor.Setter(top.Value, list);
                    }
    
                    var itemType = memberClassDescriptor.ListItemType;
    
                    if (IsAssignableFromString(itemType))
                    {
                        list.Add(value);
                        return;
                    }
    
                    var parsedValue = value.Parse(
                        Config.DefaultCulture,
                        itemType,
                        null
                        );
    
                    if (parsedValue == null)
                    {
                        // TODO: Log?
                        return;
                    }
    
                    list.Add(parsedValue);
                }
                else
                {
                    if (!memberDescriptor.HasSetter)
                    {
                        // TODO: Log?
                        return;
                    }
    
                    if (IsAssignableFromString(memberDescriptor.MemberType))
                    {
                        memberDescriptor.Setter(top.Value, value);
                        return;
                    }
    
                    var parsedValue = value.Parse(
                        Config.DefaultCulture,
                        memberDescriptor.MemberType,
                        null
                        );
    
                    if (parsedValue == null)
                    {
                        // TODO: Log?
                        return;
                    }
    
                    memberDescriptor.Setter(top.Value, parsedValue);
                }
            }
    
            public void Object_Begin(SubString name)
            {
                var top = Top;
                if (top.Value == null)
                {
                    return;
                }
    
                Type type = null;
                try
                {
                    var classDescriptor = top.ClassDescriptor;
                    var itemName = name.ToString();
                    var memberDescriptor = classDescriptor.FindMember(itemName);
    
                    if (memberDescriptor == null)
                    {
                        if (classDescriptor.IsDictionaryLike)
                        {
                            if (!IsAssignableFromString(classDescriptor.DictionaryKeyType))
                            {
                                // TODO: Log?
                                return;
                            }
    
                            var dictionary = (IDictionary)top.Value;
    
                            if (dictionary.Contains(itemName))
                            {
                                // TODO: Log?
                                return;
                            }
    
                            var itemType = classDescriptor.DictionaryValueType;
    
                            if (IsAssignableFromString(itemType))
                            {
                                // TODO: Log?
                                return;
                            }
    
                            type = itemType.GetClassDescriptor().NonNullableType;
                        }
                        else if (classDescriptor.IsListLike)
                        {
                            var itemType = classDescriptor.ListItemType;
    
                            if (IsAssignableFromString(itemType))
                            {
                                // TODO: Log?
                                return;
                            }
    
                            type = itemType.GetClassDescriptor().NonNullableType;
                        }
                        else
                        {
                            // TODO: Log?
                            return;
                        }
    
                        return;
                    }
    
                    var memberClassDescriptor = memberDescriptor.ClassDescriptor;
                    if (memberClassDescriptor.IsListLike)
                    {
                        var list = (IList)memberDescriptor.Getter(top.Value);
    
                        if (list == null)
                        {
                            if (!memberDescriptor.HasSetter)
                            {
                                // TODO: Log?
                                return;
                            }
    
                            if (!memberClassDescriptor.HasCreator)
                            {
                                // TODO: Log?
                                return;
                            }
                        }
    
                        var itemType = memberClassDescriptor.ListItemType;
    
                        if (IsAssignableFromString(itemType))
                        {
                            // TODO: Log?
                            return;
                        }
    
                        type = itemType.GetClassDescriptor().NonNullableType;
                    }
                    else
                    {
                        if (!memberDescriptor.HasSetter)
                        {
                            // TODO: Log?
                            return;
                        }
    
                        if (IsAssignableFromString(memberDescriptor.MemberType))
                        {
                            // TODO: Log?
                            return;
                        }
    
                        type = memberDescriptor.ClassDescriptor.NonNullableType;
                    }
                }
                finally
                {
                    m_stack.Push(new Item(type));
                }
            }
    
            public void Object_End(SubString name)
            {
                var value = Top;
                m_stack.Pop();
                var top = Top;
    
                if (value.Value == null)
                {
                    return;
                }
    
                if (top.Value == null)
                {
                    return;
                }
    
                var classDescriptor = top.ClassDescriptor;
                var itemName = name.ToString();
                var memberDescriptor = classDescriptor.FindMember(itemName);
    
                if (memberDescriptor == null)
                {
                    if (classDescriptor.IsDictionaryLike)
                    {
                        var dictionary = (IDictionary)top.Value;
    
                        if (dictionary.Contains(itemName))
                        {
                            // TODO: Log?
                            return;
                        }
    
                        dictionary.Add(itemName, value.Value);
                    }
                    else if (classDescriptor.IsListLike)
                    {
                        var list = (IList)top.Value;
    
                        if (list.IsReadOnly)
                        {
                            // TODO: Log?
                            return;
                        }
    
                        list.Add(value.Value);
                    }
                    else
                    {
                        // TODO: Log?
                        return;
                    }
    
                    return;
                }
                var memberClassDescriptor = memberDescriptor.ClassDescriptor;
    
                if (memberClassDescriptor.IsListLike)
                {
                    var list = (IList)memberDescriptor.Getter(top.Value);
    
                    if (list == null)
                    {
                        list = (IList)memberClassDescriptor.Creator();
    
                        if (list.IsReadOnly)
                        {
                            // TODO: Log?
                            return;
                        }
    
                        memberDescriptor.Setter(top.Value, list);
                    }
    
                    list.Add(value.Value);
                }
                else
                {
                    memberDescriptor.Setter(top.Value, value.Value);
                }
            }
    
            public void Error(int lineNo, SubString line, HRONSerializer.ParseError parseError)
            {
                Errors.Add(new HRONObjectParseError(lineNo, line.Value, parseError));
            }
        }
    
        static partial class HRONSerializer
        {
            sealed partial class CD 
            {
                public CD (Type type)
                {
                    ClassDescriptor = type.GetClassDescriptor ();

                    var isDataContract = ClassDescriptor.Attributes.Any (a => a is DataContractAttribute);

                    if (isDataContract)
                    {
                        SerializableMembers = ClassDescriptor
                            .PublicGetMembers
                            .Where (m => m.Attributes.Any (a => a is DataMemberAttribute))
                            .ToArray ()
                            ;
                    }
                    else
                    {
                        SerializableMembers = ClassDescriptor.PublicGetMembers;
                    }

                }

                public readonly ClassDescriptor     ClassDescriptor     ;
                public readonly MemberDescriptor[]  SerializableMembers ;
            }

            static readonly ConcurrentDictionary<Type, CD> s_cache = new ConcurrentDictionary<Type, CD> ();

            static readonly Func<Type, CD> s_createCD = t => new CD(t);

            static CD GetCD (this Type type)
            {
                return s_cache.GetOrAdd (type ?? typeof (object), s_createCD);
            }

            public static bool TryParseObject<T>(
                int maxErrorCount,
                IEnumerable<SubString> lines,
                out T hronObject,
                out HRONObjectParseError[] errors
                )
            {
                hronObject = default(T);
                errors = Array<HRONObjectParseError>.Empty;
    
                var visitor = new HRONObjectBuilderVisitor<T>(maxErrorCount);
    
                Parse(maxErrorCount, lines, visitor);
    
                if (visitor.Errors.Count > 0)
                {
                    errors = visitor.Errors.ToArray();
                    return false;
                }
    
                hronObject = visitor.Instance;
    
                return true;
            }
    
            public static string ObjectAsString<T>(T value, bool omitIfNullOrEmpty = true)
            {
                var visitor = new HRONWriterVisitor();
                VisitObject(value, visitor, omitIfNullOrEmpty);
                return visitor.Value;
            }
        
            public static void VisitObject(
                object value,
                IHRONVisitor visitor,
                bool omitIfNullOrEmpty = true
                )
            {
                if (value == null)
                {
                    return;
                }
        
                var type = value.GetType();
                var cd = type.GetCD();
                var classDescriptor = cd.ClassDescriptor;
        
                if (classDescriptor.IsDictionaryLike)
                {
                    var dictionary = (IDictionary)value;
                    foreach (var key in dictionary.Keys)
                    {
                        var innerValue = dictionary[key];
                        var keyAsString = key as string;
                        if (keyAsString != null)
                        {
                            VisitMember(keyAsString.ToSubString(), innerValue, visitor, omitIfNullOrEmpty);
                        }
                    }
                }
                else if (classDescriptor.IsListLike)
                {
                    var list = (IList)value;
                    for (var index = 0; index < list.Count; index++)
                    {
                        var innerValue = list[index];
                        VisitMember(new SubString(), innerValue, visitor, omitIfNullOrEmpty);
                    }
                }
                else
                {
                    for (var index = 0; index < cd.SerializableMembers.Length; index++)
                    {
                        var mi = cd.SerializableMembers[index];
                        var memberName = mi.Name.ToSubString();
                        var memberValue = mi.Getter(value);
                        VisitMember(memberName, memberValue, visitor, omitIfNullOrEmpty);
                    }
                }
            }
    
            static void VisitMember(SubString memberName, object memberValue, IHRONVisitor visitor, bool omitIfNullOrEmpty)
            {
                if (memberValue == null)
                {
                    if (!omitIfNullOrEmpty)
                    {
                        visitor.Value_Begin(memberName);
                        visitor.Value_End(memberName);
                    }
                    return;
                }
        
                var cd = memberValue.GetType().GetCD();
                var classDescriptor = cd.ClassDescriptor;
        
                if (classDescriptor.IsDictionaryLike)
                {
                    visitor.Object_Begin(memberName);
                    var dictionary = (IDictionary)memberValue;
                    foreach (var key in dictionary.Keys)
                    {
                        var innerValue = dictionary[key];
                        var keyAsString = key as string;
                        if (keyAsString != null)
                        {
                            VisitMember(keyAsString.ToSubString(), innerValue, visitor, omitIfNullOrEmpty);
                        }
                    }
                    visitor.Object_End(memberName);
                }
                else if (classDescriptor.IsListLike)
                {
                    var list = (IList)memberValue;
                    for (var index = 0; index < list.Count; index++)
                    {
                        var innerValue = list[index];
                        VisitMember(memberName, innerValue, visitor, omitIfNullOrEmpty);
                    }
                }
                else if (memberValue is string)
                {
                    var innerValue = (string)memberValue;
                    if (!innerValue.IsNullOrEmpty())
                    {
                        visitor.Value_Begin(memberName);
                        foreach (var line in innerValue.ReadLines())
                        {
                            visitor.Value_Line(line);
                        }
                        visitor.Value_End(memberName);
                    }
                    else if (!omitIfNullOrEmpty)
                    {
                        visitor.Value_Begin(memberName);
                        visitor.Value_End(memberName);
                    }
                }
                else if (classDescriptor.Type.CanParse())
                {
                    var memberAsString = memberValue.ToString();
                    // These types are never multilined, but may be empty
                    if (!memberAsString.IsNullOrEmpty())
                    {
                        visitor.Value_Begin(memberName);
                        visitor.Value_Line(memberAsString.ToSubString());
                        visitor.Value_End(memberName);
                    }
                    else if (!omitIfNullOrEmpty)
                    {
                        visitor.Value_Begin(memberName);
                        visitor.Value_End(memberName);
                    }
                }
                else
                {
                    visitor.Object_Begin(memberName);
                    VisitObject(memberValue, visitor);
                    visitor.Object_End(memberName);
                }
            }
    
        }
    }
}
// @@@ END_INCLUDE: https://raw.github.com/mrange/T4Include/master/Hron/HRONObjectSerializer.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: https://raw.github.com/mrange/T4Include/master/Hron/HRONDynamicObjectSerializer.cs
namespace HronExperiment
{
    // ----------------------------------------------------------------------------------------------
    // Copyright (c) M�rten R�nge.
    // ----------------------------------------------------------------------------------------------
    // This source code is subject to terms and conditions of the Microsoft Public License. A 
    // copy of the license can be found in the License.html file at the root of this distribution. 
    // If you cannot locate the  Microsoft Public License, please send an email to 
    // dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
    //  by the terms of the Microsoft Public License.
    // ----------------------------------------------------------------------------------------------
    // You must not remove this notice, or any other, from this software.
    // ----------------------------------------------------------------------------------------------
    
    
    
    namespace Source.HRON
    {
        using System;
        using System.Collections.Generic;
        using System.Dynamic;
        using System.Linq;
        using System.Text;
    
        using Source.Common;
        using Source.Extensions;
    
        static partial class HronExtensions
        {
            public static IHRONEntity FirstOrEmpty (this IEnumerable<IHRONEntity> entities)
            {
                if (entities == null)
                {
                    return HRONValue.Empty;
                }
    
                return entities.FirstOrDefault() ?? HRONValue.Empty;
            }
        }
    
        partial class HRONDynamicParseError
        {
            public readonly int LineNo;
            public readonly string Line;
            public readonly HRONSerializer.ParseError ParseError;
    
            public HRONDynamicParseError(int lineNo, string line, HRONSerializer.ParseError parseError)
            {
                LineNo = lineNo;
                Line = line;
                ParseError = parseError;
            }
        }
    
        partial interface IHRONEntity
        {
            HRONSerializer.DynamicType GetDynamicType();
    
            IEnumerable<string>      GetMemberNames();
            IEnumerable<IHRONEntity> GetMember(string name);
            string GetValue();
    
            void Apply(SubString name, IHRONVisitor visitor);
            void ToString(StringBuilder sb);
        }
    
        sealed partial class HRONDynamicMembers : DynamicObject
        {
            readonly IHRONEntity[] m_entities;
    
            public HRONDynamicMembers(IEnumerable<IHRONEntity> entities)
            {
                m_entities = (entities ?? Array<IHRONEntity>.Empty).ToArray ();
            }
    
            public override string ToString()
            {
                var sb = new StringBuilder();
    
                var first = true;
    
                foreach (var entity in m_entities)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        sb.Append(", ");
                    }
    
                    entity.ToString(sb);
                }
    
                return sb.ToString();
            }
    
            public int GetCount ()
            {
                return m_entities.Length;
            }
    
            public bool Exists ()
            {
                return m_entities.Length > 0;
            }
    
            public override IEnumerable<string> GetDynamicMemberNames ()
            {
                var entity = m_entities.FirstOrEmpty ();
                return entity.GetMemberNames ();
            }
    
            public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
            {
                if (indexes.Length == 1 && indexes[0] is int)
                {
                    var index = (int)indexes[0];
                    if (index < 0)
                    {
                        result = HRONValue.Empty;
                        return true;
                    }
    
                    if (index >= m_entities.Length)
                    {
                        result = HRONValue.Empty;
                        return true;
                    }
    
                    result = m_entities[index];
                    return true;
                }
    
                return base.TryGetIndex(binder, indexes, out result);
            }
    
            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                var entity = m_entities.FirstOrEmpty();
    
                var dynamicObject = entity as DynamicObject;
                if (dynamicObject != null)
                {
                    return dynamicObject.TryGetMember(binder, out result);
                }
    
                return base.TryGetMember(binder, out result);
            }
    
            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            {
                var entity = m_entities.FirstOrEmpty();
    
                var dynamicObject = entity as DynamicObject;
                if (dynamicObject != null)
                {
                    return dynamicObject.TryInvokeMember(binder, args, out result);
                }
    
                return base.TryInvokeMember(binder, args, out result);
            }
    
            public override bool TryConvert(ConvertBinder binder, out object result)
            {
                var returnType = binder.ReturnType;
                if (returnType == typeof(string))
                {
                    result = m_entities.FirstOrEmpty().GetValue ();
                    return true;
                }
                else if (returnType == typeof(string[]))
                {
                    result = m_entities.Select(e => e.GetValue()).ToArray();
                    return true;
                }
                else if (returnType ==typeof(object[]))
                {
                    result = m_entities;
                    return true;
                }
                else if (BaseHRONEntity.IsParseable (returnType))
                {
                    result = BaseHRONEntity.Parse (returnType, m_entities.FirstOrEmpty().GetValue());
                    return true;                
                }
                else if (returnType.IsArray)
                {
                    var elementType = returnType.GetElementType();
                    if (BaseHRONEntity.IsParseable (elementType))
                    {
                        var values = m_entities.Select (entity => BaseHRONEntity.Parse (elementType, entity.GetValue())).ToArray();
                        var array = Array.CreateInstance(elementType, values.Length);
                        values.CopyTo(array, 0);
                        result = array;
                        return true;
                    }
                }
                return base.TryConvert(binder, out result);
            }
        }
    
        abstract partial class BaseHRONEntity : DynamicObject, IHRONEntity
        {
            public abstract HRONSerializer.DynamicType GetDynamicType();
            public abstract IEnumerable<string> GetMemberNames();
            public abstract IEnumerable<IHRONEntity> GetMember(string name);
            public abstract string GetValue();
    
            public abstract void Apply(SubString name, IHRONVisitor visitor);
            public abstract void ToString(StringBuilder sb);
    
            internal static bool IsParseable (Type type)
            {
                return type.CanParseEnumValue() || type.CanParse();
            }
    
            public override IEnumerable<string> GetDynamicMemberNames ()
            {
                return GetMemberNames ();
            }
    
            internal static object Parse(Type type, string value)
            {
                value = value ?? "";
    
                if (type.CanParseEnumValue())                    
                {
                    return value.ParseEnumValue(type) ?? type.GetDefaultEnumValue ();
                }
    
                return value.Parse (Config.DefaultCulture, type, type.GetParsedDefaultValue());
            }
    
    
            public override string ToString()
            {
                var sb = new StringBuilder(128);
                ToString(sb);
                return sb.ToString();
            }
    
            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = new HRONDynamicMembers(GetMember(binder.Name));
                return true;
            }
    
            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            {
                result = new HRONDynamicMembers(GetMember(binder.Name));
                return true;
            }
    
            public override bool TryConvert(ConvertBinder binder, out object result)
            {
                var returnType = binder.ReturnType;
                if (returnType == typeof(string))
                {
                    result = GetValue();
                    return true;
                }
                else if (IsParseable(returnType))
                {
                    result = Parse(returnType, GetValue());
                    return true;
                }
                return base.TryConvert(binder, out result);
            }
    
        }
    
        sealed partial class HRONObject : BaseHRONEntity
        {
            public static HRONObject Empty = new HRONObject (null);
    
            public partial struct Member
            {
                readonly string m_name;
                readonly IHRONEntity m_value;
    
                public Member(string name, IHRONEntity value)
                    : this()
                {
                    m_name = name.Trim ();
                    m_value = value;
                }
    
                public string Name
                {
                    get { return m_name ?? ""; }
                }
    
                public IHRONEntity Value
                {
                    get { return m_value ?? HRONValue.Empty; }
                }
    
                public override string ToString()
                {
                    return Name + " : " + Value;
                }
            }
    
            ILookup<string, IHRONEntity> m_lookup;
            readonly Member[] m_members;
    
            public HRONObject(Member[] members)
            {
                m_members = members ?? Array<Member>.Empty;
            }
    
            ILookup<string, IHRONEntity> GetLookup()
            {
                if (m_lookup == null)
                {
                    m_lookup = m_members.ToLookup(p => p.Name, p => p.Value);
                }
    
                return m_lookup;
            }
    
            public override HRONSerializer.DynamicType GetDynamicType()
            {
                return HRONSerializer.DynamicType.Object;
            }
    
            public override IEnumerable<string> GetMemberNames()
            {
                for (int index = 0; index < m_members.Length; index++)
                {
                    var pair = m_members[index];
                    yield return pair.Name;
                }
            }
    
            public override IEnumerable<IHRONEntity> GetMember(string name)
            {
                return GetLookup()[name ?? ""]; 
            }
    
            public override string GetValue()
            {
                return "";
            }
    
            public void Visit (IHRONVisitor visitor)
            {
                if (visitor == null)
                {
                    return;
                }
    
                for (var index = 0; index < m_members.Length; index++)
                {
                    var pair = m_members[index];
                    var innerName = pair.Name.ToSubString();
                    pair.Value.Apply(innerName, visitor);
                }
            }
    
            public override void Apply(SubString name, IHRONVisitor visitor)
            {
                if (visitor == null)
                {
                    return;
                }
    
                visitor.Object_Begin(name);
                for (var index = 0; index < m_members.Length; index++)
                {
                    var pair = m_members[index];
                    var innerName = pair.Name.ToSubString();
                    pair.Value.Apply(innerName, visitor);
                }
                visitor.Object_End(name);
            }
    
            public override void ToString(StringBuilder sb)
            {
                sb.Append("{Object");
                for (var index = 0; index < m_members.Length; index++)
                {
                    var pair = m_members[index];
                    sb.Append(", '");
                    sb.Append(pair.Name);
                    sb.Append("' : ");
                    pair.Value.ToString(sb);
                }
                sb.Append('}');
            }
        }
    
        sealed partial class HRONValue : BaseHRONEntity
        {
            readonly string m_value;
            public static readonly IHRONEntity Empty = new HRONValue("");
    
            public HRONValue(string value)
            {
                m_value = value ?? "";
            }
    
            public override HRONSerializer.DynamicType GetDynamicType()
            {
                return HRONSerializer.DynamicType.Value;
            }
    
            public override IEnumerable<string> GetMemberNames()
            {
                return Array<string>.Empty;
            }
    
            public override IEnumerable<IHRONEntity> GetMember(string name)
            {
                return Array<IHRONEntity>.Empty;
            }
    
            public override string GetValue()
            {
                return m_value;
            }
    
            public override void Apply(SubString name, IHRONVisitor visitor)
            {
                if (visitor == null)
                {
                    return;
                }
    
                visitor.Value_Begin(name);
                foreach (var line in m_value.ReadLines())
                {
                    visitor.Value_Line(line);
                }
                visitor.Value_End(name);
            }
    
            public override void ToString(StringBuilder sb)
            {
                sb.Append('"');
    
                var first = true;
    
                foreach (var line in m_value.ReadLines())
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        sb.Append(", ");
                    }
                    sb.AppendSubString(line);                
                }
                sb.Append('"');
            }
        }
    
        sealed partial class HRONDynamicBuilderVisitor : IHRONVisitor
        {
            public struct Item
            {
                public readonly SubString Name;
                public readonly List<HRONObject.Member> Pairs;
    
                public Item(SubString name)
                    : this()
                {
                    Name = name;
                    Pairs = new List<HRONObject.Member>();
                }
            }
    
            readonly Stack<Item> m_stack = new Stack<Item>();
            readonly StringBuilder m_value = new StringBuilder(128);
            bool m_firstLine = true;
    
            public readonly List<HRONDynamicParseError> Errors = new List<HRONDynamicParseError>();
            public readonly int MaxErrors;
    
            public HRONDynamicBuilderVisitor(int maxErrors)
            {
                MaxErrors = maxErrors;
                m_stack.Push(new Item("Root".ToSubString()));
            }
    
            public Item Top
            {
                get { return m_stack.Peek(); }
            }
    
            public void Document_Begin()
            {
            }
    
            public void Document_End()
            {
            }
    
            public void PreProcessor(SubString line)
            {
            }
    
            public void Empty(SubString line)
            {
            }
    
            public void Comment(int indent, SubString comment)
            {
            }
    
            public void Value_Begin(SubString name)
            {
                m_value.Clear();
                m_firstLine = true;
            }
    
            public void Value_Line(SubString value)
            {
                if (m_firstLine)
                {
                    m_firstLine = false;
                }
                else
                {
                    m_value.AppendLine();
                }
                m_value.Append(value);
            }
    
            public void Value_End(SubString name)
            {
                AddEntity(name.Value, new HRONValue(m_value.ToString()));
            }
    
            public void Object_Begin(SubString name)
            {
                m_stack.Push(new Item(name));
            }
    
            public void Object_End(SubString name)
            {
                var pop = m_stack.Pop();
                AddEntity(pop.Name.Value, new HRONObject(pop.Pairs.ToArray()));
            }
    
            void AddEntity(string name, IHRONEntity entity)
            {
                Top.Pairs.Add(new HRONObject.Member(name, entity));
            }
    
            public void Error(int lineNo, SubString line, HRONSerializer.ParseError parseError)
            {
                Errors.Add(new HRONDynamicParseError(lineNo, line.Value, parseError));
            }
        }
    
    
        static partial class HRONSerializer
        {
            public enum DynamicType
            {
                Value,
                Object,
            }
    
            public static void VisitDynamic (
                HRONObject hronObject,
                IHRONVisitor visitor
                )
            {
                if (hronObject == null)
                {
                    return;
                }
    
                hronObject.Visit(visitor);
            }
    
            public static string DynamicAsString (
                HRONObject hronObject
                )
            {
                if (hronObject == null)
                {
                    return "";
                }
    
                var v = new HRONWriterVisitor();
                VisitDynamic(hronObject, v);
                return v.Value;
            }
    
            public static bool TryParseDynamic(
                int maxErrorCount,
                IEnumerable<SubString> lines,
                out dynamic dyn,
                out HRONDynamicParseError[] errors
                )
            {
                HRONObject hronObject;
    
                if (!TryParseDynamic(maxErrorCount, lines, out hronObject, out errors))
                {
                    dyn = null;
                    return false;
                }
    
                dyn = hronObject;
                return true;
            }
    
            public static bool TryParseDynamic(
                int maxErrorCount,
                IEnumerable<SubString> lines,
                out HRONObject hronObject,
                out HRONDynamicParseError[] errors
                )
            {
                hronObject = null;
                errors = Array<HRONDynamicParseError>.Empty;
    
                var visitor = new HRONDynamicBuilderVisitor(maxErrorCount);
    
                Parse(maxErrorCount, lines, visitor);
    
                if (visitor.Errors.Count > 0)
                {
                    errors = visitor.Errors.ToArray();
                    return false;
                }
    
                hronObject = new HRONObject(visitor.Top.Pairs.ToArray());
    
                return true;
            }
    
        }
    }
}
// @@@ END_INCLUDE: https://raw.github.com/mrange/T4Include/master/Hron/HRONDynamicObjectSerializer.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: https://raw.github.com/mrange/T4Include/master/Common/Array.cs
namespace HronExperiment
{
    // ----------------------------------------------------------------------------------------------
    // Copyright (c) M�rten R�nge.
    // ----------------------------------------------------------------------------------------------
    // This source code is subject to terms and conditions of the Microsoft Public License. A 
    // copy of the license can be found in the License.html file at the root of this distribution. 
    // If you cannot locate the  Microsoft Public License, please send an email to 
    // dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
    //  by the terms of the Microsoft Public License.
    // ----------------------------------------------------------------------------------------------
    // You must not remove this notice, or any other, from this software.
    // ----------------------------------------------------------------------------------------------
    
    namespace Source.Common
    {
        static class Array<T>
        {
            public static readonly T[] Empty = new T[0];
        }
    }
}
// @@@ END_INCLUDE: https://raw.github.com/mrange/T4Include/master/Common/Array.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: https://raw.github.com/mrange/T4Include/master/Common/Config.cs
namespace HronExperiment
{
    // ----------------------------------------------------------------------------------------------
    // Copyright (c) M�rten R�nge.
    // ----------------------------------------------------------------------------------------------
    // This source code is subject to terms and conditions of the Microsoft Public License. A 
    // copy of the license can be found in the License.html file at the root of this distribution. 
    // If you cannot locate the  Microsoft Public License, please send an email to 
    // dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
    //  by the terms of the Microsoft Public License.
    // ----------------------------------------------------------------------------------------------
    // You must not remove this notice, or any other, from this software.
    // ----------------------------------------------------------------------------------------------
    
    
    namespace Source.Common
    {
        using System.Globalization;
    
        sealed partial class InitConfig
        {
            public CultureInfo DefaultCulture = CultureInfo.InvariantCulture;
        }
    
        static partial class Config
        {
            static partial void Partial_Constructed(ref InitConfig initConfig);
    
            public readonly static CultureInfo DefaultCulture;
    
            static Config ()
            {
                var initConfig = new InitConfig();
    
                Partial_Constructed (ref initConfig);
    
                initConfig = initConfig ?? new InitConfig();
    
                DefaultCulture = initConfig.DefaultCulture;
            }
        }
    }
}
// @@@ END_INCLUDE: https://raw.github.com/mrange/T4Include/master/Common/Config.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: https://raw.github.com/mrange/T4Include/master/Common/Log.cs
namespace HronExperiment
{
    // ----------------------------------------------------------------------------------------------
    // Copyright (c) M�rten R�nge.
    // ----------------------------------------------------------------------------------------------
    // This source code is subject to terms and conditions of the Microsoft Public License. A 
    // copy of the license can be found in the License.html file at the root of this distribution. 
    // If you cannot locate the  Microsoft Public License, please send an email to 
    // dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
    //  by the terms of the Microsoft Public License.
    // ----------------------------------------------------------------------------------------------
    // You must not remove this notice, or any other, from this software.
    // ----------------------------------------------------------------------------------------------
    
    
    
    namespace Source.Common
    {
        using System;
        using System.Globalization;
    
        static partial class Log
        {
            static partial void Partial_LogLevel (Level level);
            static partial void Partial_LogMessage (Level level, string message);
            static partial void Partial_ExceptionOnLog (Level level, string format, object[] args, Exception exc);
    
            public static void LogMessage (Level level, string format, params object[] args)
            {
                try
                {
                    Partial_LogLevel (level);
                    Partial_LogMessage (level, GetMessage (format, args));
                }
                catch (Exception exc)
                {
                    Partial_ExceptionOnLog (level, format, args, exc);
                }
                
            }
    
            static string GetMessage (string format, object[] args)
            {
                format = format ?? "";
                try
                {
                    return (args == null || args.Length == 0)
                               ? format
                               : string.Format (Config.DefaultCulture, format, args)
                        ;
                }
                catch (FormatException)
                {
    
                    return format;
                }
            }
        }
    }
}
// @@@ END_INCLUDE: https://raw.github.com/mrange/T4Include/master/Common/Log.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: https://raw.github.com/mrange/T4Include/master/Hron/HRONSerializer.cs
namespace HronExperiment
{
    // ----------------------------------------------------------------------------------------------
    // Copyright (c) M�rten R�nge.
    // ----------------------------------------------------------------------------------------------
    // This source code is subject to terms and conditions of the Microsoft Public License. A 
    // copy of the license can be found in the License.html file at the root of this distribution. 
    // If you cannot locate the  Microsoft Public License, please send an email to 
    // dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
    //  by the terms of the Microsoft Public License.
    // ----------------------------------------------------------------------------------------------
    // You must not remove this notice, or any other, from this software.
    // ----------------------------------------------------------------------------------------------
    
    
    
    
    namespace Source.HRON
    {
        using System.Collections.Generic;
        using System.Text;
        using Source.Common;
    
        partial interface IHRONVisitor
        {
            void Document_Begin ();
            void Document_End ();
            void PreProcessor (SubString line);
            void Empty (SubString line);
    
            void Comment(int indent, SubString comment);
    
            void Value_Begin(SubString name);
            void Value_Line(SubString value);
            void Value_End(SubString name);
    
            void Object_Begin(SubString name);
            void Object_End(SubString name);
    
            void Error(int lineNo, SubString line, HRONSerializer.ParseError parseError);
        }
    
        abstract partial class BaseHRONWriterVisitor : IHRONVisitor
        {
            readonly    StringBuilder   m_sb    = new StringBuilder();
            bool                        m_first = true  ;
            int                         m_indent        ;
    
            protected abstract void Write       (StringBuilder line);
            protected abstract void WriteLine   ();
            void                    WriteLine   (StringBuilder line)
            {
                if (m_first)
                {
                    m_first = false;
                }
                else
                {
                    WriteLine ();
                }
    
                Write (line);
            }
    
            public abstract void Document_Begin();
            public abstract void Document_End();
    
            public void PreProcessor(SubString line)
            {
                m_sb.Remove(0, m_sb.Length);
                m_sb.Append('!');
                m_sb.AppendSubString(line);
                WriteLine(m_sb);
            }
    
            public void Empty (SubString line)
            {
                m_sb.Remove(0, m_sb.Length);
                m_sb.AppendSubString(line);
                WriteLine(m_sb);
            }
    
            public void Comment(int indent, SubString comment)
            {
                m_sb.Remove(0, m_sb.Length);
                m_sb.Append('\t', indent);
                m_sb.Append('#');
                m_sb.AppendSubString(comment);
                WriteLine(m_sb);
            }
    
            public void Value_Begin(SubString name)
            {
                m_sb.Remove(0, m_sb.Length);
                m_sb.Append('\t', m_indent);
                m_sb.Append('=');
                m_sb.Append(name);
                ++m_indent;
                WriteLine(m_sb);
            }
    
            public void Value_Line(SubString value)
            {
                m_sb.Remove(0, m_sb.Length);
                m_sb.Append('\t', m_indent);
                m_sb.AppendSubString(value);
                WriteLine(m_sb);
            }
    
            public void Value_End(SubString name)
            {
                --m_indent;
            }
    
            public void Object_Begin(SubString name)
            {
                m_sb.Remove(0, m_sb.Length);
                m_sb.Append('\t', m_indent);
                m_sb.Append('@');
                m_sb.AppendSubString(name);
                WriteLine(m_sb);
                ++m_indent;
            }
    
            public void Object_End(SubString name)
            {
                --m_indent;
            }
    
            public void Error(int lineNo, SubString line, HRONSerializer.ParseError parseError)
            {
                m_sb.Remove(0, m_sb.Length);
                m_sb.AppendFormat(Config.DefaultCulture, "# Error at line {0}: {1}", lineNo, parseError);
                WriteLine(m_sb);
            }
    
        }
    
        sealed partial class HRONWriterVisitor : BaseHRONWriterVisitor
        {
            readonly StringBuilder m_sb = new StringBuilder(128);
    
            public string Value
            {
                get
                {
                    return m_sb.ToString();                
                }
            }
    
            protected override void Write(StringBuilder line)
            {
                m_sb.Append(line.ToString());
            }
    
            protected override void WriteLine()
            {
                m_sb.AppendLine();
            }
    
            public override void Document_Begin()
            {
            }
    
            public override void Document_End()
            {
            }
        }
    
        static partial class HRONSerializer
        {
            enum ParseState
            {
                ExpectingTag    ,
                ExpectingValue  ,
            }
    
            public enum ParseError
            {
                ProgrammingError                ,
                IndentIncreasedMoreThanExpected ,
                TagIsNotCorrectlyFormatted      ,
            }
    
            public static void Parse(
                int maxErrorCount,
                IEnumerable<SubString> lines,
                IHRONVisitor visitor
                )
            {
                if (visitor == null)
                {
                    return;
                }
    
                visitor.Document_Begin();
    
                try
                {
                    var errorCount = 0;
    
                    lines = lines ?? Array<SubString>.Empty;
    
                    var state = ParseState.ExpectingTag;
                    var expectedIndent = 0;
                    var lineNo = 0;
                    var context = new Stack<SubString>();
    
                    var acceptsPreProcessor = true;
    
                    foreach (var line in lines)
                    {
                        ++lineNo;
    
                        var lineLength = line.Length;
                        var begin = line.Begin;
                        var end = line.End;
    
                        var currentIndent = 0;
                        var baseString = line.BaseString;
    
                        if (acceptsPreProcessor)
                        {
                            if (lineLength > 0 && baseString[begin] == '!')
                            {
                                visitor.PreProcessor(line.ToSubString(1));
                                continue;
                            }
                            else
                            {
                                acceptsPreProcessor = false;
                            }
                        }
    
                        for (var iter = begin; iter < end; ++iter)
                        {
                            var ch = baseString[iter];
                            if (ch == '\t')
                            {
                                ++currentIndent;
                            }
                            else
                            {
                                break;
                            }
                        }
    
                        bool isComment;
                        switch (state)
                        {
                            case ParseState.ExpectingTag:
                                isComment = currentIndent < lineLength
                                            && baseString[currentIndent + begin] == '#'
                                    ;
                                break;
                            case ParseState.ExpectingValue:
                            default:
                                isComment = currentIndent < expectedIndent
                                            && currentIndent < lineLength
                                            && baseString[currentIndent + begin] == '#'
                                    ;
                                break;
                        }
    
                        var isWhiteSpace = line.ToSubString(currentIndent).IsWhiteSpace;
    
                        if (isComment)
                        {
                            visitor.Comment(currentIndent, line.ToSubString(currentIndent + 1));
                        }
                        else if (isWhiteSpace && currentIndent < expectedIndent)
                        {
                            switch (state)
                            {
                                case ParseState.ExpectingValue:
                                    visitor.Value_Line(SubString.Empty);
                                    break;
                                case ParseState.ExpectingTag:
                                default:
                                    visitor.Empty(line);
                                    break;
                            }
                        }
                        else if (isWhiteSpace)
                        {
                            switch (state)
                            {
                                case ParseState.ExpectingValue:
                                    visitor.Value_Line(line.ToSubString(expectedIndent));
                                    break;
                                case ParseState.ExpectingTag:
                                default:
                                    visitor.Empty(line);
                                    break;
                            }
                        }
                        else
                        {
                            if (currentIndent < expectedIndent)
                            {
                                switch (state)
                                {
                                    case ParseState.ExpectingTag:
                                        for (var iter = currentIndent; iter < expectedIndent; ++iter)
                                        {
                                            visitor.Object_End(context.Peek());
                                            context.Pop();
                                        }
                                        break;
                                    case ParseState.ExpectingValue:
                                    default:
                                        visitor.Value_End(context.Peek());
                                        // Popping the value name
                                        context.Pop();
                                        for (var iter = currentIndent + 1; iter < expectedIndent; ++iter)
                                        {
                                            visitor.Object_End(context.Peek());
                                            context.Pop();
                                        }
                                        break;
                                }
    
                                expectedIndent = currentIndent;
                                state = ParseState.ExpectingTag;
                            }
    
                            switch (state)
                            {
                                case ParseState.ExpectingTag:
                                    if (currentIndent > expectedIndent)
                                    {
                                        visitor.Error(lineNo, line, ParseError.IndentIncreasedMoreThanExpected);
                                        if (++errorCount > 0)
                                        {
                                            return;
                                        }
                                    }
                                    else if (currentIndent < lineLength)
                                    {
                                        var first = baseString[currentIndent + begin];
                                        switch (first)
                                        {
                                            case '@':
                                                state = ParseState.ExpectingTag;
                                                ++expectedIndent;
                                                context.Push(line.ToSubString(currentIndent + 1));
                                                visitor.Object_Begin(context.Peek());
                                                break;
                                            case '=':
                                                state = ParseState.ExpectingValue;
                                                ++expectedIndent;
                                                context.Push(line.ToSubString(currentIndent + 1));
                                                visitor.Value_Begin(context.Peek());
                                                break;
                                            default:
                                                visitor.Error(lineNo, line, ParseError.TagIsNotCorrectlyFormatted);
                                                if (++errorCount > 0)
                                                {
                                                    return;
                                                }
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        visitor.Error(lineNo, line, ParseError.ProgrammingError);
                                        if (++errorCount > 0)
                                        {
                                            return;
                                        }
                                    }
                                    break;
                                case ParseState.ExpectingValue:
                                    visitor.Value_Line(line.ToSubString(expectedIndent));
                                    break;
                            }
                        }
                    }
    
                    switch (state)
                    {
                        case ParseState.ExpectingTag:
                            for (var iter = 0; iter < expectedIndent; ++iter)
                            {
                                visitor.Object_End(context.Peek());
                                context.Pop();
                            }
                            break;
                        case ParseState.ExpectingValue:
                        default:
                            visitor.Value_End(context.Peek());
                            // Popping the value name
                            context.Pop();
                            for (var iter = 0 + 1; iter < expectedIndent; ++iter)
                            {
                                visitor.Object_End(context.Peek());
                                context.Pop();
                            }
                            break;
                    }
    
                }
                finally
                {
                    visitor.Document_End();
                }
            }
        }
    
    }
}
// @@@ END_INCLUDE: https://raw.github.com/mrange/T4Include/master/Hron/HRONSerializer.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: https://raw.github.com/mrange/T4Include/master/Extensions/ParseExtensions.cs
namespace HronExperiment
{
    
    
    
    // ############################################################################
    // #                                                                          #
    // #        ---==>  T H I S  F I L E  I S   G E N E R A T E D  <==---         #
    // #                                                                          #
    // # This means that any edits to the .cs file will be lost when its          #
    // # regenerated. Changes should instead be applied to the corresponding      #
    // # template file (.tt)                                                      #
    // ############################################################################
    
    
    
    
    
    
    
    namespace Source.Extensions
    {
        using System;
        using System.Collections.Generic;
        using System.Globalization;
    
        static partial class ParseExtensions
        {
            static readonly Dictionary<Type, Func<object>> s_defaultValues = new Dictionary<Type, Func<object>> 
                {
    #if !T4INCLUDE__SUPPRESS_BOOLEAN_PARSE_EXTENSIONS
                    { typeof(Boolean)      , () => default (Boolean)},
                    { typeof(Boolean?)     , () => default (Boolean?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_CHAR_PARSE_EXTENSIONS
                    { typeof(Char)      , () => default (Char)},
                    { typeof(Char?)     , () => default (Char?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_SBYTE_PARSE_EXTENSIONS
                    { typeof(SByte)      , () => default (SByte)},
                    { typeof(SByte?)     , () => default (SByte?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_INT16_PARSE_EXTENSIONS
                    { typeof(Int16)      , () => default (Int16)},
                    { typeof(Int16?)     , () => default (Int16?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_INT32_PARSE_EXTENSIONS
                    { typeof(Int32)      , () => default (Int32)},
                    { typeof(Int32?)     , () => default (Int32?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_INT64_PARSE_EXTENSIONS
                    { typeof(Int64)      , () => default (Int64)},
                    { typeof(Int64?)     , () => default (Int64?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_BYTE_PARSE_EXTENSIONS
                    { typeof(Byte)      , () => default (Byte)},
                    { typeof(Byte?)     , () => default (Byte?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_UINT16_PARSE_EXTENSIONS
                    { typeof(UInt16)      , () => default (UInt16)},
                    { typeof(UInt16?)     , () => default (UInt16?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_UINT32_PARSE_EXTENSIONS
                    { typeof(UInt32)      , () => default (UInt32)},
                    { typeof(UInt32?)     , () => default (UInt32?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_UINT64_PARSE_EXTENSIONS
                    { typeof(UInt64)      , () => default (UInt64)},
                    { typeof(UInt64?)     , () => default (UInt64?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_SINGLE_PARSE_EXTENSIONS
                    { typeof(Single)      , () => default (Single)},
                    { typeof(Single?)     , () => default (Single?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_DOUBLE_PARSE_EXTENSIONS
                    { typeof(Double)      , () => default (Double)},
                    { typeof(Double?)     , () => default (Double?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_DECIMAL_PARSE_EXTENSIONS
                    { typeof(Decimal)      , () => default (Decimal)},
                    { typeof(Decimal?)     , () => default (Decimal?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_TIMESPAN_PARSE_EXTENSIONS
                    { typeof(TimeSpan)      , () => default (TimeSpan)},
                    { typeof(TimeSpan?)     , () => default (TimeSpan?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_DATETIME_PARSE_EXTENSIONS
                    { typeof(DateTime)      , () => default (DateTime)},
                    { typeof(DateTime?)     , () => default (DateTime?)},
    #endif
                };
            static readonly Dictionary<Type, Func<string, CultureInfo, object>> s_parsers = new Dictionary<Type, Func<string, CultureInfo, object>> 
                {
    #if !T4INCLUDE__SUPPRESS_BOOLEAN_PARSE_EXTENSIONS
                    { typeof(Boolean)  , (s, ci) => { Boolean value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(Boolean?) , (s, ci) => { Boolean value; return s.TryParse(ci, out value) ? (object)(Boolean?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_CHAR_PARSE_EXTENSIONS
                    { typeof(Char)  , (s, ci) => { Char value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(Char?) , (s, ci) => { Char value; return s.TryParse(ci, out value) ? (object)(Char?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_SBYTE_PARSE_EXTENSIONS
                    { typeof(SByte)  , (s, ci) => { SByte value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(SByte?) , (s, ci) => { SByte value; return s.TryParse(ci, out value) ? (object)(SByte?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_INT16_PARSE_EXTENSIONS
                    { typeof(Int16)  , (s, ci) => { Int16 value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(Int16?) , (s, ci) => { Int16 value; return s.TryParse(ci, out value) ? (object)(Int16?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_INT32_PARSE_EXTENSIONS
                    { typeof(Int32)  , (s, ci) => { Int32 value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(Int32?) , (s, ci) => { Int32 value; return s.TryParse(ci, out value) ? (object)(Int32?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_INT64_PARSE_EXTENSIONS
                    { typeof(Int64)  , (s, ci) => { Int64 value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(Int64?) , (s, ci) => { Int64 value; return s.TryParse(ci, out value) ? (object)(Int64?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_BYTE_PARSE_EXTENSIONS
                    { typeof(Byte)  , (s, ci) => { Byte value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(Byte?) , (s, ci) => { Byte value; return s.TryParse(ci, out value) ? (object)(Byte?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_UINT16_PARSE_EXTENSIONS
                    { typeof(UInt16)  , (s, ci) => { UInt16 value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(UInt16?) , (s, ci) => { UInt16 value; return s.TryParse(ci, out value) ? (object)(UInt16?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_UINT32_PARSE_EXTENSIONS
                    { typeof(UInt32)  , (s, ci) => { UInt32 value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(UInt32?) , (s, ci) => { UInt32 value; return s.TryParse(ci, out value) ? (object)(UInt32?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_UINT64_PARSE_EXTENSIONS
                    { typeof(UInt64)  , (s, ci) => { UInt64 value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(UInt64?) , (s, ci) => { UInt64 value; return s.TryParse(ci, out value) ? (object)(UInt64?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_SINGLE_PARSE_EXTENSIONS
                    { typeof(Single)  , (s, ci) => { Single value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(Single?) , (s, ci) => { Single value; return s.TryParse(ci, out value) ? (object)(Single?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_DOUBLE_PARSE_EXTENSIONS
                    { typeof(Double)  , (s, ci) => { Double value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(Double?) , (s, ci) => { Double value; return s.TryParse(ci, out value) ? (object)(Double?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_DECIMAL_PARSE_EXTENSIONS
                    { typeof(Decimal)  , (s, ci) => { Decimal value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(Decimal?) , (s, ci) => { Decimal value; return s.TryParse(ci, out value) ? (object)(Decimal?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_TIMESPAN_PARSE_EXTENSIONS
                    { typeof(TimeSpan)  , (s, ci) => { TimeSpan value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(TimeSpan?) , (s, ci) => { TimeSpan value; return s.TryParse(ci, out value) ? (object)(TimeSpan?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_DATETIME_PARSE_EXTENSIONS
                    { typeof(DateTime)  , (s, ci) => { DateTime value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(DateTime?) , (s, ci) => { DateTime value; return s.TryParse(ci, out value) ? (object)(DateTime?)value : null;}},
    #endif
                };
    
            public static bool CanParse (this Type type)
            {
                if (type == null)
                {
                    return false;
                }
    
                return s_parsers.ContainsKey (type);
            }
    
            public static object GetParsedDefaultValue (this Type type)
            {
                type = type ?? typeof (object);
    
                Func<object> getValue;
    
                return s_defaultValues.TryGetValue (type, out getValue) ? getValue () : null;
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, Type type, out object value)
            {
                value = null;
                if (type == null)
                {
                    return false;
                }                
                
                Func<string, CultureInfo, object> parser;
    
                if (s_parsers.TryGetValue (type, out parser))
                {
                    value = parser (s, cultureInfo);
                }
    
                return value != null;
            }
    
            public static bool TryParse (this string s, Type type, out object value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, type, out value);
            }
    
            public static object Parse (this string s, CultureInfo cultureInfo, Type type, object defaultValue)
            {
                object value;
                return s.TryParse (cultureInfo, type, out value) ? value : defaultValue;
            }
    
            public static object Parse (this string s, Type type, object defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, type, defaultValue);
            }
    
            // Boolean (BoolLike)
    
    #if !T4INCLUDE__SUPPRESS_BOOLEAN_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out Boolean value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static Boolean Parse (this string s, CultureInfo cultureInfo, Boolean defaultValue)
            {
                Boolean value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static Boolean Parse (this string s, Boolean defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out Boolean value)
            {
                return Boolean.TryParse (s ?? "", out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_BOOLEAN_PARSE_EXTENSIONS
    
            // Char (CharLike)
    
    #if !T4INCLUDE__SUPPRESS_CHAR_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out Char value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static Char Parse (this string s, CultureInfo cultureInfo, Char defaultValue)
            {
                Char value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static Char Parse (this string s, Char defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out Char value)
            {
                return Char.TryParse (s ?? "", out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_CHAR_PARSE_EXTENSIONS
    
            // SByte (IntLike)
    
    #if !T4INCLUDE__SUPPRESS_SBYTE_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out SByte value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static SByte Parse (this string s, CultureInfo cultureInfo, SByte defaultValue)
            {
                SByte value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static SByte Parse (this string s, SByte defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out SByte value)
            {
                return SByte.TryParse (s ?? "", NumberStyles.Integer, cultureInfo, out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_SBYTE_PARSE_EXTENSIONS
    
            // Int16 (IntLike)
    
    #if !T4INCLUDE__SUPPRESS_INT16_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out Int16 value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static Int16 Parse (this string s, CultureInfo cultureInfo, Int16 defaultValue)
            {
                Int16 value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static Int16 Parse (this string s, Int16 defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out Int16 value)
            {
                return Int16.TryParse (s ?? "", NumberStyles.Integer, cultureInfo, out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_INT16_PARSE_EXTENSIONS
    
            // Int32 (IntLike)
    
    #if !T4INCLUDE__SUPPRESS_INT32_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out Int32 value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static Int32 Parse (this string s, CultureInfo cultureInfo, Int32 defaultValue)
            {
                Int32 value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static Int32 Parse (this string s, Int32 defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out Int32 value)
            {
                return Int32.TryParse (s ?? "", NumberStyles.Integer, cultureInfo, out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_INT32_PARSE_EXTENSIONS
    
            // Int64 (IntLike)
    
    #if !T4INCLUDE__SUPPRESS_INT64_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out Int64 value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static Int64 Parse (this string s, CultureInfo cultureInfo, Int64 defaultValue)
            {
                Int64 value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static Int64 Parse (this string s, Int64 defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out Int64 value)
            {
                return Int64.TryParse (s ?? "", NumberStyles.Integer, cultureInfo, out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_INT64_PARSE_EXTENSIONS
    
            // Byte (IntLike)
    
    #if !T4INCLUDE__SUPPRESS_BYTE_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out Byte value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static Byte Parse (this string s, CultureInfo cultureInfo, Byte defaultValue)
            {
                Byte value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static Byte Parse (this string s, Byte defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out Byte value)
            {
                return Byte.TryParse (s ?? "", NumberStyles.Integer, cultureInfo, out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_BYTE_PARSE_EXTENSIONS
    
            // UInt16 (IntLike)
    
    #if !T4INCLUDE__SUPPRESS_UINT16_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out UInt16 value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static UInt16 Parse (this string s, CultureInfo cultureInfo, UInt16 defaultValue)
            {
                UInt16 value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static UInt16 Parse (this string s, UInt16 defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out UInt16 value)
            {
                return UInt16.TryParse (s ?? "", NumberStyles.Integer, cultureInfo, out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_UINT16_PARSE_EXTENSIONS
    
            // UInt32 (IntLike)
    
    #if !T4INCLUDE__SUPPRESS_UINT32_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out UInt32 value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static UInt32 Parse (this string s, CultureInfo cultureInfo, UInt32 defaultValue)
            {
                UInt32 value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static UInt32 Parse (this string s, UInt32 defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out UInt32 value)
            {
                return UInt32.TryParse (s ?? "", NumberStyles.Integer, cultureInfo, out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_UINT32_PARSE_EXTENSIONS
    
            // UInt64 (IntLike)
    
    #if !T4INCLUDE__SUPPRESS_UINT64_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out UInt64 value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static UInt64 Parse (this string s, CultureInfo cultureInfo, UInt64 defaultValue)
            {
                UInt64 value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static UInt64 Parse (this string s, UInt64 defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out UInt64 value)
            {
                return UInt64.TryParse (s ?? "", NumberStyles.Integer, cultureInfo, out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_UINT64_PARSE_EXTENSIONS
    
            // Single (FloatLike)
    
    #if !T4INCLUDE__SUPPRESS_SINGLE_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out Single value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static Single Parse (this string s, CultureInfo cultureInfo, Single defaultValue)
            {
                Single value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static Single Parse (this string s, Single defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out Single value)
            {                                                  
                return Single.TryParse (s ?? "", NumberStyles.Float, cultureInfo, out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_SINGLE_PARSE_EXTENSIONS
    
            // Double (FloatLike)
    
    #if !T4INCLUDE__SUPPRESS_DOUBLE_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out Double value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static Double Parse (this string s, CultureInfo cultureInfo, Double defaultValue)
            {
                Double value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static Double Parse (this string s, Double defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out Double value)
            {                                                  
                return Double.TryParse (s ?? "", NumberStyles.Float, cultureInfo, out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_DOUBLE_PARSE_EXTENSIONS
    
            // Decimal (FloatLike)
    
    #if !T4INCLUDE__SUPPRESS_DECIMAL_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out Decimal value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static Decimal Parse (this string s, CultureInfo cultureInfo, Decimal defaultValue)
            {
                Decimal value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static Decimal Parse (this string s, Decimal defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out Decimal value)
            {                                                  
                return Decimal.TryParse (s ?? "", NumberStyles.Float, cultureInfo, out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_DECIMAL_PARSE_EXTENSIONS
    
            // TimeSpan (TimeSpanLike)
    
    #if !T4INCLUDE__SUPPRESS_TIMESPAN_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out TimeSpan value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static TimeSpan Parse (this string s, CultureInfo cultureInfo, TimeSpan defaultValue)
            {
                TimeSpan value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static TimeSpan Parse (this string s, TimeSpan defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out TimeSpan value)
            {                                                  
                return TimeSpan.TryParse (s ?? "", cultureInfo, out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_TIMESPAN_PARSE_EXTENSIONS
    
            // DateTime (DateTimeLike)
    
    #if !T4INCLUDE__SUPPRESS_DATETIME_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out DateTime value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static DateTime Parse (this string s, CultureInfo cultureInfo, DateTime defaultValue)
            {
                DateTime value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static DateTime Parse (this string s, DateTime defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out DateTime value)
            {                                                  
                return DateTime.TryParse (s ?? "", cultureInfo, DateTimeStyles.AssumeLocal, out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_DATETIME_PARSE_EXTENSIONS
    
        }
    }
    
    
}
// @@@ END_INCLUDE: https://raw.github.com/mrange/T4Include/master/Extensions/ParseExtensions.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: https://raw.github.com/mrange/T4Include/master/Reflection/ClassDescriptor.cs
namespace HronExperiment
{
    // ----------------------------------------------------------------------------------------------
    // Copyright (c) M�rten R�nge.
    // ----------------------------------------------------------------------------------------------
    // This source code is subject to terms and conditions of the Microsoft Public License. A 
    // copy of the license can be found in the License.html file at the root of this distribution. 
    // If you cannot locate the  Microsoft Public License, please send an email to 
    // dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
    //  by the terms of the Microsoft Public License.
    // ----------------------------------------------------------------------------------------------
    // You must not remove this notice, or any other, from this software.
    // ----------------------------------------------------------------------------------------------
    
    
    namespace Source.Reflection
    {
        using System;
        using System.Collections;
        using System.Collections.Concurrent;
        using System.Collections.Generic;
        using System.Linq;
        using System.Linq.Expressions;
        using System.Reflection;
    
        static class ClassDescriptorExtensions
        {
            public static ClassDescriptor GetClassDescriptor (this Type type)
            {
                return ClassDescriptor.GetClassDescriptor(type);
            }
        }
    
        partial class ClassDescriptor
        {
            static readonly ConcurrentDictionary<Type, ClassDescriptor> s_classDescriptors =
                new ConcurrentDictionary<Type, ClassDescriptor>();
    
            readonly Dictionary<string, MemberDescriptor>           m_memberLookup           ;
    
            public readonly string                                  Name                ;
    
            public readonly MemberDescriptor[]                      Members             ;
            public readonly MemberDescriptor[]                      PublicGetMembers    ;
            public readonly Type                                    Type                ;
            public readonly object[]                                Attributes          ;
    
            public readonly Func<object>                            Creator             ;
            public readonly bool                                    HasCreator          ;
    
            public readonly bool                                    IsNullable          ;
            public readonly Type                                    NonNullableType     ;
    
            public readonly bool                                    IsListLike          ;
            public readonly Type                                    ListItemType        ;
    
            public readonly bool                                    IsDictionaryLike    ;
            public readonly Type                                    DictionaryKeyType   ;
            public readonly Type                                    DictionaryValueType ;
    
            public ClassDescriptor(Type type)
            {
                Type        = type ?? typeof(object);
                Attributes  = Type.GetCustomAttributes(inherit: true);
                Name        = Type.Name;
                Members     = Type.IsPrimitive 
                    ?   new MemberDescriptor[0]
                    :   Type
                        .GetMembers(
                                BindingFlags.Instance
                            |   BindingFlags.Public
                            |   BindingFlags.NonPublic
                            )
                        .Where(mi => mi.MemberType == MemberTypes.Property || mi.MemberType == MemberTypes.Field)
                        .Where(mi => !HasIndexParameters(mi))
                        .Select(mi => new MemberDescriptor(mi))
                        .ToArray()
                        ;
    
                PublicGetMembers= Members.Where (mi => mi.HasPublicGetter).ToArray ();
                m_memberLookup  = Members.ToDictionary (mi => mi.Name);
    
                Creator = GetCreator(Type);
                HasCreator = !ReferenceEquals(Creator, s_defaultCreator);
    
                var isNullableType  = Type.IsGenericType && Type.GetGenericTypeDefinition() == typeof (Nullable<>);;
                IsNullable          = isNullableType || !Type.IsValueType;
                NonNullableType     = isNullableType ? Type.GetGenericArguments()[0] : Type;
    
                IsListLike          = false;
                IsDictionaryLike    = false;
                ListItemType        = typeof (object);
                DictionaryKeyType   = typeof (object);
                DictionaryValueType = typeof (object);
    
                var possibleDictionaryType = AsGenericType(Type, typeof(IDictionary<,>));
                var possibleListType = AsGenericType(Type, typeof(IList<>));
    
                IsDictionaryLike = possibleDictionaryType  != null || typeof (IDictionary).IsAssignableFrom (Type);
                if (possibleDictionaryType != null)
                {
                    var genericArguments = possibleDictionaryType.GetGenericArguments();
                    DictionaryKeyType   = genericArguments[0];
                    DictionaryValueType = genericArguments[1];
                }
    
                IsListLike = possibleListType  != null || typeof (IList).IsAssignableFrom (Type);
                if (possibleListType != null)
                {
                    ListItemType = possibleListType.GetGenericArguments()[0];
                }
            }
    
            static bool HasIndexParameters (MemberInfo mi)
            {
                var pi = mi as PropertyInfo;
                return pi != null && pi.GetIndexParameters().Length > 0;
            }
    
            static Type AsGenericType (Type type, Type asType)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition () == asType)
                {
                    return type;
                }
                
                return type
                    .GetInterfaces()
                    .FirstOrDefault(t =>  t.IsGenericType && t.GetGenericTypeDefinition() == asType)
                    ;
            }
    
            public MemberDescriptor FindMember (string name, bool requirePublicGet = true, bool requirePublicSet = true)
            {
                MemberDescriptor value;
                if (!m_memberLookup.TryGetValue(name ?? "", out value))
                {
                    return null;
                }
    
                return      (!requirePublicGet || value.HasPublicGetter)
                       &&   (!requirePublicSet || value.HasPublicSetter)
                            ?   value
                            :   null
                            ;
            }
    
            Func<object> GetCreator(Type type)
            {
                if (type.IsAbstract || type.IsInterface)
                {
                    return s_defaultCreator;
                }
    
                if (type.IsValueType)
                {
                    return Expression.Lambda<Func<object>>(Expression.Convert (Expression.New(type), typeof(object))).Compile();                
                }
    
                var ci = type
                    .GetConstructors(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic)
                    .FirstOrDefault (c => c.GetParameters ().Length == 0)
                    ;
    
                if (ci == null)
                {
                    return s_defaultCreator;
                }
    
                return Expression.Lambda<Func<object>>(Expression.New(ci)).Compile();
            }
    
            static readonly Func<Type, ClassDescriptor> s_createClassDescriptor = t => new ClassDescriptor(t);
            static readonly Func<object> s_defaultCreator = () => null;
    
            public static ClassDescriptor GetClassDescriptor(Type type)
            {
                return s_classDescriptors.GetOrAdd(
                    type ?? typeof (object),
                    s_createClassDescriptor
                    );
            }
        }
    
        partial class MemberDescriptor
        {
            public readonly string                  Name                ;
    
            public readonly MemberInfo              MemberInfo          ;
            public readonly Type                    MemberType          ;
            public readonly object[]                Attributes          ;
    
            public readonly bool                    HasPublicGetter     ;
            public readonly bool                    HasPublicSetter     ;
    
            public readonly bool                    HasGetter           ;
            public readonly bool                    HasSetter           ;
            public readonly Func<object, object>    Getter              ;
            public readonly Action<object, object>  Setter              ;
    
            ClassDescriptor m_lazyClassDescriptor;
    
            static readonly Func<object, object>    s_defaultGetter    = instance => null  ;
            static readonly Action<object, object>  s_defaultSetter  = (x, v) => { }     ;
    
            public MemberDescriptor(MemberInfo memberInfo)
            {
                if (memberInfo == null)
                {
                    throw new ArgumentNullException("memberInfo");
                }
    
                MemberInfo  = memberInfo;
                Attributes  = MemberInfo.GetCustomAttributes(inherit: true);
                Name        = MemberInfo.Name; 
                Getter      = GetGetter(MemberInfo);
                Setter      = GetSetter(MemberInfo);
    
                HasGetter   = !ReferenceEquals(Getter, s_defaultGetter);
                HasSetter   = !ReferenceEquals(Setter, s_defaultSetter);
    
                var pi = MemberInfo as PropertyInfo;
                var fi = MemberInfo as FieldInfo;
                if (pi != null)
                {
                    MemberType      =   pi.PropertyType                                             ;
                    HasPublicGetter =   HasGetter && pi.GetGetMethod(nonPublic: true).IsPublic   ;
                    HasPublicSetter =   HasSetter && pi.GetSetMethod(nonPublic: true).IsPublic   ;
                }
                else if (fi != null)
                {
                    MemberType      = fi.FieldType;
                    HasPublicGetter    =   HasGetter && fi.IsPublic    ;
                    HasPublicSetter    =   HasSetter && fi.IsPublic    ;
                }
                else
                {
                    MemberType = typeof (object);
                }
    
            }
    
            public ClassDescriptor ClassDescriptor
            {
                get { return m_lazyClassDescriptor ?? (m_lazyClassDescriptor = MemberType.GetClassDescriptor()); }
            }
    
            static Func<object, object> GetGetter(MemberInfo mi)
            {
                var pi = mi as PropertyInfo;
                var fi = mi as FieldInfo;
    
                if (pi != null && pi.GetGetMethod(nonPublic:true) != null && pi.GetGetMethod(nonPublic:true).GetParameters().Length == 0)
                {
                    var instance = Expression.Parameter(typeof(object), "instance");
    
                    var expr = Expression.Lambda<Func<object, object>>(
                        Expression.Convert(
                            Expression.Property(
                                Expression.Convert(instance, pi.DeclaringType ?? typeof(object)), 
                                pi), 
                            typeof(object)),
                        instance);
    
                    return expr.Compile();                
                }
                else if (fi != null)
                {
                    var instance = Expression.Parameter(typeof(object), "instance");
    
                    var expr = Expression.Lambda<Func<object, object>>(
                        Expression.Convert(
                            Expression.Field(
                                Expression.Convert(instance, fi.DeclaringType ?? typeof(object)), fi),
                            typeof(object)),
                        instance);
    
                    return expr.Compile();
                }
                else
                {
                    return s_defaultGetter;
                }
            }
    
            static Action<object, object> GetSetter(MemberInfo mi)
            {
                var pi = mi as PropertyInfo;
                var fi = mi as FieldInfo;
    
                if (pi != null && pi.GetSetMethod(nonPublic:true) != null && pi.GetSetMethod(nonPublic:true).GetParameters().Length == 1)
                {
                    var instance = Expression.Parameter(typeof(object), "instance");
                    var value = Expression.Parameter(typeof(object), "value");
    
                    var expr = Expression.Lambda<Action<object, object>>(
                        Expression.Assign(
                            Expression.Property(Expression.Convert(instance, pi.DeclaringType ?? typeof(object)), pi), 
                            Expression.Convert(value, pi.PropertyType)),
                        instance,
                        value
                        );
    
                    return expr.Compile();
                }
                else if (fi != null && !fi.IsInitOnly)
                {
                    var instance = Expression.Parameter(typeof(object), "instance");
                    var value = Expression.Parameter(typeof(object), "value");
    
                    var expr = Expression.Lambda<Action<object, object>>(
                        Expression.Assign(
                            Expression.Field(Expression.Convert(instance, fi.DeclaringType ?? typeof(object)), fi),
                            Expression.Convert(value, fi.FieldType)),
                        instance,
                        value
                        );
    
                    return expr.Compile();
                }
                else
                {
                    return s_defaultSetter;
                }
            }
    
        }
    
    
    }
}
// @@@ END_INCLUDE: https://raw.github.com/mrange/T4Include/master/Reflection/ClassDescriptor.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: https://raw.github.com/mrange/T4Include/master/Reflection/StaticReflection.cs
namespace HronExperiment
{
    // ----------------------------------------------------------------------------------------------
    // Copyright (c) M�rten R�nge.
    // ----------------------------------------------------------------------------------------------
    // This source code is subject to terms and conditions of the Microsoft Public License. A 
    // copy of the license can be found in the License.html file at the root of this distribution. 
    // If you cannot locate the  Microsoft Public License, please send an email to 
    // dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
    //  by the terms of the Microsoft Public License.
    // ----------------------------------------------------------------------------------------------
    // You must not remove this notice, or any other, from this software.
    // ----------------------------------------------------------------------------------------------
    
    namespace Source.Reflection
    {
        using System;
        using System.Linq.Expressions;
        using System.Reflection;
    
        static partial class StaticReflection
        {
            public static MethodInfo GetMethodInfo (Expression<Action> expr)
            {
                return ((MethodCallExpression)expr.Body).Method;
            }
    
            public static MemberInfo GetMemberInfo<TReturn> (Expression<Func<TReturn>> expr)
            {
                return ((MemberExpression)expr.Body).Member;
            }
    
            public static ConstructorInfo GetConstructorInfo<TReturn> (Expression<Func<TReturn>> expr)
            {
                return ((NewExpression)expr.Body).Constructor;
            }
        }
    
        static partial class StaticReflection<T>
        {
            public static MethodInfo GetMethodInfo (Expression<Action<T>> expr)
            {
                return ((MethodCallExpression)expr.Body).Method;
            }
    
            public static MemberInfo GetMemberInfo<TReturn>(Expression<Func<T, TReturn>> expr)
            {
                return ((MemberExpression)expr.Body).Member;
            }
        }
    }
}
// @@@ END_INCLUDE: https://raw.github.com/mrange/T4Include/master/Reflection/StaticReflection.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: https://raw.github.com/mrange/T4Include/master/Extensions/EnumParseExtensions.cs
namespace HronExperiment
{
    // ----------------------------------------------------------------------------------------------
    // Copyright (c) M�rten R�nge.
    // ----------------------------------------------------------------------------------------------
    // This source code is subject to terms and conditions of the Microsoft Public License. A 
    // copy of the license can be found in the License.html file at the root of this distribution. 
    // If you cannot locate the  Microsoft Public License, please send an email to 
    // dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
    //  by the terms of the Microsoft Public License.
    // ----------------------------------------------------------------------------------------------
    // You must not remove this notice, or any other, from this software.
    // ----------------------------------------------------------------------------------------------
    
    
    
    namespace Source.Extensions
    {
        using System;
        using System.Collections.Concurrent;
        using System.Reflection;
    
        using Source.Reflection;
    
        static partial class EnumParseExtensions
        {
            enum Dummy {}
    
            sealed partial class EnumParser
            {
                public Func<string, object> ParseEnum   ;
                public Func<object>         DefaultEnum ;
            }
    
            static readonly MethodInfo s_parseEnum = StaticReflection.GetMethodInfo (() => ParseEnum<Dummy>(default (string)));
            static readonly MethodInfo s_genericParseEnum = s_parseEnum.GetGenericMethodDefinition ();
    
            static readonly MethodInfo s_defaultEnum = StaticReflection.GetMethodInfo (() => DefaultEnum<Dummy>());
            static readonly MethodInfo s_genericDefaultEnum = s_defaultEnum.GetGenericMethodDefinition ();
    
            static readonly MethodInfo s_parseNullableEnum = StaticReflection.GetMethodInfo(() => ParseNullableEnum<Dummy>(default(string)));
            static readonly MethodInfo s_genericParseNullableEnum = s_parseNullableEnum.GetGenericMethodDefinition();
    
            static readonly MethodInfo s_defaultNullableEnum = StaticReflection.GetMethodInfo(() => DefaultNullableEnum<Dummy>());
            static readonly MethodInfo s_genericDefaultNullableEnum = s_defaultNullableEnum.GetGenericMethodDefinition();
    
            static readonly ConcurrentDictionary<Type, EnumParser> s_enumParsers = new ConcurrentDictionary<Type, EnumParser>();
            static readonly Func<Type, EnumParser> s_createParser = type => CreateParser (type);
    
            static EnumParser CreateParser(Type type)
            {
                if (type.IsEnum)
                {
                    return new EnumParser
                               {
                                   ParseEnum = (Func<string, object>)Delegate.CreateDelegate(
                                                        typeof(Func<string, object>),
                                                        s_genericParseEnum.MakeGenericMethod(type)
                                                        ),
                                   DefaultEnum = (Func<object>)Delegate.CreateDelegate(
                                                       typeof(Func<object>),
                                                       s_genericDefaultEnum.MakeGenericMethod(type)
                                                       ),
                               };
    
                }
                else if (
                        type.IsGenericType
                    && type.GetGenericTypeDefinition() == typeof(Nullable<>)
                    && type.GetGenericArguments()[0].IsEnum
                    )
                {
                    var enumType = type.GetGenericArguments()[0];
                    return new EnumParser
                               {
                                   ParseEnum = (Func<string, object>)Delegate.CreateDelegate(
                                                        typeof(Func<string, object>),
                                                        s_genericParseNullableEnum.MakeGenericMethod(enumType)
                                                        ),
                                   DefaultEnum = (Func<object>)Delegate.CreateDelegate(
                                                       typeof(Func<object>),
                                                       s_genericDefaultNullableEnum.MakeGenericMethod(enumType)
                                                       ),
                               };
    
                }
                else
                {
                    return null;
                }
            }
    
            static object ParseEnum<TEnum>(string value)
                where TEnum : struct
            {
                TEnum result;
                return Enum.TryParse (value, true, out result)
                    ? (object)result
                    : null
                    ;
            }
    
            static object DefaultEnum<TEnum>()
                where TEnum : struct
            {
                return default (TEnum);
            }
    
            static object ParseNullableEnum<TEnum>(string value)
                where TEnum : struct
            {
                TEnum result;
                return Enum.TryParse(value, true, out result)
                    ? (object)(TEnum?)result
                    : null
                    ;
            }
    
            static object DefaultNullableEnum<TEnum>()
                where TEnum : struct
            {
                return default(TEnum?);
            }
    
            public static bool TryParseEnumValue(this string s, Type type, out object value)
            {
                value = null;
                if (string.IsNullOrEmpty (s))
                {
                    return false;
                }
    
                var enumParser = TryGetParser (type);
                if (enumParser == null)
                {
                    return false;
    
                }
                
    
                value = enumParser.ParseEnum (s);
    
                return value != null;
            }
    
            public static bool CanParseEnumValue (this Type type)
            {
                var enumParser = TryGetParser (type);
    
                return enumParser != null;
            }
    
            static EnumParser TryGetParser (Type type)
            {
                if (type == null)
                {
                    return null;
                }
    
                var enumParser = s_enumParsers.GetOrAdd (type, s_createParser);
    
                return enumParser;
            }
    
            public static object ParseEnumValue (this string s, Type type)
            {
                object value;
                return s.TryParseEnumValue (type, out value)
                    ? value
                    : null
                    ;
            }
    
            public static object GetDefaultEnumValue (this Type type)
            {
                var enumParser = TryGetParser (type);
                return enumParser != null ? enumParser.DefaultEnum () : null;
            }
    
            public static TEnum ParseEnumValue<TEnum>(this string s, TEnum defaultValue) 
                where TEnum : struct
            {
                TEnum value;
                return Enum.TryParse (s, true, out value)
                    ? value
                    : defaultValue
                    ;
            }
    
            public static TEnum? ParseEnumValue<TEnum>(this string s)
                where TEnum : struct
            {
                TEnum value;
                return Enum.TryParse(s, true, out value)
                    ? (TEnum?)value
                    : null
                    ;
            }
    
        }
    }
}
// @@@ END_INCLUDE: https://raw.github.com/mrange/T4Include/master/Extensions/EnumParseExtensions.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: https://raw.github.com/mrange/T4Include/master/Common/Generated_Log.cs
namespace HronExperiment
{
    // ############################################################################
    // #                                                                          #
    // #        ---==>  T H I S  F I L E  I S   G E N E R A T E D  <==---         #
    // #                                                                          #
    // # This means that any edits to the .cs file will be lost when its          #
    // # regenerated. Changes should instead be applied to the corresponding      #
    // # template file (.tt)                                                      #
    // ############################################################################
    
    
    
    
    
    
    namespace Source.Common
    {
        using System;
    
        partial class Log
        {
            public enum Level
            {
                Success = 1000,
                HighLight = 2000,
                Info = 3000,
                Warning = 10000,
                Error = 20000,
                Exception = 21000,
            }
    
            public static void Success (string format, params object[] args)
            {
                LogMessage (Level.Success, format, args);
            }
            public static void HighLight (string format, params object[] args)
            {
                LogMessage (Level.HighLight, format, args);
            }
            public static void Info (string format, params object[] args)
            {
                LogMessage (Level.Info, format, args);
            }
            public static void Warning (string format, params object[] args)
            {
                LogMessage (Level.Warning, format, args);
            }
            public static void Error (string format, params object[] args)
            {
                LogMessage (Level.Error, format, args);
            }
            public static void Exception (string format, params object[] args)
            {
                LogMessage (Level.Exception, format, args);
            }
    #if !NETFX_CORE && !SILVERLIGHT && !WINDOWS_PHONE
            static ConsoleColor GetLevelColor (Level level)
            {
                switch (level)
                {
                    case Level.Success:
                        return ConsoleColor.Green;
                    case Level.HighLight:
                        return ConsoleColor.White;
                    case Level.Info:
                        return ConsoleColor.Gray;
                    case Level.Warning:
                        return ConsoleColor.Yellow;
                    case Level.Error:
                        return ConsoleColor.Red;
                    case Level.Exception:
                        return ConsoleColor.Red;
                    default:
                        return ConsoleColor.Magenta;
                }
            }
    #endif
            static string GetLevelMessage (Level level)
            {
                switch (level)
                {
                    case Level.Success:
                        return "SUCCESS  ";
                    case Level.HighLight:
                        return "HIGHLIGHT";
                    case Level.Info:
                        return "INFO     ";
                    case Level.Warning:
                        return "WARNING  ";
                    case Level.Error:
                        return "ERROR    ";
                    case Level.Exception:
                        return "EXCEPTION";
                    default:
                        return "UNKNOWN  ";
                }
            }
    
        }
    }
    
}
// @@@ END_INCLUDE: https://raw.github.com/mrange/T4Include/master/Common/Generated_Log.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: https://raw.github.com/mrange/T4Include/master/Common/SubString.cs
namespace HronExperiment
{
    // ----------------------------------------------------------------------------------------------
    // Copyright (c) M�rten R�nge.
    // ----------------------------------------------------------------------------------------------
    // This source code is subject to terms and conditions of the Microsoft Public License. A 
    // copy of the license can be found in the License.html file at the root of this distribution. 
    // If you cannot locate the  Microsoft Public License, please send an email to 
    // dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
    //  by the terms of the Microsoft Public License.
    // ----------------------------------------------------------------------------------------------
    // You must not remove this notice, or any other, from this software.
    // ----------------------------------------------------------------------------------------------
    
    
    
    
    namespace Source.Common
    {
        using System;
        using System.Collections;
        using System.Collections.Generic;
        using System.Text;
    
    
        static class SubStringExtensions
        {
            public static void AppendSubString (this StringBuilder sb, SubString ss)
            {
                sb.Append (ss.BaseString, ss.Begin, ss.Length);
            }
    
            public static string Concatenate (this IEnumerable<SubString> values, string delimiter = null)
            {
                if (values == null)
                {
                    return "";
                }
    
                delimiter = delimiter ?? ", ";
    
                var first = true;
    
                var sb = new StringBuilder ();
                foreach (var value in values)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        sb.Append (delimiter);
                    }
    
                    sb.AppendSubString (value);
                }
    
                return sb.ToString ();
            }
    
    
    
            public static SubString ToSubString (this string value, int begin = 0, int count = int.MaxValue / 2)
            {
                return new SubString (value, begin, count);
            }
    
            public static SubString ToSubString (this StringBuilder value, int begin = 0, int count = int.MaxValue / 2)
            {
                return new SubString (value.ToString (), begin, count);
            }
    
            public static SubString ToSubString (this SubString value, int begin = 0, int count = int.MaxValue / 2)
            {
                return new SubString (value, begin, count);
            }
    
            enum ParseLineState
            {
                NewLine     ,
                Inline      ,
                ConsumedCR  ,
            }
    
            public static IEnumerable<SubString> ReadLines (this string value)
            {
                return value.ToSubString ().ReadLines ();
            }
    
            public static IEnumerable<SubString> ReadLines (this SubString subString)
            {
                var baseString = subString.BaseString;
                var begin = subString.Begin;
                var end = subString.End;
    
                var beginLine   = begin ;
                var count       = 0     ;
    
                var state       = ParseLineState.NewLine;
    
                for (var iter = begin; iter < end; ++iter)
                {
                    var ch = baseString[iter];
    
                    switch (state)
                    {
                        case ParseLineState.ConsumedCR:
                            yield return new SubString (baseString, beginLine, count);
                            switch (ch)
                            {
                                case '\r':
                                    beginLine = iter;
                                    count = 0;
                                    state = ParseLineState.ConsumedCR;
                                    break;
                                case '\n':
                                    state = ParseLineState.NewLine;
                                    break;
                                default:
                                    beginLine = iter;
                                    count = 1;
                                    state = ParseLineState.Inline;
                                    break;
                            }
    
                            break;
                        case ParseLineState.NewLine:
                            beginLine   = iter;
                            count       = 0;
                            switch (ch)
                            {
                                case '\r':
                                    state = ParseLineState.ConsumedCR;
                                    break;
                                case '\n':
                                    yield return new SubString (baseString, beginLine, count);
                                    state = ParseLineState.NewLine;
                                    break;
                                default:
                                    state = ParseLineState.Inline;
                                    ++count;
                                    break;
                            }
                            break;
                        case ParseLineState.Inline:
                        default:
                            switch (ch)
                            {
                                case '\r':
                                    state = ParseLineState.ConsumedCR;
                                    break;
                                case '\n':
                                    yield return new SubString (baseString, beginLine, count);
                                    state = ParseLineState.NewLine;
                                    break;
                                default:
                                    ++count;
                                    break;
                            }
                            break;
                    }
                }
    
                switch (state)
                {
                    case ParseLineState.NewLine:
                        yield return new SubString (baseString, 0, 0);
                        break;
                    case ParseLineState.ConsumedCR:
                        yield return new SubString (baseString, beginLine, count);
                        yield return new SubString (baseString, 0, 0);
                        break;
                    case ParseLineState.Inline:
                    default:
                        yield return new SubString (baseString, beginLine, count);
                        break;
                }
            }
    
        }
    
        struct SubString 
            :   IComparable
            ,   ICloneable
            ,   IComparable<SubString>
            ,   IEnumerable<char>
            ,   IEquatable<SubString>
        {
            readonly string m_baseString;
            readonly int m_begin;
            readonly int m_end;
    
            string m_value;
            int m_hashCode;
            bool m_hasHashCode;
    
            static int Clamp (int v, int l, int r)
            {
                if (v < l)
                {
                    return l;
                }
    
                if (r < v)
                {
                    return r;
                }
    
                return v;
            }
    
            public static readonly SubString Empty = new SubString (null, 0,0);
    
            public SubString (SubString subString, int begin, int count) : this ()
            {
                m_baseString    = subString.BaseString;
                var length      = subString.Length;
    
                begin           = Clamp (begin, 0, length);
                count           = Clamp (count, 0, length - begin);
                var end         = begin + count;
    
                m_begin         = subString.Begin + begin;
                m_end           = subString.Begin + end;
            }
    
            public SubString (string baseString, int begin, int count) : this ()
            {
                m_baseString    = baseString;
                var length      = BaseString.Length;
    
                begin           = Clamp (begin, 0, length);
                count           = Clamp (count, 0, length - begin);
                var end         = begin + count;
    
                m_begin         = begin;
                m_end           = end;
            }
    
            public static bool operator== (SubString left, SubString right)
            {
                return left.CompareTo (right) == 0;
            }
    
            public static bool operator!= (SubString left, SubString right)
            {
                return left.CompareTo (right) != 0;
            }
    
            public bool Equals (SubString other)
            {
                return CompareTo (other) == 0;
            }
    
            public override int GetHashCode  ()
            {
                if (!m_hasHashCode)
                {
                    m_hashCode = Value.GetHashCode ();
                    m_hasHashCode = true;
                }
    
                return m_hashCode;
            }
    
            IEnumerator IEnumerable.GetEnumerator ()
            {
                return GetEnumerator ();
            }
    
            public object Clone ()
            {
                return this;
            }
    
            public int CompareTo (object obj)
            {
                return obj is SubString ? CompareTo ((SubString) obj) : 1;
            }
    
    
            public override bool Equals (object obj)
            {
                return obj is SubString && Equals ((SubString) obj);
            }
    
    
            public int CompareTo (SubString other)
            {
                if (Length < other.Length)
                {
                    return -1;
                }
    
                if (Length > other.Length)
                {
                    return 1;
                }
    
                return String.Compare (
                    BaseString          ,
                    Begin               ,
                    other.BaseString    ,
                    other.Begin         ,
                    Length
                    );
            }
    
            public IEnumerator<char> GetEnumerator ()
            {
                for (var iter = Begin; iter < End; ++iter)
                {
                    yield return BaseString[iter];
                }
            }
    
            public override string ToString ()
            {
                return Value;
            }
    
            public string Value
            {
                get
                {
                    if (m_value == null)
                    {
                        m_value = BaseString.Substring (Begin, Length);
                    }
                    return m_value;
                }
            }
    
            public string BaseString
            {
                get { return m_baseString ?? ""; }
            }
    
            public int Begin
            {
                get { return m_begin; }
            }
    
            public int End
            {
                get { return m_end; }
            }
    
            public char this[int idx]
            {
                get
                {
                    if (idx < 0)
                    {
                        throw new IndexOutOfRangeException ("idx");
                    }
    
                    if (idx >= Length)
                    {
                        throw new IndexOutOfRangeException ("idx");
                    }
    
                    return BaseString[idx + Begin];
                }
            }
    
            public int Length
            {
                get { return End - Begin; }
            }
    
            public bool IsEmpty
            {
                get { return Length == 0; }
            }
    
            public bool IsWhiteSpace
            {
                get
                {
                    if (IsEmpty)
                    {
                        return true;
                    }
    
                    for (var iter = Begin; iter < End; ++iter)
                    {
                        if (!Char.IsWhiteSpace (BaseString[iter]))
                        {
                            return false;
                        }
                    }
    
                    return true;
                }
            }
    
            public bool All (Func<char,bool> test)
            {
                if (test == null)
                {
                    return true;
                }
    
                if (IsEmpty)
                {
                    return true;
                }
    
                for (var iter = Begin; iter < End; ++iter)
                {
                    if (!test (BaseString[iter]))
                    {
                        return false;
                    }
                }
    
                return true;
            }
    
            static readonly char[] s_defaultTrimChars = " \t\r\n".ToCharArray ();
    
            static bool Contains (char[] trimChars, char ch)
            {
                for (int index = 0; index < trimChars.Length; index++)
                {
                    var trimChar = trimChars[index];
    
                    if (trimChar == ch)
                    {
                        return true;
                    }
                }
    
                return false;
            }
    
            public SubString TrimStart (params char[] trimChars)
            {
                if (trimChars == null || trimChars.Length == 0)
                {
                    trimChars = s_defaultTrimChars;
                }
    
                for (var iter = Begin; iter < End; ++iter)
                {
                    var ch = BaseString[iter];
    
                    if (!Contains (trimChars, ch))
                    {
                        return new SubString (BaseString, iter, End - iter);
                    }
                }
    
                return new SubString (BaseString, Begin, 0);
            }
    
            public SubString TrimEnd (params char[] trimChars)
            {
                if (trimChars == null || trimChars.Length == 0)
                {
                    trimChars = s_defaultTrimChars;
                }
    
                for (var iter = End - 1; iter >= Begin; --iter)
                {
                    var ch = BaseString[iter];
    
                    if (!Contains (trimChars, ch))
                    {
                        return new SubString (BaseString, Begin, iter - Begin + 1);
                    }
                }
    
                return new SubString (BaseString, Begin, 0);
            }
    
            public SubString Trim (params char[] trimChars)
            {
                return TrimStart (trimChars).TrimEnd (trimChars);
            }
        }
    }
}
// @@@ END_INCLUDE: https://raw.github.com/mrange/T4Include/master/Common/SubString.cs
// ############################################################################

// ############################################################################
namespace HronExperiment.Include
{
    static partial class MetaData
    {
        public const string RootPath        = @"https://raw.github.com/";
        public const string IncludeDate     = @"2013-10-09T20:00:21";

        public const string Include_0       = @"https://raw.github.com/mrange/T4Include/master/Extensions/BasicExtensions.cs";
        public const string Include_1       = @"https://raw.github.com/mrange/T4Include/master/Hron/HRONObjectSerializer.cs";
        public const string Include_2       = @"https://raw.github.com/mrange/T4Include/master/Hron/HRONDynamicObjectSerializer.cs";
        public const string Include_3       = @"https://raw.github.com/mrange/T4Include/master/Common/Array.cs";
        public const string Include_4       = @"https://raw.github.com/mrange/T4Include/master/Common/Config.cs";
        public const string Include_5       = @"https://raw.github.com/mrange/T4Include/master/Common/Log.cs";
        public const string Include_6       = @"https://raw.github.com/mrange/T4Include/master/Hron/HRONSerializer.cs";
        public const string Include_7       = @"https://raw.github.com/mrange/T4Include/master/Extensions/ParseExtensions.cs";
        public const string Include_8       = @"https://raw.github.com/mrange/T4Include/master/Reflection/ClassDescriptor.cs";
        public const string Include_9       = @"https://raw.github.com/mrange/T4Include/master/Reflection/StaticReflection.cs";
        public const string Include_10       = @"https://raw.github.com/mrange/T4Include/master/Extensions/EnumParseExtensions.cs";
        public const string Include_11       = @"https://raw.github.com/mrange/T4Include/master/Common/Generated_Log.cs";
        public const string Include_12       = @"https://raw.github.com/mrange/T4Include/master/Common/SubString.cs";
    }
}
// ############################################################################


