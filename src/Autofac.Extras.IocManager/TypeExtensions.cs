﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using FluentAssemblyScanner;

namespace Autofac.Extras.IocManager
{
    public static class TypeExtensions
    {
        public static Type[] GetDefaultInterfacesWithSelf(this Type @this)
        {
            var types = @this.GetInterfaces()
                             .Where(x => @this.Name.Contains(x.Name.TrimStart('I')))
                             .ToArray();
            return types.Prepend(@this).ToArray();
        }

        public static Type[] GetDefaultInterfaces(this Type @this)
        {
            return @this.GetInterfaces()
                        .Where(x => @this.Name.Contains(x.Name.TrimStart('I')))
                        .ToArray();
        }

        public static List<Type> AssignedTypesInAssembly(this Type @this, Assembly assembly)
        {
            return AssemblyScanner.FromAssembly(assembly)
                                  .IncludeNonPublicTypes()
                                  .BasedOn(@this)
                                  .Filter()
                                  .Classes()
                                  .NonStatic()
                                  .Scan();
        }

        /// <summary>Appends the item to the specified sequence.</summary>
        /// <typeparam name="T">The type of element in the sequence.</typeparam>
        /// <param name="sequence">The sequence.</param>
        /// <param name="trailingItem">The trailing item.</param>
        /// <returns>The sequence with an item appended to the end.</returns>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> sequence, T trailingItem)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException(nameof(sequence));
            }

            foreach (var obj in sequence)
            {
                yield return obj;
            }

            yield return trailingItem;
        }

        /// <summary>Prepends the item to the specified sequence.</summary>
        /// <typeparam name="T">The type of element in the sequence.</typeparam>
        /// <param name="sequence">The sequence.</param>
        /// <param name="leadingItem">The leading item.</param>
        /// <returns>The sequence with an item prepended.</returns>
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> sequence, T leadingItem)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException(nameof(sequence));
            }

            yield return leadingItem;

            foreach (var obj in sequence)
            {
                yield return obj;
            }
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var obj in items)
            {
                collection.Add(obj);
            }
        }
    }
}
